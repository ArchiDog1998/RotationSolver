using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Game;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Resource;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Combos.Disciplines;

namespace XIVAutoAttack
{
    internal class Watcher : IDisposable
    {

        private static class Signatures
        {
            internal const string PlaySpecificSound = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 33 F6 8B DA 48 8B F9 0F BA E2 0F";

            internal const string GetResourceSync = "E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 48 89 87 ?? ?? ?? ?? 48 8D 54 24";

            internal const string GetResourceAsync = "E8 ?? ?? ?? ?? 48 8B D8 EB 07 F0 FF 83";

            internal const string LoadSoundFile = "E8 ?? ?? ?? ?? 48 85 C0 75 04 B0 F6";

            internal const string MusicManagerOffset = "48 8B 8E ?? ?? ?? ?? 39 78 20 0F 94 C2 45 33 C0";
        }

        private unsafe delegate void* PlaySpecificSoundDelegate(long a1, int idx);

        private unsafe delegate void* GetResourceSyncPrototype(IntPtr pFileManager, uint* pCategoryId, char* pResourceType, uint* pResourceHash, char* pPath, void* pUnknown);

        private unsafe delegate void* GetResourceAsyncPrototype(IntPtr pFileManager, uint* pCategoryId, char* pResourceType, uint* pResourceHash, char* pPath, void* pUnknown, bool isUnknown);

        private delegate void ReceiveActionEffectDelegate(int sourceObjectId, IntPtr sourceActor, IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);

        private delegate IntPtr LoadSoundFileDelegate(IntPtr resourceHandle, uint a2);

        private const int ResourceDataPointerOffset = 176;

        private const int MusicManagerStreamingOffset = 50;

        private Hook<PlaySpecificSoundDelegate> PlaySpecificSoundHook { get; set; }

        private Hook<GetResourceSyncPrototype> GetResourceSyncHook { get; set; }

        private Hook<GetResourceAsyncPrototype> GetResourceAsyncHook { get; set; }


        private Hook<LoadSoundFileDelegate> LoadSoundFileHook { get; set; }
        private Hook<ReceiveActionEffectDelegate> ReceiveActionEffectHook { get; set; }

        internal ConcurrentDictionary<IntPtr, FishType> Scds { get; } = new ConcurrentDictionary<IntPtr, FishType>();

        internal unsafe void Enable()
        {
            if (PlaySpecificSoundHook == null && Service.SigScanner.TryScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 33 F6 8B DA 48 8B F9 0F BA E2 0F", out var playPtr))
            {
                PlaySpecificSoundHook = Hook<PlaySpecificSoundDelegate>.FromAddress(playPtr, PlaySpecificSoundDetour);
            }
            if (GetResourceSyncHook == null && Service.SigScanner.TryScanText("E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 48 89 87 ?? ?? ?? ?? 48 8D 54 24", out var syncPtr))
            {
                GetResourceSyncHook = Hook<GetResourceSyncPrototype>.FromAddress(syncPtr, GetResourceSyncDetour);
            }
            if (GetResourceAsyncHook == null && Service.SigScanner.TryScanText("E8 ?? ?? ?? ?? 48 8B D8 EB 07 F0 FF 83", out var asyncPtr))
            {
                GetResourceAsyncHook = Hook<GetResourceAsyncPrototype>.FromAddress(asyncPtr, GetResourceAsyncDetour);
            }
            if (LoadSoundFileHook == null && Service.SigScanner.TryScanText("E8 ?? ?? ?? ?? 48 85 C0 75 04 B0 F6", out var soundPtr))
            {
                LoadSoundFileHook = Hook<LoadSoundFileDelegate>.FromAddress(soundPtr, LoadSoundFileDetour);
            }
            if(ReceiveActionEffectHook == null && Service.SigScanner.TryScanText("E8 ?? ?? ?? ?? 48 8B 8D F0 03 00 00", out var effectptr))
            {
                ReceiveActionEffectHook = Hook<ReceiveActionEffectDelegate>.FromAddress(effectptr, ReceiveActionEffectDetour);
            }

            PlaySpecificSoundHook?.Enable();
            LoadSoundFileHook?.Enable();
            GetResourceSyncHook?.Enable();
            GetResourceAsyncHook?.Enable();
            Service.ChatGui.ChatMessage += ChatGui_ChatMessage;
        }

        private void ReceiveActionEffectDetour(int sourceObjectId, IntPtr sourceActor, IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
        {
            ReceiveActionEffectHook?.Original(sourceObjectId, sourceActor, position, effectHeader, effectArray, effectTrail);
            //TimeLastActionUsed = DateTime.Now;

            ActionEffectHeader header = Marshal.PtrToStructure<ActionEffectHeader>(effectHeader);

            //if (header.ActionId is not 8 or 7&&
            //    sourceObjectId == Service.ClientState.LocalPlayer.ObjectId)
            //{
            //    LastActionUseCount++;
            //    if (header.ActionId != LastAction)
            //    {
            //        LastActionUseCount = 1;
            //    }

            //    LastAction = header.ActionId;

            //    var cate = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(header.ActionId).ActionCategory.Value;

            //    if (cate != null)
            //    {
            //        switch (cate.Name)
            //        {
            //            case "魔法":
            //                Service.ChatGui.Print("魔法" + cate.RowId);
            //                LastSpell = header.ActionId;
            //                break;
            //            case "战技":
            //                Service.ChatGui.Print("战技" + cate.RowId);

            //                LastWeaponskill = header.ActionId;
            //                break;
            //            case "能力":
            //                Service.ChatGui.Print("能力" + cate.RowId);

            //                LastAbility = header.ActionId;
            //                break;
            //        }
            //    }

            //    CombatActions.Add(header.ActionId);
            //}
        }

        private void ChatGui_ChatMessage(Dalamud.Game.Text.XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (message.TextValue.Contains(FSHCombo.Actions.Mooch.Action.Name) && (byte)type == 67)
            {
#if DEBUG
                Service.ChatGui.Print("Send!");
#endif
                TargetHelper.Fish = FishType.Mooch;
            }
        }

        public void Dispose()
        {
            PlaySpecificSoundHook?.Dispose();
            LoadSoundFileHook?.Dispose();
            GetResourceSyncHook?.Dispose();
            GetResourceAsyncHook?.Dispose();
            Service.ChatGui.ChatMessage -= ChatGui_ChatMessage;
        }

        private unsafe void* PlaySpecificSoundDetour(long a1, int idx)
        {
            CheckSound(a1, idx);
            return PlaySpecificSoundHook!.Original(a1, idx);
        }

        private unsafe void CheckSound(long a1, int idx)
        {
            if (a1 == 0L) return;
            byte* scdData = *(byte**)(a1 + 8);
            if (scdData == null) return;
            if (!Scds.TryGetValue((IntPtr)scdData, out var fishType)) return;
            TargetHelper.Fish = fishType;
#if DEBUG
            Service.ChatGui.Print(fishType.ToString());
#endif
        }

        private unsafe void* GetResourceSyncDetour(IntPtr pFileManager, uint* pCategoryId, char* pResourceType, uint* pResourceHash, char* pPath, void* pUnknown)
        {
            return ResourceDetour(isSync: true, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown: false);
        }

        private unsafe void* GetResourceAsyncDetour(IntPtr pFileManager, uint* pCategoryId, char* pResourceType, uint* pResourceHash, char* pPath, void* pUnknown, bool isUnknown)
        {
            return ResourceDetour(isSync: false, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown);
        }

        private unsafe void* ResourceDetour(bool isSync, IntPtr pFileManager, uint* pCategoryId, char* pResourceType, uint* pResourceHash, char* pPath, void* pUnknown, bool isUnknown)
        {
            void* ret = CallOriginalResourceHandler(isSync, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown);
            var type = GetFishType(ReadTerminatedBytes((byte*)pPath));
            if (type != FishType.None && ret != null)
            {
                IntPtr scdData = Marshal.ReadIntPtr((IntPtr)ret + ResourceDataPointerOffset);
                if (scdData != IntPtr.Zero)
                {
                    Scds[scdData] = type;
                }
            }
            return ret;
        }

        private unsafe void* CallOriginalResourceHandler(bool isSync, IntPtr pFileManager, uint* pCategoryId, char* pResourceType, uint* pResourceHash, char* pPath, void* pUnknown, bool isUnknown)
        {
            if (!isSync)
            {
                return GetResourceAsyncHook!.Original(pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown);
            }
            return GetResourceSyncHook!.Original(pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown);
        }

        private unsafe IntPtr LoadSoundFileDetour(IntPtr resourceHandle, uint a2)
        {
            IntPtr ret = LoadSoundFileHook!.Original(resourceHandle, a2);
            try
            {
                ResourceHandle* handle = (ResourceHandle*)(void*)resourceHandle;
                var type = GetFishType(handle->FileName.ToString());
                if (type != FishType.None)
                {
                    IntPtr dataPtr = Marshal.ReadIntPtr(resourceHandle + ResourceDataPointerOffset);
                    Scds[dataPtr] = type;
                    return ret;
                }
                return ret;
            }
            catch
            {
                return ret;
            }
        }


        private unsafe static string ReadTerminatedBytes(byte* ptr)
        {
            if (ptr == null)
            {
                return string.Empty;
            }
            List<byte> bytes = new List<byte>();
            while (*ptr != 0)
            {
                bytes.Add(*ptr);
                ptr++;
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        private static FishType GetFishType(string name)
        {
            name = name.ToLowerInvariant();
            if (name.Contains("sound/vibration/live/vib_live_fish_hit01.scd")) return FishType.Small;
            if (name.Contains("sound/vibration/live/vib_live_fish_hit02.scd")) return FishType.Medium;
            if (name.Contains("sound/vibration/live/vib_live_fish_hit03.scd")) return FishType.Large;
            return FishType.None;
        }
    }

    internal enum FishType : byte
    {
        None,
        Small,
        Medium,
        Large,
        Mooch,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ActionEffectHeader
    {
        [FieldOffset(0x0)] public long TargetObjectId;
        [FieldOffset(0x8)] public uint ActionId;
        [FieldOffset(0x14)] public uint UnkObjectId;
        [FieldOffset(0x18)] public ushort Sequence;
        [FieldOffset(0x1A)] public ushort Unk_1A;
    }
}
