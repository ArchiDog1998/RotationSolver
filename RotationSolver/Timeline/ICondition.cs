using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RotationSolver.Rotations.CustomRotation;
using System;

namespace RotationSolver.Timeline;

internal interface ICondition
{
    const float DefaultHeight = 33;
    bool IsTrue(ICustomRotation rotation);
    void Draw(ICustomRotation rotation);
    float Height { get; }
}


