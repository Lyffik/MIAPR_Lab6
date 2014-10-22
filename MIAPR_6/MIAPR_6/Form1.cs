using System;
using System.Windows.Forms;

namespace MIAPR_6
{
    public partial class Form1 : Form
    {
        private double[,] distances;

        public Form1()
        {
            InitializeComponent();
        }

        private double[,] RandomGrid(int size)
        {
            dataGridView.ColumnCount = size;
            dataGridView.RowCount = size;

            var result = new double[size, size];
            var rnd = new Random();
            for (int i = 0; i < size; i++)
            {
                result[i, i] = 0;
            }
            for (int i = 1; i < size; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    result[i, j] = rnd.Next(30) + 1;
                    result[j, i] = result[i, j];
                }
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    dataGridView[i, j].Value = result[i, j];
                }
            }
            if (radioBtnMaximum.Checked)
            {
                for (int i = 1; i < (int) numericUpDownGridSize.Value; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        result[i, j] = 1/result[i, j];
                        result[j, i] = result[i, j];
                    }
                }
            }
            return result;
        }

        private void btn_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            distances = RandomGrid((int) numericUpDownGridSize.Value);
            var hierarchical = new HierarchicalGrouping(distances, (int) numericUpDownGridSize.Value);
            hierarchical.FindGroups();
            hierarchical.Draw(chart1);
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView[e.ColumnIndex, e.RowIndex].Value != dataGridView[e.RowIndex, e.ColumnIndex].Value)
            {
                dataGridView[e.RowIndex, e.ColumnIndex].Value = dataGridView[e.ColumnIndex, e.RowIndex].Value;
            }
        }
    }
}