using System;

namespace FXCMTrader
{
    class Program
    {
        static void Main(string[] args)
        {
            Trader app = new Trader();
            bool c = app.run();
            Console.WriteLine(c);
            Console.WriteLine(app.Status.ToString());
            app.stop();
        }
    }
}
