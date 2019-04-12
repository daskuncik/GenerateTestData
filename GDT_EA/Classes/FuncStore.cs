using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GDT_EA.Classes.FunctionItems;
using System.Text.RegularExpressions;

namespace GDT_EA.Classes
{
    public class FuncStore
    {
        Dictionary<string, string> vars;
        List<IItem> list;
        string name;
        
        public FuncStore(string path, int pos)
        {
            vars = new Dictionary<string, string>();
            StreamReader reader = new StreamReader(path);
            list = new List<IItem>();
            string line = String.Empty;
            int i;
            if (pos > 0)
            {
                for (i = 0; i < pos; i++)
                    line = reader.ReadLine();
                //start read func
                line = reader.ReadLine();
            }
            string pattern;
            Regex reg;
            i = pos;

            //считать шапку
            line = reader.ReadLine();
            string func_body = line;

            while (line != "}")
            {
                line = reader.ReadLine();
                line = Trim(line);
                func_body += line;
            }
            reader.Close();
            //взять название и тело функции
            pattern = @"(.+)\s(.+)\s{1,}\((.+)\)\{(.+)\}";
            reg = new Regex(pattern);
            Match match = reg.Match(func_body);
            if (!match.Success)
                return;
            name = match.Groups[2].Value;
            //формирование списка переменных. 
            List<string> lst = match.Groups[3].Value.Split(new char[] { ',' }).ToList();
            foreach (var it in lst)
            {
                string type1 = it.Substring(0, it.IndexOf(" "));
                string name = it.Substring(it.IndexOf(" ")+1);
                vars.Add(name, type1);
            }

            func_body = match.Groups[4].Value;
            list = ParseFunction(func_body);
        }


        private List<IItem> ParseFunction(string func_body)
        {
            List<IItem> lst = new List<IItem>();
            while (!String.IsNullOrEmpty(func_body))
            {
                IItem item = FindItem(ref func_body);
                if (item != null)
                    lst.Add(item);
            }
            return lst;
        }

        private IItem FindItem(ref string func_body)
        {
            int pos_end = func_body.IndexOf(ConstClass.semicolon);
            int pos_if = func_body.IndexOf(ConstClass._if);
            IItem item = null;
            int min = pos_end;
            bool op = true;
            if (pos_end < 0 && pos_if < 0)
                return null;
            if (pos_if >= 0 && pos_if < pos_end)
            {
                min = pos_if;
                op = false;
            }
            string line = "";
            if (op) //выражение
            {
                line = func_body.Substring(0, func_body.IndexOf(ConstClass.semicolon)+1).Trim(new char[] { '\t', ' ' });
                //удалить все до символа ;
                func_body = func_body.Remove(0, func_body.IndexOf(ConstClass.semicolon)+1).Trim(new char[] { '\t', ' '});

                //выяснить, что это
                item = Know_type(line);
            }
            else if (pos_if >= 0 && pos_if < pos_end)
            {
                item = createCondition(ref func_body, pos_if);
            }
            return item;
        }

        private IItem createCondition(ref string func_body, int pos_if)
        {
            //выяснить, тело условия состоит из 1 оператора или многих
            int open_pos = func_body.IndexOf('(', pos_if);
            //найти конец условия
            int close_pos = getEndOfBracket(func_body, '(', ')');
            string condition = func_body.Substring(pos_if, close_pos - pos_if + 1);
            //удалить if (...)
            func_body = func_body.Remove(0, close_pos - pos_if + 1);
            IItem cond = new Condition(condition);

            //если после условия стоит {, то тело состоит из нескольих операций
            if (func_body.IndexOf('{') > 0 && func_body.IndexOf('{') < 5)
            {
                open_pos = func_body.IndexOf('{');
                close_pos = getEndOfBracket(func_body, '{', '}');
                string line = func_body.Substring(open_pos, close_pos - open_pos + 1);
                func_body = func_body.Remove(func_body.IndexOf(line), line.Count());
                line = line.Trim(new char[] { '{', '}' }).Trim(new char[] { '\t', ' '});
                List<IItem> lst = ParseFunction(line);
                (cond as Condition).addOperationsToBody(lst, true);
            }
            else // из одной
            {
                IItem item = FindItem(ref func_body);
                if (item != null)
                    (cond as Condition).addOperationToBody(item, true);
            }

            //убедиться, что отработтые символы удалены!!!
            return cond;

            //проверить наличие ветки else
            
        }


        private int getEndOfBracket(string line, char open_bracket, char close_bracket)
        {
            int len = line.Length;
            int c = 0, pos = 0;
            char bb;
            for (int i=0; i<len; i++)
            {
                bb = line[i];
                if (line[i] == close_bracket)
                {
                    c--;
                    if (c == 0)
                    {
                        pos = i;
                        break;
                    }
                }
                if (line[i] == open_bracket)
                    c++;
            }
            return pos;
        }

        private IItem Know_type(string line)
        {
            IItem item;
            if (line.Contains(ConstClass.eq))
            {
                item = new Init(line);
                return item;
            }
            if (line.Contains(ConstClass.ret))
            {
                item = new Return(line);
                return item;
            }

            if (line.Contains(ConstClass.inc))
            {
                item = new Increment(line);
                return item;
            }

            item = new IItem(line);
            return item;
        }

        private string Trim(string line)
        {
            string pattern = @"\s{2,}";
            Regex reg = new Regex(pattern);
            line = reg.Replace(line, " ");
            pattern = @"\t+";
            line = reg.Replace(line, "");
            return line;
        }

    }
}
