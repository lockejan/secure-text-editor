using System;
using System.IO;

namespace SecureTextEditor.FileHandler
{
    /// <summary>
    /// Static class holding helper functions needed by file handlers or views. 
    /// </summary>
    public static class SteHelper
    {
        /// <summary>
        /// Function that returns the full path of the project root DIR.
        /// </summary>
        public static String WorkingDirectory
        {
            get
            {
                string codeBase = Directory.GetCurrentDirectory() + "/../../../../";
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return $"{Path.GetDirectoryName(path)}/";
            }
        }
    }
}