namespace KfcQueueModels;

public static class ModelOutput
{
    public static IEnumerable<(string Name, List<CustomerObservation> Rows)> Sessions(IReadOnlyList<CustomerObservation> data)
    {
        var first = data.Where(row => row.Arrival <= 90).ToList();
        var second = data.Where(row => row.Arrival > 90).ToList();
        if (first.Count > 0) yield return ("Session 1 (arrival <= 90 min)", first);
        if (second.Count > 0) yield return ("Session 2 (arrival > 90 min)", second);
    }

    public static void PrintTrace(QueueMetrics metrics)
    {
        Console.WriteLine("Trace-driven FCFS replay of the measured Queue 1 data:");
        Console.WriteLine($"  Customers = {metrics.Customers}; mean IAT = {metrics.MeanInterarrival:F4} min; mean service = {metrics.MeanService:F4} min");
        Console.WriteLine($"  Observed-trace utilization = {metrics.Utilization:P2}; Wq = {metrics.Wq:F4} min; W = {metrics.W:F4} min");
        Console.WriteLine($"  Little's Law estimates: Lq = {metrics.Lq:F4}, L = {metrics.L:F4}; maximum line length = {metrics.MaxQueue}");
    }
}
