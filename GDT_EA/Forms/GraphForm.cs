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
        public GraphForm()
        {
            graph = new ControlGraph();

        }

        public GraphForm(List<Classes.FunctionItems.IItem> instructions)
        {
            graph = new ControlGraph();
            graph.createControlGraph(instructions);
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = graph.GetViewer();
            panel1.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            panel1.Controls.Add(viewer);
            panel1.ResumeLayout();
            viewer.MouseClick += Viewer_MouseClick;
        }

        public void setText(string text)
        {
            rtb.AppendText(text);
        }

        private void Viewer_MouseClick(object sender, MouseEventArgs e)
        {
            //(panel1.Controls[0] as Microsoft.Msagl.GraphViewerGdi.GViewer).SelectedObject
            if (graph.GetViewer().SelectedObject is Node)
            {
                Node selected = (graph.GetViewer().SelectedObject as Node);
                if (selected.Attr.Color != graph.selectedColor)
                {
                    //выделить цветом узел и текст
                    selected.Attr.Color = graph.selectedColor;
                    searchStrInTExtBox((selected.UserData as Classes.FunctionItems.IItem).getLine(), true);

                }

            }
        }

        //найти и выделить цвето строку в текстбоксе
        private void searchStrInTExtBox(string str, bool select)
        {
            int index = rtb.Text.IndexOf(str);
            if (index > 0)
            {
                rtb.SelectionStart = index;
                rtb.SelectionLength = str.Length;
                rtb.SelectionColor = select ? selectedColor : nonSelectedColor;
            }
        }
    }
}
