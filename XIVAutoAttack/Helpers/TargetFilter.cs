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
        internal static BattleChara DefaultChooseFriend(IEnumerable<BattleChara> availableCharas)
        {
            if (availableCharas == null || !availableCharas.Any()) return null;

            //根据默认设置排序怪且没有大招
            availableCharas = DefaultTargetingType(availableCharas).Where(p => !p.HasStatus(false, StatusHelper.TankBreakStatus));

            //找到体积一样小的
            float radius = availableCharas.FirstOrDefault().HitboxRadius;

            return availableCharas.Where(c => c.HitboxRadius == radius)
                .OrderBy(ObjectHelper.GetHealthRatio).First();
        }

        internal static BattleChara DefaultFindHostile(IEnumerable<BattleChara> availableCharas)
        {
            if (availableCharas == null || !availableCharas.Any()) return null;

            //找到被标记攻击的怪
            if (GetAttackMarkChara(availableCharas) is BattleChara b && b != null) return b;

            //去掉停止标记的怪
            if (Service.Configuration.FilterStopMark)
            {
                var charas = MarkingController.FilterStopCharaes(availableCharas);
                if (charas?.Any() ?? false) availableCharas = charas;
            }

            //根据默认设置排序怪
            availableCharas = DefaultTargetingType(availableCharas);


            //找到体积一样小的
            float radius = availableCharas.FirstOrDefault().HitboxRadius;

            return availableCharas.Where(c => c.HitboxRadius == radius)
                .OrderBy(DistanceToPlayer).First();
        }

        internal static BattleChara FindTargetForMoving(IEnumerable<BattleChara> charas)
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

        private static BattleChara FindMoveTargetFaceDirection(IEnumerable<BattleChara> charas)
        {
            Vector3 pPosition = Service.ClientState.LocalPlayer.Position;
            float rotation = Service.ClientState.LocalPlayer.Rotation;
            Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

            var tars = charas.Where(t =>
            {
                Vector3 dir = t.Position - pPosition;
                Vector2 dirVec = new Vector2(dir.Z, dir.X);
                double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());
                return angle <= Math.PI * Service.Configuration.MoveTargetAngle / 360;
            }).OrderByDescending(t => Vector3.Distance(t.Position, pPosition));

            return tars.FirstOrDefault();
        }

        private static BattleChara FindMoveTargetScreenCenter(IEnumerable<BattleChara> charas)
        {
            var pPosition = Service.ClientState.LocalPlayer.Position;
            if (!Service.GameGui.WorldToScreen(pPosition, out var playerScrPos)) return null;

            var tars = charas.Where(t =>
            {
                if (!Service.GameGui.WorldToScreen(t.Position, out var scrPos)) return false;

                var dir = scrPos - playerScrPos;

                if (dir.Y > 0) return false;

                return Math.Abs(dir.X / dir.Y) < Math.Tan(Math.PI * Service.Configuration.MoveTargetAngle / 360);
            }).OrderByDescending(t => Vector3.Distance(t.Position, pPosition));

            return tars.FirstOrDefault();
        }

        /// <summary>
        /// 发现被攻击的目标
        /// </summary>
        /// <param name="charas"></param>
        /// <returns></returns>
        internal static BattleChara FindAttackedTarget(IEnumerable<BattleChara> charas)
        {
            if (!charas.Any()) return null;
            var attachedT = charas.Where(tank => tank.TargetObject?.TargetObject == tank);

            return (attachedT.Any() ? attachedT : charas).OrderBy(ObjectHelper.GetHealthRatio).FirstOrDefault();
        }

        /// <summary>
        /// 挑衅目标
        /// </summary>
        /// <param name="inputCharas"></param>
        /// <param name="needDistance"></param>
        /// <returns></returns>
        internal static IEnumerable<BattleChara> ProvokeTarget(IEnumerable<BattleChara> inputCharas, bool needDistance = false)
        {
            var tankIDS = GetJobCategory(TargetUpdater.AllianceMembers, JobRole.Tank).Select(member => member.ObjectId);
            var loc = Service.ClientState.LocalPlayer.Position;
            var id = Service.ClientState.LocalPlayer.ObjectId;

            var targets = inputCharas.Where(target =>
            {
                //有目标
                if (target.TargetObject?.IsValid() ?? false)
                {
                    //居然在打非T！
                    if (!tankIDS.Contains(target.TargetObjectId) && (!needDistance || Vector3.Distance(target.Position, loc) > 5))
                    {
                        return true;
                    }
                }
                return false;
            });

            //没有敌对势力，那随便用
            if (!targets.Any()) return inputCharas;
            //返回在打队友的讨厌鬼！
            return targets;
        }

        /// <summary>
        /// 获得死亡的角色
        /// </summary>
        /// <param name="deathAll"></param>
        /// <param name="deathParty"></param>
        /// <returns></returns>
        internal static BattleChara GetDeathPeople(IEnumerable<BattleChara> deathAll, IEnumerable<BattleChara> deathParty)
        {
            if (deathParty.Any())
            {
                //确认一下死了的T有哪些。

                var deathT = GetJobCategory(deathParty, JobRole.Tank);
                int TCount = TargetUpdater.PartyTanks.Count();

                //如果全死了，赶紧复活啊。
                if (TCount > 0 && deathT.Count() == TCount)
                {
                    return deathT.First();
                }

                //确认一下死了的H有哪些。
                var deathH = GetJobCategory(deathParty, JobRole.Healer);

                //如果H死了，就先救他。
                if (deathH.Count() != 0) return deathH.First();

                //如果T死了，就再救他。
                if (deathT.Count() != 0) return deathT.First();

                //T和H都还活着，那就随便救一个。
                return deathParty.First();
            }

            if (deathAll.Any())
            {
                //确认一下死了的H有哪些。
                var deathAllH = GetJobCategory(deathAll, JobRole.Healer);
                if (deathAllH.Count() != 0) return deathAllH.First();

                //确认一下死了的T有哪些。
                var deathAllT = GetJobCategory(deathAll, JobRole.Tank);
                if (deathAllT.Count() != 0) return deathAllT.First();

                return deathAll.First();
            }

            return null;
        }

        internal unsafe static IEnumerable<BattleChara> GetTargetable(IEnumerable<BattleChara> charas)
        {
            return charas.Where(item => ((FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)(void*)item.Address)->GetIsTargetable());
        }

        internal unsafe static IEnumerable<BattleChara> GetDeath(this IEnumerable<BattleChara> charas)
        {
            charas = GetTargetable(charas);

            return charas.Where(item =>
            {
                //如果还有血，就算了。
                if (item.CurrentHp != 0) return false;

                //如果已经有复活的Buff了，那就算了。
                if (item.HasStatus(false, StatusID.Raise)) return false;

                //如果濒死了，那给我TMD冷静冷静！等着另一个奶大发慈悲吧。
                if (!Service.Configuration.RaiseBrinkofDeath && item.HasStatus(false, StatusID.BrinkofDeath)) return false;

                //如果有人在对着他咏唱，那就算了。
                if (TargetUpdater.AllianceMembers.Any(c => c.CastTargetObjectId == item.ObjectId)) return false;

                return true;
            });
        }
        internal static IEnumerable<BattleChara> GetTargetCanDot(IEnumerable<BattleChara> objects)
        {
            return objects.Where(b => b.CanDot());
        }


        internal static IEnumerable<BattleChara> GetJobCategory(this IEnumerable<BattleChara> objects, params JobRole[] roles)
        {
            return roles.SelectMany(role =>
            {
                return objects.Where(obj =>
                {
                    return IsJobCategory(obj, role);
                });
            });
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

        internal static BattleChara ASTRangeTarget(IEnumerable<BattleChara> ASTTargets)
        {
            ASTTargets = ASTTargets.Where(b => !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath));

            return GetTargetByJobs(ASTTargets, JobRole.RangedMagicial, JobRole.RangedPhysical, JobRole.Melee);
        }

        internal static BattleChara ASTMeleeTarget(IEnumerable<BattleChara> ASTTargets)
        {
            ASTTargets = ASTTargets.Where(b => !b.HasStatus(false, StatusID.Weakness, StatusID.BrinkofDeath));


            return GetTargetByJobs(ASTTargets, JobRole.Melee, JobRole.RangedMagicial, JobRole.RangedPhysical);
        }

        private static BattleChara GetTargetByJobs(this IEnumerable<BattleChara> tars, params JobRole[] roles)
        {
            foreach (var role in roles)
            {
                var targets = GetASTCardTargets(GetJobCategory(tars, role));
                if (targets.Count() > 0) return RandomObject(targets);
            }
            var ts = GetASTCardTargets(tars);
            if (ts.Count() > 0) return RandomObject(ts);

            return null;
        }

        private static IEnumerable<BattleChara> GetASTCardTargets(IEnumerable<BattleChara> sources)
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
            return sources.Where((t) => !t.HasStatus(true, allStatus));
        }

        private static BattleChara RandomObject(IEnumerable<BattleChara> objs)
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            return objs.ElementAt(ran.Next(objs.Count()));
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
        internal static IEnumerable<T> GetObjectInRadius<T>(this IEnumerable<T> objects, float radius) where T : GameObject
        {
            return objects.Where(o => DistanceToPlayer(o) <= radius);
        }

        private static IEnumerable<T> GetMostObject<T>(IEnumerable<T> canAttack, float radius, float range, Func<T, IEnumerable<T>, float, byte> HowMany, bool mostCount, int maxCount) where T : BattleChara
        {
            //能够打到的所有怪。
            IEnumerable<T> canGetObj = GetObjectInRadius(canAttack, radius);
            return GetMostObject(canAttack, canGetObj, range, HowMany, mostCount, maxCount);
        }

        private static IEnumerable<T> GetMostObject<T>(IEnumerable<T> canAttack, IEnumerable<T> canGetObj, float range, Func<T, IEnumerable<T>, float, byte> HowMany, bool mostCount, int maxCount) where T : BattleChara
        {
            //能打到MaxCount以上数量的怪的怪。
            List<T> objectMax = new List<T>(canGetObj.Count());

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

            return objectMax;
        }

        internal static IEnumerable<T> GetMostObjectInRadius<T>(IEnumerable<T> objects, float radius, float range, bool mostCount, int maxCount) where T : BattleChara
        {
            //可能可以被打到的怪。
            var canAttach = GetObjectInRadius(objects, radius + range);
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObjectInRadius(canAttach, canGet, radius, range, mostCount, maxCount);

        }

        internal static IEnumerable<T> GetMostObjectInRadius<T>(IEnumerable<T> objects, IEnumerable<T> canGetObjects, float radius, float range, bool mostCount, int maxCount) where T : BattleChara
        {
            var canAttach = GetObjectInRadius(objects, radius + range);

            return GetMostObject(canAttach, canGetObjects, range, CalculateCount, mostCount, maxCount);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, IEnumerable<T> objects, float range)
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

        internal static IEnumerable<T> GetMostObjectInArc<T>(IEnumerable<T> objects, float radius, bool mostCount, int maxCount) where T : BattleChara
        {
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObject(canGet, radius, radius, CalculateCount, mostCount, maxCount);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, IEnumerable<T> objects, float _)
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

        internal static IEnumerable<T> GetMostObjectInLine<T>(IEnumerable<T> objects, float radius, bool mostCount, int maxCount) where T : BattleChara
        {
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObject(canGet, radius, radius, CalculateCount, mostCount, maxCount);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, IEnumerable<T> objects, float _)
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

        internal static BattleChara GetAttackMarkChara(IEnumerable<BattleChara> charas)
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

        private static IEnumerable<BattleChara> DefaultTargetingType(IEnumerable<BattleChara> charas)
        {
            switch (CommandController.RightTargetingType)
            {
                default:
                case TargetingType.Big:
                    return charas.OrderByDescending(p => p.HitboxRadius);

                case TargetingType.Small:
                    return charas.OrderBy(p => p.HitboxRadius);

                case TargetingType.HighHP:
                    return charas.OrderByDescending(p => p.CurrentHp);

                case TargetingType.LowHP:
                    return charas.OrderBy(p => p.CurrentHp);

                case TargetingType.HighMaxHP:
                    return charas.OrderByDescending(p => p.MaxHp);

                case TargetingType.LowMaxHP:
                    return charas.OrderBy(p => p.MaxHp);
            }
        }
    }
}
