using Newtonsoft.Json;
using System;
using System.Text;

namespace Common.DTO.Response
{
    public class AccessTokenModel
	{

		[JsonProperty("access_token")]
		public string AccessToken { get; set; } = String.Empty;
		[JsonProperty("token_type")]
		public string TokenType { get; set; }= String.Empty;
		[JsonProperty("expires_in")]
		public long ExpiresIn { get; set; }
		[JsonProperty("state")]
		public string State { get; set; } = String.Empty;
		[JsonProperty("user")]
		public string User { get; set; } = String.Empty;
		[JsonProperty("roles")]
		public string Roles { get; set; } = String.Empty;
		[JsonProperty("error")]
		public string ErrorHeader { get; set; } = String.Empty;
		[JsonProperty("error_description")]
		public string Error { get; set; } = String.Empty;

	}
}
