using ECommons.DalamudServices;
using RotationSolver.Basic.Configuration;
using RotationSolver.UI.SearchableConfigs;
using RotationSolver.UI.SearchableSettings;

namespace RotationSolver.UI;
internal readonly record struct SearchPair(UIAttribute Attribute, ISearchable Searchable);

internal class SearchableCollection
{
    private readonly List<SearchPair> _items;

    public SearchableCollection()
    {
        var properties = typeof(Configs).GetRuntimeProperties();
        var count = properties.Count();
        var pairs = new List<SearchPair>(count);
        var parents = new Dictionary<string, CheckBoxSearch>(count);

        foreach (var property in properties)
        {
            var ui = property.GetCustomAttribute<UIAttribute>();
            if (ui == null) continue;

            var item = CreateSearchable(property);
            if (item == null) continue;

            item.PvEFilter = new(ui.PvEFilter);
            item.PvPFilter = new(ui.PvPFilter);

            pairs.Add(new(ui, item));

            if (item is not CheckBoxSearch search) continue;
            parents[property.Name] = search;
        }

        _items = new List<SearchPair>(pairs.Count);

        foreach (var pair in pairs)
        {
            var parentName = pair.Attribute.Parent;
            if (string.IsNullOrEmpty(parentName)
                || !parents.TryGetValue(parentName, out var parent))
            {
                _items.Add(pair);
                continue;
            }

            parent.AddChild(pair.Searchable);
        }

        static ISearchable? CreateSearchable(PropertyInfo property)
        {
            var type = property.PropertyType;

            if (property.Name == nameof(Configs.AutoHeal))
            {
                return new AutoHealCheckBox(property);
            }
            else if (type.IsEnum)
            {
                return new EnumSearch(property);
            }
            else if (type == typeof(bool))
            {
                return new CheckBoxSearchNoCondition(property);
            }
            else if (type == typeof(ConditionBoolean))
            {
                return new CheckBoxSearchCondition(property);
            }
            else if (type == typeof(float))
            {
                return new DragFloatSearch(property);
            }
            else if (type == typeof(int))
            {
                return new DragIntSearch(property);
            }
            else if (type == typeof(Vector2))
            {
                return new DragFloatRangeSearch(property);
            }
            else if (type == typeof(Vector2Int))
            {
                return new DragIntRangeSearch(property);
            }
            else if (type == typeof(Vector4))
            {
                return new ColorEditSearch(property);
            }

#if DEBUG
            Svc.Log.Warning($"Failed to create search item, the type is {type.Name}");
#endif
            return null;
        }
    }

    public void DrawItems(string filter)
    {
        bool isFirst = true;
        foreach (var grp in _items.Where(i => i.Attribute.Filter == filter)
            .GroupBy(i => i.Attribute.Section))
        {
            if (!isFirst)
            {
                ImGui.Separator();
            }
            foreach(var item in grp.OrderBy(i => i.Attribute.Order))
            {
                item.Searchable.Draw();
            }

            isFirst = false;
        }
    }

    private static readonly char[] _splitChar = [' ', ',', '、', '.', '。'];
    public ISearchable[] SearchItems(string searchingText)
    {
        if (string.IsNullOrEmpty(searchingText)) return [];

        const int MAX_RESULT_LENGTH = 20;

        var results = new ISearchable[MAX_RESULT_LENGTH];

        var enumerator = _items.Select(i => i.Searchable).SelectMany(GetChildren)
            .OrderByDescending(i => Similarity(i.SearchingKeys, searchingText))
            .Select(GetParent).GetEnumerator();

        int index = 0;
        while (enumerator.MoveNext() && index < MAX_RESULT_LENGTH)
        {
            if (results.Contains(enumerator.Current)) continue;
            results[index++] = enumerator.Current;
        }

        return results;


        static IEnumerable<ISearchable> GetChildren(ISearchable searchable)
        {
            var myself = new ISearchable[] { searchable };
            if (searchable is CheckBoxSearch c && c.Children != null)
            {
                return c.Children.SelectMany(GetChildren).Union(myself);
            }
            else return myself;
        }

        static ISearchable GetParent(ISearchable searchable)
        {
            if (searchable.Parent == null) return searchable;
            return GetParent(searchable.Parent);
        }
    }

    public static float Similarity(string text, string key)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        var chars = text.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries);
        var keys = key.Split(_splitChar, StringSplitOptions.RemoveEmptyEntries);

        var startWithCount = chars.Count(i => keys.Any(k => i.StartsWith(k, StringComparison.OrdinalIgnoreCase)));

        var containCount = chars.Count(i => keys.Any(k => i.Contains(k, StringComparison.OrdinalIgnoreCase)));

        return startWithCount * 3 + containCount;
    }

}
