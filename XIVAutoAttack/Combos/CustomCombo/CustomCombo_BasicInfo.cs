using ImGuiScene;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public abstract partial class CustomCombo<TCmd> : ICustomCombo, IDisposable where TCmd : Enum
    {
        internal static readonly uint[] RangePhysicial = new uint[] { 23, 31, 38 };
        public abstract uint JobID { get; }
        public Role Role => (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == JobID).Role;

        public string JobName => XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == JobID).Name;

        internal static bool IsTargetDying
        {
            get
            {
                if (Target == null) return false;
                return Target.IsDying();
            }
        }

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
            get => Service.Configuration.EnabledActions.Contains(JobName);
            set
            {
                if (value)
                {
                    Service.Configuration.EnabledActions.Add(JobName);
                }
                else
                {
                    Service.Configuration.EnabledActions.Remove(JobName);
                }
            }
        }
        public virtual SortedList<DescType, string> Description { get; } = new SortedList<DescType, string>();


        internal static bool HaveSwift => Player.HaveStatus(GeneralActions.Swiftcast.BuffsProvide);

        internal virtual bool HaveShield => true;


        public TextureWrap Texture { get; private set; }
        private protected CustomCombo()
        {
            Texture = Service.DataManager.GetImGuiTextureIcon(IconSet.GetJobIcon(this));
        }

        public ActionConfiguration Config
        {
            get
            {
                var con = CreateConfiguration();
                if (Service.Configuration.ActionsConfigurations.TryGetValue(JobName, out var lastcom))
                {
                    if (con.IsTheSame(lastcom)) return lastcom;
                }
                //con.Supply(lastcom);
                Service.Configuration.ActionsConfigurations[JobName] = con;
                Service.Configuration.Save();
                return con;
            }
        }
        private protected virtual ActionConfiguration CreateConfiguration()
        {
            return new ActionConfiguration();
        }

        public void Dispose()
        {
            Texture.Dispose();
        }

        ~CustomCombo()
        {
            Dispose();
        }
    }
}
