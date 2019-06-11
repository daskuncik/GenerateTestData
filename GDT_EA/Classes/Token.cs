using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT_EA.Classes
{
    public class Token
    {
        public int Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public int Line
        {
            get;
            set;
        }

        public int Column
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            var token = obj as Token;
            return Name == token.Name &&
                    Value == token.Value &&
                    Line == token.Line &&
                    Column == token.Column;
        }
    }
}
