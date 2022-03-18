using Dalamud.Game.ClientState.Conditions;

namespace XIVComboExpandedPlugin.Combos;

internal class SCHDotFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCHDotFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[3] { 17869u, 3584u, 7435u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 17869 || actionID == 3584 || actionID == 7435)
		{
			if ((((double?)TargetBuffDuration(1895) < 2.5 && level >= 72) || (TargetBuffDuration(179) < 2f && level > 1 && level < 26) || (TargetBuffDuration(189) < 2f && level >= 26 && level < 72)) && CustomCombo.HasCondition((ConditionFlag)26))
			{
				return CustomCombo.OriginalHook(17864u);
			}
			return CustomCombo.OriginalHook(17869u);
		}
		return actionID;
	}
}
