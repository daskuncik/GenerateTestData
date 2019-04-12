using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT_EA.Classes.FunctionItems
{
    public class Increment : IItem
    {
        public Increment(string line = null, int _pos = 0) : base(line)
        {
            id = new Token();
            id.Name = (int)ConstClass.TokensEnum.Increment;
        }
    }
}
