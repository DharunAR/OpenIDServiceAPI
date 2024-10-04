using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAPIOpenIDConnect
{
    public class FileCache
    {
        public static FileCache GetUserCache(string cacheFilePath)
        {
            return new FileCache(cacheFilePath);
        }

        private readonly string CacheFilePath;
        private readonly object FileLock = new object();

        private FileCache(string cacheFilePath)
        {
            CacheFilePath = cacheFilePath;
        }

        public void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            try
            {
                lock (FileLock)
                {
                    args.TokenCache.DeserializeMsalV3(File.Exists(CacheFilePath)
                        ? File.ReadAllBytes(CacheFilePath)
                        : null);
                }
            }
            catch (MsalClientException)
            {
                lock (FileLock)
                {
                    //The cache file deserialization would have failed, create a new token cache.
                    File.Delete(CacheFilePath);
                }
            }
        }

        public void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                lock (FileLock)
                {
                    // Reflect changes in the persistent store
                    File.WriteAllBytes(CacheFilePath, args.TokenCache.SerializeMsalV3());
                }
            }
        }
    }
}
