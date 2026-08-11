using RealStatePortal.Application.Abstractions.Storage;
using Microsoft.Extensions.Configuration;

namespace RealStatePortal.Infrastructure.Storage;

public sealed class LocalImageStorage(IConfiguration configuration) : IImageStorage
{
    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        var safeFileName = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            throw new ArgumentException("A valid file name is required.", nameof(fileName));
        }

        var relativePath = configuration["Storage:LocalPath"] ?? Path.Combine("wwwroot", "uploads");
        var directory = Path.GetFullPath(relativePath);
        Directory.CreateDirectory(directory);
        var storedFileName = $"{Guid.NewGuid():N}{Path.GetExtension(safeFileName)}";
        var filePath = Path.Combine(directory, storedFileName);

        await using var file = File.Create(filePath);
        await content.CopyToAsync(file, cancellationToken);
        return $"/uploads/{storedFileName}";
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(url);
        var relativePath = configuration["Storage:LocalPath"] ?? Path.Combine("wwwroot", "uploads");
        var filePath = Path.Combine(Path.GetFullPath(relativePath), fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}