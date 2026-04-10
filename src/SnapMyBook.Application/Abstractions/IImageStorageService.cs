namespace SnapMyBook.Application.Abstractions;

public interface IImageStorageService
{
    Task<string?> SaveBase64ImageAsync(string? base64Image, CancellationToken cancellationToken = default);
}
