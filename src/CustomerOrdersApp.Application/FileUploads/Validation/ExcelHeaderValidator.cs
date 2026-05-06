namespace CustomerOrdersApp.Application.FileUploads;

public static class FileHeaderValidator
{
    public static HeaderValidationResult ValidateHeaders(
        IEnumerable<string> actualHeaders,
        IEnumerable<string> requiredHeaders)
    {
        var result = new HeaderValidationResult();

        var cleanedHeaders = actualHeaders
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToList();

        var duplicateHeaders = cleanedHeaders
            .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        result.DuplicateHeaders.AddRange(duplicateHeaders);

        var actualHeaderSet = cleanedHeaders
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var requiredHeader in requiredHeaders)
        {
            if (!actualHeaderSet.Contains(requiredHeader))
            {
                result.MissingHeaders.Add(requiredHeader);
            }
        }

        return result;
    }
}