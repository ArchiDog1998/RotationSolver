using ImGuiScene;
using Lumina.Data.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.CustomCombo
{
    internal abstract partial class CustomCombo<TCmd> : CustomComboActions, ICustomCombo where TCmd : Enum
    {
        internal static readonly uint[] RangePhysicial = new uint[] { 23, 31, 38, 5 };
        public abstract ClassJobID[] JobIDs { get; }
        public Role Role => (Role)XIVAutoAttackPlugin.AllJobs.First(job => (uint)JobIDs[0] == job.RowId).Role;

        public string Name => XIVAutoAttackPlugin.AllJobs.First(job => (uint)JobIDs[0] == job.RowId).Name;
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
        public string Description => string.Join('\n', DescriptionDict.Select(pair => pair.Key.ToString() + " → " + pair.Value));

        /// <summary>
        /// 说明字典
        /// </summary>
        public virtual SortedList<DescType, string> DescriptionDict { get; } = new SortedList<DescType, string>();

        /// <summary>
        /// 有即刻相关Buff
        /// </summary>
        internal static bool HaveSwift => Player.HaveStatusFromSelf(Swiftcast.BuffsProvide);

        internal virtual bool HaveShield => true;


        public uint IconID { get; }
        private protected CustomCombo()
        {
            IconID = IconSet.GetJobIcon(this);
        }

        public ActionConfiguration Config
        {
            get
            {
                var con = CreateConfiguration();
                if (Service.Configuration.CombosConfigurations.TryGetValue((uint)JobIDs[0], out var lastcom))
                {
                    if(lastcom.TryGetValue(Author, out var lastCon))
                    {
                        if (con.IsTheSame(lastCon)) return lastCon;
                    }
                    lastcom[Author] = con;
                }
                else
                {
					Service.Configuration.CombosConfigurations.Add((uint)JobIDs[0], new Dictionary<string, ActionConfiguration>() { {Author,con } });
				}
                Service.Configuration.Save();
                return con;
            }
        }

        private protected virtual ActionConfiguration CreateConfiguration()
        {
            return new ActionConfiguration();
        }
    }
}
