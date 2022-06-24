using Metadata.Models;

namespace Metadata.Services
{
    public interface IMetadatasService
    {
        IAsyncEnumerable<FileItemDto> BrowseAsync(string? token, CancellationToken cancellationToken);
        Task<string?> GetNextTokenAsync(string? token, CancellationToken cancellationToken);
    }
}
