using HappyHeadlines.Contracts.Articles;
using Microsoft.AspNetCore.Mvc;
using HappyHeadlines.ArticleApi.Extensions;
using HappyHeadlines.ArticleApi.Services;

namespace HappyHeadlines.ArticleApi.Controllers;

[ApiController]
[Route("api/v1/regions/{region}/articles")]
public class ArticlesController(IArticleService articleService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDto>> GetById(Region region, Guid id, CancellationToken ct)
    {
        var article = await articleService.GetByIdAsync(region, id, ct);
        if (article is null)
        {
            return NotFound();
        }

        return Ok(article.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll(Region region, DateTime? fromDate,
        CancellationToken ct)
    {
        var articles = await articleService.GetAllAsync(region, fromDate, ct);

        return Ok(articles.Select(a => a.ToDto()));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Region region, Guid id, UpdateArticleRequest request,
        CancellationToken ct)
    {
        var updated = await articleService.UpdateAsync(region, id, request, ct);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Region region, Guid id, CancellationToken ct)
    {
        var deleted = await articleService.DeleteAsync(region, id, ct);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("/api/v1/latest/articles")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetLatest(int count, CancellationToken ct)
    {
        var articles = await articleService.GetLatestAsync(count, ct);

        return Ok(articles.Select(a => a.ToDto()));
    }
}
