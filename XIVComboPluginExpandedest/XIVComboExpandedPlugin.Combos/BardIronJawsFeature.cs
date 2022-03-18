using Dalamud.Game.ClientState.Statuses;

namespace XIVComboExpandedPlugin.Combos;

internal class BardIronJawsFeature : CustomCombo
{
	protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardIronJawsFeature;


	protected internal override uint[] ActionIDs { get; } = new uint[1] { 3560u };


	protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
	{
		if (actionID == 3560)
		{
			if (level < 30)
			{
				return 100u;
			}
			if (level < 56)
			{
				Status val = CustomCombo.FindTargetEffect(124);
				Status val2 = CustomCombo.FindTargetEffect(129);
				if (val == null)
				{
					return 100u;
				}
				if (val2 == null)
				{
					return 113u;
				}
				if (((val != null) ? new float?(val.RemainingTime) : null) < ((val2 != null) ? new float?(val2.RemainingTime) : null))
				{
					return 100u;
				}
				return 113u;
			}
			if (level < 64)
			{
				bool num = CustomCombo.TargetHasEffect(124);
				bool flag = CustomCombo.TargetHasEffect(129);
				if (num && flag)
				{
					return 3560u;
				}
				if (flag)
				{
					return 100u;
				}
				return 113u;
			}
			bool num2 = CustomCombo.TargetHasEffect(1200);
			bool flag2 = CustomCombo.TargetHasEffect(1201);
			if (num2 && flag2)
			{
				return 3560u;
			}
			if (flag2)
			{
				return 7406u;
			}
			return 7407u;
		}
		return actionID;
	}
}
