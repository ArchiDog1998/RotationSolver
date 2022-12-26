using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Localization;

namespace XIVAutoAttack.Combos.Crafting
{
    internal abstract partial class CraftingCombo : ICustomCombo
    {
        public ClassJob Job => Service.DataManager.GetExcelSheet<ClassJob>().GetRow((uint)JobIDs[0]);

        public ClassJobID[] JobIDs => new ClassJobID[]
        {
            ClassJobID.Carpenter,
            ClassJobID.Blacksmith,
            ClassJobID.Armorer,
            ClassJobID.Goldsmith,
            ClassJobID.Leatherworker,
            ClassJobID.Weaver,
            ClassJobID.Alchemist,
            ClassJobID.Culinarian,
        };

        public abstract string Description { get; }

        public abstract string GameVersion { get; }

        public abstract string Author { get; }

        public ActionConfiguration Config => ActionConfiguration.GetConfig((uint)JobIDs[0], Author, CreateConfiguration());

        public BattleChara MoveTarget => null;

        public SortedList<DescType, string> DescriptionDict => new();

        public Dictionary<string, string> CommandShow => new();

        public IAction[] AllActions => GetBaseActions(GetType());

        private IAction[] GetBaseActions(Type type)
        {
            if (type == null) return new IAction[0];

        
            var acts = from prop in type.GetProperties()
                       where typeof(IAction).IsAssignableFrom(prop.PropertyType)
                       select (IAction)prop.GetValue(this) into act
                       orderby act.ID
                       select act;

            return acts.Union(GetBaseActions(type.BaseType)).ToArray();
        }

        public PropertyInfo[] AllBools => new PropertyInfo[0];

        public PropertyInfo[] AllBytes => new PropertyInfo[0];

        public MethodInfo[] AllTimes =>  new MethodInfo[0];

        public MethodInfo[] AllLast => new MethodInfo[0];

        public MethodInfo[] AllOther => new MethodInfo[0];

        public MethodInfo[] AllGCDs => new MethodInfo[0];

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

        public uint IconID { get; } = 61816;

        public string Name => LocalizationManager.RightLang.CraftingCombo_Name;

        private protected CraftingCombo()
        {
        }

        private protected virtual ActionConfiguration CreateConfiguration()
        {
            return new ActionConfiguration();
        }

        public string OnCommand(string args)
        {
            return string.Empty;
        }

        public abstract bool TryInvoke(out IAction newAction);
    }
}
