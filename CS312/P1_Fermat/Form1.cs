using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P1_Fermat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.ActiveControl = m_tbInput;
            m_tbInput.Focus();
        }

        private void On_SolveClick(object sender, EventArgs e)
        {
            Primality(Convert.ToInt32(m_tbInput.Text), Convert.ToInt32(m_tbK.Text));
        }

        private void Primality(int _num, int _numTests)
        {
            Random rand = new Random();
            List<int> baseNums = new List<int>();
            int baseNum;
            double probability = 1.0;
            bool prime = true;
            for (int i = 0; i < _numTests; i++)
            {
                baseNum = rand.Next(1, _num / 2);
                while (baseNums.Contains(baseNum))
                {
                    baseNum = rand.Next(1, _num / 2);
                }
                baseNums.Add(baseNum);

                if (modEx(baseNum, _num-1, _num) != 1)
                {

                    m_tbOutput.Text = "No";
                    prime = false;
                    break;
                }
                probability /= 2;
            }

            if (prime)
            {
                m_tbOutput.Text = "Yes with probability: " + probability;
            }
            else
            {
                m_tbOutput.Text = "No";
            }
        }

        private int modEx(int _baseNum, int _exp, int _mod)
        {
            if (_exp == 0) return 1;
            int z = modEx(_baseNum, _exp / 2, _mod);
            if (_exp % 2 == 0)
            {
                return (z * z) % _mod;
            }
            else
            {
                return (_baseNum * z * z) % _mod;
            }
            
        }

        private void On_WindowKeyDown(object sender, KeyEventArgs e)
        {
            int num, k;
            if (e.KeyCode == Keys.Enter && int.TryParse(m_tbInput.Text, out num) && int.TryParse(m_tbK.Text, out k))
            {
                Primality(num, k);
            }
        }
    }
}
