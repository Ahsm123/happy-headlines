using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.ArticleApi.Extensions;

public static class ArticleMappingExtensions
{
    public static ArticleDto ToDto(this Article article) =>
        new(
            article.Id,
            article.Title,
            article.Content,
            article.Author,
            article.PublishDate,
            article.Region
        );
}
