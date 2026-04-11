using Microsoft.Extensions.Configuration;

namespace NilArea.Common;

public static partial class Extensions
{
    extension(IConfiguration configuration)
    {
        public string SafeGetConfigureValue(string key)
        {
            return configuration[key] ??
                   throw new KeyNotFoundException($"\"{key}\" is a required configure key");
        }

        public string SafeGetConfigureValue(string key, string defaultValue)
        {
            return configuration[key] ?? defaultValue;
        }

        public T SafeGetConfigureValue<T>(string key)
        {
            return configuration.GetValue<T>(key) ??
                   throw new KeyNotFoundException($"\"{key}\" is a required configure key");
        }

        public T SafeGetConfigureValue<T>(string key, T defaultValue)
        {
            return configuration.GetValue<T>(key) ?? defaultValue;
        }

        public string SafeGetConnectionString(string key)
        {
            return configuration.GetConnectionString(key) ??
                   throw new KeyNotFoundException($"\"{key}\" is a required configure key");
        }

        public string SafeGetConnectionString(string key, string defaultValue)
        {
            return configuration.GetConnectionString(key) ?? defaultValue;
        }

        public string GetSecretFromFile(string key, string defaultSuffix = "_FILE")
        {
            var secret = configuration[key];
            if (string.IsNullOrWhiteSpace(secret)) return secret;
            var path = configuration[key + defaultSuffix];
            if (string.IsNullOrWhiteSpace(path))
                throw new KeyNotFoundException($"\"{key}\" is a required configure key");
            try
            {
                return File.ReadAllText(Path.GetFullPath(path));
            }
            catch (Exception _)
            {
                throw new KeyNotFoundException(
                    $"\"{key}\" is a required configure key, but the file \"{path}\" is not found");
            }
        }
    }
}