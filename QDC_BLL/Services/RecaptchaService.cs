using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using QDC_BLL.Interfaces;

namespace QDC_BLL.Services
{
    public class RecaptchaService : IRecaptchaService
    {
        private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public RecaptchaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _secretKey = configuration.GetValue<string>("RecaptchaSettings:SecretKey")
                ?? throw new InvalidOperationException("RecaptchaSettings:SecretKey is not configured.");
        }

        public async Task<bool> VerifyAsync(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["secret"] = _secretKey,
                ["response"] = token
            });

            var response = await _httpClient.PostAsync(VerifyUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<RecaptchaVerifyResponse>(stream);
            return result?.Success ?? false;
        }

        private class RecaptchaVerifyResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }
        }
    }
}
