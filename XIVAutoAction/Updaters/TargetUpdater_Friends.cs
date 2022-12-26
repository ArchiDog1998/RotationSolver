using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Localization;

namespace XIVAutoAttack.Updaters
{
    internal static partial class TargetUpdater
    {
        /// <summary>
        /// 小队成员们
        /// </summary>
        public static IEnumerable<BattleChara> PartyMembers { get; private set; } = new PlayerCharacter[0];
        /// <summary>
        /// 团队成员们
        /// </summary>
        internal static IEnumerable<BattleChara> AllianceMembers { get; private set; } = new PlayerCharacter[0];

        /// <summary>
        /// 小队坦克们
        /// </summary>
        internal static IEnumerable<BattleChara> PartyTanks { get; private set; } = new PlayerCharacter[0];
        /// <summary>
        /// 小队治疗们
        /// </summary>
        internal static IEnumerable<BattleChara> PartyHealers { get; private set; } = new PlayerCharacter[0];

        /// <summary>
        /// 团队坦克们
        /// </summary>
        internal static IEnumerable<BattleChara> AllianceTanks { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static IEnumerable<BattleChara> DeathPeopleAll { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static IEnumerable<BattleChara> DeathPeopleParty { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static IEnumerable<BattleChara> WeakenPeople { get; private set; } = new PlayerCharacter[0];

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static IEnumerable<BattleChara> DyingPeople { get; private set; } = new PlayerCharacter[0];
        /// <summary>
        /// 小队成员HP
        /// </summary>
        internal static IEnumerable<float> PartyMembersHP { get; private set; } = new float[0];
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
        /// 有陆行鸟
        /// </summary>
        internal static bool HaveChocobo { get; private set; } = false;
        /// <summary>
        /// 血量没有满
        /// </summary>
        internal static bool HPNotFull { get; private set; } = false;

        internal unsafe static void UpdateFriends()
        {
            #region Friend
            var party = Service.PartyList;
            PartyMembers = party.Length == 0 ? Service.ClientState.LocalPlayer == null ? new BattleChara[0] : new BattleChara[] { Service.ClientState.LocalPlayer } :
                party.Where(obj => obj != null && obj.GameObject is BattleChara)
                .Select(obj => obj.GameObject as BattleChara);

            //添加亲信
            PartyMembers = PartyMembers.Union(Service.ObjectTable.Where(obj => obj.SubKind == 9 && obj is BattleChara).Cast<BattleChara>());

            HavePet = Service.ObjectTable.Where(obj => obj != null && obj is BattleNpc npc
                    && npc.BattleNpcKind == BattleNpcSubKind.Pet
                    && npc.OwnerId == Service.ClientState.LocalPlayer.ObjectId).Count() > 0;

            HaveChocobo = Service.ObjectTable.Where(obj => obj != null && obj is BattleNpc npc
                    && npc.BattleNpcKind == BattleNpcSubKind.Chocobo
                    && npc.OwnerId == Service.ClientState.LocalPlayer.ObjectId).Count() > 0;

            AllianceMembers = Service.ObjectTable.OfType<PlayerCharacter>();

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
            });

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
                StatusID.Necrosis,
            };
            DyingPeople = WeakenPeople.Where(p => p.HasStatus(false, dangeriousStatus));

            SayHelloToAuthor();
            #endregion

            #region Health
            var members = PartyMembers;

            PartyMembersHP = TargetFilter.GetObjectInRadius(members, 30).Where(r => r.CurrentHp > 0).Select(p => (float)p.CurrentHp / p.MaxHp);

            float averHP = 0;
            foreach (var hp in PartyMembersHP)
            {
                averHP += hp;
            }
            PartyMembersAverHP = averHP / PartyMembersHP.Count();

            double differHP = 0;
            float average = PartyMembersAverHP;
            foreach (var hp in PartyMembersHP)
            {
                differHP += Math.Pow(hp - average, 2);
            }
            PartyMembersDifferHP = (float)Math.Sqrt(differHP / PartyMembersHP.Count());

            var job = (ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id;

            var hotSubArea = Service.Configuration.HealingOfTimeSubtractAreas.TryGetValue(job, out var value) ? value : 0.3f;

            var areaHots = new StatusID[]
            {
                 StatusID.AspectedHelios,
                 StatusID.Medica2,
                 StatusID.TrueMedica2,
            };

            //TODO:少了所有罩子类技能
            var ratio = GetHealingOfTimeRatio(Service.ClientState.LocalPlayer, areaHots) * hotSubArea;

            var healAreability = Service.Configuration.HealthAreaAbilitys.TryGetValue(job, out value) ? value : Service.Configuration.HealthAreaAbility;

            var healAreaspell = Service.Configuration.HealthAreafSpells.TryGetValue(job, out value) ? value : Service.Configuration.HealthAreafSpell;

            CanHealAreaAbility = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < healAreability - ratio;

            CanHealAreaSpell = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < healAreaspell - ratio;

            var hotSubSingle = Service.Configuration.HealingOfTimeSubtractSingles.TryGetValue(job, out value) ? value : 0.3f;

            var singleHots = new StatusID[]
            {
                StatusID.AspectedBenefic,
                StatusID.Regen1,
                StatusID.Regen2,
                StatusID.Regen3
            };

            var healsingAbility = Service.Configuration.HealthSingleAbilitys.TryGetValue(job, out value) ? value : Service.Configuration.HealthSingleAbility;
            //Hot衰减
            var abilityCount = PartyMembers.Count(p =>
            {
                var ratio = GetHealingOfTimeRatio(p, singleHots);

                var h = p.GetHealthRatio();
                if (h == 0 || !StatusHelper.NeedHealing(p)) return false;

                return h < healsingAbility - hotSubSingle * ratio;
            });
            CanHealSingleAbility = abilityCount > 0;

            var healsingSpell = Service.Configuration.HealthSingleSpells.TryGetValue(job, out value) ? value : Service.Configuration.HealthSingleSpell;

            var gcdCount = PartyMembers.Count(p =>
            {
                var ratio = GetHealingOfTimeRatio(p, singleHots);
                var h = p.GetHealthRatio();
                if (h == 0 || !StatusHelper.NeedHealing(p)) return false;

                return h < healsingSpell - hotSubSingle * ratio;
            });
            CanHealSingleSpell = gcdCount > 0;

            PartyMembersMinHP = PartyMembersHP.Any() ? PartyMembersHP.Min() : 0;
            HPNotFull = PartyMembersMinHP < 1;
            #endregion
        }

        static float GetHealingOfTimeRatio(BattleChara target, params StatusID[] statusIds)
        {
            const float buffWholeTime = 15;

            var buffTime = target.StatusTime(false, statusIds);

            return Math.Min(1, buffTime / buffWholeTime);
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

        private static IEnumerable<BattleChara> FilterForDeath(IEnumerable<BattleChara> battleCharas)
        {
            return battleCharas.Where(b =>
            {
                if (!_locations.TryGetValue(b.ObjectId, out var loc)) return false;

                return loc == b.Position;
            });
        }


        /// <summary>
        /// 作者本人
        /// </summary>
        static DateTime foundTime = DateTime.Now;
        static TimeSpan relayTime = TimeSpan.Zero;
        static readonly string[] authorKeys = new string[]
        {
            "LwA5GZE3hRgUtxmCB59xqQ==",
            "EJ/sAztgwh8D9PKF3IscmA==",
            "b4/6WcxY3Xhh1Ln5LZKMVA==",
        };
        static List<string> macroToAuthor = new List<string>()
        {
            "blush",
            "hug",
            "thumbsup",
            "yes",
            "clap",
            "cheer",
            "stroke",
        };
        private static void SayHelloToAuthor()
        {
            //只有任务中才能执行此操作
            if (!Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInQuestEvent]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.WaitingForDuty]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.WaitingForDutyFinder]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51]) return;

            //战斗中不执行
            if (ActionUpdater.InCombat) return;

            //已经干过此事，不执行
            if (foundTime == DateTime.MinValue) return;

            //找作者
            var author = AllianceMembers.FirstOrDefault(c => c is PlayerCharacter player && authorKeys.Contains(EncryptString(player))
                            && c.ObjectId != Service.ClientState.LocalPlayer.ObjectId) as PlayerCharacter;

            //没找到作者
            if (author == null) return;

            //随机事件
            if (relayTime == TimeSpan.Zero)
            {
                foundTime = DateTime.Now;
                relayTime = new TimeSpan(new Random().Next(30000, 80000));
            }

            if (DateTime.Now - foundTime > relayTime)
            {
                Service.TargetManager.SetTarget(author);
                CommandController.SubmitToChat($"/{macroToAuthor[new Random().Next(macroToAuthor.Count)]} <t>");
                Service.ChatGui.PrintChat(new Dalamud.Game.Text.XivChatEntry()
                {
                    Message = string.Format(LocalizationManager.RightLang.Commands_SayHelloToAuthor, author.Name),
                    Type = Dalamud.Game.Text.XivChatType.Notice,
                });
                UIModule.PlaySound(20, 0, 0, 0);
                foundTime = DateTime.MinValue;
                Service.TargetManager.SetTarget(null);
            }
        }

        internal static string EncryptString(PlayerCharacter player)
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(player.HomeWorld.GameData.InternalName.ToString() + " - " + player.Name.ToString() + "U6Wy.zCG");

            var tmpHash = MD5.Create().ComputeHash(inputByteArray);
            var retB = Convert.ToBase64String(tmpHash.ToArray());
            return retB;
        }
    }
}
