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
        //OperationSet body_true;
        //OperationSet body_false;
        
        //List<OperationSet> bodies;
        public Condition(string _line, int pos=0) : base(_line)
        {
            id = new Token();
            isComposite = true;
            id.Name = (int)ConstClass.TokensEnum.Condition;

            string pattern = @"if \((.+)\)";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(line);
            if (!match.Success)
                return;
            condition = match.Groups[1].Value;
            // bodies = Array.CreateInstance(typeof(OperationSet), 2);
            bodies = new OperationSet[2];
            for (int i = 0; i < 2; i++)
                bodies[i] = new OperationSet();

            //body_true = new OperationSet();
            //body_false = new OperationSet();
        }

        //public void addOperationToBody(IItem b, bool state) { if (state) body_true.addOperation(b); else body_false.addOperation(b); }
        public void addOperationToBody(IItem b, bool state) { if (state) bodies[0].addOperation(b); else bodies[1].addOperation(b); }

        //public void addOperationsToBody(List<IItem> list, bool state) { if (state) body_true.addRangeOperation(list); else body_false.addRangeOperation(list); }
        public void addOperationsToBody(List<IItem> list, bool state) { if (state) bodies[0].addRangeOperation(list); else bodies[1].addRangeOperation(list); }

        public override IItem getCurrentItem()
        {
            if (current_i < 0)
                current_i = -1;
            

            current_i++;
            if (current_i >= bodies[0].Count())
            {
                current_i = -1;
                return null;
            }
            return bodies[0].getItem(current_i);
        }

        public OperationSet getBody(bool body)
        {
            if (body)
                return bodies[0];
            else
                return bodies[1];
        }

        public override int getChildCount()
        {
            return 2;
        }

        public override OperationSet getChild(int i)
        {
            return bodies[i];
        }
    }
}
