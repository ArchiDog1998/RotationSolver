using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos.Disciplines
{
    internal class FSHCombo : DisciplinesCombo
    {
        internal override uint JobID => 18;
        internal struct Actions
        {
            public static readonly BaseAction
                //抛竿
                Cast = new BaseAction(289),

                //提钩
                Hook = new BaseAction(296),

                //精准提钩
                PrecisionHookset = new BaseAction(4179),

                //强力提钩
                PowerfulHookset = new BaseAction(4103),

                //沙利亚克的恩宠
                ThaliaksFavor = new BaseAction(26804),

                //耐心
                Patience = new BaseAction(4102)
                {
                    BuffsProvide = new ushort[] { ObjectStatus.Patience },
                },

                //钓组
                Snagging = new BaseAction(4100)
                {

                };

            public static readonly BaseItem
                //强心剂
                Strong = new BaseItem(6141, 65535);
        }
        private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
        {
            act = null;
            return false;
        }

        private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
        {
            var maxgp = Service.ClientState.LocalPlayer.MaxGp;
            var gp = Service.ClientState.LocalPlayer.CurrentGp;
            bool fishing = Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Fishing];

            if (fishing && TargetHelper.Fish != FishType.None)
            {
                if(BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Patience) && gp >= 50)
                {
                    switch (TargetHelper.Fish)
                    {
                        case FishType.Small:
                            if (Actions.PrecisionHookset.ShouldUseAction(out act)) return true;
                            break;
                        case FishType.Medium:
                            if (Actions.PowerfulHookset.ShouldUseAction(out act)) return true;
                            break;
                        case FishType.Large:
                            if (Service.Configuration.UsePowerfulHookset && Actions.PowerfulHookset.ShouldUseAction(out act)) return true;
                            if (Actions.PrecisionHookset.ShouldUseAction(out act)) return true;
                            break;
                    }
                }
                else if (Actions.Hook.ShouldUseAction(out act)) return true;
            }

            if (!fishing)
            {
                var status = Service.ClientState.LocalPlayer.StatusList.Where(s => s.StatusId == ObjectStatus.AnglersArt);
                byte stack = 0;
                if (status != null && status.Count() > 0)
                {
                    stack = status.First().StackCount;
                }

                //补充GP
                if (stack > 2 && maxgp - gp >= 150)
                {
                    if (Actions.ThaliaksFavor.ShouldUseAction(out act)) return true;
                }
                if(maxgp - gp >= 300)
                {
                    if (Actions.Strong.ShoudUseItem(out act)) return true;
                }
                if (gp >= 300)
                {
                    if (Actions.Patience.ShouldUseAction(out act)) return true;
                }

                if (Actions.Cast.ShouldUseAction(out act)) return true;
            }

            act = null;
            return false;
        }
    }
}
