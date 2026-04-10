namespace SnapMyBook.Application.Highlights;

public class OcrWorkflowService : IOcrWorkflowService
{
    public OcrResultDto NormalizeClientResult(string? rawText, decimal? confidence, string? language, string? error)
    {
        var normalizedText = rawText?.Trim() ?? string.Empty;
        var normalizedLanguage = string.IsNullOrWhiteSpace(language) ? "por" : language.Trim();
        var failed = !string.IsNullOrWhiteSpace(error) || string.IsNullOrWhiteSpace(normalizedText);

        return new OcrResultDto(normalizedText, confidence, normalizedLanguage, failed, error);
    }
}
