using System;
using fxcore2;
using System.Threading;

namespace HistoricalDataDownloader
{
    public class SessionStatusListener : IO2GSessionStatus
    {
        private bool mConnected = false;
        private bool mDisconnected = false;
        private bool mError = false;
        private O2GSession mSession;
        private object mEvent = new object();

        public SessionStatusListener(O2GSession session)
        {
            mSession = session;
        }
        public bool Connected
        {
            get
            {
                return mConnected;
            }
        }
        public bool Disconnected
        {
            get
            {
                return mDisconnected;
            }
        }
        public bool Error
        {
            get
            {
                return mError;
            }
        }

        public void onSessionStatusChanged(O2GSessionStatusCode status)
        {
            Console.WriteLine("Status: " + status.ToString());
            if (status == O2GSessionStatusCode.Connected)
                mConnected = true;
            else
                mConnected = false;

            if (status == O2GSessionStatusCode.Disconnected)
                mDisconnected = true;
            else
                mDisconnected = false;

            if (status == O2GSessionStatusCode.TradingSessionRequested)
            {
                if (Program.SessionID == "")
                    Console.WriteLine("Argument for trading session ID is missing");
                else
                    mSession.setTradingSession(Program.SessionID, Program.Pin);
            }
            else if (status == O2GSessionStatusCode.Connected)
            {
                lock (mEvent)
                    Monitor.PulseAll(mEvent);
            }

        }

        public void onLoginFailed(string error)
        {
            Console.WriteLine("Login error: " + error);
            mError = true;
            lock (mEvent)
                Monitor.PulseAll(mEvent);
        }

        public void login(string sUserID, string sPassword, string sURL, string sConnection)
        {
            Program.Session.login(sUserID, sPassword, sURL, sConnection);
            lock (mEvent)
                Monitor.Wait(mEvent, 60000);
        }
    }
}
