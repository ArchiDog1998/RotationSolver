using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Linq;
using XIVAutoAttack.Actions;

namespace XIVAutoAttack.Combos.CustomCombo
{
    internal abstract class JobGaugeCombo<T> : CustomCombo where T : JobGaugeBase
    {
        private static T _gauge;

        public static T JobGauge
        {
            get
            {
                if (_gauge == null)
                {
                    _gauge = Service.JobGauges.Get<T>();
                }
                return _gauge;
            }
        }



        internal struct BreakItems
        {
            internal static readonly BaseItem
                //刚力
                TinctureofStrength6 = new BaseItem(36109, 196625),
                //巧力
                TinctureofDexterity6 = new BaseItem(36110, 65535),
                //意力
                TinctureofMind6 = new BaseItem(36113, 65535),
                //意力
                TinctureofIntelligence6 = new BaseItem(36112, 65535);

        }

        protected bool UseBreakItem(out IAction act)
        {
            act = null;
            if (Service.PartyList.Count() < 8) return false;
            if (Service.ClientState.LocalPlayer.Level < 90) return false;

            Role role = (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == Service.ClientState.LocalPlayer.ClassJob.Id).Role;
            switch (role)
            {
                case Role.近战:
                case Role.防护:
                    if (BreakItems.TinctureofStrength6.ShoudUseItem(out act)) return true;
                    break;
                case Role.远程:
                    if (RangePhysicial.Contains(Service.ClientState.LocalPlayer.ClassJob.Id))
                    {
                        if (BreakItems.TinctureofDexterity6.ShoudUseItem(out act)) return true;
                    }
                    else
                    {
                        if (BreakItems.TinctureofIntelligence6.ShoudUseItem(out act)) return true;
                    }
                    break;
                case Role.治疗:
                    if (BreakItems.TinctureofMind6.ShoudUseItem(out act)) return true;
                    break;
            }
            return false;
        }
    }
}
