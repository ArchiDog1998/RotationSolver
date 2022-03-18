using System;
using System.Runtime.CompilerServices;

namespace XIVComboExpandedPlugin.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class CustomComboInfoAttribute : Attribute
{
	public string FancyName { get; }

	public string Description { get; }

	public byte JobID { get; }

	public int Order { get; }

	public string JobName => JobIDToName(JobID);

	internal CustomComboInfoAttribute(string fancyName, string description, byte jobID, [CallerLineNumber] int order = 0)
	{
		FancyName = fancyName;
		Description = description;
		JobID = jobID;
		Order = order;
	}

	private static string JobIDToName(byte key)
	{
		return key switch
		{
			0 => "冒险者", 
			1 => "剑术师", 
			2 => "格斗家", 
			3 => "斧术师", 
			4 => "枪术师", 
			5 => "弓箭手", 
			6 => "幻术师", 
			7 => "咒术师", 
			8 => "刻木匠", 
			9 => "锻铁匠", 
			10 => "铸甲匠", 
			11 => "雕金匠", 
			12 => "制革匠", 
			13 => "裁衣匠", 
			14 => "炼金术士", 
			15 => "烹调师", 
			16 => "采矿工", 
			17 => "园艺工", 
			18 => "捕鱼人", 
			19 => "骑士", 
			20 => "武僧", 
			21 => "战士", 
			22 => "龙骑士", 
			23 => "诗人", 
			24 => "白魔法师", 
			25 => "黑魔法师", 
			26 => "秘术师", 
			27 => "召唤师", 
			28 => "学者", 
			29 => "双剑师", 
			30 => "忍者", 
			31 => "机工士", 
			32 => "暗黑骑士", 
			33 => "占星术士", 
			34 => "武士", 
			35 => "赤魔法师", 
			36 => "青魔法师", 
			37 => "绝枪战士", 
			38 => "舞者", 
			39 => "镰刀", 
			40 => "贤者", 
			_ => "Unknown", 
		};
	}
}
