using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.ArticleApi.Extensions;

public static class ArticleMappingExtensions
{
    public static ArticleDto ToDto(this Article article) =>
        new()
        {
            Id = article.Id,
            Author = article.Author,
            Content = article.Content,
            PublishDate = article.PublishDate,
            Title = article.Title,
            Region = article.Region
        };
}
