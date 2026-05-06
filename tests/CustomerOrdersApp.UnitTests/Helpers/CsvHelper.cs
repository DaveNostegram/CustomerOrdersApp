using System.Text;

namespace CustomerOrdersApp.UnitTests;

public static class CsvHelper
{
    public static Stream CreateCsvStream(string[][] rows)
    {
        var csv = string.Join(
            Environment.NewLine,
            rows.Select(row => string.Join(",", row)));

        return new MemoryStream(Encoding.UTF8.GetBytes(csv));
    }
}