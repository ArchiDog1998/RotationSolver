using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos.Attributes
{
    public class ComboDevInfoAttribute : Attribute
    {
        public string URL { get; }
        public ComboAuthor[] Authors { get; set; }
        public ComboDevInfoAttribute(string uRL, params ComboAuthor[] authors)
        {
            URL = uRL;
            if(authors == null || authors.Length == 0)
            {
                authors = new ComboAuthor[] {ComboAuthor.None};
            }
            Authors = authors;
        }
    }
}
