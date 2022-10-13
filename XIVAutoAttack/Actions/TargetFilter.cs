using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Combos;

namespace XIVAutoAttack.Actions
{
    internal static class TargetFilter
    {
        #region Find one target
        internal static BattleChara DefaultChooseFriend(BattleChara[] availableCharas)
        {
            if (availableCharas == null || availableCharas.Length == 0) return null;

            //判断一下要选择打体积最大的，还是最小的。
            if (IconReplacer.AttackBig)
            {
                availableCharas = availableCharas.OrderByDescending(player => player.HitboxRadius).ToArray();
            }
            else
            {
                availableCharas = availableCharas.OrderBy(player => player.HitboxRadius).ToArray();
            }

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

            return availableCharas.OrderBy(StatusHelper.GetHealthRatio).First();
        }

        internal static BattleChara DefaultFindHostile(BattleChara[] availableCharas)
        {
            if (availableCharas == null || availableCharas.Length == 0) return null;

            //判断一下要选择打体积最大的，还是最小的。
            if (IconReplacer.AttackBig)
            {
                availableCharas = availableCharas.OrderByDescending(player => player.HitboxRadius).ToArray();
            }
            else
            {
                availableCharas = availableCharas.OrderBy(player => player.HitboxRadius).ToArray();
            }

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

        internal static BattleChara FindMoveTarget(BattleChara[] charas)
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

            if (DistanceToPlayer(tars.ElementAt(0)) < 5) return null;

            return tars.ElementAt(0);
        }

        private  static BattleChara FindMoveTargetScreenCenter(BattleChara[] charas)
        {
            var pPosition = Service.ClientState.LocalPlayer.Position;
            if (!Service.GameGui.WorldToScreen(pPosition, out var playerScrPos)) return null;

            var tars = charas.Where(t =>
            {
                if(!Service.GameGui.WorldToScreen(t.Position, out var scrPos)) return false;

                var dir = scrPos - playerScrPos;

                if (dir.Y > 0) return false;

                return Math.Abs(dir.X / dir.Y) < Math.Tan(Math.PI / 6);
            }).OrderByDescending(t => Vector3.Distance(t.Position, pPosition));

            if (tars.Count() == 0) return null;

            if (DistanceToPlayer(tars.ElementAt(0)) < 5) return null;

            return tars.ElementAt(0);
        }

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

            return (attachedT.Count > 0 ? attachedT.ToArray() : charas).OrderBy(StatusHelper.GetHealthRatio).First();
        }

        internal static BattleChara[] ProvokeTarget(BattleChara[] inputCharas, bool needDistance = false)
        {
            var tankIDS = GetJobCategory(TargetHelper.AllianceMembers, Role.防护).Select(member => member.ObjectId);
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


        internal static BattleChara GetDeathPeople(BattleChara[] deathAll, BattleChara[] deathParty)
        {
            if (deathParty.Length != 0)
            {
                //确认一下死了的T有哪些。

                var deathT = GetJobCategory(deathParty, Role.防护);
                int TCount = TargetHelper.PartyTanks.Length;

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

        internal static BattleChara[] GetDeath(BattleChara[] charas)
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
                foreach (var character in TargetHelper.AllianceMembers)
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

        internal static BattleChara[] GetTargetCanDot(BattleChara[] objects)
        {
            var health = GetHealthFromMulty(0.8f);

            return objects.Where(b => b.CurrentHp >= health).ToArray();
        }

        internal static uint GetHealthFromMulty(float mult)
        {
            if (Service.ClientState.LocalPlayer == null) return 0;

            var multi = GetJobCategory(new BattleChara[] { Service.ClientState.LocalPlayer }, Role.防护).Length == 0 ? mult : mult * 1.5f;
            if(TargetHelper.PartyMembers.Length > 4)
            {
                multi *= 2;
            }

            return (uint)(multi * Service.ClientState.LocalPlayer.MaxHp);
        }

        private static bool GetJobCategory(BattleChara obj, SortedSet<byte> validJobs)
        {
            return validJobs.Contains((byte)obj.ClassJob.GameData?.RowId);
        }
        #endregion

        #region Find many targets
        internal static T[] GetObjectInRadius<T>(T[] objects, float radius) where T : GameObject
        {
            return objects.Where(o => DistanceToPlayer(o) <= radius).ToArray();
        }

        private static T[] GetMostObject<T>(T[] canAttack, float radius, float range, Func<T, T[], float, byte> HowMany, bool isfriend, bool mustUse) where T : BattleChara
        {
            //能够打到的所有怪。
            T[] canGetObj = GetObjectInRadius(canAttack, radius);
            return GetMostObject(canAttack, canGetObj, range, HowMany, isfriend, mustUse);
        }

        private static T[] GetMostObject<T>(T[] canAttack, T[] canGetObj, float range, Func<T, T[], float, byte> HowMany, bool isfriend, bool mustUse) where T : BattleChara
        {

            //能打到MaxCount以上数量的怪的怪。
            List<T> objectMax = new List<T>(canGetObj.Length);

            int maxCount = mustUse ? 1 : isfriend ? Service.Configuration.PartyCount : Service.Configuration.HostileCount;

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
                    maxCount = count;
                    objectMax.Clear();
                    objectMax.Add(t);
                }
            }

            return objectMax.ToArray();
        }

        internal static T[] GetMostObjectInRadius<T>(T[] objects, float radius, float range, bool isfriend, bool mustUse) where T : BattleChara
        {
            //可能可以被打到的怪。
            var canAttach = GetObjectInRadius(objects, radius + range);
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObjectInRadius(canAttach, canGet, radius, range, isfriend, mustUse);

        }

        internal static T[] GetMostObjectInRadius<T>(T[] objects, T[] canGetObjects, float radius, float range, bool isfriend, bool mustUse) where T : BattleChara
        {
            var canAttach = GetObjectInRadius(objects, radius + range);

            return GetMostObject(canAttach, canGetObjects, range, CalculateCount, isfriend, mustUse);

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

        internal static T[] GetMostObjectInArc<T>(T[] objects, float radius, bool mustUse) where T : BattleChara
        {
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObject(canGet, radius, radius, CalculateCount, false, mustUse);

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

        internal static T[] GetMostObjectInLine<T>(T[] objects, float radius, bool mustUse) where T : BattleChara
        {
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObject(canGet, radius, radius, CalculateCount, false, mustUse);

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

        internal static float DistanceToPlayer(GameObject obj)
        {
            var distance = Vector3.Distance(Service.ClientState.LocalPlayer.Position, obj.Position) - Service.ClientState.LocalPlayer.HitboxRadius;
            distance -= Math.Max(obj.HitboxRadius, Service.Configuration.ObjectMinRadius);
            return distance;
        }
    }
}
