using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;

namespace GDT_EA.Classes.FunctionItems
{
    public class Condition : IItem
    {
        string condition;
        OperationSet body_true;
        OperationSet body_false;
        public Condition(string _line, int pos=0) : base(_line)
        {
            id = new Token();
            id.Name = (int)ConstClass.TokensEnum.Condition;

            string pattern = @"if \((.+)\)";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(line);
            if (!match.Success)
                return;
            condition = match.Groups[1].Value;
            body_true = new OperationSet();
            body_false = new OperationSet();
        }

        public void addOperationToBody(IItem b, bool state) { if (state) body_true.addOperation(b); else body_false.addOperation(b); }

        public void addOperationsToBody(List<IItem> list, bool state) { if (state) body_true.addRangeOperation(list); else body_false.addRangeOperation(list); }
    }
}
