using System.Collections.Generic;

namespace Sling.Utils
{
  public static class CollectionsExtensions
  {
    public static List<TValue> GetOrAdd<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key)
    {
      if (!dict.TryGetValue(key, out List<TValue> list))
      {
        list = new List<TValue>();
        dict[key] = list;
      }

      return list;
    }
  }
}