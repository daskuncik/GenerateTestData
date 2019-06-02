using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl;
using GDT_EA.Classes.FunctionItems;
using System.Windows.Forms;

namespace GDT_EA.Classes
{
    //const Color selectedColor = Color.Red;
    //const Color nonSelectedColor = Color.Black;
    class ControlGraph
    {
        private Graph gr;
        private Microsoft.Msagl.GraphViewerGdi.GViewer viewer;
        private int cur_i = -1;
        public Color selectedColor;
        public Color nonSelectedColor;
        public ControlGraph()
        {
            viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();

            gr = new Graph("graph");
            selectedColor = Color.Red;
            nonSelectedColor = Color.Black;
        }

        public Microsoft.Msagl.GraphViewerGdi.GViewer GetViewer() { return viewer; }

        public void createControlGraph(List<IItem> instructions)
        {
            Node prev = new Node("Start");
            prev.Label.Width = 40;
            prev.Label.Height = 40;
            prev.Attr.Shape = Shape.Ellipse;

            Node cur;
            List<Node> local_ends = new List<Node>()
            { prev };
            foreach (var item in instructions)
            {
                cur = new Node((cur_i++).ToString())
                {
                    UserData = item
                };
                cur.Label.Width = 40;
                cur.Label.Height = 40;
                cur.Attr.Shape = Shape.Ellipse;
                foreach (var pr in local_ends)
                {
                    Edge e = new Edge(pr, cur, ConnectionToGraph.Connected);
                }
                gr.AddNode(cur);
                local_ends.Clear();
                if (item.IsComposite())
                    local_ends = createItem(item, cur);
                else
                    local_ends.Add(cur);

            }

            cur = new Node("End");
            foreach (var pr in local_ends)
            {
                Edge e = new Edge(pr, cur, ConnectionToGraph.Connected);
            }
            gr.AddNode(cur);

            gr.LayoutAlgorithmSettings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings();
            viewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.SugiyamaScheme;
            viewer.Graph = gr;

            
        }

        //возвращает последние элементы ветки
        private List<Node> createItem(IItem parent_item, Node parent_node)
        {
            List<Node> result = new List<Node>();
            List<Node> local_ends = new List<Node>();
            int count = parent_item.getChildCount();
            for (int i=0; i<count; i++)
            {
                Node prev = parent_node;
                Node cur = null;
                OperationSet inst = parent_item.getChild(i);
                for (int j=0; j<inst.Count(); j++)
                {
                    IItem item = inst.getItem(j);
                    cur = new Node((cur_i++).ToString())
                    {
                        UserData = item
                    };
                    cur.Label.Width = 40;
                    cur.Label.Height = 40;
                    cur.Attr.Shape = Shape.Ellipse;
                    foreach (var pr in local_ends)
                    {
                        Edge e = new Edge(pr, cur, ConnectionToGraph.Connected);
                    }
                    gr.AddNode(cur);
                    local_ends.Clear();
                    if (item.IsComposite())
                        local_ends = createItem(item, cur);
                    else
                        local_ends.Add(cur);
                }
                result.AddRange(local_ends);
            }
            return result;
        }

        private void Viewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (viewer.SelectedObject is Node)
            {
                Node selected = (viewer.SelectedObject as Node);
                if (selected.Attr.Color != selectedColor)
                {
                    //выделить цветом узел и текст
                    selected.Attr.Color = selectedColor;

                }
            }
        }

        

    }
}
