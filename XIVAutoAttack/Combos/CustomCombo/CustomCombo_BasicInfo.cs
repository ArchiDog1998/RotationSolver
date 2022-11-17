using ImGuiScene;
using Lumina.Data.Parsing;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.CustomCombo
{
    internal abstract partial class CustomCombo<TCmd> : CustomComboActions, ICustomCombo where TCmd : Enum
    {
        public abstract ClassJobID[] JobIDs { get; }

        public ClassJob Job => Service.DataManager.GetExcelSheet<ClassJob>().GetRow((uint)JobIDs[0]);

        public string Name => Job.Name;

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
        internal static bool HaveSwift => Player.HasStatus(true, Swiftcast.BuffsProvide);

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
                        if (con.IsTheSame(lastCon))
                        {
                            return con;
                        }
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
