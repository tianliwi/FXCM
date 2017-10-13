using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataRepo;
using System.Globalization;

namespace Chart
{
    public partial class Chart : Form
    {
        public Chart()
        {
            InitializeComponent();
            Load += new EventHandler(GetData);
        }
        private void GetData(object sender, System.EventArgs e)
        {
            DataRepo.DataRepo dataRepo = new DataRepo.DataRepo();
            dataRepo.Load(new int[] {2014, 2015, 2016, 2017 }, DataRepo.DataRepo.Symbol);
            
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("Open", typeof(double));
            dt.Columns.Add("High", typeof(double));
            dt.Columns.Add("Low", typeof(double));
            dt.Columns.Add("Close", typeof(double));
            double yMin = 2;
            double yMax = 0;
            int lineNum = 0;
            foreach (var line in dataRepo.H4)
            {
                DataRow row = dt.NewRow();
                row["ID"] = lineNum++;
                row["Date"] = DateTime.ParseExact(line.Value.openTime, "yyyyMMdd HH:mm:ss", CultureInfo.CurrentCulture);
                row["Open"] = Convert.ToDouble(line.Value.BidOpen);
                row["High"] = Convert.ToDouble(line.Value.BidHigh);
                row["Low"] = Convert.ToDouble(line.Value.BidLow);
                row["Close"] = Convert.ToDouble(line.Value.BidClose);
                if ((double)row["Low"] < yMin) yMin = (double)row["Low"];
                if ((double)row["High"] > yMax) yMax = (double)row["High"];
                dt.Rows.Add(row);
            }
            chart1.ChartAreas[0].AxisY.Minimum = yMin;
            chart1.ChartAreas[0].AxisY.Maximum = yMax;
            chart1.ChartAreas[0].AxisX.Interval = 60;
            chart1.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chart1.DataSource = dt;
            chart1.Series["M1"].XValueMember = "Date";
            chart1.Series["M1"].YValueMembers = "High, Low, Open, Close";
            chart1.Series["M1"].CustomProperties = "PriceDownColor=Red, PriceUpColor=Green";
            chart1.Series["M1"]["ShowOpenClose"] = "Both";
            chart1.DataManipulator.IsStartFromFirst = true;
            chart1.DataBind();
        }
    }
}
