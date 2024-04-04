﻿using Lumina.Excel.GeneratedSheets;
using Microsoft.CodeAnalysis;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.GameData.Getters.Actions;

internal abstract class ActionGetterBase<TSyntax>(Lumina.GameData gameData)
    : ExcelRowGetter<Action, TSyntax>(gameData)
        where TSyntax : SyntaxNode

{
    private readonly string[] _notCombatJobs = [.. gameData.GetExcelSheet<ClassJob>()!.Where(c =>
        {
            return c.ClassJobCategory.Row is 32 or 33;
        }).Select(c => c.Abbreviation.RawString)];

    protected override bool AddToList(Action item)
    {
        if (item.RowId is 3 or 120) return true; //Sprint and cure.
        if (item.ClassJobCategory.Row == 0) return false;
        var name = item.Name.RawString;
        if (string.IsNullOrEmpty(name)) return false;
        if (!name.All(char.IsAscii)) return false;
        if (item.Icon is 0 or 405) return false;

        if (item.ActionCategory.Row 
            is 6 or 7 // No DoL or DoH Action
            or 8 //No Event.
            or 12 // No Mount,
            or 14 // No item manipulation
            or > 15 // and other thing.
            ) return false;

        //No crafting or gathering.
        var category = item.ClassJobCategory.Value;
        if (category == null) return false;

        if (category.RowId == 1) return true;

        if (_notCombatJobs.Any(name =>
        {
            return (bool?)category.GetType().GetRuntimeProperty(name)?.GetValue(category) ?? false;
        }))
        {
            return false;
        }

        return true;
    }

    protected sealed override string ToName(Action item)
    {
        return item.Name.RawString + (item.IsPvP ? "PvP" : "PvE");
    }
}
