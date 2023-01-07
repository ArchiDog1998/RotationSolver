using System;
using AutoAction.Data;
using AutoAction.Helpers;
using AutoAction.Updaters;

namespace AutoAction.Actions.BaseCraftAction;

internal partial class BaseCraftAction
{
    protected static ulong Craftsmanship => Service.Address.getBaseParam(Service.Address.playerStaticAddress, 70);
    protected static ulong Control => Service.Address.getBaseParam(Service.Address.playerStaticAddress, 71);
    internal Func<byte, double> GetBaseMultiply { get; }

    public uint Quality
    {
        get
        {
            var player = Service.ClientState.LocalPlayer;
            var innerMulti = player.StatusStack(false, StatusID.InnerQuiet) * 0.1 + 1;
            double innovation = player.HasStatus(false, StatusID.Innovation) ? 0.5 : 0;
            double greatStrides = player.HasStatus(false, StatusID.GreatStrides) ? 1 : 0;

            var multiply = SynthesisUpdater.CraftCondition switch
            {
                CraftCondition.Poor => 0.5,
                CraftCondition.Normal => 1,
                CraftCondition.Good => 1.5,
                CraftCondition.Excellent => 4,
                _ => 1
            };

            return (uint)(QualityBase * multiply * innerMulti * (innovation + greatStrides + 1));
        }
    }

    public double QualityBase
    {
        get
        {
            var recipe = SynthesisUpdater.Recipe;

            if (recipe == null) return 0;

            var baseValue = Control * 10 / recipe.RecipeLevelTable.Value.QualityDivider + 35;
            if (Service.ClientState.LocalPlayer.Level <= recipe.RecipeLevelTable.Value.ClassJobLevel)
            {
                baseValue *= recipe.RecipeLevelTable.Value.QualityModifier / 100u;
            }

            var multiply = GetBaseMultiply(Service.ClientState.LocalPlayer.Level);

            return baseValue * multiply;
        }
    }

    public uint Progress
    {
        get
        {
            var player = Service.ClientState.LocalPlayer;
            double veneration = player.HasStatus(false, StatusID.Veneration) ? 0.5 : 0;
            double muscleMemory = player.HasStatus(false, StatusID.MuscleMemory) ? 1 : 0;


            var multiply = (SynthesisUpdater.CraftCondition == CraftCondition.Malleable ? 1.5 : 1);

            return (uint)(ProgressBase * multiply * (veneration + muscleMemory + 1));
        }
    }

    public double ProgressBase
    {
        get
        {
            var recipe = SynthesisUpdater.Recipe;

            if (recipe == null) return 0;

            var baseValue = Craftsmanship * 10 / recipe.RecipeLevelTable.Value.ProgressDivider + 2;
            if (Service.ClientState.LocalPlayer.Level <= recipe.RecipeLevelTable.Value.ClassJobLevel)
            {
                baseValue *= recipe.RecipeLevelTable.Value.ProgressModifier / 100u;
            }

            var multiply = GetBaseMultiply(Service.ClientState.LocalPlayer.Level);

            return baseValue * multiply;
        }
    }
}
