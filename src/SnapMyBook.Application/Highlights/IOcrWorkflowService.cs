namespace SnapMyBook.Application.Highlights;

public interface IOcrWorkflowService
{
    OcrResultDto NormalizeClientResult(string? rawText, decimal? confidence, string? language, string? error);
}
