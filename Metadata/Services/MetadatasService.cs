using HashidsNet;
using Metadata.Models;
using System.Runtime.CompilerServices;

namespace Metadata.Services
{
    public sealed class MetadatasService : IMetadatasService
    {
        private const int NumberOfResultsPerRequest = 10;

        private readonly Hashids _tokenProvider = new();
        private readonly Hashids _fileIdProvider = new("1234567", 7);

        public async IAsyncEnumerable<FileItemDto> BrowseAsync(string? token, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!_tokenProvider.TryDecodeSingle(token, out var rawPage))
            {
                rawPage = 1;
            }

            for (int i = rawPage; i < rawPage + NumberOfResultsPerRequest; i++)
            {
                var rawFileId = i * 673;
                string fileId = _fileIdProvider.Encode(rawFileId);

                yield return new FileItemDto(fileId, rawFileId);
            }
        }

        public async Task<string?> GetNextTokenAsync(string? token, CancellationToken cancellationToken)
        {
            _tokenProvider.TryDecodeSingle(token, out var rawPage);
            return _tokenProvider.Encode(rawPage + NumberOfResultsPerRequest);
        }
    }
}
