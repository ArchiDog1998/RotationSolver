using System;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.CustomCombo
{
    internal class CustomComboActions
    {
        /// <summary>
        /// 昏乱
        /// </summary>
        public static BaseAction Addle { get; } = new BaseAction(ActionID.Addle)
        {
            OtherCheck = b => !b.HasStatus(false, StatusID.Addle),
        };

        /// <summary>
        /// 即刻咏唱
        /// </summary>
        public static BaseAction Swiftcast { get; } = new BaseAction(ActionID.Swiftcast)
        {
            BuffsProvide = new StatusID[]
                {
                    StatusID.Swiftcast1,
                    StatusID.Swiftcast2,
                    StatusID.Swiftcast3,
                    StatusID.Triplecast,
                    StatusID.Dualcast,
                }
        };

        /// <summary>
        /// 康复
        /// </summary>
        public static BaseAction Esuna { get; } = new BaseAction(ActionID.Esuna)
        {
            ChoiceTarget = (tars) =>
            {
                if (TargetUpdater.DyingPeople.Length > 0)
                {
                    return TargetUpdater.DyingPeople.OrderBy(b => TargetFilter.DistanceToPlayer(b)).First();
                }
                else if (TargetUpdater.WeakenPeople.Length > 0)
                {
                    return TargetUpdater.WeakenPeople.OrderBy(b => TargetFilter.DistanceToPlayer(b)).First();
                }
                return null;
            },
        };

        /// <summary>
        /// 营救
        /// </summary>
        public static BaseAction Rescue { get; } = new BaseAction(ActionID.Rescue);

        /// <summary>
        /// 沉静
        /// </summary>
        public static BaseAction Repose { get; } = new BaseAction(ActionID.Repose);

        /// <summary>
        /// 醒梦（如果MP低于6000那么使用）
        /// </summary>
        public static BaseAction LucidDreaming { get; } = new BaseAction(ActionID.LucidDreaming)
        {
            OtherCheck = b => Service.ClientState.LocalPlayer.CurrentMp < 6000,
        };

        /// <summary>
        /// 内丹
        /// </summary>
        public static BaseAction SecondWind { get; } = new BaseAction(ActionID.SecondWind)
        {
            OtherCheck = b => Service.ClientState.LocalPlayer?.GetHealthRatio() < Service.Configuration.HealthSingleAbility,
        };

        /// <summary>
        /// 亲疏自行
        /// </summary>
        public static BaseAction ArmsLength { get; } = new BaseAction(ActionID.ArmsLength, shouldEndSpecial: true);

        /// <summary>
        /// 铁壁
        /// </summary>
        public static BaseAction Rampart { get; } = new BaseAction(ActionID.Rampart, true)
        {
            BuffsProvide = new StatusID[]
              {
                    StatusID.Holmgang, StatusID.WillDead, StatusID.WalkingDead, StatusID.Superbolide, StatusID.HallowedGround,
                    StatusID.Rampart1, StatusID.Rampart2, StatusID.Rampart3,
                    //原初的直觉和血气
                    StatusID.RawIntuition, StatusID.Bloodwhetting,
                    //复仇
                    StatusID.Vengeance,
                    //预警
                    StatusID.Sentinel,
                    //暗影墙
                    StatusID.ShadowWall,
                    //星云
                    StatusID.Nebula,
              },
            OtherCheck = BaseAction.TankDefenseSelf,
        };

        /// <summary>
        /// 挑衅
        /// </summary>
        public static BaseAction Provoke { get; } = new BaseAction(ActionID.Provoke)
        {
            FilterForTarget = b => TargetFilter.ProvokeTarget(b),
        };

        /// <summary>
        /// 雪仇
        /// </summary>
        public static BaseAction Reprisal { get; } = new BaseAction(ActionID.Reprisal);

        /// <summary>
        /// 退避
        /// </summary>
        public static BaseAction Shirk { get; } = new BaseAction(ActionID.Shirk, true)
        {
            ChoiceTarget = friends =>
            {
                var tanks = TargetFilter.GetJobCategory(friends, JobRole.Tank);
                if (tanks == null || tanks.Length == 0) return null;
                return tanks[0];
            },
        };

        /// <summary>
        /// 浴血
        /// </summary>
        public static BaseAction Bloodbath { get; } = new BaseAction(ActionID.Bloodbath)
        {
            OtherCheck = SecondWind.OtherCheck,
        };

        /// <summary>
        /// 牵制
        /// </summary>
        public static BaseAction Feint { get; } = new BaseAction(ActionID.Feint)
        {
            OtherCheck = b => !b.HasStatus(false, StatusID.Feint),
        };

        /// <summary>
        /// 插言
        /// </summary>
        public static BaseAction Interject { get; } = new BaseAction(ActionID.Interject);

        /// <summary>
        /// 下踢
        /// </summary>
        public static BaseAction LowBlow { get; } = new BaseAction(ActionID.LowBlow)
        {
            OtherCheck = b => !b.IsBoss() && !MovingUpdater.IsMoving,
        };

        /// <summary>
        /// 扫腿
        /// </summary>
        public static BaseAction LegSweep { get; } = new BaseAction(ActionID.LegSweep);

        /// <summary>
        /// 伤头
        /// </summary>
        public static BaseAction HeadGraze { get; } = new BaseAction(ActionID.HeadGraze);

        /// <summary>
        /// 沉稳咏唱
        /// </summary>
        public static BaseAction Surecast { get; } = new BaseAction(ActionID.Surecast, shouldEndSpecial: true);

        /// <summary>
        /// 真北
        /// </summary>
        public static BaseAction TrueNorth { get; } = new BaseAction(ActionID.TrueNorth, shouldEndSpecial: true)
        {
            BuffsProvide = new StatusID[] { StatusID.TrueNorth },
        };

        /// <summary>
        /// 速行
        /// </summary>
        public static BaseAction Peloton { get; } = new BaseAction(ActionID.Peloton, shouldEndSpecial: true)
        {
            BuffsProvide = new StatusID[] { StatusID.Peloton },
        };

        private protected virtual BaseAction Raise => null;
        private protected virtual BaseAction Shield => null;
    }
}
