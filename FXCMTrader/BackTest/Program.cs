using System;
using System.IO;
using MathNet.Numerics;
using System.Linq;
using System.Collections.Generic;
using DataRepo;

namespace BackTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DataRepo.DataRepo data = new DataRepo.DataRepo();
            data.Load(new int[] { 2016, 2017 } , DataRepo.DataRepo.Symbol);

            LongStrategy longStra = new LongStrategy(data);

            string row = string.Empty;
            string filename = DataRepo.DataRepo.DataDir + "matrix_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            for (double a = 0; a < .4; a += .02)
            {
                for (double b = 0; b < .8; b += .02)
                {
                    Tuple<int, int, double> r = longStra.Start(a, b, true);

                    List<double> xdata = new List<double>();
                    List<double> ydata = new List<double>();
                    for (int k = 0; k < longStra.pnlList.Count; k++)
                    {
                        xdata.Add((double)k);
                        double rev = longStra.pnlList.Values.ElementAt(k);
                        ydata.Add(rev);

                    }

                    Tuple<double, double> p = Fit.Line(xdata.ToArray(), ydata.ToArray());
                    double la = p.Item1;
                    double lb = p.Item2;
                    double R2 = GoodnessOfFit.RSquared(xdata.Select(x => la + lb * x).ToArray(), ydata.ToArray());
                    if (R2 > 0.7 && r.Item3 > 0)
                    {
                        Console.WriteLine("{0}, {1}: {2}, {3}", a, b, r.Item3.ToString(), R2.ToString());
                    }
                    if (string.IsNullOrEmpty(row)) row = R2.ToString();
                    else row = row + "," + Math.Round(R2, 2).ToString();
                }
                row = string.Empty;
            }
        }
    }
}
