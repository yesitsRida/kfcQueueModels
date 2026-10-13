namespace KfcQueueModels;

/// <summary>M/M/1 steady-state formulas plus replay of the measured customer trace.</summary>
public static class Mm1Model
{
    public static void Run(IReadOnlyList<CustomerObservation> data)
    {
        Console.WriteLine("\n=== M/M/1: Poisson arrivals, exponential service, one server ===");
        foreach (var session in ModelOutput.Sessions(data))
        {
            var trace = QueueSimulation.RunSingleServer(session.Rows.Select(row => new ArrivalService(row.Id, row.Arrival, row.ServiceQ1)));
            var rho = trace.ServiceRate > 0 ? trace.ArrivalRate / trace.ServiceRate : double.PositiveInfinity;
            Console.WriteLine($"  {session.Name}");
            ModelOutput.PrintTrace(trace);
            Console.WriteLine("  Theoretical M/M/1 steady state:");
            if (rho >= 1 || !double.IsFinite(rho))
            {
                Console.WriteLine($"    rho = {rho:F4}; steady state is unstable (requires rho < 1). No finite steady-state W or L is reported.");
                continue;
            }

            var wq = rho / (trace.ServiceRate - trace.ArrivalRate);
            var w = 1 / (trace.ServiceRate - trace.ArrivalRate);
            Console.WriteLine($"    lambda = {trace.ArrivalRate:F4} customers/min, mu = {trace.ServiceRate:F4} customers/min, rho = {rho:F4}");
            Console.WriteLine($"    Wq = {wq:F4} min, W = {w:F4} min, Lq = {trace.ArrivalRate * wq:F4}, L = {trace.ArrivalRate * w:F4}");
        }
    }
}
