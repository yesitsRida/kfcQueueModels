namespace KfcQueueModels;

/// <summary>M/G/1 Pollaczek-Khinchine approximation and empirical FCFS replay.</summary>
public static class Mg1Model
{
    public static void Run(IReadOnlyList<CustomerObservation> data)
    {
        Console.WriteLine("\n=== M/G/1: Poisson arrivals, general service, one server ===");
        foreach (var session in ModelOutput.Sessions(data))
        {
            var rows = session.Rows;
            var trace = QueueSimulation.RunSingleServer(rows.Select(row => new ArrivalService(row.Id, row.Arrival, row.ServiceQ1)));
            var services = rows.Select(row => row.ServiceQ1).ToList();
            var cs2 = Statistics.SquaredCoefficientOfVariation(services);
            var rho = trace.ServiceRate > 0 ? trace.ArrivalRate / trace.ServiceRate : double.PositiveInfinity;

            Console.WriteLine($"  {session.Name}: empirical service Cs^2 = {cs2:F4}; Poisson arrivals assumed for theory.");
            ModelOutput.PrintTrace(trace);
            if (rho >= 1 || !double.IsFinite(rho))
            {
                Console.WriteLine($"  Pollaczek-Khinchine steady-state approximation unavailable: rho = {rho:F4} (requires rho < 1).");
                continue;
            }

            var wq = rho / (1 - rho) * (1 + cs2) / 2 * trace.MeanService;
            var w = wq + trace.MeanService;
            Console.WriteLine("  Pollaczek-Khinchine approximation:");
            Console.WriteLine($"    lambda = {trace.ArrivalRate:F4}, mu = {trace.ServiceRate:F4}, rho = {rho:F4}");
            Console.WriteLine($"    Wq = {wq:F4} min, W = {w:F4} min, Lq = {trace.ArrivalRate * wq:F4}, L = {trace.ArrivalRate * w:F4}");
        }
    }
}
