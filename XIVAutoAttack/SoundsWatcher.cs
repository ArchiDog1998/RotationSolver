using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.System.Resource;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;

namespace XIVAutoAttack
{
    internal class SoundsWatcher : IDisposable
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

        private delegate IntPtr LoadSoundFileDelegate(IntPtr resourceHandle, uint a2);

        private const int ResourceDataPointerOffset = 176;

        private const int MusicManagerStreamingOffset = 50;

        private Hook<PlaySpecificSoundDelegate> PlaySpecificSoundHook { get; set; }

        private Hook<GetResourceSyncPrototype> GetResourceSyncHook { get; set; }

        private Hook<GetResourceAsyncPrototype> GetResourceAsyncHook { get; set; }

        private Hook<LoadSoundFileDelegate> LoadSoundFileHook { get; set; }

        internal ConcurrentDictionary<IntPtr, FishType> Scds { get; } = new ConcurrentDictionary<IntPtr, FishType>();


        internal unsafe void Enable()
        {
            if (PlaySpecificSoundHook == null && Service.SigScanner.TryScanText("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 33 F6 8B DA 48 8B F9 0F BA E2 0F", out var playPtr))
            {
                PlaySpecificSoundHook = new Hook<PlaySpecificSoundDelegate>(playPtr, PlaySpecificSoundDetour);
            }
            if (GetResourceSyncHook == null && Service.SigScanner.TryScanText("E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 48 89 87 ?? ?? ?? ?? 48 8D 54 24", out var syncPtr))
            {
                GetResourceSyncHook = new Hook<GetResourceSyncPrototype>(syncPtr, GetResourceSyncDetour);
            }
            if (GetResourceAsyncHook == null && Service.SigScanner.TryScanText("E8 ?? ?? ?? ?? 48 8B D8 EB 07 F0 FF 83", out var asyncPtr))
            {
                GetResourceAsyncHook = new Hook<GetResourceAsyncPrototype>(asyncPtr, GetResourceAsyncDetour);
            }
            if (LoadSoundFileHook == null && Service.SigScanner.TryScanText("E8 ?? ?? ?? ?? 48 85 C0 75 04 B0 F6", out var soundPtr))
            {
                LoadSoundFileHook = new Hook<LoadSoundFileDelegate>(soundPtr, LoadSoundFileDetour);
            }
            PlaySpecificSoundHook?.Enable();
            LoadSoundFileHook?.Enable();
            GetResourceSyncHook?.Enable();
            GetResourceAsyncHook?.Enable();
        }

        internal void Disable()
        {
            PlaySpecificSoundHook?.Disable();
            LoadSoundFileHook?.Disable();
            GetResourceSyncHook?.Disable();
            GetResourceAsyncHook?.Disable();
        }

        public void Dispose()
        {
            PlaySpecificSoundHook?.Dispose();
            LoadSoundFileHook?.Dispose();
            GetResourceSyncHook?.Dispose();
            GetResourceAsyncHook?.Dispose();
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
            catch (Exception ex)
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
            if (name.Contains("sound/battle/live/se_craft_fsh_m_hit_s.scd")) return FishType.Small;
            if (name.Contains("sound/battle/live/se_craft_fsh_m_hit_m.scd")) return FishType.Medium;
            if (name.Contains("sound/battle/live/se_craft_fsh_m_hit_l.scd")) return FishType.Large;
            return FishType.None;
        }
    }

    internal enum FishType : byte
    {
        None,
        Small,
        Medium,
        Large,
    }
}
