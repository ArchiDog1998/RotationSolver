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
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.SigReplacers
{
    internal static class Watcher
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
            var targetId = Marshal.ReadInt32(effectHeader);
            var action = Marshal.ReadInt32(effectHeader, 0x8);
            var type = Marshal.ReadByte(effectHeader, 31);
            _receivAbilityHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);

            //if (DalamudApi.ClientState.LocalPlayer == null) return;
            if (type != 1 || action == 7 || sourceId != Service.ClientState.LocalPlayer.ObjectId) return;
            RecordAction((uint)targetId, (uint)action, type);
        }

        private static DateTime _timeLastSpeak = DateTime.Now;
        private static unsafe void RecordAction(uint targetId, uint id, byte type)
        {
            if (type != 1) return;

            var action = Service.DataManager.GetExcelSheet<Action>().GetRow(id);
            var cate = action.ActionCategory.Value;
            var tar = Service.ObjectTable.SearchById(targetId);

            //Record
            _timeLastActionUsed = DateTime.Now;
            LastAction = id;

            if(_actions.Count >= QUEUECAPACITY)
            {
                _actions.Dequeue();
            }
            _actions.Enqueue(new ActionRec(_timeLastActionUsed, action));

            if (cate != null)
            {
                switch (cate.RowId)
                {
                    case 2: //魔法
                        LastSpell = id;
                        break;
                    case 3: //战技
                        LastWeaponskill = id;
                        break;
                    case 4: //能力
                        LastAbility = id;
                        break;
                }
            }

#if DEBUG
            Service.ChatGui.Print($"{action.Name}, {tar.Name}, {ActionUpdater.WeaponRemain}");
#endif
            //Macro
            foreach (var item in Service.Configuration.Events)
            {
                if (item.Name == action.Name)
                {
                    if (item.MacroIndex < 0 || item.MacroIndex > 99) break;

                    MacroUpdater.Macros.Enqueue(new MacroItem(tar, item.IsShared ? RaptureMacroModule.Instance->Shared[item.MacroIndex] :
                        RaptureMacroModule.Instance->Individual[item.MacroIndex]));
                }
            }


            //事后骂人！
            if (DateTime.Now - _timeLastSpeak > new TimeSpan(0, 0, 0, 0, 200))
            {
                _timeLastSpeak = DateTime.Now;
                if (Service.Configuration.SayoutLocationWrong
                    && StatusHelper.ActionLocations.TryGetValue(id, out var loc)
                    && tar.HasLocationSide()
                    && loc != tar.FindEnemyLocation()
                    && !Service.ClientState.LocalPlayer.HaveStatus(ObjectStatus.TrueNorth))
                {
                    Service.FlyTextGui.AddFlyText(Dalamud.Game.Gui.FlyText.FlyTextKind.NamedIcon, 0, 0, 0, $"要打{loc.ToName()}", "", ImGui.GetColorU32(new Vector4(0.4f, 0, 0, 1)), action.Icon);
                    if (!string.IsNullOrEmpty(Service.Configuration.LocationText))
                    {
                        CustomCombo.Speak(Service.Configuration.LocationText);
                    }
                }
            }
        }

        public static void Dispose()
        {
            _receivAbilityHook?.Dispose();
        }
    }
}
