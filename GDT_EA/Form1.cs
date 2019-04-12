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


namespace GDT_EA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FuncStore store = new FuncStore("a.c", 0);
        }
    }
}
