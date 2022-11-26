using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Data;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Helpers
{
    internal static class TargetFilter
    {
        #region Find one target
        internal static BattleChara DefaultChooseFriend(BattleChara[] availableCharas)
        {
            if (availableCharas == null || availableCharas.Length == 0) return null;

            //根据默认设置排序怪
            availableCharas = DefaultTargetingType(availableCharas);

            //找到体积一样小的
            List<BattleChara> canGet = new List<BattleChara>(availableCharas.Length) { availableCharas[0] };

            float radius = availableCharas[0].HitboxRadius;
            for (int i = 1; i < availableCharas.Length; i++)
            {
                if (availableCharas[i].HitboxRadius == radius)
                {
                    canGet.Add(availableCharas[i]);
                }
                else break;
            }

            return availableCharas.OrderBy(ObjectHelper.GetHealthRatio).First();

        }

        internal static BattleChara DefaultFindHostile(BattleChara[] availableCharas)
        {
            if (availableCharas == null || availableCharas.Length == 0) return null;

            //找到被标记攻击的怪
            if (GetAttackMarkChara(availableCharas) is BattleChara b && b != null) return b;

            //去掉停止标记的怪
            if (Service.Configuration.FilterStopMark)
            {
                availableCharas = MarkingController.FilterStopCharaes(availableCharas);
            }

            //根据默认设置排序怪
            availableCharas = DefaultTargetingType(availableCharas);

            //找到体积一样小的
            List<BattleChara> canGet = new List<BattleChara>(availableCharas.Length) { availableCharas[0] };

            float radius = availableCharas[0].HitboxRadius;
            for (int i = 1; i < availableCharas.Length; i++)
            {
                if (availableCharas[i].HitboxRadius == radius)
                {
                    canGet.Add(availableCharas[i]);
                }
                else break;
            }

            return canGet.OrderBy(b => DistanceToPlayer(b)).First();
        }

        internal static BattleChara FindTargetForMoving(BattleChara[] charas)
        {
            if (Service.Configuration.MoveTowardsScreen)
            {
                return FindMoveTargetScreenCenter(charas);
            }
            else
            {
                return FindMoveTargetFaceDirection(charas);
            }
        }

        private static BattleChara FindMoveTargetFaceDirection(BattleChara[] charas)
        {
            Vector3 pPosition = Service.ClientState.LocalPlayer.Position;
            float rotation = Service.ClientState.LocalPlayer.Rotation;
            Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

            var tars = charas.Where(t =>
            {
                Vector3 dir = t.Position - pPosition;
                Vector2 dirVec = new Vector2(dir.Z, dir.X);
                double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());
                return angle <= Math.PI / 6;
            }).OrderByDescending(t => Vector3.Distance(t.Position, pPosition));

            if (tars.Count() == 0) return null;

            return tars.ElementAt(0);
        }

        private static BattleChara FindMoveTargetScreenCenter(BattleChara[] charas)
        {
            var pPosition = Service.ClientState.LocalPlayer.Position;
            if (!Service.GameGui.WorldToScreen(pPosition, out var playerScrPos)) return null;

            var tars = charas.Where(t =>
            {
                if (!Service.GameGui.WorldToScreen(t.Position, out var scrPos)) return false;

                var dir = scrPos - playerScrPos;

                if (dir.Y > 0) return false;

                return Math.Abs(dir.X / dir.Y) < Math.Tan(Math.PI / 6);
            }).OrderByDescending(t => Vector3.Distance(t.Position, pPosition));

            if (tars.Count() == 0) return null;

            return tars.ElementAt(0);
        }

        /// <summary>
        /// 发现被攻击的目标
        /// </summary>
        /// <param name="charas"></param>
        /// <returns></returns>
        internal static BattleChara FindAttackedTarget(BattleChara[] charas)
        {
            if (charas.Length == 0) return null;
            List<BattleChara> attachedT = new List<BattleChara>(charas.Length);
            foreach (var tank in charas)
            {
                if (tank.TargetObject?.TargetObject == tank)
                {
                    attachedT.Add(tank);
                }
            }

            return (attachedT.Count > 0 ? attachedT.ToArray() : charas).OrderBy(ObjectHelper.GetHealthRatio).First();
        }

        /// <summary>
        /// 挑衅目标
        /// </summary>
        /// <param name="inputCharas"></param>
        /// <param name="needDistance"></param>
        /// <returns></returns>
        internal static BattleChara[] ProvokeTarget(BattleChara[] inputCharas, bool needDistance = false)
        {
            var tankIDS = GetJobCategory(TargetUpdater.AllianceMembers, JobRole.Tank).Select(member => member.ObjectId);
            var loc = Service.ClientState.LocalPlayer.Position;
            var id = Service.ClientState.LocalPlayer.ObjectId;

            List<BattleChara> targets = new List<BattleChara>();
            foreach (var target in inputCharas)
            {
                //有目标
                if (target.TargetObject?.IsValid() ?? false)
                {
                    //居然在打非T！
                    if (!tankIDS.Contains(target.TargetObjectId) && (!needDistance || Vector3.Distance(target.Position, loc) > 5))
                    {
                        targets.Add(target);
                    }
                }
            }
            //没有敌对势力，那随便用
            if (targets.Count == 0) return inputCharas;
            //返回在打队友的讨厌鬼！
            return targets.ToArray();
        }

        /// <summary>
        /// 获得死亡的角色
        /// </summary>
        /// <param name="deathAll"></param>
        /// <param name="deathParty"></param>
        /// <returns></returns>
        internal static BattleChara GetDeathPeople(BattleChara[] deathAll, BattleChara[] deathParty)
        {
            if (deathParty.Length != 0)
            {
                //确认一下死了的T有哪些。

                var deathT = GetJobCategory(deathParty, JobRole.Tank);
                int TCount = TargetUpdater.PartyTanks.Length;

                //如果全死了，赶紧复活啊。
                if (TCount > 0 && deathT.Length == TCount)
                {
                    return deathT[0];
                }

                //确认一下死了的H有哪些。
                var deathH = GetJobCategory(deathParty, JobRole.Healer);

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
            var deathAllH = GetJobCategory(deathAll, JobRole.Healer);
            if (deathAllH.Length != 0) return deathAllH[0];

            //确认一下死了的T有哪些。
            var deathAllT = GetJobCategory(deathAll, JobRole.Tank);
            if (deathAllT.Length != 0) return deathAllT[0];

            return deathAll[0];
        }

        internal unsafe static BattleChara[] GetTargetable(BattleChara[] charas)
        {
            return charas.Where(item => ((FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)item.Address)->GetIsTargetable()).ToArray();
        }

        internal unsafe static BattleChara[] GetDeath(this BattleChara[] charas)
        {
            charas = GetTargetable(charas);

            List<BattleChara> list = new List<BattleChara>(charas.Length);
            foreach (var item in charas)
            {
                //如果还有血，就算了。
                if (item.CurrentHp != 0) continue;

                //如果已经有复活的Buff了，那就算了。
                if (item.HasStatus(false, StatusID.Raise)) continue;

                //如果濒死了，那给我TMD冷静冷静！等着另一个奶大发慈悲吧。
                if (!Service.Configuration.RaiseBrinkofDeath && item.HasStatus(false, StatusID.BrinkofDeath)) continue;

                //如果有人在对着他咏唱，那就算了。
                if (TargetUpdater.AllianceMembers.Any(c => c.CastTargetObjectId == item.ObjectId)) continue;

                list.Add(item);
            }
            return list.ToArray();
        }
        internal static BattleChara[] GetTargetCanDot(BattleChara[] objects)
        {
            return objects.Where(b => b.CanDot()).ToArray();
        }


        internal static BattleChara[] GetJobCategory(this BattleChara[] objects, params JobRole[] roles)
        {
            List<BattleChara> result = new(objects.Length);

            foreach (var role in roles)
            {
                foreach (var obj in objects)
                {
                    if (IsJobCategory(obj, role)) result.Add(obj);
                }
            }

            return result.ToArray();
        }

        private static bool IsJobCategory(this BattleChara obj, JobRole role)
        {
            SortedSet<byte> validJobs = new(Service.DataManager.GetExcelSheet<ClassJob>()
                .Where(job => role == job.GetJobRole())
                .Select(job => (byte)job.RowId));

            return IsJobCategory(obj, validJobs);
        }


        private static bool IsJobCategory(this BattleChara obj, SortedSet<byte> validJobs)
        {
            return validJobs.Contains((byte)obj.ClassJob.GameData?.RowId);
        }

        internal static BattleChara ASTRangeTarget(BattleChara[] ASTTargets)
        {
            ASTTargets = ASTTargets.Where(b => !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath)).ToArray();

            return GetTargetByJobs(ASTTargets, JobRole.RangedMagicial, JobRole.RangedPhysical, JobRole.Melee);
        }

        internal static BattleChara ASTMeleeTarget(BattleChara[] ASTTargets)
        {
            ASTTargets = ASTTargets.Where(b => !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath)).ToArray();


            return GetTargetByJobs(ASTTargets, JobRole.Melee, JobRole.RangedMagicial, JobRole.RangedPhysical);
        }

        private static BattleChara GetTargetByJobs(this BattleChara[] tars, params JobRole[] roles)
        {
            foreach (var role in roles)
            {
                var targets = GetASTCardTargets(GetJobCategory(tars, role));
                if (targets.Length > 0) return RandomObject(targets);
            }
            var ts = GetASTCardTargets(tars);
            if (ts.Length > 0) return RandomObject(ts);

            return null;
        }

        private static BattleChara[] GetASTCardTargets(BattleChara[] sources)
        {
            var allStatus = new StatusID[]
            {
            StatusID.TheArrow,
            StatusID.TheBalance,
            StatusID.TheBole,
            StatusID.TheEwer,
            StatusID.TheSpear,
            StatusID.TheSpire,
            };
            return sources.Where((t) => !t.HasStatus(true, allStatus)).ToArray();
        }

        private static BattleChara RandomObject(BattleChara[] objs)
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            return objs[ran.Next(objs.Length)];
        }

        #endregion

        #region Find many targets
        /// <summary>
        /// 获得范围<paramref name="radius"/>内对象<paramref name="objects"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        internal static T[] GetObjectInRadius<T>(this T[] objects, float radius) where T : GameObject
        {
            return objects.Where(o => DistanceToPlayer(o) <= radius).ToArray();
        }

        //internal unsafe static T[] GetCanAttack<T>(uint actionId, params T[] objects) where T : GameObject
        //{
        //    return objects;
        //    return objects.Where(o =>
        //    {
        //        var objAdress = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)o.Address;
        //        return Service.Address.CanUseAction(actionId, objAdress) != 0 &&
        //        ActionManager.Instance()->GetActionStatus(ActionType.Spell, actionId, objAdress->ObjectID, 0, 0) == 0;
        //    }).ToArray();
        //}

        private static T[] GetMostObject<T>(T[] canAttack, float radius, float range, Func<T, T[], float, byte> HowMany, bool mostCount, int maxCount) where T : BattleChara
        {
            //能够打到的所有怪。
            T[] canGetObj = GetObjectInRadius(canAttack, radius);
            return GetMostObject(canAttack, canGetObj, range, HowMany, mostCount, maxCount);
        }

        private static T[] GetMostObject<T>(T[] canAttack, T[] canGetObj, float range, Func<T, T[], float, byte> HowMany, bool mostCount, int maxCount) where T : BattleChara
        {
            //能打到MaxCount以上数量的怪的怪。
            List<T> objectMax = new List<T>(canGetObj.Length);

            //循环能打中的目标。
            foreach (var t in canGetObj)
            {
                //计算能达到的所有怪的数量。
                byte count = HowMany(t, canAttack, range);

                if (count == maxCount)
                {
                    objectMax.Add(t);
                }
                else if (count > maxCount)
                {
                    if (mostCount)
                    {
                        maxCount = count;
                        objectMax.Clear();
                    }
                    objectMax.Add(t);
                }
            }

            return objectMax.ToArray();
        }

        internal static T[] GetMostObjectInRadius<T>(T[] objects, float radius, float range, bool mostCount, int maxCount) where T : BattleChara
        {
            //可能可以被打到的怪。
            var canAttach = GetObjectInRadius(objects, radius + range);
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObjectInRadius(canAttach, canGet, radius, range, mostCount, maxCount);

        }

        internal static T[] GetMostObjectInRadius<T>(T[] objects, T[] canGetObjects, float radius, float range, bool mostCount, int maxCount) where T : BattleChara
        {
            var canAttach = GetObjectInRadius(objects, radius + range);

            return GetMostObject(canAttach, canGetObjects, range, CalculateCount, mostCount, maxCount);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, T[] objects, float range)
            {
                byte count = 0;
                foreach (T obj in objects)
                {
                    if (Vector3.Distance(t.Position, obj.Position) <= range + obj.HitboxRadius)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        internal static T[] GetMostObjectInArc<T>(T[] objects, float radius, bool mostCount, int maxCount) where T : BattleChara
        {
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObject(canGet, radius, radius, CalculateCount, mostCount, maxCount);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, T[] objects, float _)
            {
                byte count = 0;

                Vector3 dir = t.Position - Service.ClientState.LocalPlayer.Position;

                foreach (T obj in objects)
                {
                    Vector3 tdir = obj.Position - Service.ClientState.LocalPlayer.Position;

                    double cos = Vector3.Dot(dir, tdir) / (dir.Length() * tdir.Length());
                    if (cos >= 0.5)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        internal static T[] GetMostObjectInLine<T>(T[] objects, float radius, bool mostCount, int maxCount) where T : BattleChara
        {
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObject(canGet, radius, radius, CalculateCount, mostCount, maxCount);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, T[] objects, float _)
            {
                byte count = 0;

                Vector3 dir = t.Position - Service.ClientState.LocalPlayer.Position;

                foreach (T obj in objects)
                {
                    Vector3 tdir = obj.Position - Service.ClientState.LocalPlayer.Position;

                    double distance = Vector3.Cross(dir, tdir).Length() / dir.Length();
                    if (distance <= 2)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        #endregion

        /// <summary>
        /// 对象<paramref name="obj"/>距玩家的距离
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static float DistanceToPlayer(this GameObject obj)
        {
            if (obj == null) return float.MaxValue;
            var distance = Vector3.Distance(Service.ClientState.LocalPlayer.Position, obj.Position) - Service.ClientState.LocalPlayer.HitboxRadius;
            distance -= Math.Max(obj.HitboxRadius, Service.Configuration.ObjectMinRadius);
            return distance;
        }

        internal static BattleChara GetAttackMarkChara(BattleChara[] charas)
        {
            if (!Service.Configuration.ChooseAttackMark) return null;

            var b = MarkingController.Attack1Chara(charas);
            if (b != null) return b;

            b = MarkingController.Attack2Chara(charas);
            if (b != null) return b;

            b = MarkingController.Attack3Chara(charas);
            if (b != null) return b;

            b = MarkingController.Attack4Chara(charas);
            if (b != null) return b;

            b = MarkingController.Attack5Chara(charas);
            if (b != null) return b;
            return null;
        }

        private static BattleChara[] DefaultTargetingType(BattleChara[] charas)
        {
            switch (CommandController.RightTargetingType)
            {
                default:
                case TargetingType.Big:
                    return charas.OrderByDescending(p => p.HitboxRadius).ToArray();

                case TargetingType.Small:
                    return charas.OrderBy(p => p.HitboxRadius).ToArray();

                case TargetingType.HighHP:
                    return charas.OrderByDescending(p => p.CurrentHp).ToArray();

                case TargetingType.LowHP:
                    return charas.OrderBy(p => p.CurrentHp).ToArray();

                case TargetingType.HighMaxHP:
                    return charas.OrderByDescending(p => p.MaxHp).ToArray();

                case TargetingType.LowMaxHP:
                    return charas.OrderBy(p => p.MaxHp).ToArray();
            }
        }
    }
}
