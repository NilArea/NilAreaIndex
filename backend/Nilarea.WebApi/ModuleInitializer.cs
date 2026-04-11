using System.Runtime.CompilerServices;

internal partial class Program
{
    [ModuleInitializer]
    internal static void Initialization()
    {
        NilareaGenerator.ValidateEnvironmentVariables();
    }
}