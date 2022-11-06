using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.Melee;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.SigReplacers
{
    public static class Watcher
    {
        public record ActionRec(DateTime UsedTime, Action action);

        private delegate void ReceiveAbiltyDelegate(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);

        private static Hook<ReceiveAbiltyDelegate> _receivAbilityHook;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static uint LastAction { get; set; } = 0;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static uint LastWeaponskill { get; set; } = 0;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static uint LastAbility { get; set; } = 0;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static uint LastSpell { get; set; } = 0;
        internal static TimeSpan TimeSinceLastAction => DateTime.Now - _timeLastActionUsed;

        private static DateTime _timeLastActionUsed = DateTime.Now;

        const int QUEUECAPACITY = 64;
        private static Queue<ActionRec> _actions = new Queue<ActionRec>(QUEUECAPACITY);

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static ActionRec[] RecordActions => _actions.Reverse().ToArray();

        internal static unsafe void Enable()
        {
            _receivAbilityHook = Hook<ReceiveAbiltyDelegate>.FromAddress(Service.Address.ReceiveAbilty, ReceiveAbilityEffect);

            _receivAbilityHook?.Enable();
        }


        private static void ReceiveAbilityEffect(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
        {
            _receivAbilityHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);

            //不是自己放出来的
            if (Service.ClientState.LocalPlayer == null || sourceId != Service.ClientState.LocalPlayer.ObjectId) return;

            //不是一个Spell
            if (Marshal.ReadByte(effectHeader, 31) != 1) return;

            //获得行为
            var action = Service.DataManager.GetExcelSheet<Action>().GetRow((uint)Marshal.ReadInt32(effectHeader, 0x8));

            //获得目标
            var tar = Service.ObjectTable.SearchById((uint)Marshal.ReadInt32(effectHeader)) ?? Service.ClientState.LocalPlayer;

            //获得身为技能是否正确flag
            var flag = Marshal.ReadByte(effectArray + 3);
            RecordAction(tar, action,flag);
        }

        private static unsafe void RecordAction(GameObject tar, Action action,byte flag)
        {
            var id = action.RowId;

            //Record
            switch (action.GetActinoType())
            {
                case ActionCate.Spell: //魔法
                    LastSpell = id;
                    break;
                case ActionCate.Weaponskill: //战技
                    LastWeaponskill = id;
                    break;
                case ActionCate.Ability: //能力
                    LastAbility = id;
                    break;
                default:
                    return;
            }
            _timeLastActionUsed = DateTime.Now;
            LastAction = id;

            if(_actions.Count >= QUEUECAPACITY)
            {
                _actions.Dequeue();
            }
            _actions.Enqueue(new ActionRec(_timeLastActionUsed, action));


            //Macro
            foreach (var item in Service.Configuration.Events)
            {
                if (item.Name != action.Name) continue;
                if (item.MacroIndex < 0 || item.MacroIndex > 99) break;

                MacroUpdater.Macros.Enqueue(new MacroItem(tar, item.IsShared ? RaptureMacroModule.Instance->Shared[item.MacroIndex] :
                    RaptureMacroModule.Instance->Individual[item.MacroIndex]));
            }

            //事后骂人！
            if (Service.Configuration.SayoutLocationWrong
                && StatusHelper.ActionLocations.TryGetValue(id, out var loc)
                && (flag != 0 || NINCombo.Actions.TrickAttack.ID == id && loc != tar.FindEnemyLocation() && tar.HasLocationSide())
                && !Service.ClientState.LocalPlayer.HaveStatus(ObjectStatus.TrueNorth))
            {
                Service.FlyTextGui.AddFlyText(Dalamud.Game.Gui.FlyText.FlyTextKind.NamedIcon, 0, 0, 0, $"要打{loc.ToName()}", "", ImGui.GetColorU32(new Vector4(0.4f, 0, 0, 1)), action.Icon);
                if (!string.IsNullOrEmpty(Service.Configuration.LocationText))
                {
                    CustomCombo.Speak(Service.Configuration.LocationText);
                }
            }
        }

        public static void Dispose()
        {
            _receivAbilityHook?.Dispose();
        }
    }
}
