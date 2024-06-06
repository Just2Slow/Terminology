using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    public class Term
    {
        public string Name;
        public string Definition;
        public List<string> References;
        public List<string> Tags;

        public Term(string name, string definition, List<string> references, List<string> tags)
        {
            Name = name;
            Definition = definition;
            References = references;
            Tags = tags;
        }
    }
}
