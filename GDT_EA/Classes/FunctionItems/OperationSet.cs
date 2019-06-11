using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT_EA.Classes.FunctionItems
{
    public class OperationSet : IItem
    {
        public List<IItem> op_list { get;  }
        public OperationSet(string _line=null) : base(_line)
        {
            op_list = new List<IItem>();
        }

        ~OperationSet() { op_list.Clear(); }

        public void addOperation(IItem item)
        {
            op_list.Add(item);
        }

        public void addRangeOperation(List<IItem> lst)
        {
            op_list.AddRange(lst);
        }

        public int Count() { return op_list.Count; }

        public IItem getItem(int i) { return op_list[i]; }

        public override bool Equals(object obj)
        {
            var set = obj as OperationSet;
            if (op_list.Count != set.op_list.Count)
                return false;
            bool res = true;
            for (int i=0; i<op_list.Count; i++)
            {
                if (op_list[i].getLine() != set.op_list[i].getLine())
                    res = false;
            }
            return res;
        }
    }
}
