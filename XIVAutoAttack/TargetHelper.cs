using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVAutoAttack
{
    internal class TargetHelper
    {
        //private static Vector3 _lastPosition;

        private static readonly Stopwatch _weaponDelayStopwatch = new Stopwatch();
        private static readonly Stopwatch _weaponAbilityStopwatch = new Stopwatch();

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
        internal static BattleChara[] TarOnMeTargets { get; private set; } = new BattleChara[0];
        internal static BattleChara[] CanInterruptTargets { get; private set; } = new BattleChara[0];

        internal static bool HaveHostileInRange { get; private set; } = false;

        internal static float WeaponRemain { get; private set; } = 0;
        internal static float WeaponTotal { get; private set; } = 0;
        internal static float Weaponelapsed { get; private set; } = 0;
        internal static bool InBattle { get; private set; } = false;
        internal static byte AbilityRemainCount { get; private set; } = 0;

        public static BattleChara[] PartyMembers { get; private set; } = new PlayerCharacter[0];
        /// <summary>
        /// 玩家们
        /// </summary>
        internal static BattleChara[] AllianceMembers { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyTanks { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyHealers { get; private set; } = new PlayerCharacter[0];

        internal static BattleChara[] AllianceTanks { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] DeathPeopleAll { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] DeathPeopleParty { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] WeakenPeople { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] DyingPeople { get; private set; } = new PlayerCharacter[0];
        internal static float[] PartyMembersHP { get; private set; } = new float[0];
        internal static float PartyMembersMinHP { get; private set; } = 0;
        internal static float PartyMembersAverHP { get; private set; } = 0;
        internal static float PartyMembersDifferHP { get; private set; } = 0;

        internal static bool CanHealAreaAbility { get; private set; } = false;
        internal static bool CanHealAreaSpell { get; private set; } = false;
        internal static bool CanHealSingleAbility { get; private set; } = false;
        internal static bool CanHealSingleSpell { get; private set; } = false;
        internal static bool HavePet { get; private set; } = false;
        internal static bool HPNotFull { get; private set; } = false;
        internal static bool ShouldUseArea { get; private set; } = false;


        public static bool IsHostileAOE { get; private set; } = false;
        public static bool IsHostileTank { get; private set; } = false;
        internal static  uint[] BLUActions { get;} = new uint[24];

        internal static readonly Queue<MacroItem> Macros = new Queue<MacroItem>();
        internal static MacroItem DoingMacro;
        private static float _lastCastingTotal = 0;
#if DEBUG
        private static readonly Dictionary<int, bool> _valus = new Dictionary<int, bool>();
#endif
        private static bool restartCombatTimer = true;
        private static TimeSpan combatDuration = new();
        private static DateTime combatStart;
        private static DateTime combatEnd;
        [Obsolete]
        public static TimeSpan CombatEngageDuration => combatDuration;
        private static void UpdateCombatTime()
        {
            if (InBattle)
            {
                if (restartCombatTimer)
                {
                    restartCombatTimer = false;
                    combatStart = DateTime.Now;
                }

                combatEnd = DateTime.Now;
            }
            else
            {
                restartCombatTimer = true;
                combatDuration = TimeSpan.Zero;
            }

            combatDuration = combatEnd - combatStart;

        }

        internal unsafe static void Framework_Update(Framework framework)
        {
            if (!Service.Conditions.Any()) return;

            UpdateCastBar();
            UpdateCombatTime();
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
                    if (_valus.ContainsKey(i) && _valus[i] != newValue && indexs[i] != 48 && indexs[i] != 27)
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
            if (Fish != FishType.None && !Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Gathering])
            {
                Fish = FishType.None;
            }

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
                || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.RolePlaying]) 
                IconReplacer.AutoAttack = false;

            InBattle = Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat];

            var instance =ActionManager.Instance();
            var spell = ActionType.Spell;

            var weapontotal = instance->GetRecastTime(spell, 11);
            Weaponelapsed = instance->GetRecastTimeElapsed(spell, 11);
            WeaponRemain = Math.Max(weapontotal - Weaponelapsed, 
                Service.ClientState.LocalPlayer.TotalCastTime - Service.ClientState.LocalPlayer.CurrentCastTime);

            var min = Math.Max(weapontotal - Service.Configuration.WeaponInterval, 0);
            AbilityRemainCount = (byte)(Math.Min(WeaponRemain, min) / Service.Configuration.WeaponInterval);
            WeaponTotal = InBattle ? Math.Max(WeaponTotal, weapontotal) : 0;

            UpdateTargets();
            if(Service.ClientState.LocalPlayer.ClassJob.Id == 36)
            {
                for (int i = 0; i < 24; i++)
                {
                    BLUActions[i] = instance->GetActiveBlueMageActionInSlot(i);
                }
            }

            DoAction(Weaponelapsed);

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


            var castBar = Service.GameGui.GetAddonByName("_CastBar", 1);
            if (castBar == IntPtr.Zero) return;
            AtkResNode* progressBar = ((AtkUnitBase*)castBar)->UldManager.NodeList[5];

            bool canMove = !Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInEvent]
                && Service.Configuration.CheckForCasting && !Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Casting];

            ByteColor c = canMove ? greenColor : redColor;
            XIVAutoAttackPlugin.movingController.IsMoving = canMove;

            progressBar->AddRed = c.R;
            progressBar->AddGreen = c.G;
            progressBar->AddBlue = c.B;
        }

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

            //确定读条时间。
            if (weaponelapsed < 0.3)
            {
                //能力技就不用提前了。
                _lastCastingTotal = Service.ClientState.LocalPlayer.TotalCastTime;

                //补上读条税
                if (_lastCastingTotal > 0) _lastCastingTotal += 0.1f + Service.Configuration.WeaponFaster;
            }

            //要超出GCD了，那就不放技能了。
            if (WeaponRemain < Service.Configuration.WeaponInterval
                || weaponelapsed < Service.Configuration.WeaponInterval)
            {
                return;
            }

            //还在咏唱，就不放技能了。
            if (weaponelapsed <= _lastCastingTotal) return;

            if ((weaponelapsed - _lastCastingTotal) % Service.Configuration.WeaponInterval <= Service.Configuration.WeaponFaster)
            {
                Service.IconReplacer.DoAnAction(false);
                return;
            }
        }

        private static void UpdateTargets()
        {
            #region Hostile
            //能打的目标
            AllTargets = TargetFilter.GetTargetable(TargetFilter.GetObjectInRadius(Service.ObjectTable.ToArray(), 30).Where(obj =>
            {
                if (obj is BattleChara c && c.CurrentHp != 0)
                {
                    foreach (var status in c.StatusList)
                    {
                        if (Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>()
                        .GetRow(status.StatusId).Icon == 15024) return false;
                    }
                    if (CanAttack(obj)) return true;
                }
                return false;
            }).Select(obj => (BattleChara)obj).ToArray());
            uint[] ids = GetEnemies() ?? new uint[0];
            
            if (AllTargets != null && AllTargets.Length > 0)
            {
                HostileTargets = AllTargets.Where(t => t.TargetObject is PlayerCharacter || ids.Contains(t.ObjectId)).ToArray();

                switch (Service.Configuration.TargetToHostileType)
                {
                    case 0:
                        HostileTargets = AllTargets;
                        break;
                    default:
                    case 1:
                        if (HostileTargets.Length == 0)
                            HostileTargets = AllTargets;
                        break;

                    case 2:
                        break;
                }

                CanInterruptTargets = HostileTargets.Where(tar => tar.IsCasting && tar.IsCastInterruptible && tar.TotalCastTime >= 2).ToArray();
                TarOnMeTargets = HostileTargets.Where(tar => tar.TargetObjectId == Service.ClientState.LocalPlayer.ObjectId).ToArray();

                float radius = 25;
                switch (XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == Service.ClientState.LocalPlayer.ClassJob.Id).Role)
                {
                    case (byte)Role.防护:
                    case (byte)Role.近战:
                        radius = 3;
                        break;
                }
                HaveHostileInRange = TargetFilter.GetObjectInRadius(HostileTargets, radius).Length > 0;
            }
            else
            {
                AllTargets = HostileTargets = CanInterruptTargets = new BattleChara[0];
                HaveHostileInRange = false;
            }

            if (HostileTargets.Length == 1)
            {
                var tar = HostileTargets[0];

                IsHostileTank = IsHostileCastingTank(tar);
                IsHostileAOE = IsHostileCastingArea(tar);
            }
            #endregion

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

            if(PartyMembers.Length >= Service.Configuration.PartyCount)
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

            ShouldUseArea = PartyMembersHP.Count(t => t < 1) > Service.Configuration.PartyCount;
            #endregion
        }

        internal static bool IsHostileCastingTank(BattleChara h)
        {
            return IsHostileCastingBase(h, (act) =>
            {
                return h.CastTargetObjectId == h.TargetObjectId;
            });
        }

        internal static bool IsHostileCastingArea(BattleChara h)
        {
            return IsHostileCastingBase(h, (act) =>
            {
                if (h.CastTargetObjectId == h.TargetObjectId) return false;
                if ((act.CastType == 1 || act.CastType == 2) &&
                    act.Range == 0 &&
                    (act.EffectRange >= 40))
                    return true;
                return false;
            });
        }

        private static bool IsHostileCastingBase(BattleChara h, Func<Action, bool> check)
        {
            if (h.IsCasting)
            {
                if (h.IsCastInterruptible) return false;
                var last = h.TotalCastTime - h.CurrentCastTime;

                if (!(h.TotalCastTime > 2 && last < 6 && last > 0.5)) return false;

                var action = Service.DataManager.GetExcelSheet<Action>().GetRow(h.CastActionId);
                return check?.Invoke(action) ?? false;
            }
            return false;
        }

        public static bool CanAttack(GameObject actor)
        {
            return OjbectType(actor) == 1;
        }

        private unsafe static long OjbectType(GameObject actor)
        {
            if (actor == null) return long.MinValue;
            return ((delegate*<long, IntPtr, long>)Service.Address.CanAttackFunction)(142L, actor.Address);
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

        public static void Dispose()
        {
            _weaponDelayStopwatch.Stop();
            _fisherTimer.Stop();
            _unfishingTimer.Stop();
        }
    }
}
