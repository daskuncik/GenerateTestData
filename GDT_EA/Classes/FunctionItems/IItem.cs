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
            bodies = null;
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

        public override bool Equals(object obj)
        {
            var item = obj as IItem;
            bool e = true;
            if (bodies != null && item.bodies != null && getChildCount() == item.getChildCount())
                for (int i=0; i<getChildCount(); i++)
                {
                    if (!bodies[i].Equals(item.bodies[i]))
                        e = false;
                }
            return item != null &&
                   id.Equals(item.id) &&
                   line == item.line &&
                   isComposite == item.isComposite &&
                   e;
                   //EqualityComparer<OperationSet[]>.Default.Equals(bodies, item.bodies);
        }

        public override int GetHashCode()
        {
            var hashCode = -1569828690;
            hashCode = hashCode * -1521134295 + EqualityComparer<Token>.Default.GetHashCode(id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(line);
            hashCode = hashCode * -1521134295 + isComposite.GetHashCode();
            hashCode = hashCode * -1521134295 + current_i.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<OperationSet[]>.Default.GetHashCode(bodies);
            return hashCode;
        }

        public static bool operator ==(IItem obj1, IItem obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;

            if (ReferenceEquals(obj1, null))
                return false;
            if (ReferenceEquals(obj2, null))
                return false;
            return obj1.Equals(obj2);
           
        }

        public static bool operator !=(IItem obj1, IItem obj2)
        {
            return !(obj1 == obj2);
        }

    }
}
