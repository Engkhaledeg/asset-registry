using AssetRegistry.Api.Contracts;
using AssetRegistry.Application.Assets;
using AssetRegistry.Application.Assets.Dtos;
using AssetRegistry.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace AssetRegistry.Api.Controllers;

[ApiController]
[Authorize]
[RequiredScope("access_as_user")]
[Route("api/assets")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assets;

    public AssetsController(IAssetService assets)
    {
        _assets = assets;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<AssetDto>>> Search(
        [FromQuery] AssetSearchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _assets.SearchAsync(request.ToCriteria(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AssetDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var asset = await _assets.GetByIdAsync(id, cancellationToken);
        return Ok(asset);
    }

    [HttpPost]
    public async Task<ActionResult<AssetDto>> Create(
        [FromBody] CreateAssetRequest request,
        CancellationToken cancellationToken)
    {
        var asset = await _assets.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AssetDto>> Update(
        int id,
        [FromBody] UpdateAssetRequest request,
        CancellationToken cancellationToken)
    {
        var asset = await _assets.UpdateAsync(id, request, cancellationToken);
        return Ok(asset);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _assets.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
