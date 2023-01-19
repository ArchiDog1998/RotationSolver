using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Configuration.RotationConfig
{
    internal interface IRotationConfigSet
    {
        IRotationConfigSet SetFloat(string name, float value, string displayName, float min = 0, float max = 1, float speed = 0.002f);

        IRotationConfigSet SetString(string name, string value, string displayName);

        IRotationConfigSet SetBool(string name, bool value, string displayName);

        IRotationConfigSet SetCombo(string name, int value, string displayName, params string[] items);

        void SetValue(string name, string value);

        int GetCombo(string name);

        bool GetBool(string name);

        float GetFloat(string name);

        string GetString(string name);

        void Draw(bool canAddButton);

    }
}
