using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Easy2FA
{
    public partial class StepTwoResponse
    {
        [JsonProperty("requiresTwoFA")]
        public object RequiresTwoFa { get; set; }

        [JsonProperty("twoFactorAuthTokenId")]
        public object TwoFactorAuthTokenId { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("agErrorCode")]
        public long AgErrorCode { get; set; }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("isNotFound")]
        public object IsNotFound { get; set; }

        [JsonProperty("isUnauthorized")]
        public object IsUnauthorized { get; set; }

        [JsonProperty("errors")]
        public object Errors { get; set; }
    }

    public partial class StepTwoResponse
    {
        public static StepTwoResponse FromJson(string json) => JsonConvert.DeserializeObject<StepTwoResponse>(json, Easy2FA.Converter.Settings);
    }
}
