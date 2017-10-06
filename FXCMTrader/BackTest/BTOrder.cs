using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTest
{
    public enum BTOrderType
    {
        Pending = 0,
        Open = 1,
        Closed = 2,
    }
    public enum BTOrderDir
    {
        Long = 0,
        Short = 0,
    }
    public class BTOrder
    {
        public int ID { get; set; }
        public string symbol { get; set; }
        public int size { get; set; }
        public BTOrderType status { get; set; }
        public BTOrderDir dir { get; set; }
        public double openPrice { get; set; }
        public double closePrice { get; set; }
        public double takeProfit { get; set; }
        public double stopLoss { get; set; }
        public DateTime pendTime { get; set; }
        public DateTime openTime { get; set; }
        public DateTime closeTime { get; set; }
        public DateTime goodTill { get; set; }
        public double pnl { get; set; }
        public override string ToString()
        {
            return ID + "," + 
                size + "," +
                dir.ToString() + "," +
                pendTime + "," +
                openTime + "," +
                openPrice.ToString() + "," +
                closeTime + "," +
                closePrice.ToString() + "," +
                pnl.ToString();
        }
    }
}
