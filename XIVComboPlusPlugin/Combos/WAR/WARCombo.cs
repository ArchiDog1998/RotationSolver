using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal abstract class WARCombo : CustomComboJob<WARGauge>
{
    internal static bool HaveShield => BaseAction.FindStatusSelf(ObjectStatus.Defiance) != null;

    internal struct Actions
    {
        public static readonly BaseAction
            //重劈
            HeavySwing = new BaseAction(31),

            //凶残裂
            Maim = new BaseAction(37),

            //暴风斩 绿斧
            StormsPath = new BaseAction(42),

            //暴风碎 红斧
            StormsEye = new BaseAction(45)
            {
                BuffsProvide = new ushort[] { ObjectStatus.SurgingTempest },
            },

            //飞斧
            Tomahawk = new BaseAction(46),

            //猛攻
            Onslaught = new BaseAction(7386),

            //动乱    
            Upheaval = new BaseAction(7387),

            //超压斧
            Overpower = new BaseAction(41),

            //秘银暴风
            MythrilTempest = new BaseAction(16462),

            //群山隆起
            Orogeny = new BaseAction(25752),


            //原初之魂
            InnerBeast = new BaseAction(49),

            //钢铁旋风
            SteelCyclone = new BaseAction(51),

            //战嚎
            Infuriate = new BaseAction(52)
            {
                BuffsProvide = new ushort[] { ObjectStatus.InnerRelease },
                OtherCheck = () => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 3).Length > 0,
            },

            //狂暴
            Berserk = new BaseAction(38)
            {
                OtherCheck = () => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 3).Length > 0,
            },

            //战栗
            ThrillofBattle = new BaseAction(40),

            //泰然自若
            Equilibrium = new BaseAction(3552)
            {
                OtherCheck = () => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6,
            },

            //原初的勇猛
            NascentFlash = new BaseAction(16464),

            ////原初的血气
            //Bloodwhetting = new BaseAction(25751),

            //守护
            Defiance = new BaseAction(48),

            //复仇
            Vengeance = new BaseAction(44)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //原初的直觉
            RawIntuition = new BaseAction(3551)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
            },

            //摆脱
            ShakeItOff = new BaseAction(7388),

            //死斗
            Holmgang = new BaseAction(43)
            {
                OtherCheck = () => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.1,
            },

            ////原初的解放
            //InnerRelease = new BaseAction(7389),

            //蛮荒崩裂
            PrimalRend = new BaseAction(25753)
            {
                BuffsNeed = new ushort[] { ObjectStatus.PrimalRendReady},
            };
    }

    protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {
        uint act;

        if (HaveShield && TargetHelper.ProvokeTarget().Length > 0)
        {
            //挑衅一下？
            if (Actions.Tomahawk.TryUseAction(level, out act)) return act;
            if (GeneralActions.Provoke.TryUseAction(level, out act)) return act;
        }

        //兽魂输出
        //钢铁旋风
        if (Actions.SteelCyclone.TryUseAction(level, out act)) return act;
        //原初之魂
        if (Actions.InnerBeast.TryUseAction(level, out act)) return act;
        //放个大 蛮荒崩裂 会往前飞
        if (Actions.PrimalRend.TryUseAction(level, out act)) return act;

        //群体
        if (Actions.MythrilTempest.TryUseAction(level, out act)) return act;
        if (Actions.Overpower.TryUseAction(level, out act)) return act;

        //单体
        if (Actions.StormsEye.TryUseAction(level, out act)) return act;
        if (Actions.StormsPath.TryUseAction(level, out act)) return act;
        if (Actions.Maim.TryUseAction(level, out act)) return act;
        if (Actions.HeavySwing.TryUseAction(level, out act)) return act;

        return 0;
    }

    protected bool CanAddAbility(byte level, out uint act)
    {
        act = 0;

        if (CanInsertAbility)
        {
            if (!IsMoving && CanAddRampart(level, out act)) return true;

            //爆发
            //狂暴
            if (Actions.Berserk.TryUseAction(level, out act)) return true;
            //战嚎
            if (Actions.Infuriate.TryUseAction(level, out act)) return true;

            //泰然自若 自奶啊！
            if (Actions.Equilibrium.TryUseAction(level, out act)) return true;

            //普通攻击
            //群山隆起
            if (Actions.Orogeny.TryUseAction(level, out act)) return true;
            //动乱 
            if (Actions.Overpower.TryUseAction(level, out act)) return true;

        }
        return false;
    }

    private bool CanAddRampart(byte level, out uint act)
    {
        act = 0;

        //死斗 如果谢不够了。
        if (Actions.Holmgang.TryUseAction(level, out act)) return true;

        //降低伤害
        //复仇（减伤30%）
        if (Actions.Vengeance.TryUseAction(level, out act)) return true;

        //铁壁（减伤20%）
        if (GeneralActions.Rampart.TryUseAction(level, out act)) return true;

        //原初的直觉（减伤10%）
        if (Actions.RawIntuition.TryUseAction(level, out act)) return true;

        //降低攻击
        //雪仇
        if (GeneralActions.Reprisal.TryUseAction(level, out act)) return true;

        //亲疏自行
        if (GeneralActions.ArmsLength.TryUseAction(level, out act)) return true;
        
        //增加血量
        //摆脱 队友套盾
        if (Actions.ShakeItOff.TryUseAction(level, out act)) return true;
        //战栗
        if (Actions.ThrillofBattle.TryUseAction(level, out act)) return true;
        return false;
    }

}
