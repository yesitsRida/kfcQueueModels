namespace KfcQueueModels;

/// <summary>Shared finite-trace FCFS engine used to compare models against the same observations.</summary>
public static class QueueSimulation
{
    public static QueueMetrics RunSingleServer(IEnumerable<ArrivalService> input)
    {
        var arrivals = input.OrderBy(item => item.Arrival).ThenBy(item => item.CustomerId).ToList();
        if (arrivals.Count == 0) throw new ArgumentException("At least one arrival is required.", nameof(input));
        if (arrivals.Any(item => item.Arrival < 0 || item.Service < 0 || !double.IsFinite(item.Arrival) || !double.IsFinite(item.Service)))
            throw new ArgumentException("Arrival and service times must be finite and non-negative.", nameof(input));

        var trace = new List<CustomerTrace>(arrivals.Count);
        var serverFreeAt = 0.0;
        var busyTime = 0.0;
        var maximumQueue = 0;
        var waiting = new List<(double Time, int Change)>();

        foreach (var item in arrivals)
        {
            var start = Math.Max(item.Arrival, serverFreeAt);
            var end = start + item.Service;
            var wait = start - item.Arrival;
            if (wait > 1e-12)
            {
                waiting.Add((item.Arrival, 1));
                waiting.Add((start, -1));
            }
            busyTime += item.Service;
            trace.Add(new CustomerTrace(item.CustomerId, item.Arrival, start, end, wait, item.Service));
            serverFreeAt = end;
        }

        var inQueue = 0;
        foreach (var evt in waiting.OrderBy(evt => evt.Time).ThenBy(evt => evt.Change))
        {
            inQueue += evt.Change;
            maximumQueue = Math.Max(maximumQueue, inQueue);
        }

        var interarrivals = arrivals.Zip(arrivals.Skip(1), (first, next) => next.Arrival - first.Arrival).ToList();
        var meanInterarrival = Statistics.Mean(interarrivals);
        var meanService = Statistics.Mean(arrivals.Select(item => item.Service).ToList());
        var lambda = meanInterarrival > 0 ? 1 / meanInterarrival : 0;
        var mu = meanService > 0 ? 1 / meanService : 0;
        var elapsed = Math.Max(0, serverFreeAt - arrivals[0].Arrival);
        var utilization = elapsed > 0 ? Math.Min(1, busyTime / elapsed) : 0;
        var wq = trace.Average(item => item.Wait);
        var w = trace.Average(item => item.Wait + item.Service);

        return new QueueMetrics(
            arrivals.Count, meanInterarrival, meanService, lambda, mu, utilization,
            wq, w, lambda * wq, lambda * w, maximumQueue, trace);
    }
}
