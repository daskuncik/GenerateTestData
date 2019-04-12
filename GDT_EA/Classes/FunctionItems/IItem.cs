using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GDT_EA.Classes.FunctionItems
{
    using System.Text.RegularExpressions;
    public class IItem
    {
        protected Token id { get; set; }
        protected string line; //вся строка (для постоения графа)
        public IItem(string _line=null)
        {
            line = _line;
            if (line != null)
                line = line.Trim();
        }
        public void setColumn(int c) { id.Column = c; }
        public void setLine(int c) { id.Line = c; }
    }
}
