using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Disciplines;
using Action = Lumina.Excel.GeneratedSheets.Action;


namespace XIVAutoAttack
{
    internal class Watcher : IDisposable
    {
        private unsafe delegate void* PlaySpecificSoundDelegate(long a1, int idx);
        private unsafe delegate void* GetResourceSyncPrototype(IntPtr pFileManager, uint* pCategoryId, char* pResourceType, uint* pResourceHash, char* pPath, void* pUnknown);
        private unsafe delegate void* GetResourceAsyncPrototype(IntPtr pFileManager, uint* pCategoryId, char* pResourceType, uint* pResourceHash, char* pPath, void* pUnknown, bool isUnknown);
        private unsafe delegate bool UseActionDelegate(IntPtr actionManager, ActionType actionType, uint actionID, uint targetID, uint param, uint useType, int pvp, bool* isGroundTarget);

        private delegate IntPtr LoadSoundFileDelegate(IntPtr resourceHandle, uint a2);

        private const int ResourceDataPointerOffset = 176;

        private const int MusicManagerStreamingOffset = 50;

        private Hook<PlaySpecificSoundDelegate> PlaySpecificSoundHook { get; set; }

        private Hook<GetResourceSyncPrototype> GetResourceSyncHook { get; set; }

        private Hook<GetResourceAsyncPrototype> GetResourceAsyncHook { get; set; }

        private Hook<LoadSoundFileDelegate> LoadSoundFileHook { get; set; }
        private Hook<UseActionDelegate> GetActionHook { get; set; }

        internal ConcurrentDictionary<IntPtr, FishType> Scds { get; } = new ConcurrentDictionary<IntPtr, FishType>();

        public static uint LastAction { get; set; } = 0;
        public static uint LastWeaponskill { get; set; } = 0;
        public static uint LastAbility { get; set; } = 0;
        public static uint LastSpell { get; set; } = 0;

        public static TimeSpan TimeSinceLastAction => DateTime.Now - TimeLastActionUsed;

        private static DateTime TimeLastActionUsed = DateTime.Now;

        internal unsafe void Enable()
        {
            PlaySpecificSoundHook = Hook<PlaySpecificSoundDelegate>.FromAddress(Service.Address.PlaySpecificSound, PlaySpecificSoundDetour);
            GetResourceSyncHook = Hook<GetResourceSyncPrototype>.FromAddress(Service.Address.GetResourceSync, GetResourceSyncDetour);
            GetResourceAsyncHook = Hook<GetResourceAsyncPrototype>.FromAddress(Service.Address.GetResourceAsync, GetResourceAsyncDetour);
            LoadSoundFileHook = Hook<LoadSoundFileDelegate>.FromAddress(Service.Address.LoadSoundFile, LoadSoundFileDetour);
            GetActionHook = Hook<UseActionDelegate>.FromAddress((IntPtr)ActionManager.fpUseAction, UseAction);

            PlaySpecificSoundHook?.Enable();
            LoadSoundFileHook?.Enable();
            GetResourceSyncHook?.Enable();
            GetResourceAsyncHook?.Enable();
            GetActionHook?.Enable();
            Service.ChatGui.ChatMessage += ChatGui_ChatMessage;
        }

        private unsafe bool UseAction(IntPtr actionManager, ActionType actionType, uint actionID, uint targetID = 3758096384u, uint param = 0u, uint useType = 0u, int pvp = 0, bool* a7 = null)
        {
#if DEBUG
        var a = actionType == ActionType.Spell ? Service.DataManager.GetExcelSheet<Action>().GetRow(actionID)?.Name : Service.DataManager.GetExcelSheet<Item>().GetRow(actionID)?.Name;
        Service.ChatGui.Print(a + ", " + actionType.ToString() + ", " + actionID.ToString() + ", " + param.ToString() + ", " + useType.ToString() + ", " + pvp.ToString());

#endif
            if (actionType == ActionType.Spell && useType == 0)
            {
                var action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
                var cate = action.ActionCategory.Value;
                var tar = Service.ObjectTable.SearchById(targetID);

                //Macro
                if (actionID != LastAction)
                {
                    foreach (var item in Service.Configuration.Events)
                    {
                        if (item.Name == action.Name)
                        {
                            if (item.MacroIndex < 0 || item.MacroIndex > 99) break;

                            TargetHelper.Macros.Enqueue(new MacroItem(tar, item.IsShared ? RaptureMacroModule.Instance->Shared[item.MacroIndex] :
                                RaptureMacroModule.Instance->Individual[item.MacroIndex]));
                        }
                    }
                }

                TimeLastActionUsed = DateTime.Now;
                LastAction = actionID;

                if (cate != null)
                {
                    switch (cate.RowId)
                    {
                        case 2: //魔法
                            LastSpell = actionID;
                            break;
                        case 3: //战技
                            LastWeaponskill = actionID;
                            break;
                        case 4: //能力
                            LastAbility = actionID;
                            break;
                    }
                }


                //事后骂人！
                if (StatusHelper.ActionLocations.TryGetValue(actionID, out var loc)
                    && loc != CustomCombo.FindEnemyLocation(tar)
                    && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.TrueNorth))
                {
                    Service.FlyTextGui.AddFlyText(Dalamud.Game.Gui.FlyText.FlyTextKind.NamedIcon, 0, 0, 0, $"要打{loc.ToName()}", "", ImGui.GetColorU32(new Vector4(0.4f, 0, 0, 1)), action.Icon);
                }
            }
            return GetActionHook.Original.Invoke(actionManager, actionType, actionID, targetID, param, useType, pvp, a7);
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

            //var texts = message.TextValue.Split(' ');
            //if(texts.Length == 2 && texts[0].Contains("AutoAttack"))
            //{
            //    XIVAutoAttackPlugin.DoAutoAttack(texts[1]);
            //}
        }

        public void Dispose()
        {
            PlaySpecificSoundHook?.Dispose();
            LoadSoundFileHook?.Dispose();
            GetResourceSyncHook?.Dispose();
            GetResourceAsyncHook?.Dispose();
            GetActionHook?.Dispose();
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
