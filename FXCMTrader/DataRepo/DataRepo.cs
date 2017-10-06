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
        public static string DataDir = @"E:\GitHub\FXCM\Data\";
        public SortedList<DateTime, Candle> M1;
        public SortedList<DateTime, Candle> H4;
        public SortedList<DateTime, Candle> D1;
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
            SortedList<DateTime, Candle> list = new SortedList<DateTime, Candle>();
            string[] lines = File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                string[] col = line.Split(',');
                Candle candle = new Candle();
                candle.openTime = col[0];
                candle.BidOpen = Convert.ToDouble(col[1]);
                candle.BidHigh = Convert.ToDouble(col[2]);
                candle.BidLow = Convert.ToDouble(col[3]);
                candle.BidClose = Convert.ToDouble(col[4]);
                candle.AskOpen = Convert.ToDouble(col[5]);
                candle.AskHigh = Convert.ToDouble(col[6]);
                candle.AskLow = Convert.ToDouble(col[7]);
                candle.AskClose = Convert.ToDouble(col[8]);
                candle.Volume = Convert.ToInt32(col[9]);
                list[DateTime.ParseExact(col[0],"yyyyMMdd HH:mm:ss", CultureInfo.CurrentCulture)] = candle;
            }
            return list;
        }
    }
}
