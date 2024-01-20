using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.GameData.Getters;
internal class TraitRotationGetter(Lumina.GameData gameData, ClassJob job)
    : ExcelRowGetter<Trait>(gameData)
{
    private readonly List<string> _addedNames = [];

    protected override void BeforeCreating()
    {
        _addedNames.Clear();
        base.BeforeCreating();
    }

    protected override bool AddToList(Trait item)
    {
        if (item.ClassJobCategory.Row == 0) return false;
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        if (item.Icon == 0) return false;

        var category = item.ClassJobCategory.Value;
        if (category == null) return false;
        var jobName = job.Abbreviation.RawString;
        return (bool?)category.GetType().GetRuntimeProperty(jobName)?.GetValue(category) ?? false;
    }

    protected override string ToCode(Trait item)
    {
        var name = item.Name.RawString.ToPascalCase() + "Trait";

        if (_addedNames.Contains(name))
        {
            name += "_" + item.RowId.ToString();
        }
        else
        {
            _addedNames.Add(name);
        }

        return $$"""
        private readonly Lazy<IBaseTrait> _{{name}}Creator = new(() => new BaseTrait({{item.RowId}}));
        /// <summary>
        /// {{GetDescName(item)}}
        /// {{GetDesc(item)}}
        /// </summary>
        public IBaseTrait {{name}} => _{{name}}Creator.Value;
        """;
    }

    private static string GetDescName(Trait item)
    {
        var jobs = item.ClassJobCategory.Value?.Name.RawString;
        jobs = string.IsNullOrEmpty(jobs) ? string.Empty : $" ({jobs})";

        return $"<see href=\"https://garlandtools.org/db/#action/{50000 + item.RowId}\"><strong>{item.Name.RawString}</strong></see>{jobs} [{item.RowId}]";
    }

    private string GetDesc(Trait item)
    {
        var desc = gameData.GetExcelSheet<TraitTransient>()?.GetRow(item.RowId)?.Description.RawString ?? string.Empty;

        return $"<para>{desc.Replace("\n", "</para>\n/// <para>")}</para>";
    }
}
