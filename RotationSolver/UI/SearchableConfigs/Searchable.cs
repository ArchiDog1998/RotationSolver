using Dalamud.Interface.Utility;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Localization;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI.SearchableConfigs;

internal readonly struct JobFilter
{
    public static readonly JobFilter
        NoJob = new()
        {
            JobRoles = [],
        },

        NoHealer = new()
        {
            JobRoles =
            [
                JobRole.Tank,
                JobRole.Melee,
                JobRole.RangedMagical,
                JobRole.RangedPhysical,
            ]
        },

        Healer = new()
        {
            JobRoles =
            [
                JobRole.Healer,
            ]
        },

        Raise = new()
        {
            JobRoles =
            [
                JobRole.Healer,
            ],
            Jobs =
            [
                Job.RDM,
                Job.SMN,
            ],
        },

        Interrupt = new()
        {
            JobRoles =
            [
                JobRole.Tank,
                JobRole.Melee,
                JobRole.RangedPhysical,
            ],
        },

        Esuna = new()
        {
            JobRoles =
            [
                JobRole.Healer,
            ],
            Jobs =
            [
                Job.BRD,
            ],
        },

        Tank = new()
        {
            JobRoles =
            [
                JobRole.Tank,
            ]
        },

        Melee = new()
        {
            JobRoles =
            [
                JobRole.Melee,
            ]
        };


    /// <summary>
    /// Only these job roles can get this setting.
    /// </summary>
    public JobRole[] JobRoles { get; init; }

    /// <summary>
    /// Or these jobs.
    /// </summary>
    public Job[] Jobs { get; init; }

    public bool CanDraw
    {
        get
        {
            var canDraw = true;

            if (JobRoles != null)
            {
                var role = DataCenter.RightNowRotation?.ClassJob?.GetJobRole();
                if (role.HasValue)
                {
                    canDraw = JobRoles.Contains(role.Value);
                }
            }

            if (Jobs != null)
            {
                canDraw |= Jobs.Contains(DataCenter.Job);
            }
            return canDraw;
        }
    }

    public Job[] AllJobs => JobRoles.SelectMany(JobRoleExtension.ToJobs).Union(Jobs ?? []).ToArray();

    public string Description
    {
        get
        {
            var roleOrJob = string.Join("\n",
                AllJobs.Select(job => Svc.Data.GetExcelSheet<ClassJob>()?.GetRow((uint)job)?.Name ?? job.ToString()));
            return string.Format("NotInJob".Local("This option is unavailable while using your current job\n \nRoles or jobs needed:\n{0}"), roleOrJob);
        }
    }
}

internal abstract class Searchable(PropertyInfo property) : ISearchable
{
    protected readonly PropertyInfo _property = property;

    public const float DRAG_WIDTH = 150;
    protected static float Scale => ImGuiHelpers.GlobalScale;
    public CheckBoxSearch? Parent { get; set; } = null;

    public virtual string SearchingKeys => Name + " " + Description;
    public virtual string Name => _property.GetCustomAttribute<UIAttribute>()?.Name ?? _property.Name;
    public virtual string Description => _property.GetCustomAttribute<UIAttribute>()?.Description ?? Name;
    public virtual string Command => ConfigTranslation.ToCommandStr(_property.Name);
    public virtual LinkDescription[]? Tooltips => [.. _property.GetCustomAttributes<LinkDescriptionAttribute>().Select(l => l.LinkDescription)];
    public virtual string ID => _property.Name;
    private string Popup_Key => "Rotation Solver RightClicking: " + ID;
    protected bool IsJob => _property.GetCustomAttribute<JobConfigAttribute>() != null
        || _property.GetCustomAttribute<JobChoiceConfigAttribute>() != null;

    public uint Color { get; set; } = 0;

    public JobFilter PvPFilter { get; set; }
    public JobFilter PvEFilter { get; set; }

    public virtual bool ShowInChild => true;

    public unsafe void Draw()
    {
        var filter = (DataCenter.Territory?.IsPvpZone ?? false) ? PvPFilter : PvEFilter;

        if (!filter.CanDraw)
        {
            if (!filter.AllJobs.Any())
            {
                return;
            }

            var textColor = *ImGui.GetStyleColorVec4(ImGuiCol.Text);

            ImGui.PushStyleColor(ImGuiCol.Text, *ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled));

            var cursor = ImGui.GetCursorPos() + ImGui.GetWindowPos() - new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
            ImGui.TextWrapped(Name);
            ImGui.PopStyleColor();

            var step = ImGui.CalcTextSize(Name);
            var size = ImGui.GetItemRectSize();
            var height = step.Y / 2;
            var wholeWidth = step.X;
            while (height < size.Y)
            {
                var pt = cursor + new Vector2(0, height);
                ImGui.GetWindowDrawList().AddLine(pt, pt + new Vector2(Math.Min(wholeWidth, size.X), 0), ImGui.ColorConvertFloat4ToU32(textColor));
                height += step.Y;
                wholeWidth -= size.X;
            }

            ImguiTooltips.HoveredTooltip(filter.Description);
            return;
        }

        DrawMain();

        ImGuiHelper.PrepareGroup(Popup_Key, Command, () => ResetToDefault());
    }

    protected abstract void DrawMain();


    protected void ShowTooltip(bool showHand = true)
    {
        var showDesc = !string.IsNullOrEmpty(Description);
        if (showDesc || Tooltips != null && Tooltips.Length > 0)
        {
            ImguiTooltips.ShowTooltip(() =>
            {
                if (showDesc)
                {
                    ImGui.TextWrapped(Description);
                }
                if (showDesc && Tooltips != null && Tooltips.Length > 0)
                {
                    ImGui.Separator();
                }
                var wholeWidth = ImGui.GetWindowWidth();

                if (Tooltips != null) foreach (var tooltip in Tooltips)
                    {
                        RotationConfigWindow.DrawLinkDescription(tooltip, wholeWidth, false);
                    }
            });
        }

        ImGuiHelper.ReactPopup(Popup_Key, Command, ResetToDefault, showHand);
    }

    protected static void DrawJobIcon()
    {
        ImGui.SameLine();

        if (IconSet.GetTexture(IconSet.GetJobIcon(DataCenter.Job, IconType.Framed), out var texture))
        {
            ImGui.Image(texture.ImGuiHandle, Vector2.One * 24 * ImGuiHelpers.GlobalScale);
            ImguiTooltips.HoveredTooltip("JobConfigTip".Local("This config is job specific"));
        }
    }

    public virtual void ResetToDefault()
    {
        var v = _property.GetValue(Service.ConfigDefault);
        _property.SetValue(Service.Config, v);
    }
}
