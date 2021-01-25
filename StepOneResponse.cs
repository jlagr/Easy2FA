using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Easy2FA
{

    public partial class StepOneResponse
    {
        [JsonProperty("requiresTwoFA")]
        public bool RequiresTwoFa { get; set; }

        [JsonProperty("twoFactorAuthTokenId")]
        public Guid TwoFactorAuthTokenId { get; set; }

        [JsonProperty("accessToken")]
        public object AccessToken { get; set; }

        [JsonProperty("refreshToken")]
        public object RefreshToken { get; set; }

        [JsonProperty("agErrorCode")]
        public long AgErrorCode { get; set; }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("isNotFound")]
        public object IsNotFound { get; set; }

        [JsonProperty("isUnauthorized")]
        public object IsUnauthorized { get; set; }

        [JsonProperty("errors")]
        public List<string> Errors { get; set; }
    }

    public partial class StepOneResponse
    {
        public static StepOneResponse FromJson(string json) => JsonConvert.DeserializeObject<StepOneResponse>(json, Easy2FA.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this StepOneResponse self) => JsonConvert.SerializeObject(self, Easy2FA.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
