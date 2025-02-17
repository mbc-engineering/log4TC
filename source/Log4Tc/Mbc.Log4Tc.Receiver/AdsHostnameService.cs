﻿using Microsoft.Extensions.Logging;
using Optional;
using System;
using System.Collections.Concurrent;
using System.Text;
using TwinCAT.Ads;

namespace Mbc.Log4Tc.Receiver
{
    /// <summary>
    /// Provides hostnames for ADS addresses.
    /// </summary>
    public class AdsHostnameService
    {
        private readonly ConcurrentDictionary<string, Option<string>> _cachedHostnames = new ConcurrentDictionary<string, Option<string>>();
        private readonly ILogger<AdsHostnameService> _logger;

        public AdsHostnameService(ILogger<AdsHostnameService> logger)
        {
            _logger = logger;
        }

        public Option<string> GetHostname(AmsNetId amsNetId)
        {
            return _cachedHostnames.GetOrAdd(amsNetId.ToString(), x => QueryHostname(AmsNetId.Parse(x)).SomeNotNull());
        }

        private string QueryHostname(AmsNetId amsNetId)
        {
            using (var client = new AdsClient(AdsClientSettings.Default))
            {
                client.Connect(amsNetId, AmsPort.SystemService);

                var readBuffer = new Memory<byte>(new byte[256]);
                var error = client.TryRead(702 /*SYSTEMSERVICE_IPHOSTNAME*/, 0, readBuffer, out int readBytes);
                if (error != AdsErrorCode.NoError)
                {
                    _logger.LogWarning("Error {error} query hostname for {amsNetId}.", error, amsNetId);
                    return null;
                }

                var hostname = Encoding.GetEncoding(1252).GetString(readBuffer.ToArray(), 0, readBytes - 1);
                return hostname;
            }
        }
    }
}
