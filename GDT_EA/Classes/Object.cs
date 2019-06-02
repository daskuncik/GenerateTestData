using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GDT_EA.Classes
{
    enum State : byte { ALONE, COMPOSITE };
    enum Operation : byte { AND, OR, SERIAL_IF, ATTACHED_IF };

    public class Object
    {
        List<Object> composite;
        byte operation;
        byte state { get; set; }
        string value;
        List<List<bool>> goal_true;
        List<List<bool>> goal_false;
        public Object() { }
        public Object(string str)
        { 
            value = str;
            state = (byte)State.ALONE;


            composite = new List<Object>();
            string obj_without_brackets = "";
            byte open_count = 0;
            string obj = null;
            bool finish = false;
            int and_count = 0, or_count = 0;
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
                                Object new_obj = new Object(obj_without_brackets);
                                composite.Add(new_obj);
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
                        Object new_obj = new Object(obj);
                        obj_without_brackets = "";
                        composite.Add(new_obj);
                        obj = null;
                        finish = false;
                    }
                }
                if (obj_without_brackets != "")
                {
                    obj_without_brackets = obj_without_brackets.TrimStart().TrimEnd();
                    if (obj_without_brackets != "")
                    {
                        Object new_obj = new Object(obj_without_brackets);
                        composite.Add(new_obj);
                    }
                }
                if (composite.Count() > 1)
                {
                    state = (byte)State.COMPOSITE;
                    if (and_count>0)
                        create_and();
                    else
                        create_or();
                }
                else
                    state = (byte)State.ALONE;
            }
            else
            {
               value = str;
               state = (byte)State.ALONE;
            }

        }

        public Object(Object obj1, byte op)
        {
            composite = new List<Classes.Object>();
            composite.Add(obj1);
            operation = op;
            set_operation(operation);
        }

        public void set_operation(byte op)
        {
            if (op == (byte)Operation.AND)
                create_and();
            if (op == (byte)Operation.OR)
                create_or();
            if (op == (byte)Operation.SERIAL_IF)
                create_if();
            if (op == (byte)Operation.ATTACHED_IF)
                create_att_if();
        }

        //если поступает целый лист, значит это все последовательные if'ы функции
        public Object(List<Object> obj_list)
        {
            composite = new List<Classes.Object>();
            composite.AddRange(obj_list);
            create_or();
        }

        public int count()
        {
            return composite.Count();
        }

        private void create_and()
        {
            operation = (byte)Operation.AND;
            if (goal_true != null && goal_false != null)
            {
                goal_true.Clear();
                goal_false.Clear();
            }
            goal_true = get_one(true);
            goal_false = get_composite();
        }

        private void create_or()
        {
            operation = (byte)Operation.OR;
            goal_false = get_one(false);
            goal_true = get_composite();
        }

        private void create_if()
        {
            operation = (byte)Operation.SERIAL_IF;
            goal_true = get_mix(composite.Count(), false);
            goal_false = null;
        }

        private void create_att_if()
        {
            operation = (byte)Operation.ATTACHED_IF;
            if (goal_true != null && goal_false != null)
            {
                goal_true.Clear();
                goal_false.Clear();
            }
            goal_true = get_one(true);
            goal_false = new List<List<bool>>();

            goal_false.Add(new List<bool>());
            goal_false.Add(new List<bool>());
            goal_false[0].Add(false);
            goal_false[1].Add(false);
        }

        public void add_object(Object obj)
        {
            composite.Add(obj);
            if (state == (byte)State.ALONE)
                state = (byte)State.COMPOSITE;
            set_operation(operation);
        }

        public void add_object(List<Object> list)
        {
            composite.AddRange(list);
            if (state == (byte)State.ALONE)
                state = (byte)State.COMPOSITE;
            set_operation(operation);
        }

        private List<List<bool>> get_one(bool var)
        {
            List<List<bool>> list = new List<List<bool>>();

            if (state != (byte)State.ALONE)
            {
                List<bool> ll = new List<bool>();
                int count = composite.Count();
                ll.Add(var);
                for (int i = 0; i < count; i++)
                    list.Add(ll);
                return list;
            }
            return null;
        }

        public List<List<bool>> get_composite()
        {
            List<List<bool>> list = new List<List<bool>>();
            if (state == (byte)State.COMPOSITE)
            {
                int count = composite.Count();
                for (int i = 0; i < count; i++)
                {
                    List<bool> ll = new List<bool>();
                    for (int j = 0; j < count; j++)
                        if (j == i)
                            ll.Add(true);
                        else
                            ll.Add(false);
                    list.Add(ll);
                }
                return list;
            }
            return null;
        }

        public List<List<bool>> get_mix(int count, bool need_x2)
        {
            List<List<bool>> global_list = new List<List<bool>>();
            double aa = Math.Pow(2, Convert.ToDouble(count));
            int future_count = Convert.ToInt32(aa);

            List<bool> ll = new List<bool>();
            int current_count = future_count / 2;
            for (int i = 0; i < future_count; i++)
            {
                if (i < current_count)
                    ll.Add(false);
                else
                    ll.Add(true);
            }
            global_list.Add(ll);
            if (count == 2)
            {
                global_list.Add(get_mix());
                global_list.ElementAt(1).AddRange(global_list[1]);
            }
            else
                if (count != 1)
                global_list.AddRange(get_mix(count - 1, true));
            if (need_x2)
                for (int i = 1; i < global_list.Count(); i++)
                {
                    global_list.ElementAt(i).AddRange(global_list[i]);
                }
            return global_list;
        }

        public List<bool> get_mix()
        {
            List<bool> ll = new List<bool>();
            ll.Add(false);
            ll.Add(true);
            //ll.Add(false);
            //ll.Add(true);
            return ll;


        }

        public List<List<bool>> get_true()
        {
            List<List<bool>> global_list = new List<List<bool>>();
            List<bool> local = new List<bool>();
            int count = composite.Count();
            if (count <= 1)
            {
                if (value != null)
                {
                    local.Add(true);
                    global_list.Add(local);
                    return global_list;
                }
                else
                    return composite[0].get_true();
            }
            else
            {
                int str_count = goal_true.Count();
                int clmn_count = goal_true[0].Count();

                for (int z = 0; z < str_count; z++)
                {
                    global_list.Add(new List<bool>());
                }

                
                //i - значения переменных композита,  j - сами переменные композита
                for (int i = 0; i < clmn_count; i++)
                {
                    List<List<bool>> temp = new List<List<bool>>();
                    for (int j = str_count - 1; j >= 0; j--)
                    {
                        List<List<bool>> curr = new List<List<bool>>();
                        if (goal_true[j][i])
                            curr = composite[j].get_true();
                        else
                            curr = composite[j].get_false();

                        temp.InsertRange(0, curr);
                        int max_len = 0;
                        foreach (List<bool> a in temp)
                            if (a.Count() > max_len)
                                max_len = a.Count();
                        foreach (List<bool> a in temp)
                            if (a.Count() != max_len)
                            {
                                int dif = max_len - a.Count();
                                for (int k = 0; k < dif; k++)
                                    a.Add(a.Last());
                            }
                    }


                    if (temp.Count() > global_list.Count())
                    {
                        int diff = temp.Count() - global_list.Count();
                        for (int z = 0; z<diff; z++)
                            global_list.Insert(0, new List<bool>());
                    }

                    for (int a = 0; a < temp.Count(); a++)
                    {
                        global_list.ElementAt(a).AddRange(temp.ElementAt(a));
                    }
                    //temp.Clear();
                }
            }
            return global_list;
        }

        public List<List<bool>> get_false()
        {
            List<List<bool>> global_list = new List<List<bool>>();
            List<bool> local = new List<bool>();
            int count = composite.Count();
            if (count <= 1)
            {
                if (value != null)
                {
                    local.Add(false);
                    global_list.Add(local);
                    return global_list;
                }
                else
                    return composite[0].get_false();
            }
            else
            {
                if (goal_false != null)
                {
                    int str_count = goal_false.Count();
                    int clmn_count = goal_false[0].Count();

                    for (int z = 0; z < str_count; z++)
                    {
                        global_list.Add(new List<bool>());
                    }


                    //i - значения переменных композита,  j - сами переменные композита
                    for (int i = 0; i < clmn_count; i++)
                    {
                        List<List<bool>> temp = new List<List<bool>>();
                        for (int j = str_count - 1; j >= 0; j--)
                        {
                            List<List<bool>> curr = new List<List<bool>>();
                            if (goal_false[j][i])
                                curr = composite[j].get_true();
                            else
                                curr = composite[j].get_false();
                            if (curr != null)
                            {
                                temp.InsertRange(0, curr);
                                int max_len = 0;
                                foreach (List<bool> a in temp)
                                    if (a.Count() > max_len)
                                        max_len = a.Count();
                                foreach (List<bool> a in temp)
                                    if (a.Count() != max_len && a.Count() > 0)
                                    {
                                        int dif = max_len - a.Count();
                                        for (int k = 0; k < dif; k++)
                                            a.Add(a.Last());
                                    }
                            }
                            else
                            {
                                int len = composite[j].count();
                                for (int z = 0; z < len; z++)
                                    temp.Insert(0, new List<bool>());
                            }
                        }


                        if (temp.Count() > global_list.Count())
                        {
                            int diff = temp.Count() - global_list.Count();
                            for (int z = 0; z < diff; z++)
                                global_list.Insert(0, new List<bool>());
                        }

                        for (int a = 0; a < temp.Count(); a++)
                        {
                            global_list.ElementAt(a).AddRange(temp.ElementAt(a));
                        }
                        //temp.Clear();

                        
                    }
                    return global_list;
                }
                return null;
            }
        }

        public List<string> get_names()
        {
            List<string> names = new List<string>();
            if (state == (byte)State.COMPOSITE)
            {
                foreach (Object elem in composite)
                    names.AddRange(elem.get_names());
            }
            else
            {
                if (value != null)
                    names.Add(value);
                else
                    names.AddRange(composite[0].get_names());

            }
            return names;
        }

        public byte getState() { return state; }

        public List<Object> getCompositeList() { return composite; }

        public string getValue() { return value; }
    }
}
