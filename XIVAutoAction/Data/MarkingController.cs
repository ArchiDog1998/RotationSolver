using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AutoAction.Data
{
    [StructLayout(LayoutKind.Explicit, Size = 688)]
    public struct MarkingController
    {
        [FieldOffset(0)]
        public unsafe IntPtr VTable;
        [FieldOffset(16)]
        public uint Attack1;
        [FieldOffset(24)]
        public uint Attack2;
        [FieldOffset(32)]
        public uint Attack3;
        [FieldOffset(40)]
        public uint Attack4;
        [FieldOffset(48)]
        public uint Attack5;
        [FieldOffset(56)]
        public uint Bind1;
        [FieldOffset(64)]
        public uint Bind2;
        [FieldOffset(72)]
        public uint Bind3;
        [FieldOffset(80)]
        public uint Stop1;
        [FieldOffset(88)]
        public uint Stop2;
        [FieldOffset(96)]
        public uint Square;
        [FieldOffset(104)]
        public uint Circle;
        [FieldOffset(112)]
        public uint Cross;
        [FieldOffset(120)]
        public uint Triangle;

        private static unsafe MarkingController* Instance => (MarkingController*)Service.Address.MarkingController;

        internal static unsafe BattleChara Attack1Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Instance->Attack1);
        internal static unsafe BattleChara Attack2Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Instance->Attack2);
        internal static unsafe BattleChara Attack3Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Instance->Attack3);
        internal static unsafe BattleChara Attack4Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Instance->Attack4);
        internal static unsafe BattleChara Attack5Chara(IEnumerable<BattleChara> charas) => GetChara(charas, Instance->Attack5);

        internal static bool HaveAttackChara(IEnumerable<BattleChara> charas)
        {
            if (Attack1Chara(charas) != null) return true;
            if (Attack2Chara(charas) != null) return true;
            if (Attack3Chara(charas) != null) return true;
            if (Attack4Chara(charas) != null) return true;
            if (Attack5Chara(charas) != null) return true;
            return false;
        }


        private static BattleChara GetChara(IEnumerable<BattleChara> charas, uint id)
        {
            if (id == 0xE0000000) return null;
            return charas.FirstOrDefault(item => item.ObjectId == id);
        }

        internal unsafe static IEnumerable<BattleChara> FilterStopCharaes(IEnumerable<BattleChara> charas)
        {
            List<uint> ids = new List<uint>(2);
            if (Instance->Stop1 != 0xE0000000)
            {
                ids.Add(Instance->Stop1);
            }
            if (Instance->Stop2 != 0xE0000000)
            {
                ids.Add(Instance->Stop2);
            }
            if (ids.Count == 0) return charas;

            return charas.Where(b => !ids.Contains(b.ObjectId));
        }
    }
}
