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

namespace XIVComboPlus
{
    internal class TargetHelper
    {
        private static IntPtr _func;

        private static Vector3 _lastPosition;
        public static bool IsMoving { get; private set; }

        public static BattleNpc[] Targets25 => GetObjectInRadius(Targets, 25f);

        private static BattleNpc[] Targets =>
        Service.ObjectTable.Where(obj => obj is BattleNpc && ((BattleNpc)obj).CurrentHp != 0 && ((BattleNpc)obj).BattleNpcKind == BattleNpcSubKind.Enemy && CanAttack(obj)).Select(obj => (BattleNpc)obj).ToArray();

        public static PlayerCharacter[] PartyMembers =>
        AllianceMembers.Where(fri => (fri.StatusFlags & StatusFlags.AllianceMember) != 0).ToArray();
        public static PlayerCharacter[] AllianceMembers =>
             Service.ObjectTable.Where(obj => obj is PlayerCharacter).Select(obj => (PlayerCharacter)obj).ToArray();
        public static PlayerCharacter[] AllHealers => GetJobCategory(AllianceMembers, (jt) => jt == JobType.Healer);
        public static PlayerCharacter[] AllDPS => GetJobCategory(AllianceMembers, (jt) => jt == JobType.Melee || jt == JobType.MagicalRanged || jt == JobType.PhysicalRanged);
        public static PlayerCharacter[] AllTanks => GetJobCategory(AllianceMembers, (jt) => jt == JobType.Tank);
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

        internal static void TargetObject(string command, string arguments)
        {
            string[] array = arguments.Split();

            switch (array[0])
            {
                case "HMHP":
                    SetTarget(Targets25.OrderByDescending(tar => tar.MaxHp).First());
                    break;

                case "LMHP":
                    SetTarget(Targets25.OrderBy(tar => tar.MaxHp).First());
                    break;

                case "Area":
                    SetTarget(GetMostObjectInRadius(Targets, 25, 5));
                    break;

                case "PLHP60":
                    PlayerCharacter lowChara = PartyMembers.Where(p => p.CurrentHp != 0).OrderBy(p => (float)p.CurrentHp / p.MaxHp).First();
                    if ((float)lowChara.CurrentHp / lowChara.MaxHp < 0.6) SetTarget(lowChara);
                    break;

                case "HArea8":
                    SetTarget(GetMostObjectInRadius(PartyMembers, 30, 8));
                    break;

                case "HArea6":
                    SetTarget(GetMostObjectInRadius(PartyMembers, 30, 6));
                    break;

                case "Aetherial":
                    PlayerCharacter target = (from heal in AllHealers
                                              select (heal, DistanceToPlayer(heal) - heal.HitboxRadius) into gr
                                              where gr.Item2 <= 25
                                              orderby gr.Item2
                                              select gr.heal).Last();
                    if(target != null)
                    {
                        SetTarget(target);
                        break;
                    }

                    target = (from heal in AllDPS
                              select (heal, DistanceToPlayer(heal) - heal.HitboxRadius) into gr
                              where gr.Item2 <= 25
                              orderby gr.Item2
                              select gr.heal).Last();

                    if (target != null)
                    {
                        SetTarget(target);
                    }
                    break;

                case "Death":
                    PlayerCharacter death = GetDeath(AllHealers);
                    if(death != null)
                    {
                        SetTarget(death);
                        break;
                    }
                    death = GetDeath(AllTanks);
                    if (death != null)
                    {
                        SetTarget(death);
                        break;
                    }
                    death = GetDeath(AllDPS);
                    if (death != null)
                    {
                        SetTarget(death);
                        break;
                    }
                    break;
            }
        }

        private static PlayerCharacter GetDeath(PlayerCharacter[] charas)
        {
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
                    if(character.CastTargetObjectId == item.ObjectId)
                    {
                        isCasting = true;
                        break;
                    }
                }
                if(isCasting) continue;

                return item;
            }
            return null;
        }

        private static void SetTarget(GameObject? obj)
        {
            Service.TargetManager.SetTarget(obj);
        }
        private static float DistanceToPlayer(GameObject obj)
        {
            return Vector3.Distance(Service.ClientState.LocalPlayer.Position, obj.Position);
        }

        private static T[] GetObjectInRadius<T>(T[] objects, float radius) where T : GameObject
        {
            return objects.Where(o => DistanceToPlayer(o) <= radius + o.HitboxRadius).ToArray();
        }

        private static T GetMostObjectInRadius<T>(T[] objects, float radius, float range) where T : BattleChara
        {
            return (from t in GetObjectInRadius(objects, radius)
                    select (t, Calculate(t, objects, radius, range)) into set
                    where set.Item2 > 2
                    orderby set.Item2
                    select set.t).Last();

            static float Calculate(T t, T[] objects, float radius, float range)
            {
                byte count = 0;
                foreach (T obj in GetObjectInRadius(objects, radius + range))
                {
                    if (Vector3.Distance(t.Position, obj.Position) <= range + obj.HitboxRadius)
                    {
                        count++;
                    }
                }
                return count + (float)t.CurrentHp / t.MaxHp;
            }
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
    }
}
