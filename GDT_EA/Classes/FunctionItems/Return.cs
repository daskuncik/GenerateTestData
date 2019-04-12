using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT_EA.Classes.FunctionItems
{
    class Return : Init
    {
        public Return(string _line, int pos = 0) : base(_line)
        {
            id = new Token();
            id.Name = (int)ConstClass.TokensEnum.Return;
            id.Line = pos;

        }
    }
}
