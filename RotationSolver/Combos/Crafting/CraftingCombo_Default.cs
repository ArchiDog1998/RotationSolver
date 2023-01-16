using XIVAutoAction.Actions;
using XIVAutoAction.Combos.Crafting;

namespace XIVAutoAction.Combos.Crafting;

internal class CraftingCombo_Default : CraftingCombo
{
    public override string Description => "测试用代码，如果要使用记得点开AutoAttack";

    public override string GameVersion => "6.2";

    public override string Author => "秋水";

    public override bool TryInvoke(out IAction newAction)
    {
        //需要推质量
        if (CanHQ && MaxQuality != CurrentQuality)
        {
            //第一步，直接闲静
            if (StepNumber < 2 && Reflect.ShouldUse(out newAction)) return true;

            bool highQuality = CraftCondition is Updaters.CraftCondition.Good or Updaters.CraftCondition.Excellent;

            //高质量。
            if (highQuality)
            {
                //比尔格说可以直接带走
                if (ByregotsBlessing.Quality > MaxQuality - CurrentQuality)
                {
                    if (ByregotsBlessing.ShouldUse(out newAction)) return true;
                }
                //集中加工
                if (PreciseTouch.ShouldUse(out newAction)) return true;
            }

            //如果没有长期简约啊。
            if (WasteNotTime == 0)
            {
                //弄点简约喽。这段还需要考量一下。
                if (WasteNot2.ShouldUse(out newAction)) return true;
            }
            //精修一下
            if (CurrentDurability <= 10)
            {
                if (MastersMend.ShouldUse(out newAction)) return true;
            }

            //比尔格说可以直接带走
            if (ByregotsBlessing.Quality > MaxQuality - CurrentQuality)
            {
                if (ByregotsBlessing.ShouldUse(out newAction)) return true;
            }
            //如果两个比尔格说可以带走,直接阔步！
            if (ByregotsBlessing.Quality * 2 > MaxQuality - CurrentQuality)
            {
                if (GreatStrides.ShouldUse(out newAction)) return true;
            }

            //如果一个普通制作还不能送走
            if (BasicSynthesis.Progress <= MaxProgress - CurrentProgress)
            {
                //一个高速制作会把它送走 最终确认一下
                if (RapidSynthesis.Progress > MaxProgress - CurrentProgress)
                {
                    if (FinalAppraisal.ShouldUse(out newAction)) return true;
                }

                //高速制作
                if (RapidSynthesis.ShouldUse(out newAction)) return true;
            }

            //能随便送走，但是质量还不行，普通推质量。
            if (Innovation.ShouldUse(out newAction)) return true;
            if (AdvancedTouch.ShouldUse(out newAction)) return true;
            if (StandardTouch.ShouldUse(out newAction)) return true;
            if (BasicTouch.ShouldUse(out newAction)) return true;
        }

        //第一步，直接坚信
        if (StepNumber < 2 && MuscleMemory.ShouldUse(out newAction)) return true;

        //好了，制作一下。
        if (BasicSynthesis.ShouldUse(out newAction)) return true;

        newAction = null;
        return false;
    }
}
