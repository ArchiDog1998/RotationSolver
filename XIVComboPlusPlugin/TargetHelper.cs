using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVComboPlus.Combos;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace XIVComboPlus
{
    internal class TargetHelper
    {
        internal enum GetTargetFunction
        {
            LowHP,
            FaceDirction,
            MajorTank,
            Esuna,
            Provoke,
            Melee,
            Range,
            Interrupt,
        }

        private static IntPtr _func;

        private static Vector3 _lastPosition;
        public static bool IsMoving { get; private set; }

        private static SortedList<uint, GetTargetFunction> _specialGetTarget = new SortedList<uint, GetTargetFunction>()
        {
            //////以太步，找面前的友军。
            //{BLMCombo.Actions.AetherialManipulation.ActionID, GetTargetFunction.FaceDirction},
            //{WHMCombo.Actions.Aquaveil.ActionID, GetTargetFunction.MajorTank },
            //{WHMCombo.Actions.DivineBenison.ActionID, GetTargetFunction.MajorTank },
            //{CustomCombo.GeneralActions.Esuna.ActionID, GetTargetFunction.Esuna},
            //{BRDCombo.Actions.WardensPaean.ActionID, GetTargetFunction.Esuna },
            //{CustomCombo.GeneralActions.Provoke.ActionID, GetTargetFunction.Provoke },
            //{WARCombo.Actions.Tomahawk.ActionID, GetTargetFunction.Provoke },
            ////{WARCombo.Actions.NascentFlash.ActionID, GetTargetFunction.MajorTank },

            //{ASTCombo.Actions.Balance.ActionID, GetTargetFunction.Melee },
            //{ASTCombo.Actions.Arrow.ActionID, GetTargetFunction.Melee },
            //{ASTCombo.Actions.Spear.ActionID, GetTargetFunction.Melee },
            //{ASTCombo.Actions.Bole.ActionID, GetTargetFunction.Range },
            //{ASTCombo.Actions.Ewer.ActionID, GetTargetFunction.Range },
            //{ASTCombo.Actions.Spire.ActionID, GetTargetFunction.Range },
            //{ASTCombo.Actions.Exaltation.ActionID, GetTargetFunction.MajorTank},

            //{CustomCombo.GeneralActions.Interject.ActionID, GetTargetFunction.Interrupt },
            //{CustomCombo.GeneralActions.LowBlow.ActionID, GetTargetFunction.Interrupt },
            //{CustomCombo.GeneralActions.LegSweep.ActionID, GetTargetFunction.Interrupt },
            //{CustomCombo.GeneralActions.HeadGraze.ActionID, GetTargetFunction.Interrupt },

            //{DRGCombo.Actions.DragonSight.ActionID, GetTargetFunction.Melee },
        };

        //All Targes
        internal static BattleChara[] AllTargets { get; private set; } = new BattleChara[0];

        internal static BattleChara[] HostileTargets { get; private set; } = new BattleChara[0];
        internal static BattleChara[] CanInterruptTargets { get; private set; } = new BattleChara[0];

        internal static bool HaveTargetAngle { get; private set; } = false;

        internal static float WeaponRemain { get; private set; } = 0;
        internal static float WeaponTotal { get; private set; } = 0;

        internal static byte AbilityRemainCount { get; private set; } = 0;

        private const float WeaponInterval = 0.68f;
        //  public static PlayerCharacter[] PartyMembers =>
        //AllianceMembers.Where(fri => (fri.StatusFlags & StatusFlags.AllianceMember) != 0).ToArray();
        public static BattleChara[] PartyMembers { get; private set; } = new PlayerCharacter[0];


        /// <summary>
        /// 玩家们
        /// </summary>
        internal static BattleChara[] AllianceMembers { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyHealers { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyMelee { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyRange { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyTanks { get; private set; } = new PlayerCharacter[0];
        internal static BattleChara[] PartyTanksAttached { get; private set; } = new PlayerCharacter[0];
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
        internal static bool HPNotFull { get; private set; } = false;


        internal static void Init(SigScanner sigScanner)
        {
            _func = sigScanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3 ");
        }

        internal unsafe static void Framework_Update(Framework framework)
        {
            try
            {
                Vector3 thisPosition = Service.ClientState.LocalPlayer.Position;
                IsMoving = Vector3.Distance(_lastPosition, thisPosition) != 0;
                _lastPosition = thisPosition;

                #region Hostile
                AllTargets = Service.ObjectTable.Where(obj => obj is BattleChara && ((BattleChara)obj).CurrentHp != 0 && CanAttack(obj)).Select(obj => (BattleChara)obj).ToArray();

                uint[] ids = GetEnemies();
                var hosts = AllTargets.Where(t => t.TargetObject?.IsValid() ?? false || ids.Contains(t.ObjectId)).ToArray();
                HostileTargets = hosts.Length == 0 ? AllTargets : hosts;

                CanInterruptTargets = HostileTargets.Where(tar => tar.IsCasting && tar.IsCastInterruptible).ToArray();

                HaveTargetAngle = BaseAction.GetObjectInRadius(HostileTargets, 25).Length > 0;
                #endregion

                unsafe
                {
                    var instance = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance();
                    var spell = FFXIVClientStructs.FFXIV.Client.Game.ActionType.Spell;

                    WeaponTotal = instance->GetRecastTime(spell, 11);
                    WeaponRemain = WeaponTotal - instance->GetRecastTimeElapsed(spell, 11);

                    AbilityRemainCount = (byte)(WeaponRemain / WeaponInterval);
                }

                #region Friend
                var party = Service.PartyList;
                PartyMembers = party.Length == 0 ? new BattleChara[] { Service.ClientState.LocalPlayer } :
                    party.Where(obj => obj != null && obj.GameObject is BattleChara).Select(obj => obj.GameObject as BattleChara).ToArray();

                AllianceMembers = Service.ObjectTable.Where(obj => obj is PlayerCharacter).Select(obj => (PlayerCharacter)obj).ToArray();
                //PartyMembers = AllianceMembers.Where(fri => (fri.StatusFlags & StatusFlags.AllianceMember) != 0).Union(new PlayerCharacter[] { Service.ClientState.LocalPlayer }).ToArray();

                PartyHealers = GetJobCategory(PartyMembers, Role.治疗);
                PartyMelee = GetJobCategory(PartyMembers, Role.近战);
                PartyRange = GetJobCategory(PartyMembers, Role.远程);
                PartyTanks = GetJobCategory(PartyMembers, Role.防护);

                List<BattleChara> attachedT = new List<BattleChara>(PartyTanks.Length);
                foreach (var tank in PartyTanks)
                {
                    if (tank.TargetObject?.TargetObject == tank)
                    {
                        attachedT.Add(tank);
                    }
                }
                PartyTanksAttached = attachedT.ToArray();


                DeathPeopleAll = BaseAction.GetObjectInRadius(GetDeath(AllianceMembers), 30);
                DeathPeopleParty = BaseAction.GetObjectInRadius(GetDeath(PartyMembers), 30);

                WeakenPeople = PartyMembers.Where(p =>
                {
                    foreach (var status in p.StatusList)
                    {
                        if (status.GameData.CanDispel) return true;
                    }
                    return false;
                }).ToArray();

                DyingPeople = PartyMembers.Where(p =>
                {
                    foreach (var status in p.StatusList)
                    {
                        if (status.StatusId == ObjectStatus.Doom) return true;
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
            catch (Exception ex)
            {
                Service.ChatGui.PrintError(ex.Message);
            }

            if (Service.ClientState.LocalPlayer.CurrentHp == 0) return;
            if (WeaponRemain < 0.05) Service.IconReplacer.DoAnAction();
            //要超出GCD了，那就不放技能了。
            else if (WeaponRemain + WeaponInterval > WeaponTotal || Service.ClientState.LocalPlayer.IsCasting) return;
            if (WeaponRemain % WeaponInterval < 0.05) Service.IconReplacer.DoAnAction();
        }


        public unsafe static bool CanAttack(GameObject actor)
        {
            if (actor == null) return false;
            return ((delegate*<long, IntPtr, long>)_func)(142L, actor.Address) == 1;
        }



        internal static void SetTarget(GameObject? obj)
        {
            Service.TargetManager.SetTarget(obj);
        }
        internal static float DistanceToPlayer(GameObject obj)
        {
            return Vector3.Distance(Service.ClientState.LocalPlayer.Position, obj.Position);
        }


        internal static BattleChara[] GetJobCategory(BattleChara[] objects, Role role)
        {
            List<BattleChara> result = new (objects.Length);

            SortedSet<byte> validJobs = new (XIVComboPlusPlugin.AllJobs.Where(job => job.Role == (byte)role).Select(job => (byte)job.RowId));

            foreach (var obj in objects)
            {
                if (GetJobCategory(obj, validJobs)) result.Add(obj);
            }
            return result.ToArray();
        }
        private static bool GetJobCategory(BattleChara obj, Role role)
        {
            
            SortedSet<byte> validJobs = new SortedSet<byte>(XIVComboPlusPlugin.AllJobs.Where(job => job.Role == (byte)role).Select(job => (byte)job.RowId));
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
        //private static GameObject ASTMeleeTarget(float range, BattleChara[] ASTTargets)
        //{

        //    var targets = GetObjectInRadius(GetJobCategory(ASTTargets, Role.近战), range);
        //    if (targets.Length > 0) return RandomObject(targets);

        //    targets = GetObjectInRadius(GetJobCategory(ASTTargets, Role.远程), range);
        //    if (targets.Length > 0) return RandomObject(targets);

        //    targets = GetObjectInRadius(ASTTargets, range);
        //    if (targets.Length > 0) return RandomObject(targets);

        //    return null;
        //}
        //private static BattleChara[] GetASTTargets()
        //{
        //    var allStatus = new uint[] 
        //    {
        //        ObjectStatus.TheArrow,
        //        ObjectStatus.TheBalance,
        //        ObjectStatus.TheBole,
        //        ObjectStatus.TheEwer,
        //        ObjectStatus.TheSpear,
        //        ObjectStatus.TheSpire,

        //    };
        //    return PartyMembers.Where((t) =>
        //    {
        //        foreach (Status status in t.StatusList)
        //        {
        //            if(allStatus.Contains(status.StatusId))
        //            {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }).ToArray();
        //}
        //private static GameObject ASTRangeTarget(float range, BattleChara[] ASTTargets)
        //{

        //    var targets = GetObjectInRadius(GetJobCategory(ASTTargets, Role.远程), range);
        //    if (targets.Length > 0) return RandomObject(targets);

        //    targets = GetObjectInRadius(GetJobCategory(ASTTargets, Role.近战), range);
        //    if (targets.Length > 0) return RandomObject(targets);

        //    targets = GetObjectInRadius(ASTTargets, range);
        //    if (targets.Length > 0) return RandomObject(targets);

        //    return null;
        //}

        //private static GameObject RandomObject(GameObject[] objs)
        //{
        //    Random ran = new Random(DateTime.Now.Millisecond);
        //    return objs[ran.Next(objs.Length)];
        //}

        //internal static GameObject GetBestTarget(Action act)
        //{
        //    //如果都没有距离，这个还需要选对象嘛？选原来的对象啊！
        //    if (act.Range == 0) return Service.TargetManager.Target ?? Service.ClientState.LocalPlayer;


        //    //首先看看是不是能对小队成员进行操作的。
        //    if (act.CanTargetParty)
        //        //if (act.CanTargetParty || act.RowId == WHMCombo.Actions.Asylum.ActionID)
        //    {
        //        //还消耗2400的蓝，那肯定是复活的。
        //        if (act.PrimaryCostType == 3 && act.PrimaryCostValue == 24)
        //        {
        //            return GetDeathPeople();
        //        }

        //        //找到没死的队友们。
        //        BattleChara[] availableCharas = PartyMembers.Where(player => player.CurrentHp != 0).ToArray();

        //        //if (!act.CanTargetSelf && act.RowId != WHMCombo.Actions.Asylum.ActionID)
        //        if (!act.CanTargetSelf)
        //            {
        //            availableCharas = availableCharas.Where(p => p.ObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();
        //        }

        //        //判断是否是范围。
        //        if (act.CastType > 1)
        //        {
        //            //找到能覆盖最多的位置，并且选血最少的来。
        //            return GetMostObjectInRadius(availableCharas, act.Range, act.EffectRange, false).OrderBy(p => (float)p.CurrentHp / p.MaxHp).First();
        //        }
        //        else
        //        {
        //            //bool shouldHeal = GetDangerousTanks(GetObjectInRadius(PartyTanks, act.Range), out var dangeriousTank);
        //            availableCharas = GetObjectInRadius(availableCharas, act.Range);

        //            //如果是特殊需求的话。
        //            if (_specialGetTarget.ContainsKey(act.RowId))
        //            {
        //                switch (_specialGetTarget[act.RowId])
        //                {
        //                    //如果特殊需求就是选最少的血量，那就跳过，到最后再处理。
        //                    case GetTargetFunction.LowHP:
        //                    default:
        //                        break;

        //                    case GetTargetFunction.Melee:
        //                        return ASTMeleeTarget(GetRange(act), GetASTTargets());

        //                    case GetTargetFunction.Range:
        //                        return ASTRangeTarget(GetRange(act), GetASTTargets());

        //                        //找到面前夹角30度中最远的那个目标。
        //                    case GetTargetFunction.FaceDirction:

        //                        //把T去掉，省的突然暴毙。
        //                        availableCharas = GetJobCategory(availableCharas, Role.防护);

        //                        Vector3 pPosition = Service.ClientState.LocalPlayer.Position;
        //                        float rotation = Service.ClientState.LocalPlayer.Rotation;
        //                        Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

        //                        return  availableCharas.Where(t =>
        //                        {
        //                            Vector3 dir = t.Position - pPosition;
        //                            Vector2 dirVec = new Vector2(dir.Z, dir.X);
        //                            double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());
        //                            return angle <= Math.PI / 6;
        //                        }).OrderBy(t => Vector3.Distance(t.Position, pPosition)).Last();

        //                        //找到被打的坦克中血最少的那个。
        //                    case GetTargetFunction.MajorTank:
        //                        var tanks = PartyTanksAttached;
        //                        if(tanks.Length == 0)
        //                        {
        //                            tanks = PartyTanks;
        //                        }
        //                        return  tanks.OrderBy(player => (float)player.CurrentHp / player.MaxHp).First();

        //                    //    //天赐给那个要死了的人！
        //                    //case GetTargetFunction.DangeriousTank:
        //                    //    if (WHMCombo.UseBenediction())
        //                    //    {
        //                    //        tanks = GetDangerousTanks(out _);
        //                    //        if (tanks.Length > 0) return tanks[0];
        //                    //    }
        //                    //    break;

        //                    case GetTargetFunction.Esuna:
        //                        availableCharas = GetObjectInRadius(DyingPeople, act.Range);

        //                        if (availableCharas.Length != 0) return availableCharas[0];
        //                        availableCharas = GetObjectInRadius(WeakenPeople, act.Range);

        //                        if (availableCharas.Length != 0) return availableCharas[0];
        //                        return PartyMembers[0];
        //                }
        //            }

        //            ////如果不用治疗，那就不治疗。
        //            //if (!shouldHeal)
        //            //{
        //            //    availableCharas = availableCharas.Except(dangeriousTank).ToArray();
        //            //}

        //            //选血量最少的那个。
        //            return availableCharas.OrderBy(player => (float)player.CurrentHp / player.MaxHp).First();

        //        }
        //    }
        //    //再看看是否可以选中敌对的。
        //    else if (act.CanTargetHostile)
        //    {
        //        switch (act.CastType)
        //        {
        //            case 1:
        //            default:

        //                BattleChara[] canReachTars = new BattleChara[0];
        //                //如果是特殊需求的话。
        //                if (_specialGetTarget.ContainsKey(act.RowId))
        //                {
        //                    switch (_specialGetTarget[act.RowId])
        //                    {
        //                        case GetTargetFunction.Provoke:
        //                            canReachTars = GetObjectInRadius(ProvokeTarget(out _), GetRange(act));
        //                            break;
        //                        case GetTargetFunction.Interrupt:
        //                            canReachTars = CanInterruptTargets;
        //                            break;

        //                    }
        //                }

        //                //找到能打到的怪。
        //                if(canReachTars.Length == 0) canReachTars = GetObjectInRadius(HostileTargets, GetRange(act));


        //                //判断一下要选择打体积最大的，还是最小的。
        //                if (Service.Configuration.IsTargetBoss)
        //                {
        //                    canReachTars = canReachTars.OrderByDescending(player => player.HitboxRadius).ToArray();
        //                }
        //                else
        //                {
        //                    canReachTars = canReachTars.OrderBy(player => player.HitboxRadius).ToArray();
        //                }

        //                //找到体积一样小的
        //                List<BattleChara> canGet = new List<BattleChara>(canReachTars.Length) { canReachTars[0] };

        //                float radius = canReachTars[0].HitboxRadius;
        //                for (int i = 1; i < canReachTars.Length; i++)
        //                {
        //                    if(canReachTars[i].HitboxRadius == radius)
        //                    {
        //                        canGet.Add(canReachTars[i]);
        //                    }
        //                    else break;
        //                }

        //                return canGet.OrderBy(player => Vector3.Distance(player.Position, Service.ClientState.LocalPlayer.Position)).First();

        //            case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
        //                return GetMostObjectInRadius(HostileTargets, GetRange(act), act.EffectRange, false).OrderByDescending(p => (float)p.CurrentHp / p.MaxHp).First();

        //            case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
        //                return GetMostObjectInArc(HostileTargets, act.EffectRange, false).OrderByDescending(p => Vector3.Distance(Service.ClientState.LocalPlayer.Position, p.Position)).First();

        //            case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
        //                return GetMostObjectInLine(HostileTargets, GetRange(act), false).OrderByDescending(p => Vector3.Distance(Service.ClientState.LocalPlayer.Position, p.Position)).First();

        //        }
        //    }
        //    else if (act.CanTargetSelf)
        //    {
        //        return Service.ClientState.LocalPlayer;
        //    }
        //    //那么这个就不需要找到目标了，要么对着自己，要么就什么都不能选中。
        //    else
        //    {
        //        return Service.TargetManager.Target ?? Service.ClientState.LocalPlayer;
        //    }
        //}



        //internal static float GetRange(Action act)
        //{
        //    sbyte range = act.Range;
        //    if (range < 0 && GetJobCategory(Service.ClientState.LocalPlayer, Role.远程))
        //    {
        //        range = 25;
        //    }
        //    return Math.Max(range, 3f);
        //}

        //internal static bool ActionGetATarget(Action act, bool isFriendly)
        //{
        //    //如果根本就不需要找目标，那肯定可以的。
        //    if (!act.CanTargetFriendly && !act.CanTargetHostile && (act.CastType == 1 ||act.CastType > 4)) return true;

        //    ////如果在打Boss呢，那就不需要考虑AOE的问题了。
        //    //if (Service.Configuration.IsTargetBoss && !isFriendly && act.CastType != 1) return false;

        //    BattleChara[] tar = isFriendly ? PartyMembers : HostileTargets;

        //    switch (act.CastType)
        //    {
        //        case 1: // 单体啊
        //            //是个救人啊！
        //            if (isFriendly && act.PrimaryCostType == 3 && act.PrimaryCostValue == 24)
        //            {
        //                return GetDeathPeople() != null;
        //            }
        //            if (_specialGetTarget.ContainsKey(act.RowId))
        //            {
        //                switch (_specialGetTarget[act.RowId])
        //                {
        //                    case GetTargetFunction.Interrupt:
        //                        tar = CanInterruptTargets;
        //                        break;
        //                }
        //            }
        //            return GetObjectInRadius(tar, GetRange(act)).Count() > 0;
        //        case 2: // 圆形范围攻击，看看人数够不够。

        //            if (act.CanTargetHostile)
        //            {
        //                return GetMostObjectInRadius(tar, GetRange(act), act.EffectRange, true).Count() > 0;
        //            }
        //            else
        //            {
        //                return GetMostObjectInRadius(tar, new PlayerCharacter[] {Service.ClientState.LocalPlayer}, GetRange(act), act.EffectRange, true).Count() > 0;
        //            }

        //        case 3: // 扇形范围攻击。看看人数够不够。
        //            return GetMostObjectInArc(tar, act.EffectRange, true).Count() > 0;

        //        case 4: //直线范围攻击。看看人数够不够。
        //            return GetMostObjectInLine(tar, GetRange(act), true).Count() > 0;
        //    }
        //    return true;
        //}

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
                var addon = (FFXIVClientStructs.FFXIV.Client.UI.AddonEnemyList*)addonByName;
                var numArray = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetRaptureAtkModule()->AtkModule.AtkArrayDataHolder.NumberArrays[19];
                List<uint> list = new List<uint>(addon->EnemyCount);
                for (var i = 0; i < addon->EnemyCount; i++)
                {
                    //var enemyChara = FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager.Instance()->LookupBattleCharaByObjectId(numArray->IntArray[8 + i * 6]);
                    //if (enemyChara is null) continue;
                    list.Add((uint  )numArray->IntArray[8 + i * 6]);
                }
                return list.ToArray();
            }
            return new uint[0];
        }

        ///// <summary>
        ///// 获得玩家某范围内的所有怪。
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="objects"></param>
        ///// <param name="radius"></param>
        ///// <returns></returns>
        //internal static T[] GetObjectInRadius<T>(T[] objects, float radius, bool needAddHitbox = true) where T : GameObject
        //{
        //    return objects.Where(o => DistanceToPlayer(o) <= (needAddHitbox ? radius + o.HitboxRadius : radius)).ToArray();
        //}

        //private static T[] GetMostObject<T>(T[] canAttack, float radius, float range, Func<T, T[], float, byte> HowMany, bool forCheck) where T : BattleChara
        //{
        //    //能够打到的所有怪。
        //    T[] canGetObj = GetObjectInRadius(canAttack, radius);
        //    return GetMostObject(canAttack, canGetObj, range, HowMany, forCheck);
        //}

        //private static T[] GetMostObject<T>(T[] canAttack, T[] canGetObj ,float range, Func<T, T[], float, byte> HowMany, bool forCheck) where T : BattleChara
        //{

        //    //能打到MaxCount以上数量的怪的怪。
        //    List<T> objectMax = new List<T>(canGetObj.Length);

        //    int maxCount = Service.Configuration.HostileCount;

        //    //循环能打中的目标。
        //    foreach (var t in canGetObj)
        //    {
        //        //计算能达到的所有怪的数量。
        //        byte count = HowMany(t, canAttack, range);

        //        //如果只是检查一下，有了就可以别算了。
        //        if(forCheck && count >= maxCount)
        //        {
        //            objectMax.Add(t);
        //            break;
        //        }

        //        if (count == maxCount)
        //        {
        //            objectMax.Add(t);
        //        }
        //        else if (count > maxCount)
        //        {
        //            maxCount = count;
        //            objectMax.Clear();
        //            objectMax.Add(t);
        //        }
        //    }

        //    return objectMax.ToArray();
        //}

        //internal static T[] GetMostObjectInRadius<T>(T[] objects, float radius, float range, bool forCheck) where T : BattleChara
        //{
        //    //可能可以被打到的怪。
        //    var canAttach = GetObjectInRadius(objects, radius + range);
        //    //能够打到的所有怪。
        //    var canGet = GetObjectInRadius(objects, radius);

        //    return GetMostObjectInRadius(canAttach, canGet, radius, range, forCheck);

        //}

        //internal static T[] GetMostObjectInRadius<T>(T[] objects, T[] canGetObjects, float radius, float range, bool forCheck) where T : BattleChara
        //{
        //    var canAttach = GetObjectInRadius(objects, radius + range);

        //    return GetMostObject(canAttach, canGetObjects,  range, CalculateCount, forCheck);

        //    //计算一下在这些可选目标中有多少个目标可以受到攻击。
        //    static byte CalculateCount(T t, T[] objects, float range)
        //    {
        //        byte count = 0;
        //        foreach (T obj in objects)
        //        {
        //            if (Vector3.Distance(t.Position, obj.Position) <= range)
        //            {
        //                count++;
        //            }
        //        }
        //        return count;
        //    }
        //}

        //internal static T[] GetMostObjectInArc<T>(T[] objects, float radius, bool forCheck) where T : BattleChara
        //{
        //    //能够打到的所有怪。
        //    var canGet = GetObjectInRadius(objects, radius, false);

        //    return GetMostObject(canGet, radius, radius, CalculateCount, forCheck);

        //    //计算一下在这些可选目标中有多少个目标可以受到攻击。
        //    static byte CalculateCount(T t, T[] objects, float _)
        //    {
        //        byte count = 0;

        //        Vector3 dir = t.Position - Service.ClientState.LocalPlayer.Position;

        //        foreach (T obj in objects)
        //        {
        //            Vector3 tdir = obj.Position - Service.ClientState.LocalPlayer.Position;

        //            double cos = Vector3.Dot(dir, tdir)/(dir.Length() * tdir.Length());
        //            if (cos >= 0.5)
        //            {
        //                count++;
        //            }
        //        }
        //        return count;
        //    }
        //}

        //private static T[] GetMostObjectInLine<T>(T[] objects, float radius, bool forCheck) where T : BattleChara
        //{
        //    //能够打到的所有怪。
        //    var canGet = GetObjectInRadius(objects, radius);

        //    return GetMostObject(canGet, radius, radius, CalculateCount, forCheck);

        //    //计算一下在这些可选目标中有多少个目标可以受到攻击。
        //    static byte CalculateCount(T t, T[] objects, float _)
        //    {
        //        byte count = 0;

        //        Vector3 dir = t.Position - Service.ClientState.LocalPlayer.Position;

        //        foreach (T obj in objects)
        //        {
        //            Vector3 tdir = obj.Position - Service.ClientState.LocalPlayer.Position;

        //            double distance = Vector3.Cross(dir, tdir).Length()/dir.Length();
        //            if (distance <= 2)
        //            {
        //                count++;
        //            }
        //        }
        //        return count;
        //    }
        //}

        ///// <summary>
        ///// 返回总共能大约回复的血量，非常大概。
        ///// </summary>
        ///// <param name="action"></param>
        ///// <param name="strength"></param>
        ///// <returns></returns>
        //internal static float GetBestHeal(Action action, uint strength)
        //{
        //    float healRange = strength * 0.000352f;

        //    //能够放到技能的队员。
        //    var canGet = GetObjectInRadius(PartyMembers, Math.Max(action.Range, 0.1f));

        //    float bestHeal = 0;
        //    foreach (var member in canGet)
        //    {
        //        float thisHeal = 0;
        //        Vector3 centerPt = member.Position;
        //        foreach (var ran in PartyMembers)
        //        {
        //            //如果不在范围内，那算了。
        //            if(Vector3.Distance(centerPt, ran.Position) > action.EffectRange)
        //            {
        //                continue;
        //            }

        //            thisHeal += Math.Min(1 - ran.CurrentHp / ran.MaxHp, healRange);
        //        }

        //        bestHeal = Math.Max(thisHeal, healRange);
        //    }
        //    return bestHeal;
        //}
    }
}
