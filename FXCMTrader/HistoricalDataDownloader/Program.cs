using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using fxcore2;

namespace HistoricalDataDownloader
{
    class Program
    {
        private static string sSessionID = "";
        public static string SessionID
        {
            get { return sSessionID; }
        }
        private static string sPin = "";
        public static string Pin
        {
            get { return sPin; }
        }

        private static string sTimeframeName;
        public static string TimeframeName
        {
            get { return sTimeframeName; }
        }

        private static string sInstrument;
        public static string Instrument
        {
            get { return sInstrument; }
        }
        public const int MAX_BARS = 300;

        private static O2GSession session = null;
        public static O2GSession Session
        {
            get { return session; }
        }
        private static SessionStatusListener sessionStatusListener;
        private static ResponseListener responseListener;


        static void Main(string[] args)
        {
            try
            {
                /*
                if (args.Length < 8)
                {
                    Console.WriteLine("Not Enough Parameters!");
                    Console.WriteLine("USAGE: [application].exe [instrument (default \"EUR/USD\"] [time frame (default \"m1\")] [datetime \"from\" or empty string for \"from now\" (default)] [datetime \"to\" or empty string for \"to now\" (default)] [user ID] [password] [URL] [connection] [session ID (if needed) or empty string] [pin (if needed) or empty string] ");
                    Console.WriteLine("\nPress enter to close the program");
                    Console.ReadLine();
                    return;
                }
                */

                sInstrument = "EUR/USD"; // default
                //sInstrument = args[0];
                sTimeframeName = "m1";   // default
                //sTimeframeName = args[1];

                string sDateTimeFrom = "";
                sDateTimeFrom = DateTime.Now.AddHours(-4).ToString();

                string sDateTimeTo = "";
                sDateTimeTo = DateTime.Now.AddHours(-3).ToString();

                string sUserID = "D25611513";
                string sPassword = "2609";
                string sURL = "http://www.fxcorporate.com/Hosts.jsp";
                string sConnection = "Demo";


                Console.WriteLine("Getting price history for instrument {0}; timeframe {1}; from {2}; to {3}", sInstrument, sTimeframeName, sDateTimeFrom, sDateTimeTo);


                session = O2GTransport.createSession();
                sessionStatusListener = new SessionStatusListener(session);
                session.subscribeSessionStatus(sessionStatusListener);
                sessionStatusListener.login(sUserID, sPassword, sURL, sConnection); //wait until login or fail

                if (sessionStatusListener.Connected)
                {
                    responseListener = new ResponseListener(session);
                    session.subscribeResponse(responseListener);

                    GetHistoryPrices(session, responseListener, sInstrument, sTimeframeName, sDateTimeFrom, sDateTimeTo);

                    session.unsubscribeResponse(responseListener);
                    session.logout();
                    while (!sessionStatusListener.Disconnected)
                        Thread.Sleep(50);
                }

                session.unsubscribeSessionStatus(sessionStatusListener);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();

                session.Dispose();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
        public static void GetHistoryPrices(O2GSession session, ResponseListener listener, string instrument, string timeframeName, string sFrom, string sTo)
        {
            O2GRequestFactory factory = session.getRequestFactory();
            if (factory != null)
            {
                O2GTimeframe tf = factory.Timeframes[timeframeName];
                O2GRequest request = factory.createMarketDataSnapshotRequestInstrument(sInstrument, tf, MAX_BARS);
                if (request == null)
                {
                    Console.WriteLine("wow");
                }
                DateTime dtFrom;
                DateTime dtTo;
                dtFrom = string.IsNullOrEmpty(sFrom) ? factory.ZERODATE : DateTime.Parse(sFrom);
                dtTo = string.IsNullOrEmpty(sTo) ? factory.ZERODATE : DateTime.Parse(sTo);
                factory.fillMarketDataSnapshotRequestTime(request, dtFrom, dtTo, false);

                session.sendRequest(request);
                responseListener.ResponseHandle.WaitOne(30000); //30 seconds timeout
            }
        }

        public static void PrintPrices(O2GSession session, O2GResponse response)
        {
            Console.WriteLine();
            O2GResponseReaderFactory factory = session.getResponseReaderFactory();
            if (factory != null)
            {
                O2GMarketDataSnapshotResponseReader reader = factory.createMarketDataSnapshotReader(response);
                for (int ii = 0; ii < reader.Count; ii++)
                {
                    if (reader.isBar)
                    {
                        Console.WriteLine("DateTime={0}, BidOpen={1}, BidHigh={2}, BidLow={3}, BidClose={4}, AskOpen={5}, AskHigh={6}, AskLow={7}, AskClose= {8}, Volume = {9}",
                                            reader.getDate(ii), reader.getBidOpen(ii), reader.getBidHigh(ii), reader.getBidLow(ii), reader.getBidClose(ii),
                                            reader.getAskOpen(ii), reader.getAskHigh(ii), reader.getAskLow(ii), reader.getAskClose(ii), reader.getVolume(ii));
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