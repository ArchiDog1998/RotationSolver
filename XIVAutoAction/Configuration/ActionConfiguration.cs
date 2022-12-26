using System;
using System.Collections.Generic;

namespace XIVAutoAttack.Configuration
{
    public class ActionConfiguration
    {
        public List<FloatConfiguration> doubles { get; set; } = new List<FloatConfiguration>();
        public List<BooleanConfiguration> bools { get; set; } = new List<BooleanConfiguration>();
        public List<TextConfiguration> texts { get; set; } = new List<TextConfiguration>();
        public List<ComboConfiguration> combos { get; set; } = new List<ComboConfiguration>();
        public ActionConfiguration SetFloat(string name, float value, string des, float min = 0, float max = 1, float speed = 0.002f)
        {
            foreach (var item in doubles)
            {
                if (item.name == name)
                {
                    item.value = value;
                    return this;
                }
            }
            doubles.Add(new FloatConfiguration()
            {
                name = name,
                value = value,
                min = min,
                max = max,
                description = des,
                speed = speed,
            });
            return this;
        }
        public float GetFloatByName(string name)
        {
            foreach (var item in doubles)
            {
                if (item.name == name)
                {
                    return item.value;
                }
            }
            return 0;
        }
        public ActionConfiguration SetBool(string name, bool value, string des)
        {
            foreach (var item in bools)
            {
                if (item.name == name)
                {
                    item.value = value;
                    return this;
                }
            }
            bools.Add(new BooleanConfiguration() { name = name, value = value, description = des });
            return this;
        }
        public bool GetBoolByName(string name)
        {
            foreach (var item in bools)
            {
                if (item.name == name)
                {
                    return item.value;
                }
            }
            return false;
        }
        public ActionConfiguration SetText(string name, string value, string des)
        {
            foreach (var item in texts)
            {
                if (item.name == name)
                {
                    item.value = value;
                    return this;
                }
            }
            texts.Add(new TextConfiguration() { name = name, value = value, description = des });
            return this;
        }
        public string GetTextByName(string name)
        {
            foreach (var item in texts)
            {
                if (item.name == name)
                {
                    return item.value;
                }
            }
            return "";
        }

        public ActionConfiguration SetCombo(string name, int value, string des, params string[] items)
        {
            foreach (var item in combos)
            {
                if (item.name == name)
                {
                    item.value = value;
                    return this;
                }
            }
            combos.Add(new ComboConfiguration() { name = name, value = value, items = items == null ? new string[0] : items, description = des });
            return this;
        }
        public int GetComboByName(string name)
        {
            foreach (var item in combos)
            {
                if (item.name == name)
                {
                    return item.value;
                }
            }
            return 0;
        }

        internal bool IsTheSame(ActionConfiguration other)
        {
            if (this.texts.Count != other.texts.Count) return false;
            for (int i = 0; i < this.texts.Count; i++)
            {
                if (this.texts[i].name != other.texts[i].name) return false;
                this.texts[i].value = other.texts[i].value;
            }

            if (this.doubles.Count != other.doubles.Count) return false;
            for (int i = 0; i < this.doubles.Count; i++)
            {
                if (this.doubles[i].name != other.doubles[i].name) return false;
                this.doubles[i].value = other.doubles[i].value;
            }

            if (this.bools.Count != other.bools.Count) return false;
            for (int i = 0; i < this.bools.Count; i++)
            {
                if (this.bools[i].name != other.bools[i].name) return false;
                this.bools[i].value = other.bools[i].value;
            }

            if (this.combos.Count != other.combos.Count) return false;
            for (int i = 0; i < this.combos.Count; i++)
            {
                if (this.combos[i].name != other.combos[i].name) return false;
                this.combos[i].value = other.combos[i].value;
            }
            return true;
        }

        internal static ActionConfiguration GetConfig(uint jobid, string author, ActionConfiguration con)
        {
            if (Service.Configuration.CombosConfigurations.TryGetValue(jobid, out var lastcom))
            {
                if (lastcom.TryGetValue(author, out var lastCon))
                {
                    if (con.IsTheSame(lastCon))
                    {
                        lastcom[author] = con;
                        return con;
                    }
                }
                lastcom[author] = con;
            }
            else
            {
                Service.Configuration.CombosConfigurations.Add(jobid, new Dictionary<string, ActionConfiguration>() { { author, con } });
            }
            Service.Configuration.Save();
            return con;
        }
    }

    public class FloatConfiguration
    {
        public string name;
        public float value;

        [NonSerialized]
        public string description;

        [NonSerialized]
        public float min;

        [NonSerialized]
        public float max;

        [NonSerialized]
        public float speed;
    }

    public class BooleanConfiguration
    {
        public string name;
        public bool value;

        [NonSerialized]
        public string description;

    }

    public class TextConfiguration
    {
        public string name;
        public string description;

        [NonSerialized]
        public string value;
    }

    public class ComboConfiguration
    {
        public string name;
        public int value;

        [NonSerialized]
        public string description;

        [NonSerialized]
        public string[] items;
    }
}
