namespace NilArea.Grains.Services;

public interface IAsyncLifetime
{
    /// <summary>
    ///     Called immediately after the class has been created, before it is used.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    ///     Called when an object is no longer needed. Called just before <see cref="IDisposable.Dispose" />
    ///     if the class also implements that.
    /// </summary>
    Task DisposeAsync();
}
