using System.Linq;
using XIVAutoAttack.Actions;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public abstract partial class CustomCombo
    {

        internal struct GeneralActions
        {
            internal static readonly PVEAction
                //昏乱
                Addle = new PVEAction(7560u)
                {
                    TargetStatus = new ushort[] { 1203 },
                },

                //即刻咏唱
                Swiftcast = new PVEAction(7561u)
                {
                    BuffsProvide = new ushort[]
                    {
                    ObjectStatus.Swiftcast1,
                    ObjectStatus.Swiftcast2,
                    ObjectStatus.Swiftcast3,
                    ObjectStatus.Triplecast,
                    ObjectStatus.Dualcast,
                    }
                },

                //康复
                Esuna = new PVEAction(7568)
                {
                    ChoiceTarget = (tars) =>
                    {
                        if (TargetHelper.DyingPeople.Length > 0)
                        {
                            return TargetHelper.DyingPeople.OrderBy(b => TargetFilter.DistanceToPlayer(b)).First();
                        }
                        else if (TargetHelper.WeakenPeople.Length > 0)
                        {
                            return TargetHelper.WeakenPeople.OrderBy(b => TargetFilter.DistanceToPlayer(b)).First();
                        }
                        return null;
                    },
                },

                //营救
                Rescue = new PVEAction(7571),

                //沉静
                Repose = new PVEAction(16560),

                //醒梦（如果MP低于6000那么使用）
                LucidDreaming = new PVEAction(7562u)
                {
                    OtherCheck = b => Service.ClientState.LocalPlayer.CurrentMp < 6000,
                },

                //内丹
                SecondWind = new PVEAction(7541)
                {
                    OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.2,
                },

                //亲疏自行
                ArmsLength = new PVEAction(7548, shouldEndSpecial: true),

                //铁壁
                Rampart = new PVEAction(7531, true)
                {
                    BuffsProvide = new ushort[]
                    {
                    ObjectStatus.Holmgang, ObjectStatus.WillDead, ObjectStatus.WalkingDead, ObjectStatus.Superbolide, ObjectStatus.HallowedGround,
                    ObjectStatus.Rampart1, ObjectStatus.Rampart2, ObjectStatus.Rampart3,
                    //原初的直觉和血气
                    ObjectStatus.RawIntuition, ObjectStatus.Bloodwhetting,
                    //复仇
                    ObjectStatus.Vengeance,
                    //预警
                    ObjectStatus.Sentinel,
                    //暗影墙
                    ObjectStatus.ShadowWall, ObjectStatus.DarkMind,
                    //伪装
                    ObjectStatus.Camouflage, ObjectStatus.Nebula, ObjectStatus.HeartofStone,
                    },
                    OtherCheck = PVEAction.TankDefenseSelf,
                },

                //挑衅
                Provoke = new PVEAction(7533)
                {
                    FilterForTarget = b => TargetFilter.ProvokeTarget(b),
                },

                //雪仇
                Reprisal = new PVEAction(7535),

                //退避
                Shirk = new PVEAction(7537, true)
                {
                    ChoiceTarget = friends =>
                    {
                        var tanks = TargetFilter.GetJobCategory(friends, Role.防护);
                        if (tanks == null || tanks.Length == 0) return null;
                        return tanks[0];
                    },
                },

                //浴血
                Bloodbath = new PVEAction(7542)
                {
                    OtherCheck = SecondWind.OtherCheck,
                },

                //牵制
                Feint = new PVEAction(7549)
                {
                    TargetStatus = new ushort[] { 1195 },
                },

                //插言
                Interject = new PVEAction(7538),

                //下踢
                LowBlow = new PVEAction(7540),

                //扫腿
                LegSweep = new PVEAction(7863),

                //伤头
                HeadGraze = new PVEAction(7551),

                //沉稳咏唱
                Surecast = new PVEAction(7559, shouldEndSpecial: true),

                //真北
                TrueNorth = new PVEAction(7546, shouldEndSpecial: true)
                {
                    BuffsProvide = new ushort[] { ObjectStatus.TrueNorth },
                };

        }
        private protected virtual PVEAction Raise => null;
        private protected virtual PVEAction Shield => null;
    }
}
