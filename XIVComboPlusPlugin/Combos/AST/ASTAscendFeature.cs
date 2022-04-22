using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.AST
{
    internal class ASTAscendFeature : ASTCombo
    {
        public override string ComboFancyName => "占星拉人";

        public override string Description => "替换即刻咏唱为拉人。";

        protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Swiftcast.ActionID};

        private protected override bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act, byte actabilityRemain)
        {
            //有人死了，看看马上救。
            if (TargetHelper.DeathPeopleParty.Length > 0)
            {
                if (Actions.Ascend.TryUseAction(level, out act, mustUse: true)) return true;
            }
            if (GeneralActions.Esuna.TryUseAction(level, out act)) return true;

            act = null;
            return false;
        }
    }
}
