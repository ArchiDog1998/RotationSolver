using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Helpers.ReflectionHelper;

namespace XIVAutoAttack.Combos.CustomCombo
{
    internal abstract partial class CustomCombo<TCmd> : ICustomCombo where TCmd : Enum
    {
        public abstract ClassJobID[] JobIDs { get; }

        public abstract string GameVersion { get; }

        public ClassJob Job => Service.DataManager.GetExcelSheet<ClassJob>().GetRow((uint)JobIDs[0]);

        public string Name => Job.Abbreviation + " - " + Job.Name;

        /// <summary>
        /// 作者
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// 目标是否将要死亡
        /// </summary>
        internal static bool IsTargetDying
        {
            get
            {
                if (Target == null) return false;
                return Target.IsDying();
            }
        }

        /// <summary>
        /// 目标是否是Boss
        /// </summary>
        internal static bool IsTargetBoss
        {
            get
            {
                if (Target == null) return false;
                return Target.IsBoss();
            }
        }

        public bool IsEnabled
        {
            get => Service.Configuration.EnabledCombos.Contains(Name);
            set
            {
                if (value)
                {
                    Service.Configuration.EnabledCombos.Add(Name);
                }
                else
                {
                    Service.Configuration.EnabledCombos.Remove(Name);
                }
            }
        }
        public string Description => string.Join('\n', DescriptionDict.Select(pair => pair.Key.ToName() + " → " + pair.Value));

        /// <summary>
        /// 说明字典
        /// </summary>
        public virtual SortedList<DescType, string> DescriptionDict { get; } = new SortedList<DescType, string>();

        /// <summary>
        /// 有即刻相关Buff
        /// </summary>
        internal static bool HaveSwift => Player.HasStatus(true, Swiftcast.BuffsProvide);

        /// <summary>
        /// 有盾姿，如果为非T那么始终为true
        /// </summary>
        [ReflectableMember]
        internal bool HaveShield => Player.HasStatus(true, StatusHelper.SheildStatus);


        public uint IconID { get; }
        private protected CustomCombo()
        {
            IconID = IconSet.GetJobIcon(this);
        }

        public ActionConfiguration Config => ActionConfiguration.GetConfig((uint)JobIDs[0], Author, CreateConfiguration());

        public BattleChara MoveTarget
        {
            get
            {
                if (MoveAbility(1, out var act) && act is BaseAction a) return a.Target;
                return null;
            }
        }

        private protected virtual ActionConfiguration CreateConfiguration()
        {
            return new ActionConfiguration();
        }
    }
}
