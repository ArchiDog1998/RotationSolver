using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus.Combos.WHM
{
    internal class WHM_Feature : WHMCombo
    {
        public override string ComboFancyName => "白魔GCD";

        public override string Description => "一键自动攻击，还有加辅助技能。";

        protected internal override uint[] ActionIDs => new uint[] {Actions.Stone.ActionID};

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
        {
            uint act;

            //有人要死了！这很刺激！
            if (UseBenediction(out _) && Actions.Benediction.TryUseAction(level, out act, mustUse: true)) return act;

            if (TargetHelper.DyingPeople.Length > 0)
            {
                if(GeneralActions.Esuna.TryUseAction(level, out act, mustUse: true)) return act;
            }

            if (TargetHelper.DeathPeopleParty.Length != 0)
            {
                //如果有人倒了，赶紧即刻拉人！
                if (GeneralActions.Swiftcast.TryUseAction(level, out act, mustUse: true)) return act;
                if (Actions.ThinAir.TryUseAction(level, out act, mustUse: true)) return act;

                bool haveSwift = false;
                foreach (var status in Service.ClientState.LocalPlayer.StatusList)
                {
                    if (GeneralActions.Swiftcast.BuffsProvide.Contains((ushort)status.StatusId))
                    {
                        haveSwift = true;
                        break;
                    }
                }
                if (haveSwift && Actions.Raise.TryUseAction(level, out act)) return act;
            }

            if (IsMoving && HaveTargetAngle)
            {
                //苦难之心
                if (Actions.AfflatusMisery.OtherCheck() && Actions.AfflatusMisery.TryUseAction(level, out act, mustUse: true)) return act;

                if (Actions.Aero.TryUseAction(level, out act, mustUse:true)) return act;
            }

            //如果现在可以增加能力技
            if (CanAddAbility(level, out act)) return act;

            //苦难之心
            if (Actions.AfflatusMisery.OtherCheck() && Actions.AfflatusMisery.TryUseAction(level, out act, mustUse :true)) return act;

            //群体输出
            if (Actions.Holy.TryUseAction(level, out act)) return act;

            //单体输出
            if (Actions.Aero.TryUseAction(level, out act)) return act;
            if (Actions.Stone.TryUseAction(level, out act)) return act;

            return actionID;
        }
    }
}
