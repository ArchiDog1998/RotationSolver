using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal abstract class WHMCombo : CustomComboJob<WHMGauge>
{

    internal struct Actions
    {
        private static readonly float hotElement = 0.5f;

        public static readonly BaseAction
            //飞石 平A
            Stone = new BaseAction(119),

            //疾风 Dot
            Aero = new BaseAction(121)
            {
                TargetStatus = new ushort[]
                {
                    ObjectStatus.Aero,
                    ObjectStatus.Aero2,
                    ObjectStatus.Dia,
                }
            },
            //苦难之心
            AfflatusMisery = new BaseAction(16535)
            {
                OtherCheck = () => JobGauge.BloodLily == 3,
            },
            //神圣
            Holy = new BaseAction(139),

            //治疗
            Cure = new BaseAction(120, true),
            //救疗
            Cure2 = new BaseAction(135, true),
            //神名
            Tetragrammaton = new BaseAction(3570, true)
            {
                OtherCheck = () =>
                {
                    foreach (float rate in TargetHelper.PartyMembersHP)
                    {
                        if (rate < 0.7) return true;
                    }
                    return false;
                },
            },
            //安慰之心 800
            AfflatusSolace = new BaseAction(16531, true)
            {
                OtherCheck = () =>
                {
                    if(JobGauge.Lily == 0) return false;
                    foreach (float rate in TargetHelper.PartyMembersHP)
                    {
                        if (rate < 0.8) return true;
                    }
                    return false;

                }
            },
            //再生
            Regen = new BaseAction(137, true)
            {
                OtherCheck = () =>
                {
                    foreach (float rate in TargetHelper.PartyMembersHP)
                    {
                        if (rate >= hotElement) return true;
                    }
                    return false;
                },
                TargetStatus = new ushort[]
                {
                    ObjectStatus.Regen1,
                    ObjectStatus.Regen2,
                    ObjectStatus.Regen3,
                }
            },
            //水流幕
            Aquaveil = new BaseAction(25861, true),
            //神祝祷
            DivineBenison = new BaseAction(7432, true),
            //天赐
            Benediction = new BaseAction(140, true)
            {
                OtherCheck = () =>
                {
                    if(Service.TargetManager.Target is BattleChara b)
                    {
                        if ((float)b.CurrentHp / b.MaxHp < 0.1) return true;
                    }
                    return false;
                },
            },

            //医治 群奶最基础的。300
            Medica = new BaseAction(124, true),
            //愈疗 600
            Cure3 = new BaseAction(131, true),
            //医济 群奶加Hot。
            Medica2 = new BaseAction(133, true)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Medica2,
                    ObjectStatus.TrueMedica2,
                },

                OtherCheck = () =>  TargetHelper.PartyMembersAverHP >= hotElement,
            },
            //庇护所
            Asylum = new BaseAction(3569, true),
            //法令
            Assize = new BaseAction(3571, true),
            //狂喜之心 400
            AfflatusRapture = new BaseAction(16534, true)
            {
                OtherCheck = () => JobGauge.Lily > 0,
            },
            //礼仪之铃
            LiturgyoftheBell = new BaseAction(25862, true),

            //复活
            Raise = new BaseAction(125, true)
            {
                OtherCheck = () => TargetHelper.DeathPeopleAll.Length > 0,
                BuffsProvide = new ushort[] { ObjectStatus.Raise },
            },

            //神速咏唱
            PresenseOfMind = new BaseAction(136),
            //无中生有
            ThinAir = new BaseAction(7430),

            //全大赦
            PlenaryIndulgence = new BaseAction(7433)
            {
                OtherCheck = () => TargetHelper.PartyMembersAverHP < hotElement,
            },
            //节制
            Temperance = new BaseAction(16536);
    }

    protected bool CanAddAbility(byte level, out uint action)
    {
        action = 0;

        if (CanInsertAbility)
        {
            //神名
            if (Actions.Tetragrammaton.TryUseAction(level, out action)) return true;

            //加个水流幕
            if (Actions.Aquaveil.TryUseAction(level, out action)) return true;

            //加个神祝祷
            if (Actions.DivineBenison.TryUseAction(level, out action)) return true;

            //加个法令
            if (Actions.Assize.TryUseAction(level, out action)) return true;

            //加个无中生有
            if (Actions.ThinAir.TryUseAction(level, out action)) return true;

            //加个神速咏唱
            if (Actions.PresenseOfMind.TryUseAction(level, out action)) return true;

            //加个醒梦
            if (GeneralActions.LucidDreaming.TryUseAction(level, out action)) return true;
        }
        return false;
    }

    internal static bool UseBenediction(out PlayerCharacter dangeriousTank)
    {
        dangeriousTank = null;
        uint[] dangerousState = new uint[] { ObjectStatus.Holmgang, ObjectStatus.WalkingDead, ObjectStatus.Superbolide };
        foreach (var member in TargetHelper.PartyMembers)
        {
            //看看有没有人要搞死自己。
            foreach (var tag in member.StatusList)
            {
                if (dangerousState.Contains(tag.StatusId))
                {
                    if (tag.RemainingTime < 1)
                    {
                        dangeriousTank = member;
                        return true;
                    }
                    else return false;
                }
            }

            //如果没有人要搞死自己，那么就看看有没有人快死了。
            if ((float)member.CurrentHp / member.MaxHp < 0.15 && member.CurrentHp != 0)
            {
                dangeriousTank = member;
                return true;
            }
        }
        return false;
    }
}
