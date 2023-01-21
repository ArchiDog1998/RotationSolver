using ImGuiNET;
using RotationSolver.Actions;
using RotationSolver.Helpers;
using RotationSolver.SigReplacers;
using RotationSolver.Timeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Updaters
{
    internal class TimeLineUpdater
    {
        static string _timelineFolder;

        static IEnumerable<MajorConditionSet> _conditionSet;
        public static MajorConditionSet RightSet => _conditionSet.ElementAtOrDefault(Service.Configuration.TimelineIndex);

        public static string[] ConditionSetsName => _conditionSet?.Select(s => s.Name).ToArray() ?? new string[0];

        public static IBaseAction TimeLineAction { get; private set; }
        public static void UpdateTimelineAction()
        {
            if (_conditionSet == null) return;
            var customRotation = IconReplacer.RightNowRotation;
            if (customRotation == null) return;

            var allActions = IconReplacer.RightRotationBaseActions;
            foreach (var conditionPair in RightSet.Conditions)
            {
                var nextAct = allActions.FirstOrDefault(a => a.ID == (uint)conditionPair.Key);
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
            Directory.Delete(_timelineFolder);
            Directory.CreateDirectory(_timelineFolder);
            foreach (var set in _conditionSet)
            {
                set.Save(_timelineFolder);
            }
        }

        private static void AddNew()
        {
            const string conditionName = "Unnamed";
            if (!_conditionSet.Any(c => c.Name == conditionName))
                _conditionSet.Append(new MajorConditionSet(conditionName));
        }


        private static void Delete(string name)
        {
            _conditionSet = _conditionSet.Where(c => c.Name != name);
        }

        public static void DrawHeader()
        {
            var set = RightSet;
            if (set == null) return;

            ImGui.InputText("##MajorConditionSet", ref set.Name, 100);

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            var combos = ConditionSetsName;
            ImGui.Combo("MajorConditionCombo", ref Service.Configuration.TimelineIndex, combos, combos.Length);

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGuiHelper.IconButton(Dalamud.Interface.FontAwesomeIcon.Cross, "##DeleteTimelineConditionSet"))
            {
                Delete(set.Name);
            }

            ImGui.SameLine();
            ImGuiHelper.Spacing();

            if (ImGuiHelper.IconButton(Dalamud.Interface.FontAwesomeIcon.Plus, "##AddNewTimelineConditionSet"))
            {
                AddNew();
            }
        }
    }
}
