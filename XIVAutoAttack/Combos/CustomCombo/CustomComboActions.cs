using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiScene;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public abstract partial class CustomCombo
    {

        internal struct GeneralActions
        {
            internal static readonly BaseAction
                //昏乱
                Addle = new BaseAction(7560u)
                {
                    TargetStatus = new ushort[] { 1203 },
                },

                //即刻咏唱
                Swiftcast = new BaseAction(7561u)
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
                Esuna = new BaseAction(7568)
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
                Rescue = new BaseAction(7571),

                //沉静
                Repose = new BaseAction(16560),

                //醒梦（如果MP低于6000那么使用）
                LucidDreaming = new BaseAction(7562u)
                {
                    OtherCheck = b => Service.ClientState.LocalPlayer.CurrentMp < 6000,
                },

                ////伤腿
                //LegGraze = new BaseAction(7554)
                //{
                //    BuffsProvide = new ushort[]
                //    {
                //        13, 564, 1345,
                //    },
                //    OtherCheck = b => TargetHelper.InBattle,
                //},

                //内丹
                SecondWind = new BaseAction(7541)
                {
                    OtherCheck = b => (float)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.2,
                },

                //亲疏自行
                ArmsLength = new BaseAction(7548, shouldEndSpecial: true),

                //铁壁
                Rampart = new BaseAction(7531, true)
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
                },

                //挑衅
                Provoke = new BaseAction(7533)
                {
                    FilterForTarget = b => TargetFilter.ProvokeTarget(b),
                },

                //雪仇
                Reprisal = new BaseAction(7535),

                //退避
                Shirk = new BaseAction(7537, true)
                {
                    ChoiceTarget = friends =>
                    {
                        var tanks = TargetFilter.GetJobCategory(friends, Role.防护);
                        if (tanks == null || tanks.Length == 0) return null;
                        return tanks[0];
                    },
                },

                //浴血
                Bloodbath = new BaseAction(7542)
                {
                    OtherCheck = SecondWind.OtherCheck,
                },

                //牵制
                Feint = new BaseAction(7549)
                {
                    TargetStatus = new ushort[] { 1195 },
                },

                //插言
                Interject = new BaseAction(7538),

                //下踢
                LowBlow = new BaseAction(7540),

                //扫腿
                LegSweep = new BaseAction(7863),

                //伤头
                HeadGraze = new BaseAction(7551),

                //沉稳咏唱
                Surecast = new BaseAction(7559, shouldEndSpecial: true),

                //真北
                TrueNorth = new BaseAction(7546, shouldEndSpecial: true)
                {
                    BuffsProvide = new ushort[] { ObjectStatus.TrueNorth },
                };

        }
        private protected virtual BaseAction Raise => null;
        private protected virtual BaseAction Shield => null;
    }
}
