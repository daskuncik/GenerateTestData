using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GDT_EA.Classes.FunctionItems
{
    public class Declaration : IItem
    {
        string name { get; }
        string type { get; }
        Declaration( string _line, int pos) : base(_line)
        {
            id = new Token();
            id.Name = (int)ConstClass.TokensEnum.Declaration;
            id.Line = pos;
            
            string pattern = @"(.+)\s(.+);";
            Regex reg = new Regex(pattern);
            Match m = reg.Match(line);
            if (!m.Success)
                return;
            type = m.Groups[1].Value;
            name = m.Groups[2].Value;

            if (name.Contains("["))
            {
                int s = name.IndexOf("[");
                type += name.Substring(s, name.IndexOf("]") - s + 1);
                name.Remove(s);
            }
        }


    }
}
