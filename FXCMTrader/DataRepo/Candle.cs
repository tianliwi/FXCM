using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepo
{
    public class Candle
    {
        public string openTime;
        public string closeTime;
        public double BidOpen;
        public double BidHigh;
        public double BidLow;
        public double BidClose;
        public double AskOpen;
        public double AskHigh;
        public double AskLow;
        public double AskClose;
        public double Volume;
        
        public override string ToString()
        {
            return openTime +
                "," + closeTime +
                "," + BidOpen +
                "," + BidHigh +
                "," + BidLow +
                "," + BidClose +
                "," + AskOpen +
                "," + AskHigh +
                "," + AskLow +
                "," + AskClose +
                "," + Volume;
        }
    }
}
