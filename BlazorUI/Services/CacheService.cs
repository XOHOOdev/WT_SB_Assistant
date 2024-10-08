namespace Sparta.BlazorUI.Services;

public static class CacheService
{
    private static Dictionary<string, string> cache = null!;

    private static readonly object cacheLock = new();

    public static Dictionary<string, string> AppCache
    {
        get
        {
            lock (cacheLock)
            {
                cache ??= new Dictionary<string, string>();
                return cache;
            }
        }
    }
}