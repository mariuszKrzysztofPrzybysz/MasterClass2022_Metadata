using Metadata.Models;
using Metadata.Services;
using Microsoft.AspNetCore.Mvc;

namespace Metadata.Controllers
{
    [Route("api/metadatas/v1")]
    [ApiController]
    public class MetadatasController : ControllerBase
    {
        private readonly IMetadatasService _service;

        public MetadatasController(IMetadatasService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> BrowseAsync(string? token, CancellationToken cancellationToken)
        {
            var result = new PagedResult<FileItemDto>
            {
                NextToken = await _service.GetNextTokenAsync(token, cancellationToken),
                Results = await _service.BrowseAsync(token, cancellationToken)
                .ToListAsync(cancellationToken)
            };

            return Ok(result);
        }
    }
}
