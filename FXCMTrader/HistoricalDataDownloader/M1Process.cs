using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRepo;

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
        private int year;
        private string symbol;

        public M1Process(int year, string symbol)
        {
            this.year = year;
            this.symbol = symbol;
            H4 = new List<Candle>();
            D1 = new List<Candle>();
        }
    }
}
