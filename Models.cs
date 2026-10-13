namespace KfcQueueModels;

/// <summary>One observed customer from the source tandem-queue CSV.</summary>
public sealed record CustomerObservation(
    int Id,
    double Arrival,
    double Q1Start,
    double Q1End,
    double Q2End)
{
    public double ObservedWaitQ1 => Q1Start - Arrival;
    public double ServiceQ1 => Q1End - Q1Start;
    public double ServiceQ2 => Q2End - Q1End;
    public double TotalObservedTime => Q2End - Arrival;
}

public sealed record ArrivalService(int CustomerId, double Arrival, double Service);
public sealed record CustomerTrace(int CustomerId, double Arrival, double Start, double End, double Wait, double Service);

/// <summary>Finite-trace FCFS measures; rates use adjacent arrival gaps.</summary>
public sealed record QueueMetrics(
    int Customers,
    double MeanInterarrival,
    double MeanService,
    double ArrivalRate,
    double ServiceRate,
    double Utilization,
    double Wq,
    double W,
    double Lq,
    double L,
    int MaxQueue,
    IReadOnlyList<CustomerTrace> CustomersTrace);

public static class Statistics
{
    public static double Mean(IReadOnlyList<double> values) => values.Count == 0 ? 0 : values.Average();

    public static double SampleVariance(IReadOnlyList<double> values)
    {
        if (values.Count < 2) return 0;
        var mean = Mean(values);
        return values.Sum(value => (value - mean) * (value - mean)) / (values.Count - 1);
    }

    public static double SquaredCoefficientOfVariation(IReadOnlyList<double> values)
    {
        var mean = Mean(values);
        return mean > 0 ? SampleVariance(values) / (mean * mean) : 0;
    }
}
