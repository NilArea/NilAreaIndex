namespace NilArea.Contracts.Annotation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field |
                AttributeTargets.Class | AttributeTargets.Struct |
                AttributeTargets.Method | AttributeTargets.Parameter,
    AllowMultiple = true)]
public sealed class RequireEnvironmentVariableAttribute(
    string EnvironmentVariableName) : Attribute
{
    public string? ErrorMessage { get; set; }
    public string? DefaultValue { get; set; }
    public bool FailFast { get; set; } = true;
}