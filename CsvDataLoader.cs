using System.Globalization;

namespace KfcQueueModels;

public static class CsvDataLoader
{
    public static List<CustomerObservation> LoadTandemCsv(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Could not find input data CSV: {path}", path);

        var records = new List<CustomerObservation>();
        var lines = File.ReadAllLines(path);
        if (lines.Length == 0)
            throw new InvalidDataException("The input CSV is empty.");

        for (var lineIndex = 1; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex].Trim();
            if (line.Length == 0) continue;

            var columns = line.Split(',');
            if (columns.Length < 5)
                throw new InvalidDataException($"CSV line {lineIndex + 1} has fewer than five columns.");

            int id = ParseInt(columns[0], lineIndex);
            double arrival = ParseDouble(columns[1], lineIndex);
            double q1Start = ParseDouble(columns[2], lineIndex);
            double q1End = ParseDouble(columns[3], lineIndex);
            double q2End = ParseDouble(columns[4], lineIndex);

            if (arrival < 0 || q1Start < arrival || q1End < q1Start || q2End < q1End)
                throw new InvalidDataException($"CSV line {lineIndex + 1} has inconsistent event times for customer {id}.");

            records.Add(new CustomerObservation(id, arrival, q1Start, q1End, q2End));
        }

        if (records.Count == 0)
            throw new InvalidDataException("The CSV contains no customer observations.");

        return records.OrderBy(record => record.Arrival).ThenBy(record => record.Id).ToList();
    }

    private static int ParseInt(string value, int lineIndex) =>
        int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : throw new InvalidDataException($"Invalid customer ID at CSV line {lineIndex + 1}.");

    private static double ParseDouble(string value, int lineIndex) =>
        double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result) && double.IsFinite(result)
            ? result
            : throw new InvalidDataException($"Invalid numeric value at CSV line {lineIndex + 1}.");
}
