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
            Esuna,
            Provoke,
            Melee,
            Range,
        }

        private static IntPtr _func;

        private static Vector3 _lastPosition;
        public static bool IsMoving { get; private set; }

        private static SortedList<uint, GetTargetFunction> _specialGetTarget = new SortedList<uint, GetTargetFunction>()
        {
            //以太步，找面前的友军。
            { BLMCombo.Actions.AetherialManipulation.ActionID, GetTargetFunction.FaceDirction},
            {WHMCombo.Actions.Aquaveil.ActionID, GetTargetFunction.MajorTank },
            {WHMCombo.Actions.DivineBenison.ActionID, GetTargetFunction.MajorTank },
            {WHMCombo.Actions.Benediction.ActionID, GetTargetFunction.MajorTank },
            {CustomCombo.GeneralActions.Esuna.ActionID, GetTargetFunction.Esuna},
            {BRDCombo.Actions.NaturesMinne.ActionID, GetTargetFunction.MajorTank },
            {BRDCombo.Actions.WardensPaean.ActionID, GetTargetFunction.Esuna },
            {CustomCombo.GeneralActions.Provoke.ActionID, GetTargetFunction.Provoke },
            {WARCombo.Actions.Tomahawk.ActionID, GetTargetFunction.Provoke },
            {WARCombo.Actions.NascentFlash.ActionID, GetTargetFunction.MajorTank },

            {ASTCombo.Actions.Balance.ActionID, GetTargetFunction.Melee },
            {ASTCombo.Actions.Arrow.ActionID, GetTargetFunction.Melee },
            {ASTCombo.Actions.Spear.ActionID, GetTargetFunction.Melee },
            {ASTCombo.Actions.Bole.ActionID, GetTargetFunction.Range },
            {ASTCombo.Actions.Ewer.ActionID, GetTargetFunction.Range },
            {ASTCombo.Actions.Spire.ActionID, GetTargetFunction.Range },
            {ASTCombo.Actions.Exaltation.ActionID, GetTargetFunction.MajorTank},
        };

        //All Targes
        internal static BattleNpc[] AllTargets =>
        Service.ObjectTable.Where(obj => obj is BattleNpc && ((BattleNpc)obj).CurrentHp != 0 && CanAttack(obj) && 
        (((BattleNpc)obj).BattleNpcKind == BattleNpcSubKind.Enemy || ((BattleNpc)obj).BattleNpcKind == BattleNpcSubKind.None)).Select(obj => (BattleNpc)obj).ToArray();

        internal static BattleNpc[] HostileTargets
        {
            get
            {
                var allTarges = AllTargets;
                uint[] ids = GetEnemies();
                var hosts = allTarges.Where(t => t.TargetObject?.IsValid() ?? false || ids.Contains(t.ObjectId)).ToArray();
                return hosts.Length == 0? allTarges : hosts;
            }
        }

        internal static BattleNpc[] HostileTargetsNotAimedMe => HostileTargets.Where(t => t.TargetObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();

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

        public static float[] PartyMembersHP => GetObjectInRadius(PartyMembers, 30).Select(p => (float)p.CurrentHp / p.MaxHp).ToArray();

        public static float PartyMembersAverHP
        {
            get
            {
                var members = PartyMembersHP;
                float averHP = 0;
                foreach (var hp in members)
                {
                    averHP += hp;
                }
                return averHP / members.Length;
            }
        }

        public static float PartyMembersDifferHP
        {
            get
            {
                var members = PartyMembersHP;
                double differHP = 0;
                float average = PartyMembersAverHP;
                foreach (var hp in members)
                {
                    differHP += Math.Pow(hp - average,2);
                }
                return (float)Math.Sqrt(differHP / members.Length);
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

        public static PlayerCharacter[] WeakenPeople
        {
            get
            {
                return PartyMembers.Where(p =>
                {
                    foreach (var status in p.StatusList)
                    {
                        if (status.GameData.CanDispel) return true;
                    }
                    return false;
                }).ToArray();
            }
        }

        public static PlayerCharacter[] DyingPeople
        {
            get
            {
                return PartyMembers.Where(p =>
                {
                    foreach (var status in p.StatusList)
                    {
                        if (status.StatusId == ObjectStatus.Doom) return true;
                    }
                    return false;
                }).ToArray();
            }
        }
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
        internal static float DistanceToPlayer(GameObject obj)
        {
            return Vector3.Distance(Service.ClientState.LocalPlayer.Position, obj.Position);
        }


        private static PlayerCharacter[] GetJobCategory(PlayerCharacter[] objects, Func<JobType, bool> check)
        {
            List<PlayerCharacter> result = new List<PlayerCharacter>(objects.Length);

            SortedSet<byte> validJobs = new SortedSet<byte>(ClassJob.AllJobs.Where(job => check(job.Type)).Select(job => job.Index));

            foreach (var obj in objects)
            {
                if (GetJobCategory(obj, validJobs)) result.Add(obj);
            }
            return result.ToArray();
        }
        private static bool GetJobCategory(PlayerCharacter obj, Func<JobType, bool> check)
        {
            SortedSet<byte> validJobs = new SortedSet<byte>(ClassJob.AllJobs.Where(job => check(job.Type)).Select(job => job.Index));
            return GetJobCategory(obj, validJobs);
        }

        internal static bool GetJobCategory(PlayerCharacter obj, SortedSet<byte> validJobs)
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

        public static PlayerCharacter[] GetDangerousTanks(out float[] times)
        {
            var tanks = PartyTanks;
            List<PlayerCharacter> dangeriousTanks = new List<PlayerCharacter>(tanks.Length);
            List<float> time = new List<float>(tanks.Length);
            uint[] dangerousState = new uint[] { ObjectStatus.Holmgang, ObjectStatus.WalkingDead, ObjectStatus.Superbolide };
            foreach (var member in tanks)
            {
                //看看有没有人要搞死自己。
                foreach (var tag in member.StatusList)
                {
                    if (dangerousState.Contains(tag.StatusId))
                    {
                        dangeriousTanks.Add(member);
                        time.Add(tag.RemainingTime);
                        break;
                    }
                }
            }
            times = time.ToArray();
            return dangeriousTanks.ToArray();
        }
        private static GameObject GetDeathPeople()
        {
            var deathAll = DeathPeopleAll;
            var deathParty = DeathPeopleParty;


            if (deathParty.Length != 0)
            {
                //确认一下死了的T有哪些。

                var deathT = GetJobCategory(deathParty, (j) => j == JobType.Tank);
                int TCount = PartyTanks.Length;

                //如果全死了，赶紧复活啊。
                if (TCount > 0 && deathT.Length == TCount)
                {
                    return deathT[0];
                }

                //确认一下死了的H有哪些。
                var deathH = GetJobCategory(deathParty, (j) => j == JobType.Healer);

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
            var deathAllH = GetJobCategory(deathAll, (j) => j == JobType.Healer);
            if (deathAllH.Length != 0) return deathAllH[0];

            //确认一下死了的T有哪些。
            var deathAllT = GetJobCategory(deathAll, (j) => j == JobType.Tank);
            if (deathAllT.Length != 0) return deathAllT[0];

            return deathAll[0];
        }
        private static GameObject ASTGetMeleeTarget(float range, PlayerCharacter[] ASTTargets)
        {

            var targets = GetObjectInRadius(GetJobCategory(ASTTargets, (jt) => jt == JobType.Melee), range);
            if (targets.Length > 0) return RandomObject(targets);

            targets = GetObjectInRadius(GetJobCategory(ASTTargets, (jt) => jt == JobType.PhysicalRanged || jt == JobType.MagicalRanged), range);
            if (targets.Length > 0) return RandomObject(targets);

            targets = GetObjectInRadius(ASTTargets, range);
            if (targets.Length > 0) return RandomObject(targets);

            return null;
        }
        private static PlayerCharacter[] GetASTTargets()
        {
            var allStatus = new uint[] 
            {
                ObjectStatus.TheArrow,
                ObjectStatus.TheBalance,
                ObjectStatus.TheBole,
                ObjectStatus.TheEwer,
                ObjectStatus.TheSpear,
                ObjectStatus.TheSpire,

            };
            return PartyMembers.Where((t) =>
            {
                foreach (Status status in t.StatusList)
                {
                    if(allStatus.Contains(status.StatusId))
                    {
                        return false;
                    }
                }
                return true;
            }).ToArray();
        }
        private static GameObject ASTGetRangeTarget(float range, PlayerCharacter[] ASTTargets)
        {

            var targets = GetObjectInRadius(GetJobCategory(ASTTargets, (jt) => jt == JobType.PhysicalRanged || jt == JobType.MagicalRanged), range);
            if (targets.Length > 0) return RandomObject(targets);

            targets = GetObjectInRadius(GetJobCategory(ASTTargets, (jt) => jt == JobType.Melee), range);
            if (targets.Length > 0) return RandomObject(targets);

            targets = GetObjectInRadius(ASTTargets, range);
            if (targets.Length > 0) return RandomObject(targets);

            return null;
        }

        private static GameObject RandomObject(GameObject[] objs)
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            return objs[ran.Next(objs.Length)];
        }

        internal static GameObject GetBestTarget(Action act)
        {
            //如果都没有距离，这个还需要选对象嘛？选原来的对象啊！
            if (act.Range == 0) return Service.TargetManager.Target ?? Service.ClientState.LocalPlayer;


            //首先看看是不是能对小队成员进行操作的。
            if (act.CanTargetParty || act.RowId == WHMCombo.Actions.Asylum.ActionID)
            {
                //还消耗2400的蓝，那肯定是复活的。
                if (act.PrimaryCostType == 3 && act.PrimaryCostValue == 24)
                {
                    return GetDeathPeople();
                }

                //找到没死的队友们。
                PlayerCharacter[] availableCharas = PartyMembers.Where(player => player.CurrentHp != 0).ToArray();

                if (!act.CanTargetSelf && act.RowId != WHMCombo.Actions.Asylum.ActionID)
                {
                    availableCharas = availableCharas.Where(p => p.ObjectId != Service.ClientState.LocalContentId).ToArray();
                }

                //判断是否是范围。
                if (act.CastType > 1)
                {
                    //找到能覆盖最多的位置，并且选血最少的来。
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

                            case GetTargetFunction.Melee:
                                return ASTGetMeleeTarget(GetRange(act), GetASTTargets());

                            case GetTargetFunction.Range:
                                return ASTGetRangeTarget(GetRange(act), GetASTTargets());

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
                                if (WHMCombo.UseBenediction())
                                {
                                    tanks = GetDangerousTanks(out _);
                                    if (tanks.Length > 0) return tanks[0];
                                }
                                break;

                            case GetTargetFunction.Esuna:
                                availableCharas = GetObjectInRadius(DyingPeople, act.Range);

                                if (availableCharas.Length != 0) return availableCharas[0];
                                availableCharas = GetObjectInRadius(WeakenPeople, act.Range);

                                if (availableCharas.Length != 0) return availableCharas[0];
                                return PartyMembers[0];
                        }
                    }

                    ////如果坦克血不多了的话，那就先奶坦克。
                    //var tank = PartyTanks.Where(player => player.CurrentHp != 0).OrderBy(play => (float)play.CurrentHp / play.MaxHp).First();
                    //if ((float)tank.CurrentHp / tank.MaxHp < 0.5) return tank;

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

                        //如果是特殊需求的话。
                        if (_specialGetTarget.ContainsKey(act.RowId))
                        {
                            switch (_specialGetTarget[act.RowId])
                            {
                                case GetTargetFunction.Provoke:
                                    var tars = GetObjectInRadius(ProvokeTarget(out _), GetRange(act));
                                    if (tars.Length != 0) return tars.OrderBy(player => Vector3.Distance(player.Position, Service.ClientState.LocalPlayer.Position)).First();
                                    break;
                            }
                        }

                        //找到能打到的怪。
                        var canReachTars = GetObjectInRadius(HostileTargets, GetRange(act));


                        //判断一下要选择打体积最大的，还是最小的。
                        if (Service.Configuration.IsTargetBoss)
                        {
                            canReachTars = canReachTars.OrderByDescending(player => player.HitboxRadius).ToArray();
                        }
                        else
                        {
                            canReachTars = canReachTars.OrderBy(player => player.HitboxRadius).ToArray();
                        }

                        //找到体积一样小的
                        List<BattleNpc> canGet = new List<BattleNpc>(canReachTars.Length) { canReachTars[0] };

                        float radius = canReachTars[0].HitboxRadius;
                        for (int i = 1; i < canReachTars.Length; i++)
                        {
                            if(canReachTars[i].HitboxRadius == radius)
                            {
                                canGet.Add(canReachTars[i]);
                            }
                            else break;
                        }

                        return canGet.OrderBy(player => Vector3.Distance(player.Position, Service.ClientState.LocalPlayer.Position)).First();

                    case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
                        return GetMostObjectInRadius(HostileTargets, GetRange(act), act.EffectRange, false).OrderByDescending(p => (float)p.CurrentHp / p.MaxHp).First();

                    case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        return GetMostObjectInArc(HostileTargets, act.EffectRange, false).OrderByDescending(p => Vector3.Distance(Service.ClientState.LocalPlayer.Position, p.Position)).First();

                    case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        return GetMostObjectInLine(HostileTargets, GetRange(act), false).OrderByDescending(p => Vector3.Distance(Service.ClientState.LocalPlayer.Position, p.Position)).First();

                }
            }
            else if (act.CanTargetSelf)
            {
                return Service.ClientState.LocalPlayer;
            }
            //那么这个就不需要找到目标了，要么对着自己，要么就什么都不能选中。
            else
            {
                return Service.TargetManager.Target ?? Service.ClientState.LocalPlayer;
            }
        }

        internal static BattleNpc[] ProvokeTarget(out bool haveTargetOnme)
        {
            var tankIDS = GetJobCategory(AllianceMembers, (jt) => jt == JobType.Tank).Select(member => member.ObjectId);
            var loc = Service.ClientState.LocalPlayer.Position;
            var id = Service.ClientState.LocalPlayer.ObjectId;

            bool someTargetsHaveTarget = false;
            haveTargetOnme = false;
            List<BattleNpc> targets = new List<BattleNpc>();
            foreach (var target in HostileTargets)
            {
                //有目标
                if (target.TargetObject?.IsValid() ?? false)
                {
                    someTargetsHaveTarget = true;

                    //居然在打非T！
                    if (!tankIDS.Contains(target.TargetObjectId) && Vector3.Distance(target.Position, loc) > 5)
                    {
                        targets.Add(target);
                    }

                    if (!haveTargetOnme && target.TargetObjectId == id)
                    {
                        haveTargetOnme = true;
                    }
                }
            }
            //没有敌对势力，那随便用
            if (!someTargetsHaveTarget) return HostileTargets;
            //返回在打队友的讨厌鬼！
            return targets.ToArray();
        }

        private static float GetRange(Action act)
        {
            sbyte range = act.Range;
            if (range < 0 && GetJobCategory(Service.ClientState.LocalPlayer, (type) => type == JobType.PhysicalRanged))
            {
                range = 25;
            }
            return Math.Max(range, 3f);
        }

        internal static bool ActionGetATarget(Action act, bool isFriendly)
        {
            //如果根本就不需要找目标，那肯定可以的。
            if (!act.CanTargetFriendly && !act.CanTargetHostile && (act.CastType == 1 ||act.CastType > 4)) return true;

            //如果在打Boss呢，那就不需要考虑AOE的问题了。
            if (Service.Configuration.IsTargetBoss && !isFriendly && act.CastType != 1) return false;

            BattleChara[] tar = isFriendly ? PartyMembers : HostileTargets;

            switch (act.CastType)
            {
                case 1: // 单体啊
                    //是个救人啊！
                    if (isFriendly && act.PrimaryCostType == 3 && act.PrimaryCostValue == 24)
                    {
                        return GetDeathPeople() != null;
                    }
                    return GetObjectInRadius(tar, GetRange(act)).Count() > 0;
                case 2: // 圆形范围攻击，看看人数够不够。

                    if (act.CanTargetHostile)
                    {
                        return GetMostObjectInRadius(tar, GetRange(act), act.EffectRange, true).Count() > 0;
                    }
                    else
                    {
                        return GetMostObjectInRadius(tar, new PlayerCharacter[] {Service.ClientState.LocalPlayer}, GetRange(act), act.EffectRange, true).Count() > 0;
                    }

                case 3: // 扇形范围攻击。看看人数够不够。
                    return GetMostObjectInArc(tar, act.EffectRange, true).Count() > 0;

                case 4: //直线范围攻击。看看人数够不够。
                    return GetMostObjectInLine(tar, GetRange(act), true).Count() > 0;
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

        /// <summary>
        /// 获得玩家某范围内的所有怪。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        internal static T[] GetObjectInRadius<T>(T[] objects, float radius, bool needAddHitbox = true) where T : GameObject
        {
            return objects.Where(o => DistanceToPlayer(o) <= (needAddHitbox ? radius + o.HitboxRadius : radius)).ToArray();
        }

        private static T[] GetMostObject<T>(T[] canAttack, float radius, float range, Func<T, T[], float, byte> HowMany, bool forCheck) where T : BattleChara
        {
            //能够打到的所有怪。
            T[] canGetObj = GetObjectInRadius(canAttack, radius);
            return GetMostObject(canAttack, canGetObj, range, range, HowMany, forCheck);
        }

        private static T[] GetMostObject<T>(T[] canAttack, T[] canGetObj ,float radius, float range, Func<T, T[], float, byte> HowMany, bool forCheck) where T : BattleChara
        {

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
            //可能可以被打到的怪。
            var canAttach = GetObjectInRadius(objects, radius + range);
            //能够打到的所有怪。
            var canGet = GetObjectInRadius(objects, radius);

            return GetMostObjectInRadius(canAttach, canGet, radius, range, forCheck);

        }

        internal static T[] GetMostObjectInRadius<T>(T[] objects, T[] canGetObjects, float radius, float range, bool forCheck) where T : BattleChara
        {
            var canAttach = GetObjectInRadius(objects, radius + range);

            return GetMostObject(canAttach, canGetObjects, radius, range, CalculateCount, forCheck);

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
            var canGet = GetObjectInRadius(objects, radius, false);

            return GetMostObject(objects, radius, radius, CalculateCount, forCheck);

            //计算一下在这些可选目标中有多少个目标可以受到攻击。
            static byte CalculateCount(T t, T[] objects, float _)
            {
                byte count = 0;

                Vector3 dir = t.Position - Service.ClientState.LocalPlayer.Position;

                foreach (T obj in objects)
                {
                    Vector3 tdir = obj.Position - Service.ClientState.LocalPlayer.Position;

                    double cos = Vector3.Dot(dir, tdir)/(dir.Length() * tdir.Length());
                    if (cos >= 0.5)
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
            var canGet = GetObjectInRadius(PartyMembers, Math.Max(action.Range, 0.1f));

            float bestHeal = 0;
            foreach (var member in canGet)
            {
                float thisHeal = 0;
                Vector3 centerPt = member.Position;
                foreach (var ran in PartyMembers)
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
