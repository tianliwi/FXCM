using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using DataRepo;

namespace BackTest
{
    public class OldEngine
    {
        // run backtest
        protected ConcurrentQueue<string> cq = new ConcurrentQueue<string>();

        public DateTime tickMinDate;
        public DateTime tickMaxDate;
        public DateTime tickCur;

        public string[] tickFiles;

        private DataRepo.DataRepo data;

        private LinkedList<Candle> barM1;
        private LinkedList<Candle> barH4;

        public OldEngine()
        {
            data = new DataRepo.DataRepo();
            barM1 = new LinkedList<Candle>();
            barH4 = new LinkedList<Candle>();
            LoadTicks();
        }
        public void LoadTicks()
        {
            Console.WriteLine("Starting loading tick data...");
            tickMinDate = data.M1.Keys.Min();
            tickMaxDate = data.M1.Keys.Max();
            tickCur = tickMinDate.AddMinutes(-1);
        }
        public void Start()
        {
            double R, open = 0, tp = 0, sl = 0, R2;
            bool orderOpened = false;
            bool orderClosed = true;
            double profit = 0;
            double loss = 0;
            int winNum = 0;
            int loseNum = 0;
            int h4Cnt = 0;
   
            while (tickCur < tickMaxDate)
            {
                Play();
                if (data.H4.ContainsKey(tickCur))
                {
                    barH4.AddFirst(data.H4[tickCur]);
                    if (h4Cnt++ >= 19)
                    {
                        R = getLastHigh(12) - getLastLow(12);
                        R2 = getLastHigh(18) - getLastLow(18);
                        if (true)//(R2 >= 0.013 && R2 <= 0.019)
                        {
                            open = barH4.First.Value.AskOpen - R * 0.12;
                            sl = -0.005;
                            tp = R * 0.32;
                            orderOpened = false;
                            orderClosed = false;
                            //Console.WriteLine("At {0}, put order with limit price {1}, stoploss {2}, take profit {3}.", tickCur, open, sl, tp);
                        }
                    }
                }
                if (data.M1.ContainsKey(tickCur))
                {
                    barM1.AddFirst(data.M1[tickCur]);
                    if (orderClosed) continue;
                    Candle cur = barM1.First.Value;
                    if (!orderOpened)
                    { 
                        if ((cur.BidLow < open) && cur.BidHigh > open)
                        {
                            orderOpened = true;
                            //Console.WriteLine("Order placed at {0}.", tickCur);
                        }
                    }
                    if (orderOpened)
                    {
                        if (cur.BidLow < open + tp && cur.BidHigh > open + tp)
                        {
                            profit += tp;
                            //Console.WriteLine("Take profit at {0}.", tickCur);
                            orderClosed = true;
                            winNum++;
                        }
                        if (cur.BidLow < open + sl && cur.BidHigh > open + sl)
                        {
                            loss += sl;
                            //Console.WriteLine("Stop loss at {0}.", tickCur);
                            orderClosed = true;
                            loseNum++;
                        }
                    }
                }
            }
            Console.WriteLine("win {0}, {1}\nlose {2},{3}", winNum, profit, loseNum, loss);
        }
        private double getLastHigh(int n)
        {
            double res = -1;
            LinkedListNode<Candle> node = barH4.First;
            while (n-- > 0)
            {
                if (node.Next == null) break;
                node = node.Next;
                res = Math.Max(res, node.Value.AskHigh);
            }
            return res;
        }
        private double getLastLow(int n)
        {
            double res = 1000;
            LinkedListNode<Candle> node = barH4.First;
            while (n-- > 0)
            {
                if (node.Next == null) break;
                node = node.Next;
                res = Math.Min(res, node.Value.AskLow);
            }
            return res;
        }
        protected void Play()
        {
            tickCur = tickCur.AddMinutes(1);
        }
    }
}