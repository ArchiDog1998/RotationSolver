using ECommons.ExcelServices;

namespace RotationSolver.Data;

internal record CustomRotationGroup(Job JobId, Job[] ClassJobIds, ICustomRotation[] Rotations);
