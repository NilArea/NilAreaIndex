using System.Runtime.CompilerServices;

namespace NilArea.Blog;

partial class Program
{
    [ModuleInitializer]
    internal static void Initialization()
    {
        NilareaGenerator.ValidateEnvironmentVariables();
    }
}