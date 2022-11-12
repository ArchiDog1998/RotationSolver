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
        public static BattleChara[] PartyMembers { get; private set; } = new PlayerCharacter[0];
        /// <summary>
        /// 玩家们
        /// </summary>
        internal static BattleChara[] AllianceMembers { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyTanks { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyHealers { get; private set; } = new PlayerCharacter[0];

        internal static BattleChara[] AllianceTanks { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static BattleChara[] DeathPeopleAll { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static BattleChara[] DeathPeopleParty { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static BattleChara[] WeakenPeople { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static BattleChara[] DyingPeople { get; private set; } = new PlayerCharacter[0];
        internal static float[] PartyMembersHP { get; private set; } = new float[0];
        internal static float PartyMembersMinHP { get; private set; } = 0;
        internal static float PartyMembersAverHP { get; private set; } = 0;
        internal static float PartyMembersDifferHP { get; private set; } = 0;


        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CanHealAreaAbility { get; private set; } = false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CanHealAreaSpell { get; private set; } = false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CanHealSingleAbility { get; private set; } = false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static bool CanHealSingleSpell { get; private set; } = false;
        internal static bool HavePet { get; private set; } = false;
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

            PartyTanks = TargetFilter.GetJobCategory(PartyMembers, Role.防护);
            PartyHealers = TargetFilter.GetJobCategory(TargetFilter.GetObjectInRadius(PartyMembers, 30), Role.治疗);
            AllianceTanks = TargetFilter.GetJobCategory(TargetFilter.GetObjectInRadius(AllianceMembers, 30), Role.防护);

            DeathPeopleAll = TargetFilter.GetObjectInRadius(TargetFilter.GetDeath(AllianceMembers), 30);
            DeathPeopleParty = TargetFilter.GetObjectInRadius(TargetFilter.GetDeath(PartyMembers), 30);
            MaintainDeathPeople();

            WeakenPeople = TargetFilter.GetObjectInRadius(PartyMembers, 30).Where(p =>
            {
                foreach (var status in p.StatusList)
                {
                    if (status.GameData.CanDispel && status.RemainingTime > 2) return true;
                }
                return false;
            }).ToArray();

            uint[] dangeriousStatus = new uint[]
            {
                StatusIDs.Doom,
                StatusIDs.Amnesia,
                StatusIDs.Stun,
                StatusIDs.Stun2,
                StatusIDs.Sleep,
                StatusIDs.Sleep2,
                StatusIDs.Sleep3,
                StatusIDs.Pacification,
                StatusIDs.Pacification2,
                StatusIDs.Silence,
                StatusIDs.Slow,
                StatusIDs.Slow2,
                StatusIDs.Slow3,
                StatusIDs.Slow4,
                StatusIDs.Slow5,
                StatusIDs.Blind,
                StatusIDs.Blind2,
                StatusIDs.Blind3,
                StatusIDs.Paralysis,
                StatusIDs.Paralysis2,
                StatusIDs.Nightmare,
            };
            DyingPeople = WeakenPeople.Where(p =>
            {
                foreach (var status in p.StatusList)
                {
                    if (dangeriousStatus.Contains(status.StatusId)) return true;
                    //if (status.StackCount > 2) return true;
                }
                return false;
            }).ToArray();
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
                    StatusIDs.AspectedHelios, StatusIDs.Medica2, StatusIDs.TrueMedica2)
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

            var singleHots = new ushort[] {StatusIDs.AspectedBenefic, StatusIDs.Regen1,
                StatusIDs.Regen2,
                StatusIDs.Regen3};

            //Hot衰减
            var abilityCount = PartyMembers.Count(p =>
            {
                var ratio = GetHealingOfTimeRatio(p, singleHots);
                return p.GetHealthRatio() < Service.Configuration.HealthSingleAbility -
                    Service.Configuration.HealingOfTimeSubstactSingle * ratio;
            });
            CanHealSingleAbility = abilityCount > 0;
            if (abilityCount >= Service.Configuration.PartyCount) CanHealAreaAbility = true;


            var gcdCount = PartyMembers.Count(p =>
            {
                var ratio = GetHealingOfTimeRatio(p, singleHots);
                return p.GetHealthRatio() < Service.Configuration.HealthSingleSpell - 
                    Service.Configuration.HealingOfTimeSubstactSingle * ratio;
            });
            CanHealSingleSpell = gcdCount > 0;
            if (gcdCount >= Service.Configuration.PartyCount) CanHealAreaSpell = true;

            PartyMembersMinHP = PartyMembersHP.Min();
            HPNotFull = PartyMembersMinHP < 1;
            #endregion
        }

        static float GetHealingOfTimeRatio(BattleChara target, params ushort[] statusIds)
        {
            var buffTime = target.FindStatusTime(statusIds);

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
