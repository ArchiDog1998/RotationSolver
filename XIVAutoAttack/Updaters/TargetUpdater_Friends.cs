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
                ObjectStatus.Doom,
                ObjectStatus.Amnesia,
                ObjectStatus.Stun,
                ObjectStatus.Stun2,
                ObjectStatus.Sleep,
                ObjectStatus.Sleep2,
                ObjectStatus.Sleep3,
                ObjectStatus.Pacification,
                ObjectStatus.Pacification2,
                ObjectStatus.Silence,
                ObjectStatus.Slow,
                ObjectStatus.Slow2,
                ObjectStatus.Slow3,
                ObjectStatus.Slow4,
                ObjectStatus.Slow5,
                ObjectStatus.Blind,
                ObjectStatus.Blind2,
                ObjectStatus.Blind3,
                ObjectStatus.Paralysis,
                ObjectStatus.Paralysis2,
                ObjectStatus.Nightmare,
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
                CanHealAreaAbility = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < Service.Configuration.HealthAreaAbility;
                CanHealAreaSpell = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < Service.Configuration.HealthAreafSpell;
            }
            else
            {
                CanHealAreaAbility = CanHealAreaSpell = false;
            }
            var abilityCount = PartyMembersHP.Count(p => p < Service.Configuration.HealthSingleAbility);
            CanHealSingleAbility = abilityCount > 0;
            if (abilityCount >= Service.Configuration.PartyCount) CanHealAreaAbility = true;

            var gcdCount = PartyMembersHP.Count(p => p < Service.Configuration.HealthSingleSpell);
            CanHealSingleSpell = gcdCount > 0;
            if (gcdCount >= Service.Configuration.PartyCount) CanHealAreaSpell = true;

            PartyMembersMinHP = PartyMembersHP.Min();
            HPNotFull = PartyMembersMinHP < 1;
            #endregion
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
