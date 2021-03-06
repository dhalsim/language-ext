﻿using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

public static class __TryOutExt
{
    /// <summary>
    /// Get a value out of a dictionary as Some, otherwise None.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    /// <param name="self">Dictionary</param>
    /// <param name="key">Key</param>
    /// <returns>OptionT filled Some(value) or None</returns>
    public static Option<V> TryGetValue<K, V>(this IDictionary<K, V> self, K key)
    {
        V value;
        return self.TryGetValue(key, out value)
            ? Some(value)
            : None;
    }

    /// <summary>
    /// Get a value out of a dictionary as Some, otherwise None.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    /// <param name="self">Dictionary</param>
    /// <param name="key">Key</param>
    /// <returns>OptionT filled Some(value) or None</returns>
    public static Option<V> TryGetValue<K, V>(this IReadOnlyDictionary<K, V> self, K key)
    {
        V value;
        return self.TryGetValue(key, out value)
            ? Some(value)
            : None;
    }
}
