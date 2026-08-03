using RealStatePortal.Application.Abstractions.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace RealStatePortal.Infrastructure.Storage;

public sealed class LocalImageStorage(IConfiguration configuration, IHostEnvironment hostEnvironment) : IImageStorage
{
    private const string DefaultRelativePath = "wwwroot/uploads";

    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        var extension = Path.GetExtension(fileName);
        var safeExtension = string.IsNullOrWhiteSpace(extension) ? ".bin" : extension.ToLowerInvariant();
        var generatedName = $"{Guid.NewGuid():N}{safeExtension}";
        var relativePath = configuration["Storage:LocalUploadPath"] ?? DefaultRelativePath;
        var directory = ResolveDirectory(relativePath);
        Directory.CreateDirectory(directory);
        var destination = Path.Combine(directory, generatedName);

        await using var output = File.Create(destination);
        await content.CopyToAsync(output, cancellationToken);
        return $"/uploads/{generatedName}";
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(url);
        var relativePath = configuration["Storage:LocalUploadPath"] ?? DefaultRelativePath;
        var path = Path.Combine(ResolveDirectory(relativePath), fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    private string ResolveDirectory(string relativePath) =>
        Path.GetFullPath(Path.Combine(hostEnvironment.ContentRootPath, relativePath));
}