using ImGuiNET;
using Newtonsoft.Json;
using RotationSolver.Actions;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using RotationSolver.Timeline;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RotationSolver.Updaters
{
    internal class TimeLineUpdater
    {
        static string _timelineFolder;

        static IEnumerable<MajorConditionSet> _conditionSet;
        public static MajorConditionSet RightSet => _conditionSet?.ElementAtOrDefault(Service.Configuration.TimelineIndex);

        public static string[] ConditionSetsName => _conditionSet?.Select(s => s.Name).ToArray() ?? new string[0];

        public static IBaseAction TimeLineAction { get; private set; }
        public static void UpdateTimelineAction()
        {
            if (_conditionSet == null) return;
            var customRotation = RotationUpdater.RightNowRotation;
            if (customRotation == null) return;

            var allActions = RotationUpdater.RightRotationBaseActions;

            var set = RightSet;
            if (set == null) return;

            foreach (var conditionPair in set.Conditions)
            {
                var nextAct = allActions.FirstOrDefault(a => a.ID == conditionPair.Key);
                if (nextAct == null) continue;

                if (!conditionPair.Value.IsTrue(customRotation)) continue;

                TimeLineAction = nextAct;
                break;
            }
        }

        public static void Enable(string folder)
        {
            _timelineFolder = folder;
            if (!Directory.Exists(_timelineFolder)) Directory.CreateDirectory(_timelineFolder);

            _conditionSet = MajorConditionSet.Read(_timelineFolder);
        }

        public static void SaveFiles()
        {
            try
            {
                Directory.Delete(_timelineFolder);
                Directory.CreateDirectory(_timelineFolder);
            }
            catch
            {

            }
            foreach (var set in _conditionSet)
            {
                set.Save(_timelineFolder);
            }
        }

        private static void AddNew()
        {
            const string conditionName = "Unnamed";
            if (!_conditionSet.Any(c => c.Name == conditionName))
            {
                _conditionSet = _conditionSet.Union(new[] { new MajorConditionSet(conditionName) });
            }
        }

        private static void Delete(string name)
        {
            _conditionSet = _conditionSet.Where(c => c.Name != name);
        }

        public static void DrawHeader()
        {
            var set = RightSet;
            bool hasSet = set != null;

            if (hasSet)
            {
                ImGuiHelper.SetNextWidthWithName(set.Name);
                ImGui.InputText("##MajorConditionSet", ref set.Name, 100);

                ImGui.SameLine();
            }

            var combos = ConditionSetsName;
            if (combos != null && combos.Length > Service.Configuration.TimelineIndex)
            {
                ImGui.SetNextItemWidth(ImGui.CalcTextSize(combos[Service.Configuration.TimelineIndex]).X + 30);
            }
            else
            {
                ImGui.SetNextItemWidth(30);
            }

            ImGui.Combo("##MajorConditionCombo", ref Service.Configuration.TimelineIndex, combos, combos.Length);

            ImGui.SameLine();


            if (hasSet)
            {
                if (ImGuiHelper.IconButton(Dalamud.Interface.FontAwesomeIcon.Ban, "##DeleteTimelineConditionSet"))
                {
                    Delete(set.Name);
                }

                ImGui.SameLine();
            }

            if (ImGuiHelper.IconButton(Dalamud.Interface.FontAwesomeIcon.Plus, "##AddNewTimelineConditionSet"))
            {
                AddNew();
            }

            ImGui.SameLine();
            if (ImGuiHelper.IconButton(Dalamud.Interface.FontAwesomeIcon.Folder, "##OpenDefinationFolder"))
            {
                Process.Start("explorer.exe", _timelineFolder);
            }
        }
    }
}
