namespace KfcQueueModels;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            var csvPath = ResolveCsvPath(args);
            var data = CsvDataLoader.LoadTandemCsv(csvPath);
            var model = GetOption(args, "--model")?.ToLowerInvariant() ?? "all";

            Console.WriteLine("KFC Queueing Simulation Model Comparison");
            Console.WriteLine("Data file: kfc_tandem_data.csv");
            Console.WriteLine($"Loaded {data.Count} records; time unit: minutes.");

            if (model is "all" or "mm1") Mm1Model.Run(data);
            if (model is "all" or "gg1") Gg1Model.Run(data);
            if (model is "all" or "mg1") Mg1Model.Run(data);

            if (model is not ("all" or "mm1" or "gg1" or "mg1"))
            {
                Console.Error.WriteLine($"Unknown model '{model}'. Use all, mm1, gg1, or mg1.");
                return 2;
            }

            Console.WriteLine("\nNotes: M/M/1, G/G/1, and M/G/1 theoretical results are steady-state approximations based on this finite dataset.");
            Console.WriteLine("Trace-driven metrics are empirical FCFS replay results; they are not interchangeable with steady-state theory.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Simulation failed: {exception.Message}");
            return 1;
        }
    }

    private static string ResolveCsvPath(string[] args)
    {
        var requested = GetOption(args, "--csv");
        if (!string.IsNullOrWhiteSpace(requested)) return Path.GetFullPath(requested);

        var besideExecutable = Path.Combine(AppContext.BaseDirectory, "kfc_tandem_data.csv");
        if (File.Exists(besideExecutable)) return besideExecutable;

        var inCurrentDirectory = Path.Combine(Environment.CurrentDirectory, "kfc_tandem_data.csv");
        return inCurrentDirectory;
    }

    private static string? GetOption(string[] args, string option)
    {
        for (var index = 0; index < args.Length; index++)
        {
            if (args[index].Equals(option, StringComparison.OrdinalIgnoreCase) && index + 1 < args.Length)
                return args[index + 1];
        }
        return null;
    }
}
