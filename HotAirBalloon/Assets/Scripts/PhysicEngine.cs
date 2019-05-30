using System;
using MathNet.Numerics.OdeSolvers;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;
using System.IO;


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
    private const double alphaCooler = -3e6;
    // private const double alphac = 3e-4;
    // private const double alphac = 7e-5;
    private const double alphac = 1e-4;
    private const double gamma = 1.4;
    private const double Ru = 8314.3;
    private const double Mo = 28.96;
    private const double Koffset = 273.15;
    private const double rho0 = 1.225;
    private const double r = 287;
    private const double a = 0.0065;
    private const double Psea = 102325.0;
    private const double T0 = 15 + Koffset;


    private const double Vcooler = 100.0;
    private const int stepNumber = 2;

    private double h0;
    private double h;
    private double v;
    private double Ti;

    public double Getv()
    {
        return v;
    }

    public double Geth()
    {
        return h;
    }

    public double GetTi()
    {
        return Ti;
    }

    public double GetTo()
    {
        return GetT(h)-Koffset;
    }

    public PhysicEngine(double h0, double v0, double Ti0)
    {
        this.h0 = h0;
        this.v = v0;
        this.h = h0;
        this.Ti = Ti0;
    }

    public void UpdateEngine(double duration, bool burnerOn, bool coolerOn)
    {
        Func<double, Vector<double>, Vector<double>> f = (t, Y) => DXdt(t, Y, duration, burnerOn);

        double[] initialValues = new double[] { h, v, Ti + Koffset };
        Vector<double> initialVector = CreateVector.Dense<double>(initialValues);

        Vector<double>[] result = RungeKutta.FourthOrder(initialVector, 0, duration, stepNumber, f);

        h = result[result.Length - 1][0];
        v = result[result.Length - 1][1];
        Ti = result[result.Length - 1][2];

        if(coolerOn)
        {
            double percentageCooler = (Vcooler*duration) / V;
            Ti = GetT(h) * percentageCooler + Ti * (1 - percentageCooler);
        }

        Ti -= Koffset;
        
        if(h<=h0)
        {
            h=h0;
            v=0;
        }
    }

    public void RunTest()
    {
        const double h0 = 900;
        double burnerTime = 30;
        const double duration = 220;
        const double stepNumber = 1000;

        Func<double, Vector<double>, Vector<double>> f = (t, Y) => DXdt(t, Y, burnerTime);

        double To0 = GetT(h0);
        double rhoo0 = GetRhoo(To0, h0);
        // double Ti0 = To0 * rhoo0 * V / (rhoo0 * V - Mtot);
        double Ti0 = 110 + 273.15;

        //110 + 273.15;
        double[] initialValues = new double[] { h0, 0, Ti0 };
        Vector<double> initialVector = CreateVector.Dense<double>(initialValues);

        Vector<double>[] result = RungeKutta.FourthOrder(initialVector, 0, duration, (int)stepNumber, f);

        PlotResult(result, duration / stepNumber);
    }

    private void PlotResult(Vector<double>[] result, double step)
    {
        string[] plotNames = new String[] { "Altitude", "Vitesse", "Température" };

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
    }

    private double GetRhoo(double T, double Z)
    {
        return rho0 * Math.Pow((1 - a / T * Z), (g / r / a + 1));
    }

    private double GetP(double h)
    {
        return Psea * Math.Pow(1.0 - (a * h) / 288.15, 5.255);
    }

    private double GetT(double Z)
    {
        return T0 - a * Z;
    }

    private Vector<double> DXdt(double t, Vector<double> Y, double burnerTime, bool activateBurner = true)
    {
        double h = Y[0];
        double v = Y[1];
        double Ti = Y[2];

        double To = GetT(h);
        double rhoo = GetRhoo(To, h);
        double P = rhoo * Ra * To;

        double L = rhoo * V * g * (1 - To / Ti);
        double W = Mtot * g;
        double DF = (1.0 / 2.0) * (rhoo * v * Math.Abs(v) * Cd * Math.PI * D * D) / 4;

        double acc = (L - W - DF) / (V * rhoo * fapp + Mtot);

        double Qa = -V * rhoo * g / (gamma - 1) * v;
        double Qb = 0;
        if (t <= burnerTime)
        {
            if (activateBurner)
            {
                Qb += alphab;
            }
        }

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
