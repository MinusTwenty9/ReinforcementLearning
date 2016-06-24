using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ReinforcementLearning.QLearning.FunctionApproximation;

namespace ReinforcementLearning.QLearning.Visualizer
{
    public partial class Graph : Form
    {
        List<List<double>> data;

        public Graph()
        {
            data = new List<List<double>>();
            data.Add( new List<double>());
            InitializeComponent();
        }

        private void weightsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data = new List<List<double>>();
            chart = new Chart();
            chart.ChartAreas.Add("w");

            chart.ChartAreas["w"].AxisX.Minimum = 0;
            chart.ChartAreas["w"].AxisX.Maximum = 99999999;
            chart.ChartAreas["w"].AxisX.Interval = 1;
            chart.ChartAreas["w"].AxisX.MajorGrid.LineColor = Color.White;
            chart.ChartAreas["w"].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            chart.ChartAreas["w"].AxisY.Minimum = -10;
            chart.ChartAreas["w"].AxisY.Maximum = 10;
            chart.ChartAreas["w"].AxisY.Interval = 0.1;
            chart.ChartAreas["w"].AxisY.MajorGrid.LineColor = Color.White;
            chart.ChartAreas["w"].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            chart.ChartAreas["w"].BackColor = Color.Black;

            chart.Series.Add("Weights");
            chart.Series["Weights"].ChartType = SeriesChartType.Line;

            chart.Series["Weights"].Color = Color.LightGray;
            chart.Series["Weights"].BorderWidth = 3;


        }

        public void Add_Data(int data_index, double[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                this.data[data_index].Add(data[i]);
                add_to_chart(data_index, data[i]);
            }
        }

        public void Add_Data(int data_index, double data)
        {
            this.data[data_index].Add(data);
            add_to_chart(data_index, data);
        }

        private void add_to_chart(int data_index, double data)
        {
            chart.Series[data_index].Points.AddXY(this.data[data_index].Count, data);
        }
    }
}
