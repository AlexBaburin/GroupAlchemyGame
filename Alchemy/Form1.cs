using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alchemy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
        private void button1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            button1.DoDragDrop(button1.Text, DragDropEffects.Copy |
               DragDropEffects.Move);
        }
    }
}
