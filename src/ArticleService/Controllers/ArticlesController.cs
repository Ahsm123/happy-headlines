using Microsoft.AspNetCore.Mvc;
using ArticleService.Models;
using ArticleService.Data;

namespace ArticleService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ArticlesController(ArticleDbContext db) : ControllerBase
{
    [HttpPost()]
    public async Task<ActionResult<Article>> CreateArticle(Article a)
    {
        db.Articles.Add(a);
        await db.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetArticle), new { id = a.Id }, a);
    }

    [HttpGet("{int id}")]
    public async Task<ActionResult<Article>> GetArticle(int id)
    {
        var article = await db.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound();
        }
        
        return article;
    }

    [HttpPut("{int id}")]
    public async Task<IActionResult> UpdateArticle(int id, Article a)
    {
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

    [HttpDelete("{int id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
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
