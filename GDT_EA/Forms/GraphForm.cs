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
//using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;

namespace GDT_EA.Forms
{
    public partial class GraphForm : Form
    {
        ControlGraph graph;
        System.Drawing.Color selectedColor = System.Drawing.Color.Red;
        System.Drawing.Color nonSelectedColor = System.Drawing.Color.Black;
        public List<Classes.FunctionItems.IItem> result;

        public GraphForm()
        {
            
            graph = new ControlGraph();
            result = null;
        }

        public GraphForm(List<Classes.FunctionItems.IItem> instructions)
        {
            InitializeComponent();
            graph = new ControlGraph();
            graph.createControlGraph(instructions);
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = graph.GetViewer();
            viewer.Dock = DockStyle.Fill;
            this.Controls.Add(viewer);
            viewer.Show();

            btn.BringToFront();
            btn.Location = new Point(0, 30);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            int a = 5;
            result = graph.getAllSelected();
        }
    }
}
