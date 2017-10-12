using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BackTest;
using System.Globalization;

namespace DrawPNL
{
    public partial class FXCMBT : Form
    {
        public FXCMBT()
        {
            InitializeComponent();
            
            Load += new EventHandler(LoadBackTest);
        }

        private void LoadCharts(object sender, System.EventArgs e)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            ChartArea ca1 = new ChartArea("index");
            ChartArea ca2 = new ChartArea("pnl");
            chart1.ChartAreas.Add(ca1);
            chart1.ChartAreas.Add(ca2);
        }

        private void LoadBackTest(object sender, System.EventArgs e)
        {
            double k1 = 0.18;
            double k2 = 0.28;
            DataRepo.DataRepo data = new DataRepo.DataRepo();
            data.Load(new int[] {2012, 2013, 2014, 2015, 2016, 2017 }, DataRepo.DataRepo.Symbol);
            LongStrategy longStra = new LongStrategy(data);
            Tuple<int, int, double> res = longStra.Start(k1, k2, true);
            chart1.Titles.Add("Win: " + res.Item1.ToString() + ", " + 
                "Loss: " + res.Item2.ToString() + ", " + 
                "Profit: " + res.Item3.ToString("C", CultureInfo.CurrentCulture) + ", " + longStra.pnlList.Count);
            var series = chart1.Series.ElementAt(0);
            series.ChartType = SeriesChartType.Line;
            List<int> x = new List<int>();
            List<double> y = new List<double>();
            for(int k = 0; k < longStra.pnlList.Count; k++)
            {
                x.Add(k);
                y.Add(longStra.pnlList.Values.ElementAt(k));

            }
            
            series.Points.DataBindXY(x, y);

            ChartArea ca1 = chart1.ChartAreas[0];

            Axis ax1 = ca1.AxisX;

            ax1.ScaleView.Zoomable = true;
            ca1.CursorX.IsUserSelectionEnabled = true;
        }
    }
}
