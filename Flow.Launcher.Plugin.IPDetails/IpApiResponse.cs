using System;
using System.Globalization;
using System.Text.Json.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Flow.Launcher.Plugin.IPDetails;

/// <summary>
/// Represents the data for the IP API response cache.
/// </summary>
public class CachedIpApiResponse
{
    /// <summary>
    /// The time when the IP API response was cached.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The ipapi.is response.
    /// </summary>
    public IpApiResponse Response { get; set; }
}

/// <summary>
/// The response from the ipapi.is API.
/// </summary>
public class IpApiResponse
{
    [JsonPropertyName("ip")] public string Ip { get; set; }

    [JsonPropertyName("rir")] public string Rir { get; set; }

    [JsonPropertyName("is_bogon")] public bool IsBogon { get; set; }

    [JsonPropertyName("is_mobile")] public bool IsMobile { get; set; }

    [JsonPropertyName("is_crawler")] public bool IsCrawler { get; set; }

    [JsonPropertyName("is_datacenter")] public bool IsDatacenter { get; set; }

    [JsonPropertyName("is_tor")] public bool IsTor { get; set; }

    [JsonPropertyName("is_proxy")] public bool IsProxy { get; set; }

    [JsonConverter(typeof(IsVpnConverter))]
    [JsonPropertyName("is_vpn")] public object IsVpn { get; set; }

    [JsonPropertyName("is_abuser")] public bool IsAbuser { get; set; }

    [JsonPropertyName("company")] public CompanyInfo Company { get; set; }

    [JsonPropertyName("abuse")] public AbuseInfo Abuse { get; set; }

    [JsonPropertyName("asn")] public AsnInfo Asn { get; set; }

    [JsonPropertyName("location")] public LocationInfo Location { get; set; }

    [JsonPropertyName("elapsed_ms")] public double ElapsedMs { get; set; }
}

public class CompanyInfo
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("abuser_score")] public string AbuserScore { get; set; }

    [JsonPropertyName("domain")] public string Domain { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("network")] public string Network { get; set; }

    [JsonPropertyName("whois")] public string Whois { get; set; }
}

public class AbuseInfo
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("address")] public string Address { get; set; }

    [JsonPropertyName("email")] public string Email { get; set; }

    [JsonPropertyName("phone")] public string Phone { get; set; }
}

public class AsnInfo
{
    [JsonPropertyName("asn")] public int Asn { get; set; }

    [JsonPropertyName("abuser_score")] public string AbuserScore { get; set; }

    [JsonPropertyName("route")] public string Route { get; set; }

    [JsonPropertyName("descr")] public string Descr { get; set; }

    [JsonPropertyName("country")] public string Country { get; set; }

    [JsonPropertyName("active")] public bool Active { get; set; }

    [JsonPropertyName("org")] public string Org { get; set; }

    [JsonPropertyName("domain")] public string Domain { get; set; }

    [JsonPropertyName("abuse")] public string Abuse { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("updated")] public string Updated { get; set; }

    [JsonPropertyName("rir")] public string Rir { get; set; }

    [JsonPropertyName("whois")] public string Whois { get; set; }
}

public class LocationInfo
{
    [JsonPropertyName("continent")] public string Continent { get; set; }

    [JsonPropertyName("country")] public string Country { get; set; }

    [JsonPropertyName("country_code")] public string CountryCode { get; set; }

    [JsonPropertyName("state")] public string State { get; set; }

    [JsonPropertyName("city")] public string City { get; set; }

    [JsonPropertyName("latitude")] public double Latitude { get; set; }

    public string LatitudeFormatted => Latitude.ToString(CultureInfo.InvariantCulture);

    [JsonPropertyName("longitude")] public double Longitude { get; set; }

    public string LongitudeFormatted => Longitude.ToString(CultureInfo.InvariantCulture);

    [JsonPropertyName("zip")] public string Zip { get; set; }

    [JsonPropertyName("timezone")] public string Timezone { get; set; }

    [JsonPropertyName("local_time")] public string LocalTime { get; set; }

    [JsonPropertyName("local_time_unix")] public long LocalTimeUnix { get; set; }

    [JsonPropertyName("is_dst")] public bool IsDst { get; set; }

    [JsonPropertyName("accuracy")] public int Accuracy { get; set; }
}