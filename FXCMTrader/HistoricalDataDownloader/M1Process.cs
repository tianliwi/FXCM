using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRepo;
using System.IO;

namespace HistoricalDataDownloader
{
    public class M1Process
    {
        private string[] H4Slots =
        {
            "01:00:00", "05:00:00", "09:00:00",
            "13:00:00", "17:00:00", "21:00:00"
        };

        private List<Candle> H4;
        private List<Candle> D1;
        private SortedList<DateTime, Candle> M1;
        private int year;
        private string symbol;
        private DataRepo.DataRepo data;

        public M1Process(int year, string symbol)
        {
            this.year = year;
            this.symbol = symbol;
            data = new DataRepo.DataRepo();
            M1 = new SortedList<DateTime, Candle>();
            data.LoadCSV(data.getFileName(year, symbol, "M1"), ref M1);
            H4 = new List<Candle>();
            D1 = new List<Candle>();
        }
        public void GenerateH4()
        {
            Candle curH4Candle = new Candle();
            clearCandle(curH4Candle);
            foreach(var entry in M1)
            {
                DateTime curTime = entry.Key;
                Candle curM1Candle = entry.Value;
                if(curH4Candle.AskOpen < 0)
                {
                    //Console.WriteLine(curTime);
                    curH4Candle.AskOpen = curM1Candle.AskOpen;
                    curH4Candle.BidOpen = curM1Candle.BidOpen;
                    curH4Candle.openTime = curM1Candle.openTime.Substring(0,9) + H4Slots[GetH4Slot(curTime)];
                    //Console.WriteLine(GetH4Slot(curTime));
                    //Console.WriteLine(curH4Candle.openTime);
                }
                curH4Candle.AskHigh = Math.Max(curH4Candle.AskHigh, curM1Candle.AskHigh);
                curH4Candle.BidHigh = Math.Max(curH4Candle.BidHigh, curM1Candle.BidHigh);
                curH4Candle.AskLow = Math.Min(curH4Candle.AskLow, curM1Candle.AskLow);
                curH4Candle.BidLow = Math.Min(curH4Candle.BidLow, curM1Candle.BidLow);

                if(GetH4Slot(curTime) != GetH4Slot(curTime.AddMinutes(1)))
                {
                    //Console.WriteLine(curTime);
                    Candle c = new Candle();
                    c.openTime = curH4Candle.openTime;
                    c.BidOpen = curH4Candle.BidOpen;
                    c.BidHigh = curH4Candle.BidHigh;
                    c.BidLow = curH4Candle.BidLow;
                    c.BidClose = curM1Candle.BidClose;
                    c.AskOpen = curH4Candle.AskOpen;
                    c.AskHigh = curH4Candle.AskHigh;
                    c.AskLow = curH4Candle.AskLow;
                    c.AskClose = curM1Candle.AskClose;
                    H4.Add(c);
                    clearCandle(curH4Candle);
                }
            }
            File.WriteAllLines(data.getFileName(year, symbol, "H4"), H4.Select(i => i.ToString()));
            Console.WriteLine("H4 data generated.");
        }
        public void clearCandle(Candle candle)
        {
            candle.openTime = "";
            candle.closeTime = "";
            candle.BidOpen = -1;
            candle.BidHigh = -1;
            candle.BidLow = 1000;
            candle.BidClose = -1;
            candle.AskOpen = -1;
            candle.AskHigh = -1;
            candle.AskLow = 1000;
            candle.AskClose = -1;
        }
        public int GetH4Slot(DateTime dt)
        {
            string t = dt.TimeOfDay.ToString();
            int c = string.Compare(t, H4Slots[0]);
            if (c < 0) return 5;
            for (int k = 5; k >= 0; k--)
            {
                int cmp = string.Compare(t, H4Slots[k]);
                if (cmp >= 0) return k;
            }
            return 0;
        }
    }
}
