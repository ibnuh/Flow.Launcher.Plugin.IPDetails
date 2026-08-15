using System;
using System.Globalization;
using System.Text.Json.Serialization;

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
    /// The ipquery.io response.
    /// </summary>
    public IpApiResponse Response { get; set; }
}

/// <summary>
/// The response from the ipquery.io API.
/// </summary>
public class IpApiResponse
{
    [JsonPropertyName("ip")] public string Ip { get; set; }

    [JsonPropertyName("isp")] public IspInfo Isp { get; set; }

    [JsonPropertyName("location")] public LocationInfo Location { get; set; }

    [JsonPropertyName("risk")] public RiskInfo Risk { get; set; }
}

public class IspInfo
{
    [JsonPropertyName("asn")] public string Asn { get; set; }

    [JsonPropertyName("org")] public string Org { get; set; }

    [JsonPropertyName("isp")] public string Isp { get; set; }

    public string DisplayName =>
        !string.IsNullOrWhiteSpace(Isp) ? Isp :
        !string.IsNullOrWhiteSpace(Org) ? Org :
        string.Empty;
}

public class LocationInfo
{
    [JsonPropertyName("country")] public string Country { get; set; }

    [JsonPropertyName("country_code")] public string CountryCode { get; set; }

    [JsonPropertyName("city")] public string City { get; set; }

    [JsonPropertyName("state")] public string State { get; set; }

    [JsonPropertyName("zipcode")] public string Zipcode { get; set; }

    [JsonPropertyName("latitude")] public double Latitude { get; set; }

    public string LatitudeFormatted => Latitude.ToString("0.#####", CultureInfo.InvariantCulture);

    [JsonPropertyName("longitude")] public double Longitude { get; set; }

    public string LongitudeFormatted => Longitude.ToString("0.#####", CultureInfo.InvariantCulture);

    public bool HasCoordinates => Math.Abs(Latitude) > 0.00001 || Math.Abs(Longitude) > 0.00001;

    [JsonPropertyName("timezone")] public string Timezone { get; set; }

    [JsonPropertyName("localtime")] public string LocalTime { get; set; }

    public string LocalTimeDisplay =>
        string.IsNullOrWhiteSpace(LocalTime) ? string.Empty : LocalTime.Replace('T', ' ');
}

public class RiskInfo
{
    [JsonPropertyName("is_mobile")] public bool IsMobile { get; set; }

    [JsonPropertyName("is_vpn")] public bool IsVpn { get; set; }

    [JsonPropertyName("is_tor")] public bool IsTor { get; set; }

    [JsonPropertyName("is_proxy")] public bool IsProxy { get; set; }

    [JsonPropertyName("is_datacenter")] public bool IsDatacenter { get; set; }

    [JsonPropertyName("risk_score")] public int RiskScore { get; set; }
}
