using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVComboPlus.Attributes;
using XIVComboPlus.Combos;
using XIVComboPlus.Combos.BLM;
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
            DangeriousTank,
        }

        private static IntPtr _func;

        private static Vector3 _lastPosition;
        public static bool IsMoving { get; private set; }

        private static SortedList<uint, GetTargetFunction> _specialGetTarget = new SortedList<uint, GetTargetFunction>()
        {
            //以太步，找面前的友军。
            { 155u, GetTargetFunction.FaceDirction},
            {WHMCombo.Actions.Aquaveil.ActionID, GetTargetFunction.MajorTank },
            {WHMCombo.Actions.DivineBenison.ActionID, GetTargetFunction.MajorTank },
            {WHMCombo.Actions.Benediction.ActionID, GetTargetFunction.MajorTank },
        };

        //All Targes
        internal static BattleNpc[] AllTargets =>
        Service.ObjectTable.Where(obj => obj is BattleNpc && ((BattleNpc)obj).CurrentHp != 0 && ((BattleNpc)obj).BattleNpcKind == BattleNpcSubKind.Enemy && CanAttack(obj)).Select(obj => (BattleNpc)obj).ToArray();

        internal static BattleNpc[] HostileTargets => AllTargets.Where(t => t.TargetObject != null).ToArray();

        internal static BattleNpc[] Targets
        {
            get
            {
                var allTarges = AllTargets;
                var hosts = allTarges.Where(t => t.TargetObject != null).ToArray();
                return hosts.Length == 0? allTarges : hosts;
            }
        }

      //  public static PlayerCharacter[] PartyMembers =>
      //AllianceMembers.Where(fri => (fri.StatusFlags & StatusFlags.AllianceMember) != 0).ToArray();
        public static PlayerCharacter[] PartyMembers
        {
            get
            {
                if (Service.PartyList.Length == 0)
                {
                    return new PlayerCharacter[] { Service.ClientState.LocalPlayer };
                }
                else
                {
                    return Service.PartyList.Select(obj => obj.GameObject as PlayerCharacter).ToArray();

                }
            }
        }
        /// <summary>
        /// 玩家们
        /// </summary>
        public static PlayerCharacter[] AllianceMembers =>
             Service.ObjectTable.Where(obj => obj is PlayerCharacter).Select(obj => (PlayerCharacter)obj).ToArray();
        public static PlayerCharacter[] PartyHealers => GetJobCategory(PartyMembers, (jt) => jt == JobType.Healer);
        public static PlayerCharacter[] PartyDPS => GetJobCategory(PartyMembers, (jt) => jt == JobType.Melee || jt == JobType.MagicalRanged || jt == JobType.PhysicalRanged);
        public static PlayerCharacter[] PartyTanks => GetJobCategory(PartyMembers, (jt) => jt == JobType.Tank);
        public static PlayerCharacter[] PartyTanksAttached
        {
            get
            {
                var tanks = PartyTanks;

                List<PlayerCharacter> attachedT = new List<PlayerCharacter>(tanks.Length);
                foreach (var tank in tanks)
                {
                    if(tank.TargetObject.TargetObject == tank)
                    {
                        attachedT.Add(tank);
                    }
                }

                return attachedT.ToArray();
            }
        }

        public static PlayerCharacter[] DeathPeopleAll => GetObjectInRadius(GetDeath(AllianceMembers), 30);
        public static PlayerCharacter[] DeathPeopleParty => GetObjectInRadius(GetDeath(PartyMembers), 30);

        internal static void Init(SigScanner sigScanner)
        {
            _func = sigScanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3 ");
        }

        internal static void Framework_Update(Framework framework)
        {
            Vector3 thisPosition = Service.ClientState.LocalPlayer.Position;
            IsMoving = Vector3.Distance(_lastPosition, thisPosition) != 0;
            _lastPosition = thisPosition;
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
        private static float DistanceToPlayer(GameObject obj)
        {
            return Vector3.Distance(Service.ClientState.LocalPlayer.Position, obj.Position);
        }


        private static PlayerCharacter[] GetJobCategory(PlayerCharacter[] objects, Func<JobType, bool> check)
        {
            List<PlayerCharacter> result = new List<PlayerCharacter>(objects.Length);

            var validJobs = new SortedSet<byte>(ClassJob.AllJobs.Where(job => check(job.Type)).Select(job => job.Index));

            foreach (var obj in objects)
            {
                if (validJobs.Contains((byte)obj.ClassJob.GameData?.RowId))
                {
                    result.Add(obj);
                }
            }
            return result.ToArray();
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

        internal static GameObject GetBestTarget(Action act)
        {
            //如果都没有距离，这个还需要选对象嘛？
            if (act.Range == 0) return null;


            //首先看看是不是能对小队成员进行操作的。
            if (act.CanTargetParty)
            {
                //如果能选中队友，还消耗2400的蓝，那肯定是复活的。
                if (act.CanTargetFriendly && act.PrimaryCostType == 3 && act.PrimaryCostValue == 24)
                {
                    var deathAll = DeathPeopleAll;
                    var deathParty = DeathPeopleParty;

                    //如果一个都没死，那为啥还要救呢？
                    if (deathAll.Length == 0) return null;

                    if(deathParty.Length != 0)
                    {
                        //确认一下死了的T有哪些。

                        var deathT = GetJobCategory(deathParty, (j) => j == JobType.Tank);
                        int TCount = PartyTanks.Length;

                        //如果全死了，赶紧复活啊。
                        if (TCount == deathT.Length)
                        {
                            return deathT[0];
                        }

                        //确认一下死了的H有哪些。
                        var deathH = GetJobCategory(deathParty, (j) => j == JobType.Healer);

                        //如果H死了，就先救他。
                        if (deathH.Length != 0) return deathH[0];

                        //如果T死了，就再救他。
                        if (deathT.Length != 0) return deathH[0];

                        //T和H都还活着，那就随便救一个。
                        return deathParty[0];
                    }

                    //确认一下死了的H有哪些。
                    var deathAllH = GetJobCategory(deathAll, (j) => j == JobType.Healer);
                    if(deathAllH.Length != 0) return deathAllH[0];

                    //确认一下死了的T有哪些。
                    var deathAllT = GetJobCategory(deathAll, (j) => j == JobType.Tank);
                    if (deathAllT.Length != 0) return deathAllT[0];

                    return deathAll[0];
                }

                //找到没死的队友们。
                PlayerCharacter[] availableCharas = PartyMembers.Where(player => player.CurrentHp != 0).ToArray();

                if (!act.CanTargetSelf)
                {
                    availableCharas = availableCharas.Where(p => p.ObjectId != Service.ClientState.LocalContentId).ToArray();
                }

                //判断是否是范围。
                if (act.CastType > 1)
                {
                    //找到能覆盖最多的位置，并且选学最少的来。
                    return GetMostObjectInRadius(availableCharas, act.Range, act.EffectRange, false).OrderBy(p => (float)p.CurrentHp / p.MaxHp).First();
                }
                else
                {
                    availableCharas = GetObjectInRadius(availableCharas, act.Range);

                    //如果是特殊需求的话。
                    if (_specialGetTarget.ContainsKey(act.RowId))
                    {
                        switch (_specialGetTarget[act.RowId])
                        {
                            //如果特殊需求就是选最少的血量，那就跳过，到最后再处理。
                            case GetTargetFunction.LowHP:
                            default:
                                break;

                                //找到面前夹角30度中最远的那个目标。
                            case GetTargetFunction.FaceDirction:

                                //把T去掉，省的突然暴毙。
                                availableCharas = GetJobCategory(availableCharas, (jt) => !(jt == JobType.Tank));

                                Vector3 pPosition = Service.ClientState.LocalPlayer.Position;
                                float rotation = Service.ClientState.LocalPlayer.Rotation;
                                Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

                                return  availableCharas.Where(t =>
                                {
                                    Vector3 dir = t.Position - pPosition;
                                    Vector2 dirVec = new Vector2(dir.Z, dir.X);
                                    double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());
                                    return angle <= Math.PI / 6;
                                }).OrderBy(t => Vector3.Distance(t.Position, pPosition)).Last();

                                //找到被打的坦克中血最少的那个。
                            case GetTargetFunction.MajorTank:
                                var tanks = PartyTanksAttached;
                                if(tanks.Length == 0)
                                {
                                    tanks = PartyTanks;
                                }
                                return  tanks.OrderBy(player => (float)player.CurrentHp / player.MaxHp).First();

                                //天赐给那个要死了的人！
                            case GetTargetFunction.DangeriousTank:
                                if (WHMCombo.UseBenediction(out PlayerCharacter tank)) return tank;
                                tanks = PartyTanksAttached;
                                if (tanks.Length == 0)
                                {
                                    tanks = PartyTanks;
                                }
                                return tanks.OrderBy(player => (float)player.CurrentHp / player.MaxHp).First();
                        }
                    }

                    //选血量最少的那个。
                    return availableCharas.OrderBy(player => (float)player.CurrentHp / player.MaxHp).First();

                }
            }
            //再看看是否可以选中敌对的。
            else if (act.CanTargetHostile)
            {
                switch (act.CastType)
                {
                    case 1:
                    default:
                        //找到能打到的怪。
                        var canReachTars = GetObjectInRadius(Targets, Math.Max(act.Range, 3f));

                        //判断一下要选择打血最大的，还是最小的。
                        if (Service.Configuration.IsTargetBoss)
                        {
                            return canReachTars.OrderBy(player => player.HitboxRadius).Last();
                        }
                        else
                        {
                            return canReachTars.OrderBy(player => player.HitboxRadius).First();
                        }
                    case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
                        return GetMostObjectInRadius(Targets, act.Range, act.EffectRange, false).OrderByDescending(p => (float)p.CurrentHp / p.MaxHp).First();

                    case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        return GetMostObjectInArc(Targets, Math.Max(act.Range, 3f), false).OrderByDescending(p => Vector3.Distance(Service.ClientState.LocalPlayer.Position, p.Position)).First();

                    case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        return GetMostObjectInLine(Targets, Math.Max(act.Range, 3f), false).OrderByDescending(p => Vector3.Distance(Service.ClientState.LocalPlayer.Position, p.Position)).First();

                }
            }
            //那么这个就不需要找到目标了，要么对着自己，要么就什么都不能选中。
            else
            {
                return null;
            }
        }

        internal static bool ActionGetATarget(Action act, bool isFriendly)
        {
            //如果根本就不需要找目标，那肯定可以的。
            if (!act.CanTargetFriendly && !act.CanTargetHostile && act.CastType == 1) return true;

            //如果在打Boss呢，那就不需要考虑AOE的问题了。
            if (Service.Configuration.IsTargetBoss && !isFriendly && act.CastType > 1) return false;

            BattleChara[] tar = isFriendly ? PartyMembers : Targets;

            switch (act.CastType)
            {
                case 1:
                    return GetObjectInRadius(tar, Math.Max(act.Range, 3f)).Count() > 0;
                case 2: // 圆形范围攻击，看看人数够不够。
                    return GetMostObjectInRadius(tar, act.Range, act.EffectRange, true).Count() > 0;

                case 3: // 扇形范围攻击。看看人数够不够。
                    return GetMostObjectInArc(tar, Math.Max(act.Range, 3f), true).Count() > 0;

                case 4: //直线范围攻击。看看人数够不够。
                    return GetMostObjectInLine(tar, Math.Max(act.Range, 3f), true).Count() > 0;
            }
            return true;
        }

        private static PlayerCharacter[] GetDeath(PlayerCharacter[] charas)
        {
            List<PlayerCharacter> list = new List<PlayerCharacter>(charas.Length);
            foreach (var item in charas)
            {
                //如果还有血，就算了。
                if (item.CurrentHp != 0) continue;

                //如果已经有复活的Buff了，那就算了。
                bool haveRase = false;
                foreach (var status in item.StatusList)
                {
                    if (status.StatusId == ObjectStatus.Raise || status.StatusId == ObjectStatus.Resurrection)
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

        /// <summary>
        /// 获得玩家某范围内的所有怪。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        internal static T[] GetObjectInRadius<T>(T[] objects, float radius) where T : GameObject
        {
            return objects.Where(o => DistanceToPlayer(o) <= radius + o.HitboxRadius).ToArray();
        }

        private static T[] GetMostObject<T>(T[] canAttack, float radius, float range, Func<T, T[], float, byte> HowMany, bool forCheck) where T : BattleChara
        {
            //能够打到的所有怪。
            T[] canGetObj = GetObjectInRadius(canAttack, radius);


            //能打到MaxCount以上数量的怪的怪。
            List<T> objectMax = new List<T>(canGetObj.Length);

            int maxCount = Service.Configuration.MultiCount;

            //循环能打中的目标。
            foreach (var t in canGetObj)
            {
                //计算能达到的所有怪的数量。
                byte count = HowMany(t, canAttack, range);

                //如果只是检查一下，有了就可以别算了。
                if(forCheck && count >= maxCount)
                {
                    objectMax.Add(t);
                    break;
                }

                if (count == maxCount)
                {
                    objectMax.Add(t);
                }
                else if (count > maxCount)
                {
                    maxCount = count;
                    objectMax.Clear();
                    objectMax.Add(t);
                }
            }

            return objectMax.ToArray();
        }


        internal static T[] GetMostObjectInRadius<T>(T[] objects, float radius, float range, bool forCheck) where T : BattleChara
        {
            //可能可以蹭到的怪。
            var canAttach = GetObjectInRadius(objects, radius + range);

            return GetMostObject(canAttach, radius, range, CalculateCount, forCheck);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, T[] objects, float range)
            {
                byte count = 0;
                foreach (T obj in objects)
                {
                    if (Vector3.Distance(t.Position, obj.Position) <= range)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        private static T[] GetMostObjectInArc<T>(T[] objects, float radius, bool forCheck) where T : BattleChara
        {
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObject(objects, radius, radius, CalculateCount, forCheck);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, T[] objects, float _)
            {
                byte count = 0;

                Vector3 dir = t.Position - Service.ClientState.LocalPlayer.Position;

                foreach (T obj in objects)
                {
                    Vector3 tdir = obj.Position - Service.ClientState.LocalPlayer.Position;

                    double angle = Math.Acos(Vector3.Dot(dir, tdir)/(dir.Length() * tdir.Length()));
                    if (angle <= Math.PI/3)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        private static T[] GetMostObjectInLine<T>(T[] objects, float radius, bool forCheck) where T : BattleChara
        {
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObject(objects, radius, radius, CalculateCount, forCheck);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, T[] objects, float _)
            {
                byte count = 0;

                Vector3 dir = t.Position - Service.ClientState.LocalPlayer.Position;

                foreach (T obj in objects)
                {
                    Vector3 tdir = obj.Position - Service.ClientState.LocalPlayer.Position;

                    double distance = Vector3.Cross(dir, tdir).Length()/dir.Length();
                    if (distance <= 2)
                    {
                        count++;
                    }
                }
                return count;
            }
        }


        internal static float GetBestHeal(PlayerCharacter[] members, Action action, byte strength)
        {
            float healRange = strength * 0.000352f;

            //能够放到技能的队员。
            var canGet = GetObjectInRadius(members, action.Range);

            float bestHeal = 0;
            foreach (var member in canGet)
            {
                float thisHeal = 0;
                Vector3 centerPt = member.Position;
                foreach (var ran in members)
                {
                    //如果不在范围内，那算了。
                    if(Vector3.Distance(centerPt, ran.Position) > action.EffectRange)
                    {
                        continue;
                    }

                    thisHeal += Math.Min(1 - ran.CurrentHp / ran.MaxHp, healRange);
                }

                bestHeal = Math.Max(thisHeal, healRange);
            }
            return bestHeal;
        }
    }
}
