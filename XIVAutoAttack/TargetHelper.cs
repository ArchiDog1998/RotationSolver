using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using XIVAutoAttack.Combos;
using Action = Lumina.Excel.GeneratedSheets.Action;
using Vector3 = System.Numerics.Vector3;

namespace XIVAutoAttack
{
    internal class TargetHelper
    {

        private static IntPtr _func;

        private static Vector3 _lastPosition;
        public static bool IsMoving { get; private set; }

        private static readonly Stopwatch _weaponDelayStopwatch = new Stopwatch();

        internal static readonly Stopwatch _fisherTimer = new Stopwatch();
        internal static readonly Stopwatch _unfishingTimer = new Stopwatch();
        private static bool _isLastFishing = false;

        private static long _weaponRandomDelay = 0;

        private static FishType _fishType = FishType.None;

        internal static FishType Fish
        {
            get
            {
                if (_fisherTimer.IsRunning && _fishType != FishType.Mooch && _fisherTimer.ElapsedMilliseconds > 3000)
                {
                    _fisherTimer.Stop();
                    _fisherTimer.Reset();
                    _fishType = FishType.None;
                }
                return _fishType;
            }
            set
            {
                if (value != FishType.None)
                {
                    _fisherTimer.Restart();
                }
                _fishType = value;
            }
        }


        //All Targes
        internal static BattleChara[] AllTargets { get; private set; } = new BattleChara[0];

        internal static BattleChara[] HostileTargets { get; private set; } = new BattleChara[0];
        internal static BattleChara[] CanInterruptTargets { get; private set; } = new BattleChara[0];

        internal static bool HaveTargetAngle { get; private set; } = false;

        internal static float WeaponRemain { get; private set; } = 0;
        internal static float WeaponTotal { get; private set; } = 0;

        internal static byte AbilityRemainCount { get; private set; } = 0;

        //  public static PlayerCharacter[] PartyMembers =>
        //AllianceMembers.Where(fri => (fri.StatusFlags & StatusFlags.AllianceMember) != 0).ToArray();
        public static BattleChara[] PartyMembers { get; private set; } = new PlayerCharacter[0];


        /// <summary>
        /// 玩家们
        /// </summary>
        internal static BattleChara[] AllianceMembers { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyTanks { get; private set; } = new PlayerCharacter[0];
        //internal static BattleChara[] PartyTanksAttached { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] DeathPeopleAll { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] DeathPeopleParty { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] WeakenPeople { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] DyingPeople { get; private set; } = new PlayerCharacter[0];
        internal static float[] PartyMembersHP { get; private set; } = new float[0];
        internal static float PartyMembersAverHP { get; private set; } = 0;
        internal static float PartyMembersDifferHP { get; private set; } = 0;

        internal static bool CanHealAreaAbility { get; private set; } = false;
        internal static bool CanHealAreaSpell { get; private set; } = false;
        internal static bool CanHealSingleAbility { get; private set; } = false;
        internal static bool CanHealSingleSpell { get; private set; } = false;
        internal static bool HavePet { get; private set; } = false;
        internal static bool HPNotFull { get; private set; } = false;
        internal static bool InBattle { get; private set; } = false;


        internal static readonly Queue<MacroItem> Macros = new Queue<MacroItem>();
        internal static MacroItem DoingMacro;
        private static bool _lastCasting = false;
#if DEBUG
        private static readonly Dictionary<int, bool> _valus = new Dictionary<int, bool>();
#endif
        internal static unsafe void Init(SigScanner sigScanner)
        {
            _func = sigScanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3 ");

        }

        internal unsafe static void Framework_Update(Framework framework)
        {
            UpdateCastBar();
#if DEBUG
            //Get changed condition.
            string[] enumNames = Enum.GetNames(typeof(Dalamud.Game.ClientState.Conditions.ConditionFlag));
            int[] indexs = (int[])Enum.GetValues(typeof(Dalamud.Game.ClientState.Conditions.ConditionFlag));
            if (enumNames.Length == indexs.Length)
            {
                for (int i = 0; i < enumNames.Length; i++)
                {
                    string key = enumNames[i];
                    bool newValue = Service.Conditions[(Dalamud.Game.ClientState.Conditions.ConditionFlag)indexs[i]];
                    if (_valus.ContainsKey(i) && _valus[i] != newValue && indexs[i] != 48)
                    {
                        Service.ToastGui.ShowQuest(indexs[i].ToString() + " " + key + ": " + newValue.ToString());
                    }
                    _valus[i] = newValue;
                }
            }

            //for (int i = 0; i < 100; i++)
            //{
            //    bool newValue = Service.Conditions[i];
            //    if (_valus.ContainsKey(i) && _valus[i] != newValue)
            //    {
            //        Service.ToastGui.ShowQuest(i.ToString());
            //    }
            //    _valus[i] = newValue;
            //}
#endif
            //UpdateFishing.
            bool fishing = Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Fishing];
            if(_isLastFishing && !fishing)
            {
                _unfishingTimer.Restart();
            }
            _isLastFishing = fishing;

            //Update State.
            if (Service.Configuration.UseDtr && IconReplacer.StateString != null)
            {
                if (XIVAutoAttackPlugin.dtrEntry == null)
                {
                    XIVAutoAttackPlugin.dtrEntry = Service.DtrBar.Get("Auto Attack");
                }
                XIVAutoAttackPlugin.dtrEntry.Shown = true;
                XIVAutoAttackPlugin.dtrEntry.Text = new SeString(
                    new IconPayload(BitmapFontIcon.DPS),
                    new TextPayload(IconReplacer.StateString)
                    );
            }
            else if (XIVAutoAttackPlugin.dtrEntry != null)
            {
                XIVAutoAttackPlugin.dtrEntry.Shown = false;
            }

            if (Service.ClientState.LocalPlayer == null) return;

            if (Service.ClientState.LocalPlayer.CurrentHp == 0
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.RolePlaying]) IconReplacer.AutoAttack = false;

            InBattle = Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat];

            Vector3 thisPosition = Service.ClientState.LocalPlayer.Position;
            IsMoving = Vector3.Distance(_lastPosition, thisPosition) != 0;
            _lastPosition = thisPosition;

            var instance = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance();
            var spell = FFXIVClientStructs.FFXIV.Client.Game.ActionType.Spell;

            WeaponTotal = instance->GetRecastTime(spell, 11);
            var weaponelapsed = instance->GetRecastTimeElapsed(spell, 11);

            WeaponRemain = WeaponTotal - weaponelapsed;
            if (Service.ClientState.LocalPlayer.IsCasting)
            {
                WeaponRemain = Math.Max(WeaponRemain, Service.ClientState.LocalPlayer.TotalCastTime - Service.ClientState.LocalPlayer.CurrentCastTime);
            }
            var min = Math.Max(WeaponTotal - Service.Configuration.WeaponInterval, 0);
            AbilityRemainCount = (byte)(Math.Min(WeaponRemain, min) / Service.Configuration.WeaponInterval);

            UpdateTargets();
            DoAction(weaponelapsed);

            #region 宏
            //如果没有有正在运行的宏，弄一个出来
            if (DoingMacro == null && Macros.TryDequeue(out var macro))
            {
                DoingMacro = macro;
            }

            //如果有正在处理的宏
            if (DoingMacro != null)
            {
                //正在跑的话，就尝试停止，停止成功就放弃它。
                if (DoingMacro.IsRunning)
                {
                    if (DoingMacro.EndUseMacro())
                    {
                        DoingMacro = null;
                    }
                    else
                    {
                        return;
                    }
                }
                //否则，始终开始。
                else
                {
                    DoingMacro.StartUseMacro();
                }
            }
            #endregion
        }
        private static unsafe void UpdateCastBar()
        {
            ByteColor redColor = new ByteColor() { A = 255, R = 120, G = 0, B = 60 };
            ByteColor greenColor = new ByteColor() { A = 255, R = 60, G = 120, B = 30 };


            AtkUnitBase* castBar = (AtkUnitBase*)Service.GameGui.GetAddonByName("_CastBar", 1);
            AtkResNode* progressBar = castBar->UldManager.NodeList[5];

            bool realCasting = Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Casting];
            ByteColor c = redColor;
            if (Service.Configuration.CheckForCasting && !realCasting) c = greenColor;

            progressBar->AddRed = c.R;
            progressBar->AddGreen = c.G;
            progressBar->AddBlue = c.B;
        }

        //private unsafe static void Hide(AtkResNode* node)
        //{
        //    node->Flags &= -17;
        //    node->Flags_2 |= 1u;
        //}

        //public unsafe static void Show(AtkResNode* node)
        //{
        //    node->Flags |= 16;
        //    node->Flags_2 |= 1u;
        //}

        private static void DoAction(float weaponelapsed)
        {
            if (Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInQuestEvent]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Occupied33]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Occupied38]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Jumping61]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Mounted] 
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.SufferingStatusAffliction]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.SufferingStatusAffliction2]
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.InFlight]) return;

            if (WeaponRemain <= Service.Configuration.WeaponFaster)
            {
                if (!_weaponDelayStopwatch.IsRunning)
                {
                    _weaponDelayStopwatch.Start();
                    return;
                }
                else if (_weaponDelayStopwatch.ElapsedMilliseconds > _weaponRandomDelay)
                {
                    _weaponDelayStopwatch.Stop();
                    _weaponDelayStopwatch.Reset();

                    Service.IconReplacer.DoAnAction(true);

                    Random ran = new Random(DateTime.Now.Millisecond);
                    _weaponRandomDelay = (long)(ran.NextDouble() * Service.Configuration.WeaponDelay * 1000);

                    return;
                }
                else return;
            }


            //要超出GCD了，那就不放技能了。
            if (WeaponRemain < Service.Configuration.WeaponInterval) return;
            //IsCasting.
            if (Service.ClientState.LocalPlayer.IsCasting)
            {
                _lastCasting = true;
                return;
            }

            if ((weaponelapsed % Service.Configuration.WeaponInterval <= Service.Configuration.WeaponFaster || _lastCasting)
                && WeaponTotal - WeaponRemain > Service.Configuration.WeaponInterval)
            {
                Service.IconReplacer.DoAnAction(false);
                _lastCasting = false;
                return;
            }

        }
        private static void UpdateTargets()
        {

            #region Hostile

            AllTargets = BaseAction.GetObjectInRadius(Service.ObjectTable.ToArray(), 30).Where(obj => obj is BattleChara c && c.CurrentHp != 0 && CanAttack(obj)).Select(obj => (BattleChara)obj).ToArray();
            uint[] ids = GetEnemies() ?? new uint[0];
            //InBattle = ids.Length > 0;
            if (AllTargets != null && AllTargets.Length > 0)
            {
                var hosts = AllTargets.Where(t => t.TargetObject is PlayerCharacter || ids.Contains(t.ObjectId)).ToArray();
                HostileTargets = Service.Configuration.AllTargeAsHostile || hosts.Length == 0 ? AllTargets : hosts;

                CanInterruptTargets = HostileTargets.Where(tar => tar.IsCasting && tar.IsCastInterruptible && tar.TotalCastTime >= 2).ToArray();

                float radius = 25;
                switch (XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == Service.ClientState.LocalPlayer.ClassJob.Id).Role)
                {
                    case (byte)Role.防护:
                    case (byte)Role.近战:
                        radius = 3;
                        break;
                }
                HaveTargetAngle = BaseAction.GetObjectInRadius(HostileTargets, radius).Length > 0;
            }
            else
            {
                AllTargets = HostileTargets = CanInterruptTargets = new BattleChara[0];
                HaveTargetAngle = false;
            }

            #endregion
            #region Friend
            var party = Service.PartyList;
            PartyMembers = party.Length == 0 ? Service.ClientState.LocalPlayer == null ? new BattleChara[0] : new BattleChara[] { Service.ClientState.LocalPlayer } :
                party.Where(obj => obj != null && obj.GameObject is BattleChara).Select(obj => obj.GameObject as BattleChara).ToArray();
            HavePet = Service.ObjectTable.Where(obj => obj != null && obj is BattleNpc npc && npc.BattleNpcKind == BattleNpcSubKind.Pet && npc.OwnerId == Service.ClientState.LocalPlayer.ObjectId).Count() > 0;

            AllianceMembers = Service.ObjectTable.Where(obj => obj is PlayerCharacter).Select(obj => (PlayerCharacter)obj).ToArray();

            PartyTanks = GetJobCategory(PartyMembers, Role.防护);

            DeathPeopleAll = BaseAction.GetObjectInRadius(GetDeath(AllianceMembers), 30);
            DeathPeopleParty = BaseAction.GetObjectInRadius(GetDeath(PartyMembers), 30);

            WeakenPeople = BaseAction.GetObjectInRadius(PartyMembers, 30).Where(p =>
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
                }
                return false;
            }).ToArray();
            #endregion
            #region Health
            var members = PartyMembers;

            PartyMembersHP = BaseAction.GetObjectInRadius(members, 30).Where(r => r.CurrentHp > 0).Select(p => (float)p.CurrentHp / p.MaxHp).ToArray();

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


            CanHealAreaAbility = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < Service.Configuration.HealthAreaAbility;
            CanHealAreaSpell = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < Service.Configuration.HealthAreafSpell;
            CanHealSingleAbility = PartyMembersHP.Min() < Service.Configuration.HealthSingleAbility;
            CanHealSingleSpell = PartyMembersHP.Min() < Service.Configuration.HealthSingleSpell;
            HPNotFull = PartyMembersHP.Min() < 1;
            #endregion

        }

        public unsafe static bool CanAttack(GameObject actor)
        {
            if (actor == null) return false;
            return ((delegate*<long, IntPtr, long>)_func)(142L, actor.Address) == 1;
        }


        internal static BattleChara[] GetJobCategory(BattleChara[] objects, Role role)
        {
            List<BattleChara> result = new(objects.Length);

            SortedSet<byte> validJobs = new(XIVAutoAttackPlugin.AllJobs.Where(job => job.Role == (byte)role).Select(job => (byte)job.RowId));

            foreach (var obj in objects)
            {
                if (GetJobCategory(obj, validJobs)) result.Add(obj);
            }
            return result.ToArray();
        }
        private static bool GetJobCategory(BattleChara obj, Role role)
        {

            SortedSet<byte> validJobs = new SortedSet<byte>(XIVAutoAttackPlugin.AllJobs.Where(job => job.Role == (byte)role).Select(job => (byte)job.RowId));
            return GetJobCategory(obj, validJobs);
        }

        internal static bool GetJobCategory(BattleChara obj, SortedSet<byte> validJobs)
        {
            return validJobs.Contains((byte)obj.ClassJob.GameData?.RowId);
        }

        internal static Action GetActionsByName(string name)
        {
            var enumerator = Service.DataManager.GetExcelSheet<Action>().GetEnumerator();

            while (enumerator.MoveNext())
            {
                var action = enumerator.Current;
                if (action.Name == name && action.ClassJobLevel != 0 && !action.IsPvP)
                {
                    return action;
                }
            }
            return null;
        }

        internal static BattleChara GetDeathPeople()
        {
            var deathAll = DeathPeopleAll;
            var deathParty = DeathPeopleParty;


            if (deathParty.Length != 0)
            {
                //确认一下死了的T有哪些。

                var deathT = GetJobCategory(deathParty, Role.防护);
                int TCount = PartyTanks.Length;

                //如果全死了，赶紧复活啊。
                if (TCount > 0 && deathT.Length == TCount)
                {
                    return deathT[0];
                }

                //确认一下死了的H有哪些。
                var deathH = GetJobCategory(deathParty, Role.治疗);

                //如果H死了，就先救他。
                if (deathH.Length != 0) return deathH[0];

                //如果T死了，就再救他。
                if (deathT.Length != 0) return deathT[0];

                //T和H都还活着，那就随便救一个。
                return deathParty[0];
            }

            //如果一个都没死，那为啥还要救呢？
            if (deathAll.Length == 0) return null;

            //确认一下死了的H有哪些。
            var deathAllH = GetJobCategory(deathAll, Role.治疗);
            if (deathAllH.Length != 0) return deathAllH[0];

            //确认一下死了的T有哪些。
            var deathAllT = GetJobCategory(deathAll, Role.防护);
            if (deathAllT.Length != 0) return deathAllT[0];

            return deathAll[0];
        }

        private static BattleChara[] GetDeath(BattleChara[] charas)
        {
            List<BattleChara> list = new List<BattleChara>(charas.Length);
            foreach (var item in charas)
            {
                //如果还有血，就算了。
                if (item.CurrentHp != 0) continue;

                //如果已经有复活的Buff了，那就算了。
                bool haveRase = false;
                foreach (var status in item.StatusList)
                {
                    if (status.StatusId == ObjectStatus.Raise)
                    {
                        haveRase = true;
                        break;
                    }
                }
                if (haveRase) continue;

                //如果有人在对着他咏唱，那就算了。
                bool isCasting = false;
                foreach (var character in AllianceMembers)
                {
                    if (character.CastTargetObjectId == item.ObjectId)
                    {
                        isCasting = true;
                        break;
                    }
                }
                if (isCasting) continue;

                list.Add(item);
            }
            return list.ToArray();
        }

        internal static unsafe uint[] GetEnemies()
        {
            var addonByName = Service.GameGui.GetAddonByName("_EnemyList", 1);
            if (addonByName != IntPtr.Zero)
            {
                var addon = (AddonEnemyList*)addonByName;
                var numArray = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetRaptureAtkModule()->AtkModule.AtkArrayDataHolder.NumberArrays[19];
                List<uint> list = new List<uint>(addon->EnemyCount);
                for (var i = 0; i < addon->EnemyCount; i++)
                {
                    //var enemyChara = FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager.Instance()->LookupBattleCharaByObjectId(numArray->IntArray[8 + i * 6]);
                    //if (enemyChara is null) continue;
                    list.Add((uint)numArray->IntArray[8 + i * 6]);
                }
                return list.ToArray();
            }
            return new uint[0];
        }

        /// <summary>
        /// 返回总共能大约回复的血量，非常大概。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        internal static float GetBestHeal(Action action, uint strength)
        {
            float healRange = strength * 0.000352f;

            //能够放到技能的队员。
            var canGet = BaseAction.GetObjectInRadius(PartyMembers, Math.Max(action.Range, 0.1f));

            float bestHeal = 0;
            foreach (var member in canGet)
            {
                float thisHeal = 0;
                Vector3 centerPt = member.Position;
                foreach (var ran in PartyMembers)
                {
                    //如果不在范围内，那算了。
                    if (Vector3.Distance(centerPt, ran.Position) > action.EffectRange)
                    {
                        continue;
                    }

                    thisHeal += Math.Min(1 - ran.CurrentHp / ran.MaxHp, healRange);
                }

                bestHeal = Math.Max(thisHeal, healRange);
            }
            return bestHeal;
        }

        public static void Dispose()
        {
            _weaponDelayStopwatch.Stop();
            _fisherTimer.Stop();
            _unfishingTimer.Stop();
        }
    }
}
