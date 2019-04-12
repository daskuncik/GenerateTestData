using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDT_EA.Properties;
using GDT_EA;
using System.IO;
using GDT_EA.DB;
using GDT_EA.Classes;
using System.Text.RegularExpressions;

namespace GDT_EA
{
    
    public class Parser
    {
        public Parser()
        {
            db = new GDT_DataBaseEntities();
        }
        GDT_DataBaseEntities db;
        StreamReader reader;
        int curr_project;
        string curr_path;

        //паттерны
        string pattern_include = "^#include *";
        string pattern_global_vars = @"^(extern|const|static) (\S+) (\S+)(\s?=\s?(.+))?;";
        string pattern_define_var = @"^(#define) (.+) ([^\/]+)\n";
        string pattern_ifndef = @"^ifndef .*";
        string pattern_typedef = @"^(typedef)\s(\S+)\s((\w+)|(\((.+)\)\((.+)\)));";
        string pattern_func = @" ^ (.+) (.+)\((.*)\);?";
        string pattren_func_opredelenie = @"^(.+)\s(.+)\s?\((.+)\)\n?{(.+)}";
        string pattern_struct = @"struct (.+)\s?{?";
        int line_num;

        public void Create_project(string name, string descr, string path)
        {
            Project pr = new Project();
            pr.Name = name;
            pr.Description = descr;
            pr.Path = path;

            db.Projects.Add(pr);
            //db.Entry(pr).State = EntityState.Added;
            db.SaveChanges();
            //db.SaveChangesAsync();
            curr_project = db.Projects.Select(p => p.Id).ToList().Last();
            curr_path = path;
            create_base_types();

        }

        public int get_project_id()
        {
            return curr_project;
        }

        public string Select_project(string path)
        {
            int count = db.Projects.Where(p => p.Path == path).Count();
            if (count == 1)
            {
                Project q = db.Projects.Where(p => p.Path == path).ToList().First();

                curr_path = q.Path;
                curr_project = q.Id;
                return q.Name;
            }
            else
                return null;
        }

        public void Parsing_file(string filename)
        {
            FileStream file = new FileStream(filename, FileMode.Open);
            reader = new StreamReader(file);
            line_num = 0;

            while (!reader.EndOfStream)
            {
                int num_pos = (int)reader.BaseStream.Position;
                string line = reader.ReadLine();
                line_num++;
                //проверка всех возможных паттернов
                if (Regex.IsMatch(line, pattern_include))
                    continue;
                string new_path = filename;
                if (Regex.IsMatch(line, pattern_global_vars))
                {
                    //int num_pos = (int)reader.BaseStream.Position;
                    create_global_var(line, new_path, line_num);
                }
                if (Regex.IsMatch(line, pattern_define_var))
                {
                    //int num_pos = (int)reader.BaseStream.Position;
                    create_define(line, new_path, line_num);
                }

                if (Regex.IsMatch(line, pattern_ifndef))
                {
                    reader.ReadLine();
                    continue;
                }
                if (Regex.IsMatch(line, pattern_typedef))
                    create_typedef(line, new_path, line_num);

                //if (Regex.IsMatch(line, pattern_func_typedef))
                //    create_func_typedef(line, new_path, line_num);

                if (Regex.IsMatch(line, pattern_func))
                    if (!line.Contains("typedef"))
                        create_func(line, new_path, line_num);


                if (Regex.IsMatch(line, pattern_struct, RegexOptions.Multiline))
                    create_struct(line, new_path, line_num);

            }
        }
        private void create_global_var(string line, string new_path, int position)
        {
            string type = Regex.Match(line, pattern_global_vars).Groups[2].Value;
            string qualifier = Regex.Match(line, pattern_global_vars).Groups[1].Value;
            string name = Regex.Match(line, pattern_global_vars).Groups[3].Value;
            string val = "";
            if (Regex.Match(line, pattern_global_vars).Groups.Count >= 5)
                val = Regex.Match(line, pattern_global_vars).Groups[5].Value;
            else if (name.Contains("="))
            {
                val = name.Substring(name.IndexOf("=") + 1);
            }

            //ищем такой тип в таблице
            int type_id = 0;
            int count = -1;
            int search_type_id = -1;
            if (db.Types.Any())
            {
                count = db.Types.Where(q => q.Name == type).
                    Join(db.QuickSearches,
                                        sId => sId.SearchInfoId,
                                        qId => qId.Id,
                                        (sId, qId) => new { sId, qId }).
                                        Where(s => s.qId.Id == s.sId.SearchInfoId && s.qId.ProjectId == curr_project).Count();
            }
            if (count == 0) //создаем тип в бд
                type_id = create_type_in_DB(new_path, position, type, null);
            else //берем ID этого типа
                type_id = (from a in db.Types
                           from b in db.QuickSearches
                           where a.Name == type & a.SearchInfoId == b.Id & b.ProjectId == curr_project
                           select a).ToList().First().Id;
            //Работа с переменными
            //Сколько есть переменных с таким типом и именем
            count = db.Variables.Where(q => q.Name == name && q.TypeId == type_id).Count();
            if (count == 0)
            {
                //создать переменную
                if (search_type_id < 0)
                    search_type_id = db.QuickSearches.Select(p => p.Id).ToList().Last();
                Variable varr = new Variable
                {
                    Name = name,
                    Qualifier = ConstClass.Qualifiers.FirstOrDefault(x => x.Value == qualifier).Key,
                    TypeId = type_id,
                    SearchInfoId = search_type_id,
                    Value = val
                };
                db.Variables.Add(varr);
                db.SaveChanges();
            }
        }

        private void create_typedef(string line, string new_path, int position)
        {
            string type = "", name = "", parametrs = "";
            if (Regex.Match(line, pattern_typedef).Groups.Count >= 5)
            {
                type = Regex.Match(line, pattern_typedef).Groups[2].Value;
                //где у имени типа есть звезда, значит, это функция
                name = Regex.Match(line, pattern_typedef).Groups[6].Value;
                //if (name.Contains("*")) name.Remove(name.IndexOf("*", 1));
                //parametrs не используются
                parametrs = Regex.Match(line, pattern_typedef).Groups[7].Value;
            }
            else
            {
                type = Regex.Match(line, pattern_typedef).Groups[2].Value;
                name = Regex.Match(line, pattern_typedef).Groups[4].Value;
            }
            int count = db.Types.Where(q => q.Name == name).Count();
            if (count == 0)
                create_type_in_DB(new_path, position, name, type);
        }
        
        private void create_define(string line, string new_path, int position)
        {
            string name = Regex.Match(line, pattern_global_vars).Groups[2].Value;
            string qualifier = Regex.Match(line, pattern_global_vars).Groups[1].Value;
            string value = Regex.Match(line, pattern_global_vars).Groups[3].Value;
            string type = "";
            bool isPointer = false;
            if (value.Contains("\""))
            {
                type = "char";
                isPointer = true;
            }
            else
                type = "int";

            int count = db.Variables.Where(x => x.Name == name).ToList().Count;
            if (count == 0)
            {
                int typeId = db.Types.Where(x => x.Name == type).ToList().First().Id;
                int searchId = create_QS(new_path, position);
                Variable v = new Variable
                {
                    TypeId = typeId,
                    Name = name,
                    IsPointer = isPointer,
                    Qualifier = ConstClass.Qualifiers.FirstOrDefault(x => x.Value == qualifier).Key,
                    Value = value,
                    SearchInfoId = searchId
                };
                db.Variables.Add(v);
                db.SaveChanges();
            }
        }

        private void create_func(string line, string new_path, int position)
        {
            string out_type = Regex.Match(line, pattern_func).Groups[1].Value;
            out_type = out_type.Replace("\t", String.Empty).Replace(" ", String.Empty);
            string name = Regex.Match(line, pattern_func).Groups[2].Value;
            int count = -1;
            int type_id = 1;

            //ищем функцию в ДБ
            count = db.Funcs.Where(q => q.Name == name).
                        Join(db.QuickSearches,
                                            sId => sId.SearchInfoId,
                                            qId => qId.Id,
                                            (sId, qId) => new { sId, qId }).
                                            Where(s => s.qId.ProjectId == curr_project).Count();
            if (count > 0) //в этом проекте такая функция существует
            {
                QuickSearch qq =
                    (from ff in db.Funcs
                     from qs in db.QuickSearches
                     where qs.ProjectId == curr_project & ff.Name == name & ff.SearchInfoId == qs.Id
                     select qs).ToList().First();

                if (!qq.Path.Contains(new_path))
                {
                    qq.Path += "+" + new_path;

                    db.SaveChanges();
                }
                if (new_path.Contains(".c"))
                {
                    qq.ByteNum = position;
                    db.SaveChanges();
                }

                //return;
            }
            else //в этом проекте такой функции нет
            {
                //ищем тип в БД
                count = db.Types.Where(q => q.Name == out_type).
                        Join(db.QuickSearches,
                                            sId => sId.SearchInfoId,
                                            qId => qId.Id,
                                            (sId, qId) => new { sId, qId }).
                                            Where(s => s.qId.ProjectId == curr_project && s.qId.Id == s.sId.SearchInfoId).Count();


                if (count == 0) //создаем тип
                {
                    type_id = create_type_in_DB(new_path, position, out_type);
                }
                else //берем идентификатор сущетсвующего типа
                    type_id = db.Types.Where(q => q.Name == out_type).
                                            Join(db.QuickSearches,
                                            sId => sId.SearchInfoId,
                                            qId => qId.Id,
                                            (sId, qId) => new { sId, qId }).
                                            Where(s => s.qId.ProjectId == curr_project).
                                            Select(s => s.sId).First().Id;


                int search_id = create_QS(new_path, position);
                Func f = new Func
                {
                    Name = name,
                    OutTypeId = type_id,
                    SearchInfoId = search_id
                };
                db.Funcs.Add(f);
                db.SaveChanges();

                int func_id = db.Funcs.Where(q => q.Name == name && q.SearchInfoId == search_id).Select(w => w.Id).First();
                //разбираем аргументы функции
                string param = Regex.Match(line, pattern_func).Groups[3].Value;
                if (param != "" || param != "void")
                {
                    string[] split = param.Split(new string[] { ", " }, StringSplitOptions.None);
                    foreach (string elem in split)
                    {
                        create_param(elem, func_id, new_path, position);
                    }
                }
            }

            if (new_path.Contains(".c"))
            {
                while ((line = reader.ReadLine()) != "}")
                { line_num++; }
            }
        }

        private void create_param(string param, int func_id, string new_path, int position)
        {
            string pattern = @"(.+ )?(.+) (.+)";
            string t = Regex.Match(param, pattern).Groups[1].Value;
            string type = Regex.Match(param, pattern).Groups[2].Value;
            string param_name = Regex.Match(param, pattern).Groups[3].Value;
            int type_id = -1;
            if (type != "" && param_name != "")
            {
                int count = db.Types.Where(q => q.Name == type).
                            Join(db.QuickSearches,
                                                sId => sId.SearchInfoId,
                                                qId => qId.Id,
                                                (sId, qId) => new { sId, qId }).
                                                Where(s => s.qId.ProjectId == curr_project).Count();
                if (count > 0)
                    type_id = db.Types.Where(q => q.Name == type).
                            Join(db.QuickSearches,
                                                sId => sId.SearchInfoId,
                                                qId => qId.Id,
                                                (sId, qId) => new { sId, qId }).
                                                Where(s => s.qId.ProjectId == curr_project).
                                                Select(s => s.sId).First().Id;
                else
                    type_id = create_type_in_DB(new_path, position, type);

                InputParam ip = new InputParam
                {
                    Name = param_name,
                    TypeId = type_id,
                    FuncId = func_id
                };
                db.InputParams.Add(ip);
                db.SaveChanges();
            }
        }

        private void create_struct(string line, string new_path, int position)
        {
            string all_struct = line;
            string l = String.Empty;
            l = reader.ReadLine();
            while (String.IsNullOrEmpty(l))
            {
                all_struct += l;
                l = reader.ReadLine();
            }
            string pattern1 = @"^struct (.+)\s{(.+) (.+);}\s?(.+)?;";
            string type = "struct";
            string name = "";
            string var_name = "";
            string body = "";
            Regex regex = new Regex(pattern1);
            Match match = regex.Match(l);
            if (!match.Success)
                return;
            name = match.Groups[1].Value;
            body = match.Groups[2].Value;
            body += ";";

            int i = create_type_in_DB(new_path, position, type, name);

            if (match.Groups.Count == 3)
            {
                var_name = match.Groups[3].Value;
                int search = create_QS(new_path, position);
                Variable varr = new Variable
                {
                    Name = var_name,
                    TypeId = i,
                    Qualifier = ConstClass.Qualifiers.FirstOrDefault(x => x.Value == type).Key,
                    SearchInfoId = search
                };
                db.Variables.Add(varr);
                db.SaveChanges();
            }
            

        }

        private int create_type_in_DB(string path, int pos, string name, string user_t = "")
        {
            int search_type_id = create_QS(path, pos);
            DB.Type new_type = new DB.Type
            {
                Name = name,
                SearchInfoId = search_type_id,
                UserType = user_t
            };

            db.Types.Add(new_type);
            db.SaveChanges();
            return db.Types.Select(p => p.Id).ToList().Last();
        }

        private int create_QS(string path, int pos)
        {
            QuickSearch qss = new QuickSearch
            {
                ProjectId = curr_project,
                Path = path,
                ByteNum = pos
            };
            db.QuickSearches.Add(qss);
            db.SaveChanges();
            return db.QuickSearches.Select(p => p.Id).ToList().Last();
        }

        private void create_base_types()
        {
            int q_id = create_QS("null", 1);

            DB.Type new_type = new DB.Type
            {
                Name = "int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);


            new_type = new DB.Type
            {
                Name = "char",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "void",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);


            new_type = new DB.Type
            {
                Name = "double",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);


            new_type = new DB.Type
            {
                Name = "float",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);


            new_type = new DB.Type
            {
                Name = "unsigned char",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "signed char",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "unsigned int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "signed int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);


            new_type = new DB.Type
            {
                Name = "short int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "unsigned short int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "signed short int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "unsigned short int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "long int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "unsigned long int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "signed long int",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            new_type = new DB.Type
            {
                Name = "long double",
                SearchInfoId = q_id
            };
            db.Types.Add(new_type);

            db.SaveChanges();

        }
    }
}
