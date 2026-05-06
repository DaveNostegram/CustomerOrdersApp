namespace CustomerOrdersApp.Application.FileUploads;

public sealed class HeaderValidationResult
{
    public bool IsValid => MissingHeaders.Count == 0 && DuplicateHeaders.Count == 0;

    public List<string> MissingHeaders { get; } = [];
    public List<string> DuplicateHeaders { get; } = [];
    public List<string> Errors { get; } = [];
}