using Newtonsoft.Json;

namespace Common.DTO.Response
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("roles")]
        public string Roles { get; set; }

        [JsonProperty("error")]
        public string ErrorHeader { get; set; }

        [JsonProperty("error_description")]
        public string Error { get; set; }

    }
}
