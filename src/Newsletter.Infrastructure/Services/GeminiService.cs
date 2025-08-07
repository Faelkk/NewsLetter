using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Newsletter.Infrastructure.Config;

namespace Newsletter.Infrastructure.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiService(HttpClient httpClient, IOptions<GeminiOptions> options)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey;
    }

    public async Task<string> GenerateNewsletterContent(string topics)
    {

        var prompt = $"Crie uma newsletter com base no seguintes tópico: {topics}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        if (!_httpClient.DefaultRequestHeaders.Contains("x-goog-api-key"))
        { 
            _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);
        }

        var response = await _httpClient.PostAsync(
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent",
            content
        );

        response.EnsureSuccessStatusCode();
        

        var resultJson = await response.Content.ReadAsStringAsync();
        var result = JsonDocument.Parse(resultJson);
        var generatedText = result.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (generatedText == null)
        {
            throw new Exception("Erro ao gerar texto para news letters");
        }

        return generatedText;
    }

}
