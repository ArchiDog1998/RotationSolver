using Lumina.Excel.GeneratedSheets;

namespace RotationSolver.GameData.Getters;
internal class TraitRotationGetter(Lumina.GameData gameData, ClassJob job)
    : ExcelRowGetter<Trait>(gameData)
{
    public List<string> AddedNames { get; } = [];

    protected override void BeforeCreating()
    {
        AddedNames.Clear();
        base.BeforeCreating();
    }

    protected override bool AddToList(Trait item)
    {
        if (item.ClassJob.Row == 0) return false;
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        if (item.Icon == 0) return false;

        var category = item.ClassJob.Value;
        if (category == null) return false;
        var jobName = job.Abbreviation.RawString;
        return category.Abbreviation == jobName;
    }

    protected override string ToCode(Trait item)
    {
        var name = item.Name.RawString.ToPascalCase() + "Trait";

        if (AddedNames.Contains(name))
        {
            name += "_" + item.RowId.ToString();
        }
        else
        {
            AddedNames.Add(name);
        }

        return $$"""
        /// <summary>
        /// {{GetDescName(item)}}
        /// {{GetDesc(item)}}
        /// </summary>
        public static IBaseTrait {{name}} { get; } = new BaseTrait({{item.RowId}});
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
        var desc = _gameData.GetExcelSheet<TraitTransient>()?.GetRow(item.RowId)?.Description.RawString ?? string.Empty;

        return $"<para>{desc.Replace("\n", "</para>\n/// <para>")}</para>";
    }
}
