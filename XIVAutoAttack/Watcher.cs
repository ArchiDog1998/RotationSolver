using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.ComponentModel;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack
{
    internal class Watcher : IDisposable
    {
        private unsafe delegate bool UseActionDelegate(IntPtr actionManager, ActionType actionType, uint actionID, uint targetID, uint param, uint useType, int pvp, bool* isGroundTarget);

        private Hook<UseActionDelegate> _getActionHook { get; set; }
        public bool IsActionHookEnable => _getActionHook?.IsEnabled ?? false;

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
        private static DateTime _timeLastSpeak = DateTime.Now;

        internal unsafe void Enable()
        {
            _getActionHook = Hook<UseActionDelegate>.FromAddress((IntPtr)ActionManager.fpUseAction, UseAction);


            _getActionHook?.Enable();
        }

        public void ChangeActionHook()
        {
            if (_getActionHook == null) return;
            if (_getActionHook.IsEnabled)
            {
                _getActionHook.Disable();
            }
            else
            {
                _getActionHook.Enable();
            }
        }

        private unsafe bool UseAction(IntPtr actionManager, ActionType actionType, uint actionID, uint targetID = 3758096384u, uint param = 0u, uint useType = 0u, int pvp = 0, bool* a7 = null)
        {
            if (actionType == ActionType.Spell && useType == 0)
            {
                var id = ActionManager.Instance()->GetAdjustedActionId(actionID);

#if DEBUG
                var a = actionType == ActionType.Spell ? Service.DataManager.GetExcelSheet<Action>().GetRow(id)?.Name : Service.DataManager.GetExcelSheet<Item>().GetRow(actionID)?.Name;
                Service.ChatGui.Print($"{a} , {actionType} , {id} , {param} , {useType} , {pvp} , {ActionUpdater.WeaponRemain}");
#endif

                var action = Service.DataManager.GetExcelSheet<Action>().GetRow(id);
                var cate = action.ActionCategory.Value;
                var tar = Service.ObjectTable.SearchById(targetID);

                //Macro
                if (id != LastAction)
                {
                    foreach (var item in Service.Configuration.Events)
                    {
                        if (item.Name == action.Name)
                        {
                            if (item.MacroIndex < 0 || item.MacroIndex > 99) break;

                            MacroUpdater.Macros.Enqueue(new MacroItem(tar, item.IsShared ? RaptureMacroModule.Instance->Shared[item.MacroIndex] :
                                RaptureMacroModule.Instance->Individual[item.MacroIndex]));
                        }
                    }
                }

                _timeLastActionUsed = DateTime.Now;
                LastAction = id;

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

                //事后骂人！
                if (DateTime.Now - _timeLastSpeak > new TimeSpan(0,0,0,0,200))
                {
                    _timeLastSpeak = DateTime.Now;
                    if (Service.Configuration.SayoutLocationWrong
                        && StatusHelper.ActionLocations.TryGetValue(id, out var loc)
                        && tar.HasLocationSide()
                        && loc != tar.FindEnemyLocation()
                        && !Service.ClientState.LocalPlayer.HaveStatus(ObjectStatus.TrueNorth))
                    {
                        Service.FlyTextGui.AddFlyText(Dalamud.Game.Gui.FlyText.FlyTextKind.NamedIcon, 0, 0, 0, $"要打{loc.ToName()}", "", ImGui.GetColorU32(new Vector4(0.4f, 0, 0, 1)), action.Icon);
                        if(!string.IsNullOrEmpty(Service.Configuration.LocationText))
                        {
                            CustomCombo.Speak(Service.Configuration.LocationText);
                        }
                    }
                }


            }
            return _getActionHook.Original.Invoke(actionManager, actionType, actionID, targetID, param, useType, pvp, a7);
        }


        public void Dispose()
        {
            _getActionHook?.Dispose();
        }
    }
}
