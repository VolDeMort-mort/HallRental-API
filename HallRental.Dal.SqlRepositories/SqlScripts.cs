using System.Collections.Concurrent;

namespace HallRental.Dal.SqlRepositories;

internal static class SqlScripts
{
    private static readonly ConcurrentDictionary<string, string> Cache = new();

    public static string Get<TRepository>(string name)
    {
        var resource = $"{typeof(TRepository).Namespace}.Sql.{name}.sql";

        return Cache.GetOrAdd(resource, key =>
        {
            using var stream = typeof(SqlScripts).Assembly.GetManifestResourceStream(key)
                ?? throw new InvalidOperationException($"SQL resource '{key}' was not found");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        });
    }
}
