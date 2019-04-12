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

        public void addOperation(IItem item)
        {
            op_list.Add(item);
        }

        public void addRangeOperation(List<IItem> lst)
        {
            op_list.AddRange(lst);
        }

        
    }
}
