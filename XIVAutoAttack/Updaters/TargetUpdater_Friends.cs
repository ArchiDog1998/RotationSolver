using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Updaters
{
    internal static partial class TargetUpdater
    {
        /// <summary>
        /// 小队成员们
        /// </summary>
        public static BattleChara[] PartyMembers { get; private set; } = new PlayerCharacter[0];
        /// <summary>
        /// 团队成员们
        /// </summary>
        internal static BattleChara[] AllianceMembers { get; private set; } = new PlayerCharacter[0];

        /// <summary>
        /// 小队坦克们
        /// </summary>
        internal static BattleChara[] PartyTanks { get; private set; } = new PlayerCharacter[0];
        /// <summary>
        /// 小队治疗们
        /// </summary>
        internal static BattleChara[] PartyHealers { get; private set; } = new PlayerCharacter[0];

        /// <summary>
        /// 团队坦克们
        /// </summary>
        internal static BattleChara[] AllianceTanks { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static BattleChara[] DeathPeopleAll { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static BattleChara[] DeathPeopleParty { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static BattleChara[] WeakenPeople { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static BattleChara[] DyingPeople { get; private set; } = new PlayerCharacter[0];
        /// <summary>
        /// 小队成员HP
        /// </summary>
        internal static float[] PartyMembersHP { get; private set; } = new float[0];
        /// <summary>
        /// 小队成员最小的HP
        /// </summary>
        internal static float PartyMembersMinHP { get; private set; } = 0;
        /// <summary>
        /// 小队成员平均HP
        /// </summary>
        internal static float PartyMembersAverHP { get; private set; } = 0;
        /// <summary>
        /// 小队成员HP差值
        /// </summary>
        internal static float PartyMembersDifferHP { get; private set; } = 0;


        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CanHealAreaAbility { get; private set; } = false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CanHealAreaSpell { get; private set; } = false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CanHealSingleAbility { get; private set; } = false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CanHealSingleSpell { get; private set; } = false;

        /// <summary>
        /// 有宠物
        /// </summary>
        internal static bool HavePet { get; private set; } = false;
        /// <summary>
        /// 血量没有满
        /// </summary>
        internal static bool HPNotFull { get; private set; } = false;

        internal unsafe static void UpdateFriends()
        {
            #region Friend
            var party = Service.PartyList;
            PartyMembers = party.Length == 0 ? Service.ClientState.LocalPlayer == null ? new BattleChara[0] : new BattleChara[] { Service.ClientState.LocalPlayer } :
                party.Where(obj => obj != null && obj.GameObject is BattleChara).Select(obj => obj.GameObject as BattleChara).ToArray();

            //添加亲信
            PartyMembers = PartyMembers.Union(Service.ObjectTable.Where(obj => obj.SubKind == 9 && obj is BattleChara).Cast<BattleChara>()).ToArray();

            HavePet = Service.ObjectTable.Where(obj => obj != null && obj is BattleNpc npc && npc.BattleNpcKind == BattleNpcSubKind.Pet && npc.OwnerId == Service.ClientState.LocalPlayer.ObjectId).Count() > 0;

            AllianceMembers = Service.ObjectTable.Where(obj => obj is PlayerCharacter).Select(obj => (PlayerCharacter)obj).ToArray();

            PartyTanks = PartyMembers.GetJobCategory(JobRole.Tank);
            PartyHealers = PartyMembers.GetObjectInRadius(30).GetJobCategory(JobRole.Healer);
            AllianceTanks = AllianceMembers.GetObjectInRadius(30).GetJobCategory(JobRole.Tank);

            DeathPeopleAll = AllianceMembers.GetDeath().GetObjectInRadius(30);
            DeathPeopleParty = PartyMembers.GetDeath().GetObjectInRadius(30);
            MaintainDeathPeople();

            WeakenPeople = TargetFilter.GetObjectInRadius(PartyMembers, 30).Where(p =>
            {
                foreach (var status in p.StatusList)
                {
                    if (status.GameData.CanDispel && status.RemainingTime > 2) return true;
                }
                return false;
            }).ToArray();

            var dangeriousStatus = new StatusID[]
            {
                StatusID.Doom,
                StatusID.Amnesia,
                StatusID.Stun,
                StatusID.Stun2,
                StatusID.Sleep,
                StatusID.Sleep2,
                StatusID.Sleep3,
                StatusID.Pacification,
                StatusID.Pacification2,
                StatusID.Silence,
                StatusID.Slow,
                StatusID.Slow2,
                StatusID.Slow3,
                StatusID.Slow4,
                StatusID.Slow5,
                StatusID.Blind,
                StatusID.Blind2,
                StatusID.Blind3,
                StatusID.Paralysis,
                StatusID.Paralysis2,
                StatusID.Nightmare,
            };
            DyingPeople = WeakenPeople.Where(p => p.HasStatus(false, dangeriousStatus)).ToArray();
            #endregion

            #region Health
            var members = PartyMembers;

            PartyMembersHP = TargetFilter.GetObjectInRadius(members, 30).Where(r => r.CurrentHp > 0).Select(p => (float)p.CurrentHp / p.MaxHp).ToArray();

            float averHP = 0;
            foreach (var hp in PartyMembersHP)
            {
                averHP += hp;
            }
            PartyMembersAverHP = averHP / PartyMembersHP.Length;

            double differHP = 0;
            float average = PartyMembersAverHP;
            foreach (var hp in PartyMembersHP)
            {
                differHP += Math.Pow(hp - average, 2);
            }
            PartyMembersDifferHP = (float)Math.Sqrt(differHP / PartyMembersHP.Length);

            if (PartyMembers.Length >= Service.Configuration.PartyCount)
            {
                //TODO:少了所有罩子类技能
                var ratio = GetHealingOfTimeRatio(Service.ClientState.LocalPlayer, 
                    StatusID.AspectedHelios, StatusID.Medica2, StatusID.TrueMedica2)
                    * Service.Configuration.HealingOfTimeSubstactArea;

                CanHealAreaAbility = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < Service.Configuration.HealthAreaAbility
                    -  ratio;

                CanHealAreaSpell = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < Service.Configuration.HealthAreafSpell
                    -  ratio;
            }
            else
            {
                CanHealAreaAbility = CanHealAreaSpell = false;
            }

            var singleHots = new StatusID[] {StatusID.AspectedBenefic, StatusID.Regen1,
                StatusID.Regen2,
                StatusID.Regen3};

            //Hot衰减
            var abilityCount = PartyMembers.Count(p =>
            {
                var ratio = GetHealingOfTimeRatio(p, singleHots);

                var h = p.GetHealthRatio();
                if (h == 0) return false;

                return h < Service.Configuration.HealthSingleAbility -
                    Service.Configuration.HealingOfTimeSubstactSingle * ratio;
            });
            CanHealSingleAbility = abilityCount > 0;
            if (abilityCount >= Service.Configuration.PartyCount) CanHealAreaAbility = true;


            var gcdCount = PartyMembers.Count(p =>
            {
                var ratio = GetHealingOfTimeRatio(p, singleHots);
                var h = p.GetHealthRatio();
                if (h == 0) return false;

                return h < Service.Configuration.HealthSingleSpell - 
                    Service.Configuration.HealingOfTimeSubstactSingle * ratio;
            });
            CanHealSingleSpell = gcdCount > 0;
            if (gcdCount >= Service.Configuration.PartyCount) CanHealAreaSpell = true;

            PartyMembersMinHP = PartyMembersHP.Min();
            HPNotFull = PartyMembersMinHP < 1;
            #endregion
        }

        static float GetHealingOfTimeRatio(BattleChara target, params StatusID[] statusIds)
        {
            var buffTime = target.StatusTime(false, statusIds);

            return Math.Min(1, buffTime / 15);
        }

        static SortedDictionary<uint, Vector3> _locations = new SortedDictionary<uint, Vector3>();
        private static void MaintainDeathPeople()
        {
            SortedDictionary<uint, Vector3> locs = new SortedDictionary<uint, Vector3>();
            foreach (var item in DeathPeopleAll)
            {
                locs[item.ObjectId] = item.Position;
            }
            foreach (var item in DeathPeopleParty)
            {
                locs[item.ObjectId] = item.Position;
            }

            DeathPeopleAll = FilterForDeath(DeathPeopleAll);
            DeathPeopleParty = FilterForDeath(DeathPeopleParty);

            _locations = locs;
        }

        private static BattleChara[] FilterForDeath(BattleChara[] battleCharas)
        {
            return battleCharas.Where(b =>
            {
                if (!_locations.TryGetValue(b.ObjectId, out var loc)) return false;

                return loc == b.Position;
            }).ToArray();
        }
    }
}
