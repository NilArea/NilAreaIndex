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
    }
}