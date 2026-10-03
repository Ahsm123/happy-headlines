using HappyHeadlines.Contracts.Articles;
using Microsoft.AspNetCore.Mvc;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Caching;
using HappyHeadlines.ArticleApi.Extensions;
using Microsoft.EntityFrameworkCore;
using HappyHeadlines.ServiceDefaults;

namespace HappyHeadlines.ArticleApi.Controllers;

[ApiController]
[Route("api/v1/regions/{region}/[controller]")]
public class ArticlesController(
    Coordinator coordinator,
    ArticleCache cache) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ArticleDto>> CreateArticle(Region region, Article a)
    {
        if (region != a.Region)
        {
            return BadRequest("Region mismatch");
        }

        await using var db = coordinator.GetArticleDbContext(region);
        db.Articles.Add(a);
        await db.SaveChangesAsync();

        MonitorService.Log.Here().Information("Created article with ID: {ArticleId} in {Region}", a.Id, region);

        var dto = a.ToDto();

        return CreatedAtAction(nameof(GetArticle), new { region, id = dto.Id }, dto);
    }

    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<ArticleDto>> GetArticle(Guid id, Region region)
    {
        var article = await cache.GetArticle(id, region);
        if (article == null)
        {
            return NotFound();
        }

        return Ok(article.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetArticles(Region region, DateTime? fromDate)
    {
        await using var db = coordinator.GetArticleDbContext(region);

        var query = db.Articles.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.PublishDate >= fromDate.Value);
        }

        var articles = await query.ToListAsync();
        var dtos = articles.Select(a => a.ToDto()).ToList();

        return Ok(dtos);
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> UpdateArticle(Guid id, Region region, Article a)
    {
        if (region != a.Region)
        {
            return BadRequest("Region mismatch");
        }

        await using var db = coordinator.GetArticleDbContext(region);

        var existing = await db.Articles.FindAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.Title = a.Title;
        existing.Content = a.Content;
        existing.Author = a.Author;
        existing.PublishDate = a.PublishDate;

        await db.SaveChangesAsync();
        MonitorService.Log.Here().Information("Updated article with ID: {ArticleId} in {Region}", existing.Id, region);

        return NoContent();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteArticle(Guid id, Region region)
    {
        await using var db = coordinator.GetArticleDbContext(region);

        var article = await db.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        db.Articles.Remove(article);
        await db.SaveChangesAsync();
        MonitorService.Log.Here().Information("Deleted article with ID: {ArticleId} in {Region}", article.Id, region);

        return NoContent();
    }

    [HttpGet]
    [Route("/api/v1/newest/[Controller]")]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetNewestArticles(int count)
    {
        var articles = new List<ArticleDto>();
        foreach (var region in Enum.GetValues<Region>())
        {
            await using var db = coordinator.GetArticleDbContext(region);
            var regionArticles = await db.Articles.ToListAsync();
            articles.AddRange(regionArticles.Select(a => a.ToDto()));
        }

        var newestArticles = articles.OrderByDescending(a => a.PublishDate).Take(count);
        return Ok(newestArticles);
    }
}