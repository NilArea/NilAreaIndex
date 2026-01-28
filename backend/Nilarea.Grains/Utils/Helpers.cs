using NilArea.Grains.Services;
using NilArea.Interfaces.IGrains;

namespace NilArea.Grains.Utils;

public static class Helpers
{
    extension(IEmailServices emailServices)
    {
        public async ValueTask SendConfirmKeyAsync(string targetEmail, string confirmKey, ConfirmKey keyType)
        {
            //TODO 发送邮件
        }
    }
}
