using System.Runtime.CompilerServices;

namespace NilArea.Account;

partial class Program
{
    [ModuleInitializer]
    internal static void Initialization()
    {
        NilareaGenerator.ValidateEnvironmentVariables();
    }
}