using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = Lumina.Excel.GeneratedSheets.Action;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

namespace XIVComboPlus.Combos
{
    internal class BaseAction
    {

        internal const byte GCDCooldownGroup = 58;

        private bool _isFriendly;
        private bool _shouldEndSpecial;
        internal Action Action { get; }
        //internal IconReplacer.CooldownData CoolDown => Service.IconReplacer.GetCooldown(Action.CooldownGroup);
        internal byte Level => Action.ClassJobLevel;
        internal uint ActionID => Action.RowId;
        internal bool IsGCD { get; }

        internal EnemyLocation EnermyLocation { get; set; } = EnemyLocation.None;
        internal virtual uint MPNeed { get; }

        #region CoolDown
        /// <summary>
        /// 复唱时间
        /// </summary>
        internal unsafe float RecastTime => ActionManager.Instance()->GetRecastTime(ActionType.Spell, ActionID);
        /// <summary>
        /// 咏唱时间
        /// </summary>
        internal int Cast100 => Action.Cast100ms - (HaveStatusSelfFromSelf(ObjectStatus.LightSpeed) ? 25 : 0);
        internal float RecastTimeRemain => RecastTime - RecastTimeElapsed;
        internal unsafe float RecastTimeElapsed => ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Spell, ActionID);
        internal unsafe ushort MaxCharges => Math.Max(ActionManager.GetMaxCharges(ActionID, Service.ClientState.LocalPlayer.Level), (ushort)1);
        internal unsafe bool IsCoolDown => ActionManager.Instance()->IsRecastTimerActive(ActionType.Spell, ActionID);
        #endregion
        internal BattleChara Target { get; private set; } = Service.ClientState.LocalPlayer;
        private Vector3 _position = default;
        /// <summary>
        /// 如果之前是这些ID，那么就不会执行。
        /// </summary>
        internal uint[] OtherIDsNot { private get; set; } = null;

        /// <summary>
        /// 如果之前不是这些ID中的某个，那么就不会执行。
        /// </summary>
        internal uint[] OtherIDsCombo { private get; set; } = null;
        /// <summary>
        /// 给敌人造成的Debuff,如果有这些Debuff，那么不会执行。
        /// </summary>
        internal ushort[] TargetStatus { get; set; } = null;
        /// <summary>
        /// 使用了这个技能会得到的Buff，如果有这些Buff中的一种，那么就不会执行。 
        /// </summary>
        internal ushort[] BuffsProvide { get; set; } = null;

        /// <summary>
        /// 使用这个技能需要的前置Buff，有任何一个就好。
        /// </summary>
        internal ushort[] BuffsNeed { get; set; } = null;

        /// <summary>
        /// 如果有一些别的需要判断的，可以写在这里。True表示可以使用这个技能。
        /// </summary>
        internal Func<BattleChara, bool> OtherCheck { get; set; } = null;

        internal unsafe void SayingOut()
        {
            foreach (var item in Service.Configuration.Events)
            {
                if(item.Name == Action.Name)
                {
                    if (item.MacroIndex < 0 || item.MacroIndex > 99) return;

                    TargetHelper.Macros.Enqueue(new MacroItem(Target, item.IsShared ? RaptureMacroModule.Instance->Shared[item.MacroIndex] :
                        RaptureMacroModule.Instance->Individual[item.MacroIndex]));

                    return;
                }
            }
        }

        internal Func<BattleChara[], BattleChara> ChoiceFriend { get; set; } = availableCharas =>
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

            return availableCharas.OrderBy(player => (float)player.CurrentHp / player.MaxHp).First();
        };

        internal Func<BattleChara[], BattleChara> ChoiceHostile { get; set; } = availableCharas =>
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
        };

        internal Func<BattleChara[], BattleChara[]> FilterForHostile { get; set; } = availableCharas => availableCharas;

        internal BaseAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false)
        {
            this.Action = Service.DataManager.GetExcelSheet<Action>().GetRow(actionID);
            this._shouldEndSpecial = shouldEndSpecial;
            _isFriendly = isFriendly;
            this.IsGCD = Action.CooldownGroup == GCDCooldownGroup;

            if (Action.PrimaryCostType == 3 || Action.PrimaryCostType == 4)
            {
                this.MPNeed = Action.PrimaryCostValue * 100u;
            }
            else if (Action.SecondaryCostType == 3 || Action.SecondaryCostType == 4)
            {
                this.MPNeed = Action.SecondaryCostValue * 100u;
            }
            else
            {
                this.MPNeed = 0;
            }
        }

        internal static BattleChara FindMoveTarget(BattleChara[] charas)
        {
            Vector3 pPosition = Service.ClientState.LocalPlayer.Position;
            float rotation = Service.ClientState.LocalPlayer.Rotation;
            Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

            var tar = charas.Where(t =>
            {
                Vector3 dir = t.Position - pPosition;
                Vector2 dirVec = new Vector2(dir.Z, dir.X);
                double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());
                return angle <= Math.PI / 6;
            }).OrderBy(t => Vector3.Distance(t.Position, pPosition)).Last();

            if (TargetHelper.DistanceToPlayer(tar) < 5) return null;

            return tar;
        }

        public bool FindTarget(bool mustUse)
        {
            _position = Service.ClientState.LocalPlayer.Position;
            float range = GetRange(Action);

            //如果都没有距离，这个还需要选对象嘛？选自己啊！
            if (range == 0 &&　Action.EffectRange == 0)
            {
                Target = Service.ClientState.LocalPlayer;
                return true;
            }

            if (Action.TargetArea)
            {
                var tars = GetMostObjectInRadius(_isFriendly ? TargetHelper.PartyMembers : TargetHelper.HostileTargets, range, Action.EffectRange, _isFriendly, mustUse)
                    .OrderByDescending(p => (float)p.CurrentHp / p.MaxHp);
                Target = tars.Count() > 0 ? tars.First() : Service.ClientState.LocalPlayer;
                _position = Target.Position;
                return true;
            }
            //如果能对友方和敌方都能选中
            else if (Action.CanTargetParty && Action.CanTargetHostile)
            {
                BattleChara[] availableCharas = TargetHelper.PartyMembers.Union(TargetHelper.HostileTargets).Where(b => b.ObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();
                availableCharas = GetObjectInRadius(availableCharas, range);
                //特殊选队友的方法。
                var tar = ChoiceFriend(availableCharas);
                if (tar == null) return false;
                Target = tar;
                return true;

            }
            //首先看看是不是能对小队成员进行操作的。
            else if (Action.CanTargetParty)
            {
                //还消耗2400的蓝，那肯定是复活的。
                if (Action.PrimaryCostType == 3 && Action.PrimaryCostValue == 24)
                {
                    var tar = TargetHelper.GetDeathPeople();
                    if(tar == null) return false;
                    Target = tar;
                    return true;
                }

                //找到没死的队友们。
                BattleChara[] availableCharas = TargetHelper.PartyMembers.Where(player => player.CurrentHp != 0).ToArray();
                if (!Action.CanTargetSelf)
                {
                    availableCharas = availableCharas.Where(p => p.ObjectId != Service.ClientState.LocalPlayer.ObjectId).ToArray();
                }

                //判断是否是范围。
                if (Action.CastType > 1)
                {
                    //找到能覆盖最多的位置，并且选血最少的来。
                    var tar = GetMostObjectInRadius(availableCharas, range, Action.EffectRange, true, mustUse).OrderBy(p => (float)p.CurrentHp / p.MaxHp).First();
                    if (tar == null) return false;
                    Target = tar;
                    return true;
                }
                else
                {
                    availableCharas = GetObjectInRadius(availableCharas, range);
                    //特殊选队友的方法。
                    var tar = ChoiceFriend(availableCharas);
                    if (tar == null) return false;
                    Target = tar;
                    return true;
                }
            }
            //再看看是否可以选中敌对的。
            else if (Action.CanTargetHostile)
            {
                //如果不用自动找目标，那就直接返回。
                if (!IconReplacer.AutoTarget)
                {
                    if(Service.TargetManager.Target is BattleChara b && TargetHelper.CanAttack(b) && DistanceToPlayer(b) <= range)
                    {
                        this.Target = b;
                        return true;
                    }
                    else
                    {
                        this.Target = null;
                        return false;
                    }
                }
                switch (Action.CastType)
                {
                    case 1:
                    default:
                        BattleChara[] canReachTars = FilterForHostile(GetObjectInRadius(TargetHelper.HostileTargets, GetRange(Action)));
                        var tar = ChoiceHostile(canReachTars);
                        if (tar == null) return false;
                        Target = tar;
                        return true;

                    case 2: // 圆形范围攻击。找到能覆盖最多的位置，并且选血最多的来。
                        tar = ChoiceHostile(GetMostObjectInRadius(FilterForHostile(TargetHelper.HostileTargets), range, Action.EffectRange, false, mustUse));
                        if (tar == null) return false;
                        Target = tar;
                        return true;
                    case 3: // 扇形范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        tar = ChoiceHostile(GetMostObjectInArc(FilterForHostile(TargetHelper.HostileTargets), Action.EffectRange, mustUse));
                        if (tar == null) return false;
                        Target = tar;
                        return true;
                    case 4: //直线范围攻击。找到能覆盖最多的位置，并且选最远的来。
                        tar = ChoiceHostile(GetMostObjectInLine(FilterForHostile(TargetHelper.HostileTargets), range, mustUse));
                        if (tar == null) return false;
                        Target = tar;
                        return true;
                }
            }
            //如果只能选自己，那就选自己吧。
            Target = Service.ClientState.LocalPlayer;
            if(Action.EffectRange > 0 && !_isFriendly)
            {
                var count = GetObjectInRadius(TargetHelper.HostileTargets, Action.EffectRange).Length;
                if (count < (mustUse ? 1 :Service.Configuration.HostileCount)) return false;
            }

            return true;
        }

        internal static BattleChara[] ProvokeTarget(BattleChara[] inputCharas, out bool haveTargetOnme, bool needDistance = false)
        {
            var tankIDS = TargetHelper.GetJobCategory(TargetHelper.AllianceMembers, Role.防护).Select(member => member.ObjectId);
            var loc = Service.ClientState.LocalPlayer.Position;
            var id = Service.ClientState.LocalPlayer.ObjectId;

            bool someTargetsHaveTarget = false;
            haveTargetOnme = false;
            List<BattleChara> targets = new List<BattleChara>();
            foreach (var target in inputCharas)
            {
                //有目标
                if (target.TargetObject?.IsValid() ?? false)
                {
                    someTargetsHaveTarget = true;

                    //居然在打非T！
                    if (!tankIDS.Contains(target.TargetObjectId) &&(!needDistance || Vector3.Distance(target.Position, loc) > 5))
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
            if (!someTargetsHaveTarget) return inputCharas;
            //返回在打队友的讨厌鬼！
            return targets.ToArray();
        }
        internal static float DistanceToPlayer(GameObject obj)
        {
            return Vector3.Distance(Service.ClientState.LocalPlayer.Position, obj.Position);
        }
        internal static T[] GetObjectInRadius<T>(T[] objects, float radius) where T : GameObject
        {
            return objects.Where(o => DistanceToPlayer(o) <= radius + o.HitboxRadius).ToArray();
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

            int maxCount = mustUse ? 1 : (isfriend ? Service.Configuration.PartyCount : Service.Configuration.HostileCount);

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
                    if (Vector3.Distance(t.Position, obj.Position) <= range)
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

        private static T[] GetMostObjectInLine<T>(T[] objects, float radius, bool mustUse) where T : BattleChara
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

        private static float GetRange(Action act)
        {
            sbyte range = act.Range;
            if (range < 0 && CustomCombo.RangePhysicial.Contains( Service.ClientState.LocalPlayer.ClassJob.GameData.RowId))
            {
                range = 25;
            }
            return Math.Max(range, 3f);
        }

        //private static bool GetJobCategory(BattleChara obj, Role role)
        //{
        //    SortedSet<byte> validJobs = new SortedSet<byte>(XIVComboPlusPlugin.AllJobs.Where(job => job.Role == (byte)role).Select(job => (byte)job.RowId));
        //    return GetJobCategory(obj, validJobs);
        //}
        //private static bool GetJobCategory(BattleChara obj, SortedSet<byte> validJobs)
        //{
        //    return validJobs.Contains((byte)obj.ClassJob.GameData?.RowId);
        //}
        public bool ShouldUseAction(out BaseAction act, uint lastAct = 0, bool mustUse = false, bool Empty = false)
        {
            act = this;
            byte level = Service.ClientState.LocalPlayer.Level;

            //等级不够。
            if (level < this.Level) return false;

            //MP不够
            if (Service.ClientState.LocalPlayer.CurrentMp < this.MPNeed) return false;

            if (!mustUse)
            {
                //没有前置Buff
                if (BuffsNeed != null)
                {
                    if (!HaveStatusSelfFromSelf(BuffsNeed)) return false;
                }

                //已有提供的Buff的任何一种
                if (BuffsProvide != null)
                {
                    if (HaveStatusSelfFromSelf(BuffsProvide)) return false;
                }
            }


            //如果是能力技能，而且没法释放。
            if (!IsGCD && IsCoolDown)
            {
                //冷却时间没超过一成
                if (RecastTimeElapsed < RecastTime / MaxCharges) return false;
            }

            //看看有没有目标，如果没有，就说明不符合条件。
            if(!FindTarget(mustUse)) return false;

            if (IsGCD)
            {
                //如果有输入上次的数据，那么上次不能是上述的ID。
                if (OtherIDsNot != null)
                {
                    foreach (var id in OtherIDsNot)
                    {
                        if (lastAct == id) return false;
                    }
                }

                //如果有Combo，有LastAction，而且上次不是连击，那就不触发。
                uint[] comboActions = Action.ActionCombo.Row == 0 ? new uint[0] : new uint[] { Action.ActionCombo.Row };
                if (OtherIDsCombo != null) comboActions = comboActions.Union(OtherIDsCombo).ToArray();
                bool findCombo = false;
                foreach (var comboAction in comboActions)
                {
                    if (comboAction == lastAct)
                    {
                        findCombo = true;
                        break;
                    }
                }
                if (!findCombo && comboActions.Length > 0) return false;

                //如果是个法术需要咏唱，并且还在移动，也没有即刻相关的技能。
                if (Cast100 > 0 && TargetHelper.IsMoving)
                {
                    if (!HaveStatusSelfFromSelf(CustomCombo.GeneralActions.Swiftcast.BuffsProvide)) return false;
                }

                //目标已有充足的Debuff
                if (!mustUse && TargetStatus != null)
                {
                    var tar = Target == Service.ClientState.LocalPlayer ? TargetHelper.HostileTargets.OrderBy(p=>DistanceToPlayer(p)).First() : Target;
                    var times = FindStatusFromSelf(tar, TargetStatus);
                    if (times.Length > 0 && times.Max() > 6) return false;
                }
            }
            else
            {
                //如果是能力技能，还没填满。
                if (!(mustUse ||Empty) && RecastTimeRemain > 5) return false;
            }

            //用于自定义的要求没达到
            if (OtherCheck != null && !OtherCheck(Target)) return false;

            return true;
        }

        internal unsafe bool UseAction()
        {
            var loc = new FFXIVClientStructs.FFXIV.Client.Graphics.Vector3() { X = _position.X, Y = _position.Y,  Z = _position.Z };

            bool result = Action.TargetArea ? ActionManager.Instance()->UseActionLocation(ActionType.Spell, ActionID, Service.ClientState.LocalPlayer.ObjectId, &loc) :
             ActionManager.Instance()->UseAction(ActionType.Spell, Service.IconReplacer.OriginalHook(ActionID), Target.ObjectId);

            if (_shouldEndSpecial) IconReplacer.ResetSpecial();

            return result;
        }

        internal static bool HaveStatusSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusSelfFromSelf(effectIDs).Length > 0;
        }
        internal static float[] FindStatusSelfFromSelf(params ushort[] effectIDs)
        {
            return FindStatusFromSelf(Service.ClientState.LocalPlayer, effectIDs);
        }

        internal  static float[] FindStatusFromSelf(BattleChara obj, params ushort[] effectIDs)
        {
            uint[] newEffects = effectIDs.Select(a => (uint)a).ToArray();
            return FindStatusFromSelf(obj).Where(status => newEffects.Contains(status.StatusId)).Select(status => status.RemainingTime).ToArray();
        }

        private static Dalamud.Game.ClientState.Statuses.Status[] FindStatusFromSelf(BattleChara obj)
        {
            if (obj == null) return new Dalamud.Game.ClientState.Statuses.Status[0];

            return  obj.StatusList.Where(status => status.SourceID == Service.ClientState.LocalPlayer.ObjectId).ToArray();
        }
    }
}
