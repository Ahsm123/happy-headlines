using Microsoft.AspNetCore.Mvc;
using ArticleService.Models;
using ArticleService.Data;

namespace ArticleService.Controllers;

[ApiController]
[Route("api/v1/regions/{region}/[controller]")]
public class ArticlesController(Coordinator coordinator) : ControllerBase
{
    [HttpPost()]
    public async Task<ActionResult<Article>> CreateArticle(Region region, Article a)
    {
        if (region != a.Region)
        {
            return BadRequest("Region mismatch");
        }

        await using var db = coordinator.GetArticleDbContext(region);
        db.Articles.Add(a);
        await db.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetArticle), new { region, id = a.Id }, a);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Article>> GetArticle(int id, Region region)
    {
        await using var db = coordinator.GetArticleDbContext(region);
        
        var article = await db.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound();
        }
        
        return article;
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateArticle(int id, Region region, Article a)
    {
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

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteArticle(int id, Region region)
    {
        await using var db = coordinator.GetArticleDbContext(region);
        
        var article = await db.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        db.Articles.Remove(article);
        await db.SaveChangesAsync();

        return NoContent();
    }


}
