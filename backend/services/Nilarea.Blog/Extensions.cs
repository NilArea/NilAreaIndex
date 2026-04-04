using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace NilArea.Blog;

public static class Extensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddBlogValidator()
        {
            return collection
                .AddValidatorsFromAssembly(typeof(Extensions).Assembly);
        }
    }
}