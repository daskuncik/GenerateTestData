using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GDT_EA.Classes.FunctionItems
{
    public class Init : IItem
    {
        string name;
        string value;
        public Init(string _line, int pos=0) : base(_line)
        {
            id = new Token();
            id.Name = (int)ConstClass.TokensEnum.Init;
            id.Line = pos;
            string pattern = @"(.+) = (.+);";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(line);
            if (!match.Success)
                return;
            value = match.Groups[2].Value;
            name = match.Groups[1].Value;
            if (name.Contains(' '))
                name = name.Substring(name.IndexOf(' ') + 1);
           
        }
    }
}
