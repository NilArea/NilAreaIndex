using Microsoft.Extensions.Logging;
using NilArea.Common.Services;
using NilArea.Contracts.Enums;

namespace NilArea.Account.Infrastructure.ExternalServices;

public interface IEmailServices : IAsyncLifetime
{
    ValueTask SendEmailAsync(string targetEmail, string title, string message);
}

public static class Helpers
{
    extension(IEmailServices emailServices)
    {
        public async ValueTask<bool> SendConfirmKeyAsync(string targetEmail, string confirmKey, ConfirmType typeType)
        {
            //TODO 发送邮件
            return true;
        }
    }
}

public class EmailServices(ILogger<EmailServices> logger) : IEmailServices
{
    public Task InitializeAsync()
    {
        logger.LogInformation("Initializing email services...");
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        logger.LogInformation("Disposing email services...");
        return Task.CompletedTask;
    }


    public ValueTask SendEmailAsync(string targetEmail, string title, string message)
    {
        return ValueTask.CompletedTask;
    }
}