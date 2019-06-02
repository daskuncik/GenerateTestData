using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GDT_EA.Classes.FunctionItems
{
    public class IItem
    {
        protected Token id { get; set; }
        protected string line; //вся строка (для постоения графа)
        protected bool isComposite;
        protected int current_i;
        protected OperationSet[] bodies;
        public IItem(string _line=null)
        {
            line = _line;
            if (line != null)
                line = line.Trim();
            isComposite = false;
            current_i = -1;
        }
        public void setColumn(int c) { id.Column = c; }
        public void setLine(int c) { id.Line = c; }

        public string getLine() { return line; }
        public int getToken() { return id.Name; }
        public bool IsComposite() { return isComposite; }
        public virtual IItem getCurrentItem()
        {
            return null;
        }

        public virtual int getChildCount()
        {
            return 0;
        }

        public virtual OperationSet getChild(int i)
        {
            return null;
        }
    }
}
