namespace RealStatePortal.Application.Abstractions.Storage;

public interface IImageStorage
{
    Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string url, CancellationToken cancellationToken = default);
}