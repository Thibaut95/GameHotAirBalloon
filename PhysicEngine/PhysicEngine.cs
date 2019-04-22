using System;
using MathNet.Numerics.OdeSolvers;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;
using System.IO;

namespace PhysicEngine
{
    public class PhysicEngine
    {
        private const double Cd = 0.5;
        private const double Cp = 1005.0;
        private const double D = 18.0;
        private const double fapp = 0.5;
        private const double g = 9.81;
        private const double Mtot = 663.0;
        private const double Ra = 285.0;
        private const double V = 2973.0;
        private const double alphab = 3e6;
        // private const double alphac = 3e-4;
        private const double alphac = 7e-5;
        // private const double alphac = 2e-5;
        private const double gamma = 1.4;
        private const double Ru = 8314.3;
        private const double Mo = 28.96;
        private const double Koffset = 273.15;
        private const double rho0 = 1.225;
        private const double r = 287;
        private const double a = 0.0065;
        private const double Psea = 102325.0;
        private const double T0 = 15 + Koffset;




        private const double duration = 145;
        private const double stepNumber = 145;

        private const double h0 = 1300;
        private const double burnerTime = 0;

        public void run()
        {
            Func<double, Vector<double>, Vector<double>> f = (t, Y) => dXdt(t, Y);

            double To0 = GetT(h0);
            double rhoo0 = GetRhoo(To0, h0);
            double Ti0 = To0 * rhoo0 * V / (rhoo0 * V - Mtot);
            // double Ti0 = 110 + 273.15;

            //110 + 273.15;
            double[] initialValues = new double[] { h0, 0, Ti0 };
            Vector<double> initialVector = CreateVector.Dense<double>(initialValues);

            Vector<double>[] result = RungeKutta.FourthOrder(initialVector, 0, (int)duration, (int)stepNumber, f);

            PlotResult(result);
        }

        public void PlotResult(Vector<double>[] result)
        {
            string[] plotNames = new String[] { "Altitude", "Vitesse", "Température" };
            double step = duration / stepNumber;

            for (int j = 0; j < plotNames.Length; j++)
            {
                PlotModel model = new PlotModel { Title = plotNames[j] };
                LineSeries lineSeries = new LineSeries();
                for (int i = 0; i < result.Length; i++)
                {
                    lineSeries.Points.Add(new DataPoint(i * step, result[i][j]));
                }

                model.Series.Add(lineSeries);
                model.Background = OxyColor.FromRgb(255, 255, 255);

                using (var stream = File.Create(plotNames[j] + ".svg"))
                {
                    var exporter = new SvgExporter { Width = 600, Height = 600 };
                    exporter.Export(model, stream);
                }
            }

            string docPath = "/home/thibaut/Desktop/GameHotAirBalloon/PhysicEngine";
            Console.WriteLine(docPath);

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "results.txt")))
            {
                foreach (Vector<double> line in result)

                    outputFile.WriteLine(line[0]+"\t"+line[1]+"\t"+line[2]);
            }
        }

        public double GetRhoo(double T, double Z)
        {
            return rho0 * Math.Pow((1 - a / T * Z), (g / r / a + 1));
        }

        public double GetP(double h)
        {
            return Psea * Math.Pow(1.0 - (a * h) / 288.15, 5.255);
        }

        public double GetT(double Z)
        {
            return T0 - a * Z;
        }

        public Vector<double> dXdt(double t, Vector<double> Y)
        {
            double h = Y[0];
            double v = Y[1];
            double Ti = Y[2];

            if (h < 0)
            {
                h = 0;
                v = 0;
            }

            double To = GetT(h);
            double rhoo = GetRhoo(To, h);
            double P = rhoo * Ra * To;

            double L = rhoo * V * g * (1 - To / Ti);
            double W = Mtot * g;
            double DF = (1.0 / 2.0) * (rhoo * v * Math.Abs(v) * Cd * Math.PI * D * D) / 4;

            double acc = (L - W - DF) / (V * rhoo * fapp + Mtot);
            if (h <= 0) acc = 0;

            double Qa = -V * rhoo * g / (gamma - 1) * v;
            double Qb = 0;
            if (t <= burnerTime) Qb = alphab;

            double Qc = alphac * (Math.Pow(Ti, 4) - Math.Pow(To, 4));
            double Qm = Qb - Qa - Qc;
            double Tm = To;
            if (Qm > 0)
            {
                Tm = Ti;
            }

            double dTidt = Ra * Ti * Ti * Qm / (P * Cp * V * Tm);

            return CreateVector.Dense<double>(new double[] { v, acc, dTidt });
        }
    }
}