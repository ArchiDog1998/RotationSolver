﻿using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using System.Numerics;
using AutoAction.Actions;
using AutoAction.Data;
using AutoAction.Updaters;

namespace AutoAction.Helpers
{
    internal static class ObjectHelper
    {
        private unsafe static BNpcBase GetObjectNPC(this GameObject obj)
        {
            if (obj == null) return null;
            return Service.DataManager.GetExcelSheet<BNpcBase>().GetRow(obj.DataId);
        }

        internal static bool HasLocationSide(this GameObject obj)
        {
            if (obj == null) return false;
            return !(obj.GetObjectNPC()?.Unknown10 ?? false);
        }

        internal static bool IsBoss(this BattleChara obj)
        {
            if (obj == null) return false;
            return obj.MaxHp >= GetHealthFromMulty(1.85f)
                || !(obj.GetObjectNPC()?.IsTargetLine ?? true);
        }

        /// <summary>
        /// 获得目标的当前血量百分比
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static float GetHealthRatio(this BattleChara b)
        {
            if (b == null) return 0;
            return (float)b.CurrentHp / b.MaxHp;
        }

        internal static float GetHealingRatio(this BattleChara b)
        {
            return b.GetHealthRatio();
        }

        /// <summary>
        /// 用于倾泻所有资源来收尾
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static bool IsDying(this BattleChara b)
        {
            if (b == null) return false;
            return b.CurrentHp <= GetHealthFromMulty(0.8f) || b.GetHealthRatio() < 0.02f;
        }

        /// <summary>
        /// 用于判断是否能上Dot
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static bool CanDot(this BattleChara b)
        {
            if (b == null) return false;
            return b.CurrentHp >= GetHealthFromMulty(1.5f);
        }

        internal static EnemyLocation FindEnemyLocation(this GameObject enemy)
        {
            Vector3 pPosition = enemy.Position;
            float rotation = enemy.Rotation;
            Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

            Vector3 dir = Service.ClientState.LocalPlayer.Position - pPosition;
            Vector2 dirVec = new Vector2(dir.Z, dir.X);

            double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());

            if (angle < Math.PI / 4) return EnemyLocation.Front;
            else if (angle > Math.PI * 3 / 4) return EnemyLocation.Back;
            return EnemyLocation.Side;
        }

        public unsafe static bool CanAttack(this GameObject actor)
        {
            if (actor == null) return false;
            return ((delegate*<long, IntPtr, long>)Service.Address.CanAttackFunction)(142L, actor.Address) == 1;
        }

#if DEBUG
        internal static uint GetHealthFromMulty(float mult)
#else
        private static uint GetHealthFromMulty(float mult)
#endif
        {
            if (Service.ClientState.LocalPlayer == null) return 0;

            var role = Service.DataManager.GetExcelSheet<ClassJob>().GetRow(
                    Service.ClientState.LocalPlayer.ClassJob.Id).GetJobRole();
            float multi = mult * role switch
            {
                JobRole.Tank => 1,
                JobRole.Healer => 1.6f,
                _ => 1.5f,
            };

            var partyCount = TargetUpdater.PartyMembers.Count();
            if (partyCount > 4)
            {
                multi *= 6.4f;
            }
            else if (partyCount > 1)
            {
                multi *= 3.5f;
            }

            return (uint)(multi * Service.ClientState.LocalPlayer.MaxHp);
        }
    }
}
