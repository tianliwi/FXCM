﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using fxcore2;
using System.Globalization;
using System.IO;

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

                string[] paras = File.ReadAllLines(@"C:\FXCMHD.txt");

                string sUserID = paras[0];
                string sPassword = paras[1];
                sInstrument = paras[2];
                sTimeframeName = paras[3];
                string sStartTime = paras[4];
                string sEndTime = paras[5];
                DataRepo.DataRepo.DataDir = paras[6] + @"Data\";
                
                string sURL = "http://www.fxcorporate.com/Hosts.jsp";
                string sConnection = "Demo";


                Console.WriteLine("Getting price history for instrument {0}; timeframe {1}; from {2}; to {3}", sInstrument, sTimeframeName, sStartTime, sEndTime);


                session = O2GTransport.createSession();
                sessionStatusListener = new SessionStatusListener(session);
                session.subscribeSessionStatus(sessionStatusListener);
                sessionStatusListener.login(sUserID, sPassword, sURL, sConnection); //wait until login or fail

                DateTime startTime = DateTime.ParseExact(sStartTime, "yyyyMMddHH:mm:ss", CultureInfo.CurrentCulture).ToUniversalTime();
                DateTime endTime = DateTime.ParseExact(sEndTime, "yyyyMMddHH:mm:ss", CultureInfo.CurrentCulture).ToUniversalTime();

                if (sessionStatusListener.Connected)
                {
                    responseListener = new ResponseListener(session);
                    session.subscribeResponse(responseListener);
                    responseListener.ResponseHandle.WaitOne(30000);

                    DateTime curStart = startTime;
                    while (curStart < endTime)
                    {
                        DateTime curEnd = curStart;
                        switch (sTimeframeName.ToUpper())
                        {
                            case "M1":
                                curEnd = curStart.AddHours(4);
                                break;
                            case "H4":
                                curEnd = curStart.AddDays(50);
                                break;
                            case "D1":
                                curEnd = curStart.AddYears(1);
                                break;
                        }
                        GetHistoryPrices(session, responseListener, sInstrument, sTimeframeName, curStart, curEnd);
                        curStart = curEnd;
                    }
                    File.WriteAllLines(DataRepo.DataRepo.DataDir + sInstrument.Substring(0,3) + @"\" + sStartTime.Substring(0, 4) + "_" + sTimeframeName.ToUpper() + ".csv",
                        responseListener.candles.Values);
                    session.unsubscribeResponse(responseListener);
                    session.logout();
                    while (!sessionStatusListener.Disconnected)
                        Thread.Sleep(50);
                }

                session.unsubscribeSessionStatus(sessionStatusListener);
                session.Dispose();
                if (sTimeframeName.ToUpper() == "M1")
                {
                    M1Process m1Process = new M1Process(Convert.ToInt32(sStartTime.Substring(0, 4)), "AUD");
                    m1Process.GenerateH4();
                }
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
        public static void GetHistoryPrices(O2GSession session, ResponseListener listener, string instrument, string timeframeName, DateTime dFrom, DateTime dTo)
        {
            Console.WriteLine("{0} - {1}", dFrom, dTo);
            O2GRequestFactory factory = session.getRequestFactory();
            if (factory != null)
            {
                O2GTimeframe tf = factory.Timeframes[timeframeName];
                O2GRequest request = factory.createMarketDataSnapshotRequestInstrument(sInstrument, tf, MAX_BARS);
                if (request == null)
                {
                    throw new Exception("Cannot initialize request.");
                }
                factory.fillMarketDataSnapshotRequestTime(request, dFrom, dTo, false);

                session.sendRequest(request);
                responseListener.ResponseHandle.WaitOne(30000); //30 seconds timeout
            }
        }
    }

}