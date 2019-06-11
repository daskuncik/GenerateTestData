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
using GDT_EA.DB;


namespace GDT_EA
{
    
    public partial class Form1 : Form
    {
        FuncStore store;
        Coverage cov;
        DBFunctions DBf;
        Dictionary<int, string> funcDic;
        List<List<int>> allGoals;
        public Form1()
        {
            InitializeComponent();

            //store = new FuncStore("b.c", 0);

            //Coverage cov = new Coverage(store.getInstructions());
            //cov.startRecursion();
            
            store = null;
            cov = null;
            allGoals = null;
            panel2.Hide();
            DBf = new DBFunctions();
            funcDic = new Dictionary<int, string>();
            pBar1.Visible = false;
            //инициализация комбобокса ПРОЕКТЫ
            cmbProject.Items.AddRange(DBf.AllProjects().ToArray());
            cmbProject.Items.Add("Добавить");
            cmbProject.SelectedIndex = 0;

            dgvMCDC.RowHeadersWidth = 75;
            dgvMCDC.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTestData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //store = new FuncStore("b.c", 0);
            //Evolution ev = new Evolution("a");

            //List<int> g1 = new List<int>();
            //g1.Add(1);

            //List<int> g2 = new List<int>();
            //g2.Add(0);

            //List<OP> lst = new List<OP>();
            //OP op = new OP();
            //op.operation = 3;
            //op.value = 0;
            //op.goal = g1;
            //lst.Add(op);

            //OP op1 = new OP();
            //op1.operation = 3;
            //op1.value = 5;
            //op1.goal = g2;
            //lst.Add(op1);

            //ev.addConditions(lst);
            //int iteration = 0;
            //ev.setTestNum(0);
            //int result = ev.ResearchSolve(2,2,2, ref iteration);
            //int a = 4;

        }

        //добавить проект
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != String.Empty)
            {
                if (DBf.AddProject(textBox1.Text))
                {
                    cmbProject.Items.Add(textBox1.Text);
                    cmbProject.SelectedText = textBox1.Text;
                }
            }
            panel2.Visible = false;
            label1.Visible = true;
            cmbProject.Visible = true;
        }

        private void cmbProgect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProject.SelectedIndex == cmbProject.Items.IndexOf("Добавить"))
            {
                panel2.Visible = true;
                label1.Visible = false;
                cmbProject.Visible = false;
                return;
            }
            //инициализация комбобокса ФУНКЦИИ
            Dictionary<int, string> funcDic = DBf.GetFunctionsInProject(cmbProject.SelectedItem.ToString());
            if (funcDic.Count > 0)
            {
                foreach (var k in funcDic)
                {
                    cmbFunc.Items.Add(new { Key = k.Key, Value = k.Value });
                    cmbFunc.DisplayMember = "Value";
                }
                cmbFunc.SelectedIndex = 0;
            }
        }

        private void cmbFunc_SelectedIndexChanged(object sender, EventArgs e)
        {
            store = null;
            cov = null;
            var r = cmbFunc.SelectedItem;
            int k = (int)r.GetType().GetProperty("Key").GetValue(r, null);
            string text = DBf.GetFunctionBody(k);
            rtbFunc.Clear();
            rtbFunc.AppendText(text);
        }

        private void btnGetSituation_Click(object sender, EventArgs e)
        {
            if (store != null) store = null;
            CreateNewFuncStore();
            
            List<List<bool>> true_c = cov.get_true_coverage();
            List<List<bool>> false_c = cov.get_false_coverage();
            if (true_c.Count != false_c.Count)
                return;
            allGoals = DBf.getSituations(true_c, false_c);

            numTest.Minimum = 1;
            numTest.Maximum = allGoals[0].Count;

            List<string> names = cov.get_all_names();
            Fill_dgv_situation(allGoals, names);
        }

        private void btn_gen_Click(object sender, EventArgs e)
        {
            Fill_dgv_situation();
            List<Evolution> evolution_list = new List<Evolution>();
            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();
            Dictionary<string, List<OP>> dic = null;
            int dic_size = 0;
            int general_test_count = 0;
            int max = 0;
            pBar1.Visible = true;
            // Set Minimum to 1 to represent the first file being copied.
            pBar1.Minimum = 1;
            // Set the initial value of the ProgressBar.
            pBar1.Value = 1;
            // Set the Step property to a value of 1 to represent each file being copied.
            pBar1.Step = 1;
            if (!rBtnGraph.Checked)
            {

                if (store == null || (store == null && !rBtnGraph.Checked && cov == null))
                    return;

                dic = cov.getOperationVariables();
                dic_size = dic.Count;

                if (allGoals == null)
                    return;
                if (!rBtnOneSit.Checked)
                    general_test_count = allGoals[0].Count;
                else
                    general_test_count = 1;
                // Set Maximum to the total number of files to copy.
                pBar1.Maximum = dic_size * dic.ElementAt(0).Value.Count * general_test_count + general_test_count * dic_size;
                //для каждой переменной в словаре
                foreach (var dic_el in dic)
                {
                    //для каждой операции над переменной (типа >, <...)
                    for (int i = 0; i < dic_el.Value.Count; i++)
                    {
                        int id = dic_el.Value[i].id;
                        dic_el.Value[i].goal = new List<int>();
                            //для каждой операции над переменной: для каждого теста
                        for (int j = 0; j < general_test_count; j++)
                        {
                            int g = allGoals[id][j];
                            dic_el.Value[i].goal.Add(g);
                            pBar1.PerformStep();
                        }
                    }
                }
            }
            else
            {
                if (store == null)
                    CreateNewFuncStore();
                GraphForm formG = new GraphForm(store.getInstructions());
                formG.ShowDialog();
                List<Classes.FunctionItems.IItem> path = formG.result;
                if (path == null)
                    return;
                if (store == null)
                    CreateNewFuncStore(path);

                List<List<List<bool>>> goalss = cov.GetGoalForItem(path);
                List<string> f = new List<string>();
                foreach (var it in path)
                    f.Add(it.getLine());
                //то, что нужно отобразить в таблице MC/DC
                List<List<int>> res_goals = new List<List<int>>();
                dic = cov.CreateSituationForGraph(goalss, ref res_goals, f);
                dic_size = dic.Count;
                max = 0;
                foreach (var c in goalss)
                    if (c[0].Count > max)
                        max = c[0].Count;

                general_test_count = max;
                pBar1.Maximum =  general_test_count * dic_size;
                Fill_dgv_situation(res_goals, cov.get_all_names());
            }
            for (int i = 0; i < dic_size; i++)
            {
                Evolution ev = new Evolution(dic.ElementAt(i).Key);
                ev.addConditions(dic.ElementAt(i).Value);
                List<int> values = new List<int>();

                if (!rBtnOneSit.Checked)
                    for (int k = 0; k < general_test_count; k++)
                    {
                        ev.setTestNum(k);
                        values.Add(ev.Solve());
                        pBar1.PerformStep();
                    }
                else
                {
                    ev.setTestNum(Convert.ToInt32(numTest.Value) - 1);
                    values.Add(ev.Solve());
                    pBar1.PerformStep();
                }
                result.Add(dic.ElementAt(i).Key, values);
            }
                max = 0;
                foreach (var t in dic)
                {
                    if (t.Value[0].goal.Count > max)
                        max = t.Value.Count;
                }
                //заполнить таблицу результатов
                dgvTestData.ColumnCount = dic.Count;
                dgvTestData.RowCount = general_test_count;
                //номера тестов:
                for (int k = 0; k < dgvTestData.RowCount; k++)
                    dgvTestData.Rows[k].HeaderCell.Value = (k + 1).ToString();

                //////////// i=0;
                for (int i = 0; i < dgvTestData.ColumnCount; i++)
                {
                    dgvTestData.Columns[i].HeaderText = result.ElementAt(i).Key;
                    for (int j=0; j < dgvTestData.RowCount; j++)
                    {
                    int g = result.ElementAt(i).Value.ElementAt(j);
                    if (g != Int32.MinValue)
                        dgvTestData.Rows[j].Cells[i].Value = g;
                    else
                        dgvTestData.Rows[j].Cells[i].Value = "Нет";
                    pBar1.PerformStep();
                    }
                }
            pBar1.Visible = false;

            for (int i = 0; i < allGoals.Count; i++)
                allGoals[i].Clear();
            allGoals.Clear();
        }

        private void CreateNewFuncStore(List<Classes.FunctionItems.IItem> lst = null)
        {
            if (rtbFunc.Text == String.Empty)
            {
                int pos = -1;
                string path = DBf.getFuncPath((int)cmbFunc.SelectedValue, ref pos);
                store = new FuncStore(path, pos);
            }
            else
                store = new FuncStore(rtbFunc.Text);

            cov = new Coverage(store.getInstructions());
            cov.startRecursion(lst);
        }

        private void Fill_dgv_situation(List<List<int>> allGoals=null, List<string> names=null)
        {
            dgvMCDC.Rows.Clear();
            dgvTestData.Rows.Clear();
            if (allGoals == null || names == null)
                return;
            dgvMCDC.RowCount = allGoals.Count;
            dgvMCDC.ColumnCount = allGoals[0].Count; //true_c[0].Count() + false_c[0].Count();

            //номера тестов:
            for (int k = 0; k < dgvMCDC.ColumnCount; k++)
                dgvMCDC.Columns[k].HeaderText = (k + 1).ToString();


            //////////// i=0;
            for (int i = 0; i < dgvMCDC.RowCount; i++)
            {
                dgvMCDC.Rows[i].HeaderCell.Value = names[i];
                for (int j = 0; j < dgvMCDC.ColumnCount; j++)
                {
                    switch (allGoals[i][j])
                    {
                        case 1:
                            dgvMCDC.Rows[i].Cells[j].Value = "true";
                            break;
                        case 0:
                            dgvMCDC.Rows[i].Cells[j].Value = "false";
                            break;
                        case -1:
                            dgvMCDC.Rows[i].Cells[j].Value = "x";
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
