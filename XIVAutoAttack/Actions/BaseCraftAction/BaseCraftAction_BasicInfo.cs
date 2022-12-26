using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack.Actions.BaseCraftAction
{
    internal partial class BaseCraftAction : IAction
    {
        internal StatusID[] BuffsProvide { get; set; } = null;

        public uint ID => _crp;

        public uint AdjustedID
        {
            get
            {
                var player = Service.ClientState.LocalPlayer;
                if (player == null) return _crp;

                return player.ClassJob.Id switch
                {
                    8 => _crp,
                    9 => _bsm,
                    10 => _arm,
                    11 => _gsm,
                    12 => _ltw,
                    13 => _wvr,
                    14 => _alc,
                    15 => _cul,
                    _ => _crp,
                };
            }
        }

        internal unsafe uint CPCost => (uint)ActionManager.GetActionCost(ActionType.Spell, AdjustedID, 0, 0, 0, 0);

        public bool IsEnabled
        {
            get => !Service.Configuration.DiabledActions.Contains((uint)ID);
            set
            {
                if (value)
                {
                    Service.Configuration.DiabledActions.Remove((uint)ID);
                }
                else
                {
                    Service.Configuration.DiabledActions.Add((uint)ID);
                }
            }
        }
        public uint IconID { get; }

        public string Name { get; }

        uint _crp, _bsm, _arm, _gsm, _ltw, _wvr, _alc, _cul;
        ActionType Type => _crp > 100000 ? ActionType.CraftAction : ActionType.Spell;

        public string CateName => "Cate";

        public BaseCraftAction(uint CRPActionId, Func<byte, double> baseMultiply = null)
        {
            GetBaseMultiply = baseMultiply ?? (level => 0);

            if (CRPActionId > 100000)
            {
                var craftAction = Service.DataManager.GetExcelSheet<CraftAction>().GetRow(_crp);
                _crp = craftAction.CRP.Row;
                _bsm = craftAction.BSM.Row;
                _arm = craftAction.ARM.Row;
                _gsm = craftAction.GSM.Row;
                _ltw = craftAction.LTW.Row;
                _wvr = craftAction.WVR.Row;
                _alc = craftAction.ALC.Row;
                _cul = craftAction.CUL.Row;

                Name = craftAction.Name;
                IconID = craftAction.Icon;
            }
            else
            {
                var action = Service.DataManager.GetExcelSheet<Action>().GetRow(_crp);
                _crp = CRPActionId;
                _bsm = CRPActionId + 1;
                _arm = CRPActionId + 2;
                _gsm = CRPActionId + 3;
                _ltw = CRPActionId + 4;
                _wvr = CRPActionId + 5;
                _alc = CRPActionId + 6;
                _cul = CRPActionId + 7;

                Name = action.Name;
                IconID = action.Icon;
            }
        }

        public unsafe bool ShouldUse(out IAction act)
        {
            act = this;

            var player = Service.ClientState.LocalPlayer;
            if (player == null) return false;

            if (player.HasStatus(false, BuffsProvide)) return false;

            var actionManager = ActionManager.Instance();
            if ((IntPtr)actionManager == IntPtr.Zero) return false;

            if (actionManager->GetActionStatus(Type, ID) != 0) return false;

            return true;
        }

        public unsafe bool Use()
        {
            var actionManager = ActionManager.Instance();
            if ((IntPtr)actionManager == IntPtr.Zero) return false;

            return actionManager->UseAction(Type, ID);
        }
    }
}
