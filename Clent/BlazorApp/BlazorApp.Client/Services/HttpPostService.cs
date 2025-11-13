using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts.Posts;

namespace BlazorApp.Client.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient _client;

    public HttpPostService(HttpClient client)
    {
        _client = client;
    }

    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("Posts", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<PostDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task UpdatePostAsync(int id, UpdatePostDto request)
    {
        HttpResponseMessage httpResponse = await _client.PutAsJsonAsync($"Posts/{id}", request);
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }

    public async Task<PostDto> GetSingleAsync(int id, bool includeComments = false, bool includeAuthor = false)
    {
        string query = $"Posts/{id}?includeComments={includeComments}&includeAuthor={includeAuthor}";
        HttpResponseMessage httpResponse = await _client.GetAsync(query);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<PostDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<IEnumerable<PostDto>> GetManyAsync(string? titleContains = null, int? userId = null, string? authorNameContains = null, bool includeComments = false, bool includeAuthor = false)
    {
        List<string> queryParams = new();
        if (!string.IsNullOrWhiteSpace(titleContains))
            queryParams.Add($"titleContains={Uri.EscapeDataString(titleContains)}");
        if (userId.HasValue)
            queryParams.Add($"userId={userId.Value}");
        if (!string.IsNullOrWhiteSpace(authorNameContains))
            queryParams.Add($"authorNameContains={Uri.EscapeDataString(authorNameContains)}");
        if (includeComments)
            queryParams.Add("includeComments=true");
        if (includeAuthor)
            queryParams.Add("includeAuthor=true");

        string query = queryParams.Count > 0 ? $"Posts?{string.Join("&", queryParams)}" : "Posts";
        
        HttpResponseMessage httpResponse = await _client.GetAsync(query);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<IEnumerable<PostDto>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task DeleteAsync(int id)
    {
        HttpResponseMessage httpResponse = await _client.DeleteAsync($"Posts/{id}");
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
}

