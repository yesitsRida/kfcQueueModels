namespace KfcQueueModels;

/// <summary>G/G/1 Kingman approximation and empirical FCFS trace replay.</summary>
public static class Gg1Model
{
    public static void Run(IReadOnlyList<CustomerObservation> data)
    {
        Console.WriteLine("\n=== G/G/1: general arrivals, general service, one server ===");
        foreach (var session in ModelOutput.Sessions(data))
        {
            var rows = session.Rows;
            var trace = QueueSimulation.RunSingleServer(rows.Select(row => new ArrivalService(row.Id, row.Arrival, row.ServiceQ1)));
            var interarrivals = rows.Zip(rows.Skip(1), (first, next) => next.Arrival - first.Arrival).ToList();
            var services = rows.Select(row => row.ServiceQ1).ToList();
            var ca2 = Statistics.SquaredCoefficientOfVariation(interarrivals);
            var cs2 = Statistics.SquaredCoefficientOfVariation(services);
            var rho = trace.ServiceRate > 0 ? trace.ArrivalRate / trace.ServiceRate : double.PositiveInfinity;

            Console.WriteLine($"  {session.Name}: empirical Ca^2 = {ca2:F4}; Cs^2 = {cs2:F4}");
            ModelOutput.PrintTrace(trace);
            if (rho >= 1 || !double.IsFinite(rho))
            {
                Console.WriteLine($"  Kingman steady-state approximation unavailable: rho = {rho:F4} (requires rho < 1).");
                continue;
            }

            var wq = rho / (1 - rho) * (ca2 + cs2) / 2 * trace.MeanService;
            var w = wq + trace.MeanService;
            Console.WriteLine("  Kingman's approximation (not an exact finite-trace result):");
            Console.WriteLine($"    lambda = {trace.ArrivalRate:F4}, mu = {trace.ServiceRate:F4}, rho = {rho:F4}");
            Console.WriteLine($"    Wq = {wq:F4} min, W = {w:F4} min, Lq = {trace.ArrivalRate * wq:F4}, L = {trace.ArrivalRate * w:F4}");
        }
    }
}
