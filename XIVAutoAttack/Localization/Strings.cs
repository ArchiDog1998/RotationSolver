using System.Resources;
using System.Reflection;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Lumina.Excel.GeneratedSheets;

namespace XIVAutoAttack.Localization
{
    internal class Strings
    {
        #region ConfigWindow
        public string ConfigWindow_Header { get; set; } = "Auto Attack Settings v";
        public string ConfigWindow_AboutItem { get; set; } = "About";
        public string ConfigWindow_AttackItem { get; set; } = "Attack";
        public string ConfigWindow_ParamItem { get; set; } = "Param";
        public string ConfigWindow_EventsItem { get; set; } = "Events";
        public string ConfigWindow_ActionsItem { get; set; } = "Actions";
        public string ConfigWindow_HelpItem { get; set; } = "Help";

        public string ConfigWindow_ActionItem_Description { get; set; }
            = "In this panel, you can modify the use conditions for every action.";

        public string ConfigWindow_HelpItem_Description { get; set; }
            = "In this panel, you can see all commands for combat that can use in for XIV Auto Attack";

        public string Configwindow_HelpItem_AttackSmart { get; set; }
            = "Start Attacking in auto-target mode when not in combat. Switch the targeting condition in combat.";

        public string Configwindow_HelpItem_AttackManual { get; set; }
            = "Start Attacking in manual mode.";

        public string Configwindow_HelpItem_AttackCancel { get; set; }
            = "Stop Attacking, please always do it!";

        public string Configwindow_HelpItem_HealArea { get; set; }
            = "Open one window period to use one or serveral range healing actions.";

        public string Configwindow_HelpItem_HealSingle { get; set; }
            = "Open one window period to use one or serveral single healing actions.";

        public string Configwindow_HelpItem_DefenseArea { get; set; }
            = "Open one window period to use one or serveral range defense actions.";

        public string Configwindow_HelpItem_DefenseSingle { get; set; }
            = "Open one window period to use one or serveral single defense actions.";

        public string Configwindow_HelpItem_EsunaShield { get; set; }
            = "Open one window period to use Esuna,tank stance skills or True North.";

        public string Configwindow_HelpItem_RaiseShirk { get; set; }
            = "Open one window period to use Raise or Shirk.";

        public string Configwindow_HelpItem_AntiRepulsion { get; set; }
            = "Open one window period to use knockback-penalty skill.";

        public string Configwindow_HelpItem_BreakProvoke { get; set; }
            = "Open one window period to use break-combo or provoke skill.";

        public string Configwindow_HelpItem_Move { get; set; }
            = "Open one window period to use move skill.";

        public string Configwindow_Helper_SwitchAuthor { get; set; } = "Click to switch authors";
        public string Configwindow_Helper_GameVersion { get; set; } = "Game Version";
        public string Configwindow_Helper_EditCombo { get; set; } = "Click to edit this custom Combo.";
        public string Configwindow_Helper_AddCombo { get; set; } = "Add a custom Combo.";
        public string Configwindow_Helper_OpenSource { get; set; } = "Open the source code URL";
        public string Configwindow_Helper_RunCommand { get; set; } = "Click to execute the command";
        public string Configwindow_Helper_InsertCommand { get; set; } = "Top priority insertion within 5s \"{0}\"";
        public string Configwindow_AttackItem_Description { get; set; } = "You can choose to turn on continuous GCDs, 0GCDs for the desired profession.\r\nThe occupation in the settings is the same as the current in-game occupation, then there will be a reminder to set the corresponding macro.";
        public string Configwindow_AttackItem_ScriptFolder { get; set; } = "Custom Loop Storage Path";
        public string Configwindow_AttackItem_LoadScript { get; set; } = "Click to load a custom loop";
        public string Configwindow_AttackItem_ScriptFolderError { get; set; } = "Please set a storage path to use the custom loop properly!";
        public string Configwindow_AttackItem_KeyName { get; set; } = "The key name is";
        public string Configwindow_Events_AddEvent { get; set; } = "Adding Events";
        public string Configwindow_Events_Description { get; set; } = "In this window, you can set what macros to use after some skills are released.";
        public string Configwindow_Events_ActionName { get; set; } = "Skill Name";
        public string Configwindow_Events_MacroIndex { get; set; } = "Macro No.";
        public string Configwindow_Events_ShareMacro { get; set; } = "Shared Macro No.";
        public string Configwindow_Events_RemoveEvent { get; set; } = "Delete Event";
        public string Configwindow_Params_Description { get; set; } = "在这个窗口，你可以设定释放技能所需的参数。";
        public string Configwindow_Params_NeverReplaceIcon { get; set; } = "不替换图标";
        public string Configwindow_Params_UseOverlayWindow { get; set; } = "使用最高覆盖窗口";
        public string Configwindow_Params_UseOverlayWindowDesc { get; set; } = "这个窗口目前用于提前提示身位。";
        public string Configwindow_Params_BasicSettings { get; set; } = "基础设置";
        public string Configwindow_Params_WeaponDelay { get; set; } = "需要GCD随机手残多少秒";
        public string Configwindow_Params_WeaponFaster { get; set; } = "需要提前几秒按下技能";
        public string Configwindow_Params_WeaponInterval { get; set; } = "间隔多久释放能力技";
        public string Configwindow_Params_InterruptibleTime { get; set; } = "打断类技能延迟多久后释放";
        public string Configwindow_Params_SpecialDuration { get; set; } = "特殊状态持续多久";
        public string Configwindow_Params_AddDotGCDCount { get; set; } = "还差几个GCD就可以补DOT了";
        public string Configwindow_Params_DisplayEnhancement { get; set; } = "提示增强";
        public string Configwindow_Params_PoslockCasting { get; set; } = "使用咏唱移动锁";
        public string Configwindow_Params_PoslockModifier { get; set; } = "无视咏唱锁热键";
        public string Configwindow_Params_PoslockDescription { get; set; } = "手柄玩家为按下LT无视咏唱锁";
        public string Configwindow_Params_CheckForCasting { get; set; } = "使用咏唱结束显示";
        public string Configwindow_Params_TeachingMode { get; set; } = "循环教育模式";
        public string Configwindow_Params_TeachingModeColor { get; set; } = "教育模式颜色";
        public string Configwindow_Params_KeyBoardNoise { get; set; } = "模拟按下键盘效果";
        public string Configwindow_Params_VoiceVolume { get; set; } = "语音音量";
        public string Configwindow_Params_ShowLocation { get; set; } = "写出战技身位";
        public string Configwindow_Params_SayingLocation { get; set; } = "喊出战技身位";
        public string Configwindow_Params_ShowLocationWrong { get; set; } = "显示身位错误";
        public string Configwindow_Params_ShowLocationWrongDesc { get; set; } = "身位错误不是很准，仅供参考。";
        public string Configwindow_Params_LocationWrongText { get; set; } = "身位错误提示语";
        public string Configwindow_Params_LocationWrongTextDesc { get; set; } = "如果身位错误，你想怎么被骂!";
        public string Configwindow_Params_AutoSayingOut { get; set; } = "状态变化时喊出";
        public string Configwindow_Params_UseDtr { get; set; } = "状态显示在系统信息上";
        public string Configwindow_Params_UseToast { get; set; } = "状态显示在屏幕中央";
        public string Configwindow_Params_Actions { get; set; } = "技能使用";
        public string Configwindow_Params_UseAOEWhenManual { get; set; } = "在手动选择的时候使用AOE技能";
        public string Configwindow_Params_AutoBreak { get; set; } = "自动进行爆发";
        public string Configwindow_Params_OnlyGCD { get; set; } = "只使用GCD循环，除去能力技";
        public string Configwindow_Params_AttackSafeMode { get; set; } = "绝对单体模式";
        public string Configwindow_Params_AttackSafeModeDesc { get; set; } = "绝对保证在单目标的时候不打AOE，就算大招也是。但是如果怪的数量达到标准依然会使用AOE。";
        public string Configwindow_Params_NoDefenceAbility { get; set; } = "不使用防御能力技";
        public string Configwindow_Params_NoDefenceAbilityDesc { get; set; } = "如果要打高难，建议勾上这个，自己安排治疗和奶轴。";
        public string Configwindow_Params_AutoDefenseForTank { get; set; } = "自动上减伤(不太准)";
        public string Configwindow_Params_AutoDefenseForTankDesc { get; set; } = "自动的这个不能识别威力为0的AOE技能，请注意。";
        public string Configwindow_Params_AutoShield { get; set; } = "T自动上盾";
        public string Configwindow_Params_AutoProvokeForTank { get; set; } = "T自动挑衅";
        public string Configwindow_Params_AutoProvokeForTankDesc { get; set; } = "当有怪物在打非T的时候，会自动挑衅。";
        public string Configwindow_Params_AutoUseTrueNorth { get; set; } = "近战自动上真北";
        public string Configwindow_Params_RaisePlayerBySwift { get; set; } = "即刻拉人";
        public string Configwindow_Params_UseAreaAbilityFriendly { get; set; } = "使用友方地面放置技能";
        public string Configwindow_Params_RaisePlayerByCasting { get; set; } = "无目标时硬读条拉人";
        public string Configwindow_Params_UseHealWhenNotAHealer { get; set; } = "非奶妈用奶人的技能";
        public string Configwindow_Params_LessMPNoRaise { get; set; } = "小于多少蓝就不复活了";
        public string Configwindow_Params_UseItem { get; set; } = "使用道具";
        public string Configwindow_Params_UseItemDesc { get; set; } = "使用爆发药，目前还未写全";
        public string Configwindow_Params_Conditons { get; set; } = "触发条件";
        public string Configwindow_Params_AutoStartCountdown { get; set; } = "倒计时时自动打开攻击";
        public string Configwindow_Params_HealthDifference { get; set; } = "多少的HP标准差以下，可以用群疗";
        public string Configwindow_Params_HealthAreaAbility { get; set; } = "多少的HP，可以用能力技群疗";
        public string Configwindow_Params_HealthAreafSpell { get; set; } = "多少的HP，可以用GCD群疗";
        public string Configwindow_Params_HealingOfTimeSubstactArea { get; set; } = "如果使用群体Hot技能，阈值下降多少";
        public string Configwindow_Params_HealthSingleAbility { get; set; } = "多少的HP，可以用能力技单奶";
        public string Configwindow_Params_HealthSingleSpell { get; set; } = "多少的HP，可以用GCD单奶";
        public string Configwindow_Params_HealingOfTimeSubstactSingle { get; set; } = "如果使用单体Hot技能，阈值下降多少";
        public string Configwindow_Params_HealthForDyingTank { get; set; } = "低于多少的HP，坦克要放大招了";
        public string Configwindow_Params_Targets { get; set; } = "目标选择";
        public string Configwindow_Params_RightNowTargetToHostileType { get; set; } = "敌对目标筛选条件";
        public string Configwindow_Params_TargetToHostileType1 { get; set; } = "所有能打的目标都是敌对的目标";
        public string Configwindow_Params_TargetToHostileType2 { get; set; } = "如果处于打人的目标数量为零，所有能打的都是敌对的";
        public string Configwindow_Params_TargetToHostileType3 { get; set; } = "只有有目标的目标才是敌对的目标";
        public string Configwindow_Params_AddEnemyListToHostile { get; set; } = "将敌对列表的对象设为敌对";
        public string Configwindow_Params_ChooseAttackMark { get; set; } = "优先选中有攻击标记的目标";
        public string Configwindow_Params_AttackMarkAOE { get; set; } = "是否还要使用AOE";
        public string Configwindow_Params_AttackMarkAOEDesc { get; set; } = "如果勾选了，那么可能这个AOE打不到攻击目标的对象，因为为了追求打到更多的目标。";
        public string Configwindow_Params_FilterStopMark { get; set; } = "去掉有停止标记的目标";
        public string Configwindow_Params_ObjectMinRadius { get; set; } = "攻击对象最小底圈大小";
        public string Configwindow_Params_ChangeTargetForFate { get; set; } = "在Fate中只选择Fate怪";
        public string Configwindow_Params_MoveTowardsScreen { get; set; } = "移动技能选屏幕中心的对象";
        public string Configwindow_Params_MoveTowardsScreenDesc { get; set; } = "设为是时移动的对象为屏幕中心的那个，否为游戏角色面朝的对象。";
        public string Configwindow_Params_RaiseAll { get; set; } = "复活所有能复活的人，而非小队";
        public string Configwindow_Params_RaiseBrinkofDeath { get; set; } = "复活濒死（黑头）之人";
        public string Configwindow_Params_Hostile { get; set; } = "敌对选择";
        public string Configwindow_Params_HostileDesc { get; set; } = "你可以设定敌对的选择，以便于在战斗中灵活切换选择敌对的逻辑。";
        public string Configwindow_Params_AddHostileCondition { get; set; } = "添加选择条件";
        public string Configwindow_Params_HostileCondition { get; set; } = "敌对目标选择条件";
        public string Configwindow_Params_ConditionUp { get; set; } = "上移条件";
        public string Configwindow_Params_ConditionDown { get; set; } = "下移条件";
        public string Configwindow_Params_ConditionDelete { get; set; } = "删除条件";
        #endregion
    }
}
