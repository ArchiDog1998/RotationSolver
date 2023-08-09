using F23.StringSimilarity;
using RotationSolver.Basic.Configuration;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI;

public partial class RotationConfigWindowNew
{
    private string _searchText = string.Empty;
    private ISearchable[] _searchResults = Array.Empty<ISearchable>();
    private void SearchingBox()
    {
        if (ImGui.InputTextWithHint("##Rotation Solver Search Box", "Searching is not available", ref _searchText, 128, ImGuiInputTextFlags.AutoSelectAll))
        {
            if (!string.IsNullOrEmpty(_searchText))
            {
                const int MAX_RESULT_LENGTH = 20;

                _searchResults = new ISearchable[MAX_RESULT_LENGTH];
                var l = new Levenshtein();

                var enumerator = GetType().GetRuntimeFields()
                    .Where(f => f.FieldType == typeof(ISearchable[]) && f.IsInitOnly)
                    .SelectMany(f => (ISearchable[])f.GetValue(this))
                    .OrderBy(i => l.Distance(i.SearchingKey, _searchText))
                    .Select(GetParent).GetEnumerator();

                int index = 0;
                while (enumerator.MoveNext() && index < MAX_RESULT_LENGTH)
                {
                    _searchResults[index++] = enumerator.Current;
                }
            }
            else
            {
                _searchResults = Array.Empty<ISearchable>();
            }
        }
    }

    private static ISearchable GetParent(ISearchable searchable)
    {
        if (searchable == null) return null;
        if (searchable.Parent == null) return searchable;
        return GetParent(searchable.Parent);
    }



    private static readonly ISearchable[] _basicSearchable = new ISearchable[]
    {
        new DragFloatSearchPlugin(PluginConfigFloat.ActionAhead, 0, 0.5f, 0.002f),
        new DragFloatSearchPlugin(PluginConfigFloat.MinLastAbilityAdvanced, 0, 0.4f, 0.002f),
    };
    private static void DrawBasic()
    {
        foreach (var searchable in _basicSearchable)
        {
            searchable?.Draw(Job);
        }
    }

    private static readonly ISearchable[] _uiSearchable = new ISearchable[]
    {
        new DragFloatSearchPlugin(PluginConfigFloat.ActionAhead, 0, 0.5f, 0.002f),
    };
    private static void DrawUI()
    {
        foreach (var searchable in _uiSearchable)
        {
            searchable?.Draw(Job);
        }
    }

    private static readonly ISearchable[] _autoSearchable = new ISearchable[]
    {
        new DragFloatSearchPlugin(PluginConfigFloat.ActionAhead, 0, 0.5f, 0.002f),
    };
    private static void DrawAuto()
    {
        foreach (var searchable in _autoSearchable)
        {
            searchable?.Draw(Job);
        }
    }

    private static readonly ISearchable[] _targetSearchable = new ISearchable[]
    {
        new DragFloatSearchPlugin(PluginConfigFloat.ActionAhead, 0, 0.5f, 0.002f),
    };
    private static void DrawTarget()
    {
        foreach (var searchable in _targetSearchable)
        {
            searchable?.Draw(Job);
        }
    }

    private static readonly ISearchable[] _extraSearchable = new ISearchable[]
    {
        new DragFloatSearchPlugin(PluginConfigFloat.ActionAhead, 0, 0.5f, 0.002f),
    };
    private static void DrawExtra()
    {
        foreach (var searchable in _extraSearchable)
        {
            searchable?.Draw(Job);
        }
    }
}
