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

        public void startRecursion(List<IItem> limit = null)
        {
            List<Object> lst_obj = new List<Object>();
            for (int i = 0; i < instructions.Count; i++)
            {
                IItem item = instructions[i];
                if (item.IsComposite())
                    lst_obj.Add(recursFunction(item, limit));
            }

            if (lst_obj.Count() > 1)
                composite_object = new Object(lst_obj);
            else
                composite_object = new Object(lst_obj[0], (byte)Operation.OR);
        }



        private Object recursFunction(IItem item, List<IItem> limit = null)
        {
            string pattern = @"if \((.+)\)";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(item.getLine());
            if (!match.Success)
                return new Object();
            string str = match.Groups[1].Value;
            Object obj = new Object(str);
            if (limit != null)
            {
                foreach (var itt in limit)
                {
                    if (itt == item)
                         obj.setCompositeGoal(true);
                }
            }
            Object sub = null;
            IItem it = item.getCurrentItem();

            do
            {
                if (it.IsComposite())
                {
                    Object obj_n = recursFunction(it, limit);
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
            //добавить к sub цель из списка Лимит???
            global.add_object(sub);
            return global;
        }

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
        private void addToDictionary(string str, ref Dictionary<string, List<OP>> dic, List<int> goals = null)
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
                    OP opp = new OP
                    {
                        id = global_id,
                        operation = i,
                        value = Convert.ToInt32(value)
                    };
                    global_id++;
                    if (goals != null)
                        opp.goal = goals;
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

        public List<List<List<bool>>> GetGoalForItem(List<IItem> lst)
        {
            List<List<List<bool>>> result = new List<List<List<bool>>>();
            lst.Reverse();
            foreach (var item in lst)
            {
                string str = item.getLine();
                string pattern = @"if \((.+)\)";
                Regex reg = new Regex(pattern);
                Match match = reg.Match(str);
                if (!match.Success)
                    continue;
                str = match.Groups[1].Value;
                List<List<bool>> cur = composite_object.GetGoalValue(str);
                if (cur != null)
                    result.Add(cur);
            }
            return result;
        }

        public Dictionary<string, List<OP>> CreateSituationForGraph(List<List<List<bool>>> lst, ref List<List<int>> res_goals, List<string> items)
        {
            Dictionary<string, List<OP>> dic = new Dictionary<string, List<OP>>();
            
            int max = 0;
            foreach (var c in lst)
                if (c[0].Count > max)
                    max = c[0].Count;
            int i = 0;
            foreach (var item in instructions)
            {
                if (item.IsComposite())
                    i = RecurseAddingToDic(ref dic, ref res_goals, item, max, i, lst, items);
            }
            return dic;

        }

        private int RecurseAddingToDic(ref Dictionary<string, List<OP>> dic, ref List<List<int>> res_goals, IItem item, int max, int i, List<List<List<bool>>> lst, List<string> items)
        {
            if (item.IsComposite())
            {
                List<string> conditions = GetListAtomarCond(item.getLine());
                if (items.Contains(item.getLine()))
                {
                    AddToDic(ref dic, ref res_goals, conditions, max, lst[i]);
                    i++;
                    for (int j = 0; j < item.getChild(0).Count(); j++)
                    {
                        if (item.getChild(0).getItem(j).IsComposite())
                            i = RecurseAddingToDic(ref dic, ref res_goals, item.getChild(0).getItem(j), max, i, lst, items);
                    }
                }
                else
                {
                    AddToDic(ref dic, ref res_goals, conditions, max);

                    for (int k = 0; k < item.getChildCount(); k++)
                    {
                        for (int j = 0; j < item.getChild(k).Count(); j++)
                        {
                            if (item.getChild(k).getItem(j).IsComposite())
                                i = RecurseAddingToDic(ref dic, ref res_goals, item.getChild(k).getItem(j), max, i, lst, items);
                        }
                    }
                }
            }
            return i;
            
        }
        private List<string> GetConditionList(string str)
        {
            string obj_without_brackets = "";
            byte open_count = 0;
            string obj = null;
            bool finish = false;
            int and_count = 0, or_count = 0;
            List<string> result = new List<string>();

            if (str.Contains("&&") || str.Contains("||"))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (i + 1 < str.Length && open_count <= 0)
                        if (str.Substring(i, 2) == "&&" || str.Substring(i, 2) == "||")
                        {
                            obj_without_brackets = obj_without_brackets.TrimStart().TrimEnd();
                            if (obj_without_brackets != "")
                            {
                                result.Add(obj_without_brackets);
                                obj_without_brackets = "";
                            }

                            if (str.Substring(i, 2) == "&&")
                                and_count++;
                            else
                                or_count++;
                            i += 2;
                        }

                    if (str[i] == ')')
                    {
                        open_count--;
                        finish = true;
                    }

                    if (open_count > 0)
                        obj += str[i];
                    else
                        obj_without_brackets += str[i];

                    if (str[i] == '(')
                    {
                        open_count++;
                        obj_without_brackets = "";
                    }


                    if (open_count == 0 && finish == true)
                    {
                        result.Add(obj);
                        obj_without_brackets = "";
                        obj = null;
                        finish = false;
                    }
                }
                if (obj_without_brackets != "")
                {
                    obj_without_brackets = obj_without_brackets.TrimStart().TrimEnd();
                    if (obj_without_brackets != "")
                    {
                        result.Add(obj_without_brackets);
                    }
                }
            }
            else
            {
                result.Add(str);
            }
            return result;
        }

        private List<string> GetListAtomarCond(string str)
        {
            string pattern = @"if \((.+)\)";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(str);
            if (match.Success)
                str = match.Groups[1].Value;
            List<string> result;
            result = GetConditionList(str);
            List<string> current;
            bool has;
            do
            {
                has = false;
                for (int i=0; i<result.Count; i++)
                {
                    if (result[i].Contains("&&") || result[i].Contains("||"))
                    {
                        current = GetConditionList(result[i]);
                        has = true;
                        result.RemoveAt(i);
                        result.InsertRange(i, current);
                        break;
                    }
                }
            } while (has);
            return result;
        }

        private void AddToDic(ref Dictionary<string, List<OP>> dic, ref List<List<int>> res_goals, List<string> conditions, int n = 0, List<List<bool>> goals = null)
        {
            int i = 0;
            foreach (var str in conditions)
            {
                List<int> g = new List<int>();
                //заполнить, как true-цели
                if (goals != null)
                {
                    
                    for (int j=0; j< goals[i].Count; j++)
                    {
                        if (goals[i][j])
                            g.Add(1);
                        else
                            g.Add(0);
                    }
                    if (n > 0 && n > goals[i].Count)
                        for (int j = goals[i].Count; j < n; j++)
                        g.Add(1);
                    
                }
                else
                {
                    if (n > 0)
                        for (int j = 0; j < n; j++)
                            g.Add(0);
                }
                addToDictionary(str, ref dic, g);
                res_goals.Add(g);
                i++;
            }
        }
    }
}
