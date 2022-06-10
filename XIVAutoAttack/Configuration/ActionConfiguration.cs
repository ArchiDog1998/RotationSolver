using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Configuration
{
    public class ActionConfiguration
    {
        public List<FloatConfiguration> doubles { get; set; } = new List<FloatConfiguration>();
        public List<BooleanConfiguration> bools { get; set; } = new List<BooleanConfiguration>();
        public List<TextConfiguration> texts { get; set; } = new List<TextConfiguration>();
        public ActionConfiguration SetFloat(string name, float value, float min = 0, float max = 1, string des = "", float speed = 0.002f)
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
        public float GetDoubleByName(string name)
        {
            foreach (var item in doubles)
            {
                if(item.name == name)
                {
                    return item.value;
                }
            }
            return 0;
        }
        public ActionConfiguration SetBool(string name, bool value, string des = "")
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
        public ActionConfiguration SetText(string name, string value, string des = "")
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

        public bool IsTheSame(ActionConfiguration other)
        {
            if (this.texts.Count != other.texts.Count) return false;
            for (int i = 0; i < this.texts.Count; i++)
            {
                if (this.texts[i].name != other.texts[i].name) return false;
            }
            if (this.doubles.Count != other.doubles.Count) return false;
            for (int i = 0; i < this.doubles.Count; i++)
            {
                if (this.doubles[i].name != other.doubles[i].name) return false;
            }
            if (this.bools.Count != other.bools.Count) return false;
            for (int i = 0; i < this.bools.Count; i++)
            {
                if (this.bools[i].name != other.bools[i].name) return false;
            }
            return true;
        }

        //public void Supply(ActionConfiguration other)
        //{
        //    foreach (var item in other.texts)
        //    {
        //        if(this.texts.Count > 0 && this.texts.Any(x => x.name == item.name))
        //        {
        //            this.SetText(item.name, item.value, item.description);
        //        }
        //    }
        //    foreach (var item in other.bools)
        //    {
        //        if (this.bools.Count > 0 && this.bools.Any(x => x.name == item.name))
        //        {
        //            this.SetBool(item.name, item.value, item.description);
        //        }
        //    }
        //    foreach (var item in other.doubles)
        //    {
        //        if (this.doubles.Count > 0 && this.doubles.Any(x => x.name == item.name))
        //        {
        //            this.SetFloat(item.name, item.value, item.min, item.max, item.description, item.speed);
        //        }
        //    }
        //}
    }

    public class FloatConfiguration
    {
        public string name;
        public string description;
        public float value;
        public float min;
        public float max;
        public float speed;
    }

    public class BooleanConfiguration
    {
        public string name;
        public string description;
        public bool value;
    }

    public class TextConfiguration
    {
        public string name;
        public string description;
        public string value;
    }
}
