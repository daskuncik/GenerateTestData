using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDT_EA.Classes.FunctionItems;
using System.Text.RegularExpressions;

namespace GDT_EA.Classes
{
    public class OP
    {
        public int id { get; set; }
        public int operation;
        public int value;
        public List<int> goal; //размер списка - количество тестов, взятое из метода МСДС
    }

    class Coverage
    {
        List<string> operations = new List<string>() { "<=", ">=", "<", ">", "==", "!=" };
        Object composite_object;
        List<IItem> instructions;
        int global_id;
        public Coverage(List<IItem> lst)
        {
            instructions = lst;
            composite_object = null;
            global_id = 0;
        }

        public void startRecursion()
        {
            List<Object> lst_obj = new List<Object>();
            for (int i = 0; i < instructions.Count; i++)
            {
                IItem item = instructions[i];
                if (item.IsComposite())
                    lst_obj.Add(recursFunction(item));
            }

            if (lst_obj.Count() > 1)
                composite_object = new Object(lst_obj);
            else
                composite_object = new Object(lst_obj[0], (byte)Operation.OR);
        }


        private Object recursFunction(IItem item)
        {
            string pattern = @"if \((.+)\)";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(item.getLine());
            if (!match.Success)
                return new Object();
            string str = match.Groups[1].Value;
            Object obj = new Object(str);
            Object sub = null;
            IItem it = item.getCurrentItem();

            do
            {
                if (it.IsComposite())
                {
                    Object obj_n = recursFunction(it);
                    if (sub == null)
                        sub = new Classes.Object(obj_n, (byte)Operation.SERIAL_IF);
                    else
                        sub.add_object(obj_n);
                }
                it = item.getCurrentItem();
            } while (it != null);
            if (sub == null)
                return obj;
            Object global = new Object(obj, (byte)Operation.ATTACHED_IF);
            global.add_object(sub);
            return global;
        }

        //на вход: список: операция + цели для нее.
        //функция разбирает по словарю по имени переменной. В словарб вносится имя переменной, значение, операция, список целей для всех тестов

        //public Dictionary<string, List<OP>> getVarWithOperation(List<Little> lst)
        //{
        //    Dictionary<string, List<OP>> dic = new Dictionary<string, List<OP>>();
        //    foreach (var obj in lst)
        //    {
        //        string str = obj.name;
        //        List < int > goals = obj.goals;

        //        for (int i = 0; i < operations.Count(); i++)
        //        {
        //            string op = operations.ElementAt(i);
        //            int index = str.IndexOf(op);
        //            if (index >= 0)
        //            {
        //                string var = str.Substring(0, index).TrimEnd();
        //                string value = str.Substring(index + op.Length).TrimStart();
        //                if (!dic.ContainsKey(var))
        //                    dic.Add(var, new List<OP>());
        //                OP opp = new OP();
        //                opp.id = global_id;
        //                global_id++;
        //                opp.operation = i;
        //                opp.value = Convert.ToInt32(value);
        //                opp.goal = goals;
        //                dic[var].Add(opp);
        //            }
        //        }
        //    }
        //}

        //функция разбирает объекты метода МС/ДС, выделяет оттуда имя пременной, значение, операция (>, < ....)
        public Dictionary<string, List<OP>> getOperationVariables()
        {
            Dictionary<string, List<OP>> dic = new Dictionary<string, List<OP>>();
            if (composite_object == null && composite_object.getState() != (byte)State.COMPOSITE)
                return dic;
            List<Object> lst = composite_object.getCompositeList();
            foreach (var obj in lst)
            {
                if (obj.getState() == (byte)State.COMPOSITE)
                {
                    recurs(obj, ref dic);
                }
                else
                {
                    string str = obj.getValue();
                }
            }
            return dic;
        }

        private void recurs(Object obj, ref Dictionary<string, List<OP>> dic)
        {
            List<Object> lst = obj.getCompositeList();
            foreach (var objj in lst)
            {
                if (objj.getState() == (byte)State.COMPOSITE)
                {
                    recurs(objj, ref dic);
                }
                else
                {
                    string str = objj.getValue();
                    addToDictionary(str, ref dic);
                }
            }
        }

        //выделить из строки условия название переменной, тип операции и сравниваемое значение
        //и положить ее в словарь для определенной переменной
        private void addToDictionary(string str, ref Dictionary<string, List<OP>> dic)
        {
            for (int i=0; i<operations.Count(); i++)
            {
                string op = operations.ElementAt(i);
                int index = str.IndexOf(op);
                if (index >= 0)
                {
                    string var = str.Substring(0, index).TrimEnd();
                    string value = str.Substring(index + op.Length).TrimStart();
                    if (!dic.ContainsKey(var))
                        dic.Add(var, new List<OP>());
                    OP opp = new OP();
                    opp.id = global_id;
                    global_id++;
                    opp.operation = i;
                    opp.value = Convert.ToInt32(value);
                    dic[var].Add(opp);
                }
            }
        }


        public List<List<bool>> get_true_coverage()
        {
            return composite_object.get_true();
        }

        public List<List<bool>> get_false_coverage()
        {
            return composite_object.get_false();
        }

        public List<string> get_all_names()
        {
            return composite_object.get_names();
        }
    }
}
