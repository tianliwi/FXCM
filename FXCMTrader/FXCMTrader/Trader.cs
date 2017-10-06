using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fxcore2;
using System.Threading;

namespace FXCMTrader
{
    public class Trader
    {
        EventWaitHandle mSyncSessionEvent = null; //for synchronization with thread of calls of IO2GSessionStatus notification methods  
        O2GSession mSession; //store the session object reference        
        object csSessionStatus = new object(); //sync object for critical section to synchronize access to mSessionStatus    

        //constructor
        public Trader()
        {
            mSyncSessionEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        }
        //destructor
        ~Trader()
        {
            mSyncSessionEvent.Close();
        }

        public O2GSessionStatusCode Status
        {
            get
            {
                O2GSessionStatusCode status;
                lock (csSessionStatus)
                {
                    status = mSessionStatus;
                }
                return status;
            }
            private set
            {
                lock (csSessionStatus)
                {
                    mSessionStatus = value;
                }
            }
        }
        O2GSessionStatusCode mSessionStatus; //store current connection status

        //Session status events handlers
        void mSession_SessionStatusChanged(object sender, SessionStatusEventArgs e)
        {
            lock (csSessionStatus)
            {
                mSessionStatus = e.SessionStatus;
            }

            switch (e.SessionStatus)
            {
                case O2GSessionStatusCode.Connected:
                case O2GSessionStatusCode.Disconnected:
                    mSyncSessionEvent.Set();
                    break;
            }
        }

        void mSession_LoginFailed(object sender, LoginFailedEventArgs e)
        {
            lock (csSessionStatus)
            {
                mSessionStatus = O2GSessionStatusCode.Disconnected;
            }

            mSyncSessionEvent.Set();
        }

        //create session, connect and make other preparations
        public bool run()
        {
            try
            {
                mSession = O2GTransport.createSession(); //create IO2GSession object	        
                //subscribe to session status events
                mSession.LoginFailed += new EventHandler<LoginFailedEventArgs>(mSession_LoginFailed);
                mSession.SessionStatusChanged += new EventHandler<SessionStatusEventArgs>(mSession_SessionStatusChanged);

                //Please specify valid username and password
                mSession.login("D9452131", "109", "http://www.fxcorporate.com/Hosts.jsp", "Demo");

                //Waiting for result of async login           
                if (mSyncSessionEvent.WaitOne(5000) &&
                   this.Status != O2GSessionStatusCode.Connected)
                {
                    return false;
                }


                return true;
            }
            catch (Exception)
            {
                this.stop();
                throw;
            }
        }


        //log out and stop
        public void stop()
        {
            if (mSession != null)
            {
                mSession.logout();
                mSyncSessionEvent.WaitOne(5000); //wait for logout completion during 5 seconds

                mSession.LoginFailed -= new EventHandler<LoginFailedEventArgs>(mSession_LoginFailed);
                mSession.SessionStatusChanged -= new EventHandler<SessionStatusEventArgs>(mSession_SessionStatusChanged);

                mSession.Dispose();
                mSession = null;//to avoid second time stop
                mSessionStatus = O2GSessionStatusCode.Disconnected; //for getStatus()
            }
        }
    }
}
