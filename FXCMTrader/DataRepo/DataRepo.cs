using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepo
{
    public class DataRepo
    {
        public static string DataDir = "";
        public static string Symbol = "";
        public SortedList<DateTime, Candle> M1;
        public SortedList<DateTime, Candle> H4;
        public SortedList<DateTime, Candle> D1;
        
        public DataRepo()
        {
            if(string.IsNullOrEmpty(DataDir) || string.IsNullOrEmpty(Symbol))
            {
                string[] paras = File.ReadAllLines(@"C:\FXCMHD.txt");
                Symbol = paras[2].Substring(0, 3);
                DataDir = paras[6] + @"Data\";
            }
            M1 = new SortedList<DateTime, Candle>();
            H4 = new SortedList<DateTime, Candle>();
            D1 = new SortedList<DateTime, Candle>();
        }
        public void Load(int[] years, string symbol)
        {
            foreach (int year in years)
            {
                LoadData(year, symbol, "M1");
                LoadData(year, symbol, "H4");
                LoadData(year, symbol, "D1");
            }
        }
        public void LoadData(int year, string symbol, string granularity)
        {
            string filename = DataDir + symbol + @"\" + year.ToString() + "_" + granularity + ".csv";
            switch (granularity)
            {
                case "M1":
                    LoadCSV(filename, ref M1);
                    break;
                case "H4":
                    LoadCSV(filename, ref H4);
                    break;
                case "D1":
                    LoadCSV(filename, ref D1);
                    break;
            }
        }
        public SortedList<DateTime, Candle> LoadCSV(string filename, ref SortedList<DateTime, Candle> list)
        {
            string[] lines = File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                string[] col = line.Split(',');
                Candle candle = new Candle();
                candle.openTime = col[0];
                candle.BidOpen = Convert.ToDouble(col[2]);
                candle.BidHigh = Convert.ToDouble(col[3]);
                candle.BidLow = Convert.ToDouble(col[4]);
                candle.BidClose = Convert.ToDouble(col[5]);
                candle.AskOpen = Convert.ToDouble(col[6]);
                candle.AskHigh = Convert.ToDouble(col[7]);
                candle.AskLow = Convert.ToDouble(col[8]);
                candle.AskClose = Convert.ToDouble(col[9]);
                candle.Volume = Convert.ToInt32(col[10]);
                list[DateTime.ParseExact(col[0],"yyyyMMdd HH:mm:ss", CultureInfo.CurrentCulture)] = candle;
            }
            return list;
        }
    }
}
