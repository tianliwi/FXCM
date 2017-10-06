using System;
using fxcore2;
using System.Threading;
using System.Collections.Generic;
using DataRepo;

namespace HistoricalDataDownloader
{
    internal class ResponseListener : IO2GResponseListener
    {
        private O2GSession mSession = null;
        public Dictionary<string, string> candles;

        public WaitHandle ResponseHandle
        {
            get { return mResponseHandle; }
        }
        private EventWaitHandle mResponseHandle;
        public ResponseListener(O2GSession session)
        {
            mSession = session;
            mResponseHandle = new AutoResetEvent(false);
            candles = new Dictionary<string, string>();
        }

        public void onRequestCompleted(string requestId, O2GResponse response)
        {
            if (response.Type == O2GResponseType.MarketDataSnapshot)
            {
                GetPrices(mSession, response);
            }
            mResponseHandle.Set();
        }

        public void onRequestFailed(string requestId, string error)
        {
            if (String.IsNullOrEmpty(error)) // not an error - we are finished - no more candles
            {
                Console.WriteLine("\n There is no history data for the specified period!");
            }
            else
            {
                Console.WriteLine("Request failed requestID={0} error={1}", requestId, error);
            }
            mResponseHandle.Set();
        }

        public void onTablesUpdates(O2GResponse data)
        {
            //STUB
        }

        private void GetPrices(O2GSession session, O2GResponse response)
        {
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            O2GResponseReaderFactory factory = session.getResponseReaderFactory();
            if (factory != null)
            {
                O2GMarketDataSnapshotResponseReader reader = factory.createMarketDataSnapshotReader(response);
                for (int ii = 0; ii < reader.Count; ii++)
                {
                    if (reader.isBar)
                    {
                        Candle candle = new Candle();
                        candle.openTime = TimeZoneInfo.ConvertTimeFromUtc(reader.getDate(ii), est).ToString("yyyyMMdd HH:mm:ss");
                        //candle.openTime = reader.getDate(ii).ToString("yyyyMMdd HH:mm:ss");
                        candle.BidOpen = reader.getBidOpen(ii);
                        candle.BidHigh = reader.getBidHigh(ii);
                        candle.BidLow = reader.getBidLow(ii);
                        candle.BidClose = reader.getBidClose(ii);
                        candle.AskOpen = reader.getAskOpen(ii);
                        candle.AskHigh = reader.getAskHigh(ii);
                        candle.AskLow = reader.getAskLow(ii);
                        candle.AskClose = reader.getAskClose(ii);
                        candle.Volume = reader.getVolume(ii);
                        candles[candle.openTime] = candle.ToString();
                    }
                    else
                    {
                        Console.WriteLine("DateTime={0}, Bid={1}, Ask={2}", reader.getDate(ii), reader.getBidClose(ii), reader.getAskClose(ii));
                    }
                }
            }
        }
    }
}
