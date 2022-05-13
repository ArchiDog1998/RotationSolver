using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal class SGECombo : CustomComboJob<SGEGauge>
{
    internal override uint JobID => 40;

    private protected override BaseAction Raise => new BaseAction(24287);

    internal struct Actions
    {
        public static readonly BaseAction
            //注药
            Dosis = new BaseAction(24283),

            //发炎
            Phlegma = new BaseAction(24289),

            //诊断
            Diagnosis = new BaseAction(24284, true),

            //心关
            Kardia = new BaseAction(24285, true)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Kardia},
                ChoiceFriend = Targets =>
                {
                    Targets = Targets.Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();

                    var targets = TargetHelper.GetJobCategory(Targets, Role.防护);
                    targets = targets.Length == 0 ? Targets : targets;

                    if (targets.Length == 0) return null;

                    foreach (var tar in targets)
                    {
                        if (tar.TargetObject?.TargetObject?.ObjectId == tar.ObjectId)
                        {
                            return tar;
                        }
                    }

                    return ASTCombo.RandomObject(targets);
                },
                OtherCheck = b => BaseAction.FindStatusFromSelf(b, ObjectStatus.Kardion).Length == 0,
            },

            //预后
            Prognosis = new BaseAction(24286, true),

            //自生
            Physis = new BaseAction(24288, true),

            //自生2
            Physis2 = new BaseAction(24302, true),

            //均衡
            Eukrasia = new BaseAction(24290)
            {
                OtherCheck = b => !JobGauge.Eukrasia,
            },

            //拯救
            Soteria = new BaseAction(24294, true),

            //神翼
            Icarus = new BaseAction(24295, shouldEndSpecial: true)
            {
                ChoiceFriend = BaseAction.FindMoveTarget,
            },

            //灵橡清汁
            Druochole = new BaseAction(24296, true),

            //失衡
            Dyskrasia = new BaseAction(24297),

            //坚角清汁
            Kerachole = new BaseAction(24298, true),

            //寄生清汁
            Ixochole = new BaseAction(24299, true),

            //活化
            Zoe = new BaseAction(24300),

            //白牛清汁
            Taurochole = new BaseAction(24303, true)
            {
                ChoiceFriend = BaseAction.FindBeAttacked,
            },

            //箭毒
            Toxikon = new BaseAction(24304),

            //输血
            Haima = new BaseAction(24305, true)
            {
                ChoiceFriend = BaseAction.FindBeAttacked,
            },

            //根素
            Rhizomata = new BaseAction(24309),

            //整体论
            Holos = new BaseAction(24310, true),

            //泛输血
            Panhaima = new BaseAction(24311, true),

            //混合
            Krasis = new BaseAction(24317, true),

            //魂灵风息
            Pneuma = new BaseAction(24318);
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (nextGCD.ActionID == Actions.Diagnosis.ActionID ||
            nextGCD.ActionID == Actions.Prognosis.ActionID)
        {
            //活化
            if (Actions.Zoe.ShouldUseAction(out act)) return true;
        }

        if (nextGCD.ActionID == Actions.Diagnosis.ActionID)
        {
            //混合
            if (Actions.Krasis.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;

    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        //输血
        if (Actions.Haima.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //白牛清汁
            if (Actions.Taurochole.ShouldUseAction(out act)) return true;
        }

        //均衡
        if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

        //诊断
        if (Actions.Diagnosis.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //泛输血
        if (Actions.Panhaima.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //坚角清汁
            if (Actions.Kerachole.ShouldUseAction(out act)) return true;
        }

        //均衡
        if (Actions.Eukrasia.ShouldUseAction(out act)) return true;

        //预后
        if (Actions.Prognosis.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out BaseAction act)
    {

        if(JobGauge.Addersgall > 1)
        {
            //灵橡清汁
            if (Actions.Druochole.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        //神翼
        if (Actions.Icarus.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool GeneralAbility(byte abilityRemain, out BaseAction act)
    {
        //心关
        if (Actions.Kardia.ShouldUseAction(out act)) return true;

        //根素
        if (JobGauge.Addersgall < 2 && Actions.Rhizomata.ShouldUseAction(out act)) return true;

        foreach (var friend in TargetHelper.PartyMembers)
        {
            var statuses = friend.StatusList.Select(status => status.StatusId);
            if (statuses.Contains(ObjectStatus.Kardion))
            {
                if ((float)friend.CurrentHp/friend.MaxHp < 0.7)
                {
                    Actions.Soteria.ShouldUseAction(out act);
                    return true;
                }
                break;
            }
        }

        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //魂灵风息
        if (Actions.Pneuma.ShouldUseAction(out act, mustUse:true)) return true;

        if (JobGauge.Addersting > 0)
        {
            //箭毒
            if (Actions.Toxikon.ShouldUseAction(out act, mustUse:true)) return true;
        }

        //发炎
        if (Actions.Phlegma.ShouldUseAction(out act)) return true;

        //失衡
        if (Actions.Dyskrasia.ShouldUseAction(out act)) return true;

        Actions.Dosis.ShouldUseAction(out _);
        var times = BaseAction.FindStatusFromSelf(Actions.Dosis.Target,
            new ushort[] { ObjectStatus.EukrasianDosis, ObjectStatus.EukrasianDosis2, ObjectStatus.EukrasianDosis3 });
        if (times.Length == 0 || times.Max() < 3)
        {
            //发炎
            if (Actions.Eukrasia.ShouldUseAction(out act)) return true;
        }
        //注药
        if (Actions.Dosis.ShouldUseAction(out act)) return true;

        return false;
    }
    private protected override bool HealSingleGCD(uint lastComboActionID, out BaseAction act)
    {
        //诊断
        if (Actions.Diagnosis.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool HealAreaGCD(uint lastComboActionID, out BaseAction act)
    {
        //预后
        if (Actions.Prognosis.ShouldUseAction(out act)) return true;
        return false;
    }
    private protected override bool HealAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //整体论
        if (Actions.Holos.ShouldUseAction(out act)) return true;

        if (JobGauge.Addersgall > 0)
        {
            //寄生清汁
            if (Actions.Ixochole.ShouldUseAction(out act)) return true;
        }
        //自生2
        if (Actions.Physis2.ShouldUseAction(out act)) return true;
        //自生
        if (Actions.Physis.ShouldUseAction(out act)) return true;
        return false;
    }
}
