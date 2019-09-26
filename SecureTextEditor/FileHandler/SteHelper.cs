using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SecureTextEditor.FileHandler
{
    public static class SteHelper
    {
        /// <summary>
        /// Helper function that returns null if key is not present in dict.
        /// Prevents exceptions when instantiating json model.
        /// </summary>
        /// <param name="dictionary">result object coming from cipher engine</param>
        /// <param name="key">key which will looked after</param>
        /// <typeparam name="TKey">KeyType</typeparam>
        /// <typeparam name="TValue">ValueType</typeparam>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey,TValue>
            (this IDictionary<TKey, TValue> dictionary, TKey key) =>
            dictionary.TryGetValue(key, out var ret) ? ret : default;
        
        /// <summary>
        /// Interface which can be used by other classes to get back the full path of the working DIR.
        /// </summary>
        public static String WorkingDirectory
        {
            get
            {
                string codeBase = Directory.GetCurrentDirectory() + "/../../../../";
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}