using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDT_EA.Properties;
using GDT_EA;
using System.IO;
using GDT_EA.DB;
using System.Windows.Forms;

namespace GDT_EA.DB
{
    
    class DBFunctions
    {
        GDT_DBEntities db;
        
        public DBFunctions()
        {
            db = new GDT_DBEntities();

        }

        public List<string> AllProjects()
        {
            List<string> result = new List<string>();
            int count = db.Projects.Count();
            
            if (count == 0)
                return result;
            List<Project> projects = db.Projects.ToList();
            foreach (var pr in projects)
            {
                result.Add(pr.Name);
            }
            return result;
        }

        //возвращает ID проекта
        public int GetSelectedProject(string name)
        {
            int count = db.Projects.Where(p => p.Name == name).Count();
            if (count > 0)
            {
                Project q = db.Projects.Where(p => p.Name == name).ToList().First();
                return q.Id;
            }
            else
                return -1;
        }

        public Dictionary<int, string> GetFunctionsInProject(string projectName)
        {
            int pr_id = GetSelectedProject(projectName);
            Dictionary<int, string> results = new Dictionary<int, string>();
            if (pr_id < 0)
                return results;
            var query =
                from ff in db.Funcs
                from qs in db.QuickSearches
                where ff.SearchInfoId == qs.Id & qs.ProjectId == pr_id
                select new {Name = ff.Name, QSid = qs.Id };
            if (query.Count() < 0)
                return results;
            foreach (var q in query)
            {
                results.Add(q.QSid, q.Name);
            }
            return results;
        }

        public string GetFunctionBody(int qs_id)
        {
            var searches = db.QuickSearches.Where(i => i.Id == qs_id).ToList();
            if (searches.Count() < 0)
                return "";

            int pos = searches.FirstOrDefault().ByteNum.Value;
            string path = searches.FirstOrDefault().Path;
            StreamReader reader = new StreamReader(path);
            string line = "";
            string result = "";
            if (pos > 1)
            {
                for (int i = 0; i < pos; i++)
                    line = reader.ReadLine();
                //start read func
                //line = reader.ReadLine();
            }
            //считать шапку
            line = reader.ReadLine();
            result += line + '\n';
            while (line != "}")
            {
                line = reader.ReadLine();
                result += line+'\n';
            }
            reader.Close();
            return result;
        }

        public bool AddProject(string name)
        {
            FolderBrowserDialog folderdialog = new FolderBrowserDialog();
            Parser parser = new Parser();
            if (folderdialog.ShowDialog() == DialogResult.OK)
            {
                parser.Create_project(name, folderdialog.SelectedPath);
                DirectoryInfo dir_info = new DirectoryInfo(folderdialog.SelectedPath);
                FileInfo[] files = dir_info.GetFiles();

                foreach (FileInfo file in files)
                {
                    parser.Parsing_file(file.FullName);
                }
                return true;
            }
            else
                return false;
        }

        public string getFuncPath(int qs_id, ref int pos)
        {
            var q = db.QuickSearches.Where(i => i.Id == qs_id).ToList();
            if (q.Count < 0)
                return null;
            pos = q.ElementAt(0).ByteNum.Value;
            return q.ElementAt(0).Path;
        }

        public List<List<int>> getSituations (List<List<bool>> true_c, List<List<bool>> false_c)
        {
            List<List<int>> allGoals = new List<List<int>>();

            for (int i = 0; i < true_c.Count(); i++)
            {
                List<int> g = new List<int>();
                for (int j = 0; j < true_c[i].Count(); j++)
                {
                    if (true_c[i][j])
                        g.Add(1);
                    else
                        g.Add(0);
                }
                if (false_c[i].Count() > 0)
                    for (int j = 0; j < false_c[i].Count(); j++)
                        if (false_c[i][j])
                            g.Add(1);
                        else
                            g.Add(0);
                else
                    for (int j = 0; j < false_c[0].Count(); j++)
                        g.Add(-1);
                allGoals.Add(g);

            }
            return allGoals;
        }

        public void CreateSituations() { }
    }
}
