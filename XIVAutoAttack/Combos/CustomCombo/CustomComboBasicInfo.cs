using ImGuiScene;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.CustomCombo
{
    public abstract partial class CustomCombo
    {
        internal static readonly uint[] RangePhysicial = new uint[] { 23, 31, 38 };
        internal abstract uint JobID { get; }
        internal Role Role => (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == JobID).Role;

        internal string JobName => XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == JobID).Name;

        internal static bool IsTargetDying
        {
            get
            {
                if (Target == null) return false;
                return Target.CurrentHp <= TargetFilter.GetHealthFromMulty(1);
            }
        }
        protected static internal BaseAction ActionID => GeneralActions.Repose;

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
        internal virtual SortedList<DescType, string> Description { get; } = new SortedList<DescType, string>();


        internal static bool HaveSwift
        {
            get
            {
                foreach (var status in Service.ClientState.LocalPlayer.StatusList)
                {
                    if (GeneralActions.Swiftcast.BuffsProvide.Contains((ushort)status.StatusId))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        internal virtual bool HaveShield => true;


        internal TextureWrap Texture;
        private protected CustomCombo()
        {
            Texture = Service.DataManager.GetImGuiTextureIcon(IconSet.GetJobIcon(this));
        }

        internal ActionConfiguration Config
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
    }
}
