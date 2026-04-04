using NilArea.Blog.States;
using NilArea.Contracts.Grains.Blog;

namespace NilArea.Blog.Grains;

public class BlogGrain : Grain<BlogStates>, IBlogGrain
{
}