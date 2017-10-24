using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkRouting
{
    public partial class ScrollMessageBox : Form
    {
        public ScrollMessageBox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void setText(string text)
        {
            textBox1.Text = text;
        }

        public void appendText(string text)
        {
            textBox1.AppendText(text);
            textBox1.AppendText(Environment.NewLine);
        }
    }
}
