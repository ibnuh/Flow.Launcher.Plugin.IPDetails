using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Flow.Launcher.Plugin.SharedCommands;

namespace Flow.Launcher.Plugin.IPDetails
{
    /// <inheritdoc cref="Flow.Launcher.Plugin.IAsyncPlugin" />
    public class Main : IAsyncPlugin
    {
        private PluginInitContext Context { get; set; }
        private static readonly HttpClient HttpClient = CreateHttpClient();

        /// <summary>
        /// <a href="https://www.flaticon.com/free-icons/ip" title="IP icons">IP icons created by Design Circle - Flaticon</a>
        /// </summary>
        private const string Icon = "images/icon.png";

        private static string _cacheFilePath;

        private static readonly TimeSpan CacheExpiration = TimeSpan.FromDays(1);

        private static readonly JsonSerializerOptions JsonSerializerOptions = new();

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent",
                "Flow.Launcher.Plugin.IPDetails/1.1");
            return client;
        }

        /// <inheritdoc />
        public Task InitAsync(PluginInitContext context)
        {
            Context = context;

            _cacheFilePath =
                Path.Combine(Context.CurrentPluginMetadata.PluginDirectory, "cache/ipquery_cache.json");

            Directory.CreateDirectory(Path.GetDirectoryName(_cacheFilePath)!);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<List<Result>> QueryAsync(Query query, CancellationToken cancellationToken)
        {
            var results = new List<Result>();

            try
            {
                var response = await FetchIpApiResponse(GenerateUrl(query.Search), cancellationToken);

                if (string.IsNullOrWhiteSpace(response?.Ip))
                {
                    throw new InvalidOperationException("Provider returned an empty IP address.");
                }

                var locationInfo = response.Location ?? new LocationInfo();
                var ispInfo = response.Isp ?? new IspInfo();
                var riskInfo = response.Risk ?? new RiskInfo();

                results.Add(new Result
                {
                    Title = response.Ip,
                    SubTitle = "Public IP",
                    IcoPath = Icon,
                    Action = CreateCopyAction(response.Ip),
                    Score = 99
                });

                var location = string.Join(", ",
                    new[] { locationInfo.City, locationInfo.State, locationInfo.Country }
                        .Where(l => !string.IsNullOrEmpty(l)));

                if (!string.IsNullOrEmpty(location))
                {
                    results.Add(new Result
                    {
                        Title = location,
                        SubTitle = "Location",
                        IcoPath = Icon,
                        Action = CreateCopyAction(location),
                        Score = 98
                    });
                }

                if (!string.IsNullOrEmpty(ispInfo.DisplayName))
                {
                    results.Add(new Result
                    {
                        Title = ispInfo.DisplayName,
                        SubTitle = "ISP",
                        IcoPath = Icon,
                        Action = CreateCopyAction(ispInfo.DisplayName),
                        Score = 97
                    });
                }

                if (!string.IsNullOrEmpty(ispInfo.Asn))
                {
                    results.Add(new Result
                    {
                        Title = ispInfo.Asn,
                        SubTitle = string.IsNullOrEmpty(ispInfo.Org) ? "ASN" : "ASN / " + ispInfo.Org,
                        IcoPath = Icon,
                        Action = CreateCopyAction(ispInfo.Asn),
                        Score = 96
                    });
                }

                if (!string.IsNullOrEmpty(locationInfo.Timezone))
                {
                    results.Add(new Result
                    {
                        Title = locationInfo.Timezone,
                        SubTitle = string.IsNullOrEmpty(locationInfo.LocalTimeDisplay)
                            ? "Timezone"
                            : "Timezone / " + locationInfo.LocalTimeDisplay,
                        IcoPath = Icon,
                        Action = CreateCopyAction(locationInfo.Timezone),
                        Score = 95
                    });
                }

                var flags = new (string Title, bool IsValid, string Subtitle)[]
                {
                    ("Mobile", riskInfo.IsMobile, "IP is mobile (belongs to a mobile ISP)"),
                    ("Datacenter", riskInfo.IsDatacenter, "IP belongs to a Hosting Provider / Datacenter"),
                    ("Tor", riskInfo.IsTor, "IP is a TOR exit node"),
                    ("Proxy", riskInfo.IsProxy, "IP is a proxy"),
                    ("VPN", riskInfo.IsVpn, "IP is a VPN")
                };

                results.AddRange(flags.Where(flag => flag.IsValid)
                    .Select(flag => new Result
                    {
                        Title = flag.Title,
                        SubTitle = flag.Subtitle,
                        IcoPath = Icon,
                        Action = CreateCopyAction(flag.Subtitle),
                        Score = 94
                    }));

                if (riskInfo.RiskScore > 0)
                {
                    var riskTitle = $"Risk score: {riskInfo.RiskScore}";
                    results.Add(new Result
                    {
                        Title = riskTitle,
                        SubTitle = "Provider risk score for this IP",
                        IcoPath = Icon,
                        Action = CreateCopyAction(riskTitle),
                        Score = 93
                    });
                }

                if (locationInfo.HasCoordinates)
                {
                    var googleMapsLink = GenerateGoogleMapsLink(locationInfo.LatitudeFormatted,
                        locationInfo.LongitudeFormatted);

                    results.Add(new Result
                    {
                        Title =
                            $"Latitude: {locationInfo.LatitudeFormatted}, Longitude: {locationInfo.LongitudeFormatted}",
                        SubTitle = "Click to view coordinate on Google Maps",
                        IcoPath = Icon,
                        Action = CreateOpenBrowserAction(googleMapsLink),
                    });
                }
            }
            catch (Exception ex)
            {
                results.Add(new Result
                {
                    Title = "An error occurred while fetching IP details",
                    SubTitle = ex.Message,
                    IcoPath = Icon,
                    Action = CreateCopyAction(ex.Message)
                });
            }

            return results;
        }

        private static Func<ActionContext, bool> CreateCopyAction(string text)
        {
            return _ =>
            {
                Clipboard.SetDataObject(text);
                return false;
            };
        }

        private static Func<ActionContext, bool> CreateOpenBrowserAction(string link)
        {
            return _ =>
            {
                link.OpenInBrowserTab();
                return true;
            };
        }

        private static string GenerateGoogleMapsLink(string latitude, string longitude)
        {
            return $"https://www.google.com/maps?q={latitude},{longitude}";
        }

        private static string GenerateUrl(string ip)
        {
            var (isValid, ipFormatted) = IsValidIPv4(ip);

            if (string.IsNullOrEmpty(ip) || !isValid)
            {
                return "https://api.ipquery.io/?format=json";
            }

            return $"https://api.ipquery.io/{ipFormatted}";
        }

        private static (bool, string) IsValidIPv4(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
            {
                return (false, string.Empty);
            }

            var splitValues = ipString.Split('.');

            if (splitValues.Length != 4)
            {
                return (false, string.Empty);
            }

            if (!IPAddress.TryParse(ipString, out var ipAddress))
            {
                return (false, string.Empty);
            }

            if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return (false, string.Empty);
            }

            if (IsPrivateOrBogonIp(ipAddress))
            {
                return (false, string.Empty);
            }

            return (true, ipAddress.ToString());
        }

        private static async Task<IpApiResponse> FetchIpApiResponse(string url, CancellationToken cancellationToken)
        {
            if (TryGetCachedResponse(url, out IpApiResponse cachedResponse))
            {
                return cachedResponse;
            }

            var responseString = await HttpClient.GetStringAsync(url, cancellationToken);

            var apiResponse = JsonSerializer.Deserialize<IpApiResponse>(responseString, JsonSerializerOptions);

            if (apiResponse == null || string.IsNullOrWhiteSpace(apiResponse.Ip))
            {
                throw new InvalidOperationException("Could not parse IP details from provider.");
            }

            CacheResponse(url, apiResponse);

            return apiResponse;
        }

        private static bool TryGetCachedResponse(string url, out IpApiResponse cachedResponse)
        {
            cachedResponse = null;

            if (!File.Exists(_cacheFilePath))
            {
                return false;
            }

            Dictionary<string, CachedIpApiResponse> cacheData;
            try
            {
                cacheData = JsonSerializer.Deserialize<Dictionary<string, CachedIpApiResponse>>(
                    File.ReadAllText(_cacheFilePath), JsonSerializerOptions);
            }
            catch (JsonException)
            {
                try { File.Delete(_cacheFilePath); } catch { }
                return false;
            }

            if (cacheData == null || !cacheData.TryGetValue(url, out var cachedEntry))
            {
                return false;
            }

            // Remove all expired cache entries
            var keysToRemove = new List<string>();
            foreach (var (key, value) in cacheData)
            {
                if (DateTime.UtcNow - value.Timestamp >= CacheExpiration)
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                cacheData.Remove(key);
            }

            if (DateTime.UtcNow - cachedEntry.Timestamp < CacheExpiration)
            {
                cachedResponse = cachedEntry.Response;
                return true;
            }

            File.WriteAllText(_cacheFilePath, JsonSerializer.Serialize(cacheData, JsonSerializerOptions));

            return false;
        }

        private static void CacheResponse(string url, IpApiResponse response)
        {
            Dictionary<string, CachedIpApiResponse> cacheData;

            if (File.Exists(_cacheFilePath))
            {
                try
                {
                    cacheData = JsonSerializer.Deserialize<Dictionary<string, CachedIpApiResponse>>(
                        File.ReadAllText(_cacheFilePath), JsonSerializerOptions) ?? new Dictionary<string, CachedIpApiResponse>();
                }
                catch (JsonException)
                {
                    cacheData = new Dictionary<string, CachedIpApiResponse>();
                }
            }
            else
            {
                cacheData = new Dictionary<string, CachedIpApiResponse>();
            }

            cacheData[url] = new CachedIpApiResponse
            {
                Timestamp = DateTime.UtcNow,
                Response = response
            };

            File.WriteAllText(_cacheFilePath, JsonSerializer.Serialize(cacheData, JsonSerializerOptions));
        }

        private static bool IsPrivateOrBogonIp(IPAddress ip)
        {
            var bytes = ip.GetAddressBytes();
            switch (bytes[0])
            {
                case 10 or 127:
                case 172 when bytes[1] >= 16 && bytes[1] <= 31:
                case 192 when bytes[1] == 168:
                case 169 when bytes[1] == 254:
                case 100 when bytes[1] >= 64 && bytes[1] <= 127:
                case 198 when bytes[1] == 18 || bytes[1] == 19 || bytes[1] == 51 || bytes[1] == 52:
                case 203 when bytes[1] == 0 && bytes[2] == 113:
                case 240 or 255:
                    return true;
                default:
                    return false;
            }
        }
    }
}
