using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace XIVComboExpandedPlugin;

public sealed class XIVComboExpandedPlugin : IDalamudPlugin, IDisposable
{
	private const string Command = "/pcombo";

	private readonly WindowSystem windowSystem;

	private readonly ConfigWindow configWindow;

	public string Name => "XIV Combo Expanded";

	public XIVComboExpandedPlugin(DalamudPluginInterface pluginInterface)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		pluginInterface.Create<Service>(Array.Empty<object>());
		Service.Configuration = (pluginInterface.GetPluginConfig() as PluginConfiguration) ?? new PluginConfiguration();
		Service.Address = new PluginAddressResolver();
		((BaseAddressResolver)Service.Address).Setup();
		if (Service.Configuration.Version == 4)
		{
			UpgradeConfig4();
		}
		Service.IconReplacer = new IconReplacer();
		configWindow = new ConfigWindow();
		windowSystem = new WindowSystem("XIVComboExpanded");
		windowSystem.AddWindow((Window)(object)configWindow);
		Service.Interface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
		Service.Interface.UiBuilder.Draw += windowSystem.Draw;
		CommandManager commandManager = Service.CommandManager;
		CommandInfo val = new CommandInfo(new CommandInfo.HandlerDelegate(OnCommand));
		val.HelpMessage ="Open a window to edit custom combo settings.";
		val.ShowInHelp = true;
		commandManager.AddHandler("/pcombo", val);
	}

	public void Dispose()
	{
		Service.CommandManager.RemoveHandler("/pcombo");
		Service.Interface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
		Service.Interface.UiBuilder.Draw -= windowSystem.Draw;
		Service.IconReplacer.Dispose();
	}

	private void OnOpenConfigUi()
	{
		configWindow.IsOpen = true;
	}

	private void OnCommand(string command, string arguments)
	{
		string[] array = arguments.Split();
		switch (array[0])
		{
		case "setall":
		{
			CustomComboPreset[] values = Enum.GetValues<CustomComboPreset>();
			foreach (CustomComboPreset item in values)
			{
				Service.Configuration.EnabledActions.Add(item);
			}
			Service.ChatGui.Print("All SET");
			Service.Configuration.Save();
			break;
		}
		case "unsetall":
		{
			CustomComboPreset[] values = Enum.GetValues<CustomComboPreset>();
			foreach (CustomComboPreset item2 in values)
			{
				Service.Configuration.EnabledActions.Remove(item2);
			}
			Service.ChatGui.Print("All UNSET");
			Service.Configuration.Save();
			break;
		}
		case "set":
		{
			string text3 = array[1].ToLowerInvariant();
			CustomComboPreset[] values = Enum.GetValues<CustomComboPreset>();
			for (int i = 0; i < values.Length; i++)
			{
				CustomComboPreset customComboPreset4 = values[i];
				if (!(customComboPreset4.ToString().ToLowerInvariant() != text3))
				{
					Service.Configuration.EnabledActions.Add(customComboPreset4);
					Service.ChatGui.Print($"{customComboPreset4} SET");
				}
			}
			Service.Configuration.Save();
			break;
		}
		case "secrets":
			Service.Configuration.EnableSecretCombos = !Service.Configuration.EnableSecretCombos;
			Service.ChatGui.Print(Service.Configuration.EnableSecretCombos ? "Secret combos are now shown" : "Secret combos are now hidden");
			Service.Configuration.Save();
			break;
		case "toggle":
		{
			string text = array[1].ToLowerInvariant();
			CustomComboPreset[] values = Enum.GetValues<CustomComboPreset>();
			for (int i = 0; i < values.Length; i++)
			{
				CustomComboPreset customComboPreset2 = values[i];
				if (!(customComboPreset2.ToString().ToLowerInvariant() != text))
				{
					if (Service.Configuration.EnabledActions.Contains(customComboPreset2))
					{
						Service.Configuration.EnabledActions.Remove(customComboPreset2);
						Service.ChatGui.Print($"{customComboPreset2} UNSET");
					}
					else
					{
						Service.Configuration.EnabledActions.Add(customComboPreset2);
						Service.ChatGui.Print($"{customComboPreset2} SET");
					}
				}
			}
			Service.Configuration.Save();
			break;
		}
		case "dot":
			if (Service.Configuration.EnabledActions.Contains(CustomComboPreset.SCHDotFeature))
			{
				Service.Configuration.EnabledActions.Remove(CustomComboPreset.SCHDotFeature);
			}
			if (!Service.Configuration.EnabledActions.Contains(CustomComboPreset.SCHDotFeature))
			{
				Service.Configuration.EnabledActions.Add(CustomComboPreset.SCHDotFeature);
			}
			if (Service.Configuration.EnabledActions.Contains(CustomComboPreset.ASTdotFeature))
			{
				Service.Configuration.EnabledActions.Remove(CustomComboPreset.ASTdotFeature);
			}
			if (!Service.Configuration.EnabledActions.Contains(CustomComboPreset.ASTdotFeature))
			{
				Service.Configuration.EnabledActions.Add(CustomComboPreset.ASTdotFeature);
			}
			Service.Configuration.Save();
			break;
		case "unset":
		{
			string text2 = array[1].ToLowerInvariant();
			CustomComboPreset[] values = Enum.GetValues<CustomComboPreset>();
			for (int i = 0; i < values.Length; i++)
			{
				CustomComboPreset customComboPreset3 = values[i];
				if (!(customComboPreset3.ToString().ToLowerInvariant() != text2))
				{
					Service.Configuration.EnabledActions.Remove(customComboPreset3);
					Service.ChatGui.Print($"{customComboPreset3} UNSET");
				}
			}
			Service.Configuration.Save();
			break;
		}
		case "list":
			switch ((array.Length > 1) ? array[1].ToLowerInvariant() : "all")
			{
			case "set":
				foreach (bool item3 in from preset in Enum.GetValues<CustomComboPreset>()
					select Service.Configuration.IsEnabled(preset))
				{
					Service.ChatGui.Print(item3.ToString());
				}
				break;
			case "unset":
				foreach (bool item4 in from preset in Enum.GetValues<CustomComboPreset>()
					select !Service.Configuration.IsEnabled(preset))
				{
					Service.ChatGui.Print(item4.ToString());
				}
				break;
			case "all":
			{
				CustomComboPreset[] values = Enum.GetValues<CustomComboPreset>();
				for (int i = 0; i < values.Length; i++)
				{
					CustomComboPreset customComboPreset = values[i];
					Service.ChatGui.Print(customComboPreset.ToString());
				}
				break;
			}
			default:
				Service.ChatGui.PrintError("Available list filters: set, unset, all");
				break;
			}
			break;
		default:
			((Window)configWindow).Toggle();
			break;
		}
		Service.Configuration.Save();
	}

	private void UpgradeConfig4()
	{
		Service.Configuration.Version = 5;
		Service.Configuration.EnabledActions = (from preset in Service.Configuration.EnabledActions4
			select preset switch
			{
				(CustomComboPreset)27 => 3301, 
				(CustomComboPreset)75 => 3302, 
				(CustomComboPreset)73 => 3303, 
				(CustomComboPreset)25 => 2501, 
				(CustomComboPreset)26 => 2502, 
				(CustomComboPreset)56 => 2503, 
				(CustomComboPreset)70 => 2504, 
				(CustomComboPreset)71 => 2505, 
				(CustomComboPreset)110 => 2506, 
				(CustomComboPreset)95 => 2507, 
				(CustomComboPreset)41 => 2301, 
				(CustomComboPreset)42 => 2302, 
				(CustomComboPreset)63 => 2303, 
				(CustomComboPreset)74 => 2304, 
				(CustomComboPreset)33 => 3801, 
				(CustomComboPreset)31 => 3802, 
				(CustomComboPreset)34 => 3803, 
				(CustomComboPreset)43 => 3804, 
				(CustomComboPreset)50 => 3805, 
				(CustomComboPreset)72 => 3806, 
				(CustomComboPreset)103 => 3807, 
				(CustomComboPreset)44 => 2201, 
				CustomComboPreset.None => 2202, 
				(CustomComboPreset)1 => 2203, 
				(CustomComboPreset)2 => 2204, 
				(CustomComboPreset)3 => 3201, 
				(CustomComboPreset)4 => 3202, 
				(CustomComboPreset)57 => 3203, 
				(CustomComboPreset)85 => 3204, 
				(CustomComboPreset)20 => 3701, 
				(CustomComboPreset)52 => 3702, 
				(CustomComboPreset)96 => 3703, 
				(CustomComboPreset)97 => 3704, 
				(CustomComboPreset)22 => 3705, 
				(CustomComboPreset)30 => 3706, 
				(CustomComboPreset)83 => 3707, 
				(CustomComboPreset)84 => 3708, 
				(CustomComboPreset)23 => 3101, 
				(CustomComboPreset)24 => 3102, 
				(CustomComboPreset)47 => 3103, 
				(CustomComboPreset)58 => 3104, 
				(CustomComboPreset)66 => 3105, 
				(CustomComboPreset)102 => 3106, 
				(CustomComboPreset)54 => 2001, 
				(CustomComboPreset)82 => 2002, 
				(CustomComboPreset)106 => 2003, 
				(CustomComboPreset)17 => 3001, 
				(CustomComboPreset)18 => 3002, 
				(CustomComboPreset)19 => 3003, 
				(CustomComboPreset)87 => 3004, 
				(CustomComboPreset)88 => 3005, 
				(CustomComboPreset)89 => 3006, 
				(CustomComboPreset)90 => 3007, 
				(CustomComboPreset)91 => 3008, 
				(CustomComboPreset)92 => 3009, 
				(CustomComboPreset)107 => 3010, 
				(CustomComboPreset)108 => 3011, 
				(CustomComboPreset)5 => 1901, 
				(CustomComboPreset)6 => 1902, 
				(CustomComboPreset)59 => 1903, 
				(CustomComboPreset)7 => 1904, 
				(CustomComboPreset)55 => 1905, 
				(CustomComboPreset)86 => 1906, 
				(CustomComboPreset)69 => 1907, 
				(CustomComboPreset)48 => 3501, 
				(CustomComboPreset)49 => 3502, 
				(CustomComboPreset)68 => 3503, 
				(CustomComboPreset)53 => 3504, 
				(CustomComboPreset)93 => 3505, 
				(CustomComboPreset)101 => 3506, 
				(CustomComboPreset)94 => 3507, 
				(CustomComboPreset)11 => 3401, 
				(CustomComboPreset)12 => 3402, 
				(CustomComboPreset)13 => 3403, 
				(CustomComboPreset)14 => 3404, 
				(CustomComboPreset)15 => 3405, 
				(CustomComboPreset)81 => 3406, 
				(CustomComboPreset)60 => 3407, 
				(CustomComboPreset)61 => 3408, 
				(CustomComboPreset)64 => 3409, 
				(CustomComboPreset)65 => 3410, 
				(CustomComboPreset)109 => 3411, 
				(CustomComboPreset)29 => 2801, 
				(CustomComboPreset)37 => 2802, 
				(CustomComboPreset)39 => 2701, 
				(CustomComboPreset)40 => 2702, 
				(CustomComboPreset)8 => 2101, 
				(CustomComboPreset)9 => 2102, 
				(CustomComboPreset)10 => 2103, 
				(CustomComboPreset)78 => 2104, 
				(CustomComboPreset)79 => 2105, 
				(CustomComboPreset)67 => 2106, 
				(CustomComboPreset)104 => 2107, 
				(CustomComboPreset)35 => 2401, 
				(CustomComboPreset)36 => 2402, 
				(CustomComboPreset)76 => 2403, 
				(CustomComboPreset)77 => 2404, 
				_ => 0, 
			} into id
			where id != 0
			select (CustomComboPreset)id).ToHashSet();
		Service.Configuration.EnabledActions4 = new HashSet<CustomComboPreset>();
		Service.Configuration.Save();
	}
}
