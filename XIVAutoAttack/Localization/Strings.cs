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

        public string Configwindow_Helper_SwitchAuthor { get; set; } = "点击以切换作者";
        public string Configwindow_Helper_GameVersion { get; set; } = "游戏版本";
        public string Configwindow_Helper_EditCombo { get; set; } = "点击以编辑该自定义Combo。";
        public string Configwindow_Helper_AddCombo { get; set; } = "添加一个自定义Combo。";
        public string Configwindow_Helper_OpenSource { get; set; } = "打开源码网址";
        public string Configwindow_Helper_RunCommand { get; set; } = "单击以执行命令";
        public string Configwindow_Helper_InsertCommand { get; set; } = "5s内最高优先插入\"{0}\"";
        public string Configwindow_AttackItem_Description { get; set; } = "你可以选择开启想要的职业的连续GCD战技、技能，若职业与当前职业相同则有命令宏提示。";

        public string Configwindow_AttackItem_ScriptFolder { get; set; } = "自定义循环路径";
        public string Configwindow_AttackItem_LoadScript { get; set; } = "点击以载入自定义循环";
        public string Configwindow_AttackItem_ScriptFolderError { get; set; } = "请设定一个路径以正常使用自定义循环！";
        public string Configwindow_AttackItem_KeyName { get; set; } = "关键名称为";
        public string Configwindow_Events_AddEvent{ get; set; } = "添加事件";
        public string Configwindow_Events_Description{ get; set; } = "在这个窗口，你可以设定一些技能释放后，使用什么宏。";
        public string Configwindow_Events_ActionName{ get; set; } = "技能名称";
        public string Configwindow_Events_MacroIndex{ get; set; } = "宏编号";
        public string Configwindow_Events_ShareMacro{ get; set; } = "共享宏";
        public string Configwindow_Events_RemoveEvent{ get; set; } = "删除事件";
    }
}
