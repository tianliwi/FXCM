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
        }
        public void Load(int year, string symbol)
        {
            LoadM1(year, symbol);
            LoadH4(year, symbol);
            LoadD1(year, symbol);
        }
        public void LoadM1(int year, string symbol)
        {
            M1 = LoadData(year, symbol, "M1");
        }
        public void LoadH4(int year, string symbol)
        {
            H4 = LoadData(year, symbol, "H4");
        }
        public void LoadD1(int year, string symbol)
        {
            D1 = LoadData(year, symbol, "D1");
        }
        public SortedList<DateTime, Candle> LoadData(int year, string symbol, string granularity)
        {
            string filename = DataDir + symbol + @"\" + year.ToString() + "_" + granularity + ".csv";
            return LoadCSV(filename);
        }
        public SortedList<DateTime, Candle> LoadCSV(string filename)
        {
            int offset = 1;
            if (filename.Substring(filename.Length - 6) == "M1.csv") offset = 0;
            SortedList<DateTime, Candle> list = new SortedList<DateTime, Candle>();
            string[] lines = File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                string[] col = line.Split(',');
                Candle candle = new Candle();
                candle.openTime = col[0];
                candle.BidOpen = Convert.ToDouble(col[1 + offset]);
                candle.BidHigh = Convert.ToDouble(col[2 + offset]);
                candle.BidLow = Convert.ToDouble(col[3 + offset]);
                candle.BidClose = Convert.ToDouble(col[4 + offset]);
                candle.AskOpen = Convert.ToDouble(col[5 + offset]);
                candle.AskHigh = Convert.ToDouble(col[6 + offset]);
                candle.AskLow = Convert.ToDouble(col[7 + offset]);
                candle.AskClose = Convert.ToDouble(col[8 + offset]);
                candle.Volume = Convert.ToInt32(col[9 + offset]);
                list[DateTime.ParseExact(col[0],"yyyyMMdd HH:mm:ss", CultureInfo.CurrentCulture)] = candle;
            }
            return list;
        }
    }
}
