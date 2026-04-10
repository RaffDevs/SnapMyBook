using Microsoft.AspNetCore.Hosting;
using SnapMyBook.Application.Abstractions;

namespace SnapMyBook.Infrastructure.Services;

public class LocalImageStorageService : IImageStorageService
{
    private readonly string _uploadRoot;

    public LocalImageStorageService(IWebHostEnvironment environment)
    {
        _uploadRoot = Path.Combine(environment.WebRootPath, "uploads", "highlights");
        Directory.CreateDirectory(_uploadRoot);
    }

    public async Task<string?> SaveBase64ImageAsync(string? base64Image, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(base64Image))
        {
            return null;
        }

        var parts = base64Image.Split(',', 2);
        if (parts.Length != 2)
        {
            return null;
        }

        var bytes = Convert.FromBase64String(parts[1]);
        var fileName = $"{Guid.NewGuid():N}.jpg";
        var filePath = Path.Combine(_uploadRoot, fileName);
        await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);
        return $"/uploads/highlights/{fileName}";
    }
}
