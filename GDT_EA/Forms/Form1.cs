using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDT_EA.Classes;
using GDT_EA.Evolutions;
using GDT_EA.Forms;

namespace GDT_EA
{
    
    public partial class Form1 : Form
    {
        FuncStore store;
        
        public Form1()
        {
            InitializeComponent();
            //FuncStore store = new FuncStore("a.c", 0);
            store = new FuncStore("b.c", 0);
            Coverage cov = new Coverage(store.getInstructions());
            cov.startRecursion();
            Dictionary<string, List<OP>> dic = cov.getOperationVariables();
            List<List<bool>> true_c = cov.get_true_coverage();
            List<List<bool>> false_c = cov.get_false_coverage();
            List<List<int>> allGoals = new List<List<int>>();
            
            if (true_c.Count != false_c.Count)
                return;

            dgvMCDC.RowCount = true_c.Count() + 1;
            dgvMCDC.ColumnCount = true_c[0].Count() + false_c[0].Count();
            //номера тестов:
            for (int k = 0; k < dgvMCDC.ColumnCount; k++)
            {
                dgvMCDC.Columns[k].HeaderText = (k + 1).ToString();
            }
            List<string> names = cov.get_all_names();
            //////////// i=0;
            for (int i = 0; i < true_c.Count(); i++)
            {
                dgvMCDC.Rows[i].HeaderCell.Value = names[i];
                List<int> g = new List<int>();
                for (int j = 0; j < true_c[i].Count(); j++)
                {
                    if (true_c[i][j])
                    {
                        dgvMCDC.Rows[i].Cells[j].Value = "true";
                        g.Add(1);
                    }
                    else
                    {
                        dgvMCDC.Rows[i].Cells[j].Value = "false";
                        g.Add(0);
                    }
                    //dgvMCDC.Columns[j].HeaderCell.Value = (1+j).ToString();
                }
                if (false_c[i].Count() > 0)
                    for (int j = 0; j < false_c[i].Count(); j++)
                        if (false_c[i][j])
                        {
                            dgvMCDC.Rows[i].Cells[true_c[i].Count() + j].Value = "true";
                            g.Add(1);
                        }
                        else
                        {
                            dgvMCDC.Rows[i].Cells[true_c[i].Count() + j].Value = "false";
                            g.Add(0);
                        }
                else
                {
                    for (int j = 0; j < false_c[0].Count(); j++)
                    {
                        dgvMCDC.Rows[i].Cells[true_c[i].Count() + j].Value = "x";
                        g.Add(-1);
                    }
                }
                allGoals.Add(g);
                
            }

            int dic_size = dic.Count;
            List<Evolution> evolution_list = new List<Evolution>();
            Dictionary<string, int> result = new Dictionary<string, int>();
            int test_count = allGoals[0].Count;
            int count_Second_Variant = true_c[0].Count + false_c[0].Count;
            //для каждой переменной в словаре
            foreach (var dic_el in dic)
            {
                //для каждой операции над переменной (типа >, <...)
                for (int i = 0; i < dic_el.Value.Count; i++)
                {
                    List<bool> goals = new List<bool>();
                    //для каждой операции над переменной: для каждого теста
                    int id = dic_el.Value[i].id;
                    for (int j = 0; j < test_count; j++)
                        dic_el.Value[i].goal.Add(allGoals[id][j]);
                }
            }

            for (int i=0; i<dic_size; i++)
            {
                Evolution ev = new Evolution(dic.ElementAt(i).Key);
                ev.addConditions(dic.ElementAt(i).Value);
                int value = ev.Solve();
                result.Add(dic.ElementAt(i).Key, value);
            }

            
            //Evolution ev = new Evolution();
            //ev.addCondition((int)ConstClass.OperationType.More, 5);
            //ev.addCondition((int)ConstClass.OperationType.Less, 9);
            //double value = ev.Solve();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GraphForm formG = new GraphForm( store.getInstructions());
            formG.setText(store.getFuncText());

        }
    }
}
