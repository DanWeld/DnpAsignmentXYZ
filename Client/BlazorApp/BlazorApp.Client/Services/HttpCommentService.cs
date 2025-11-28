using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts.Comments;

namespace BlazorApp.Client.Services;
// the following whole code is about comments service which interacts with the API to perform CRUD operations on comments.
public class HttpCommentService : ICommentService
{
    private readonly HttpClient _client;

    public HttpCommentService(HttpClient client)
    {
        _client = client;
    }

    public async Task<CommentDto> AddCommentAsync(CreateCommentDto request)
    {
        HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("Comments", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<CommentDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<CommentDto> AddCommentToPostAsync(int postId, CreateCommentDto request)
    {
        HttpResponseMessage httpResponse = await _client.PostAsJsonAsync($"Posts/{postId}/comments", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<CommentDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task UpdateCommentAsync(int id, UpdateCommentDto request)
    {
        HttpResponseMessage httpResponse = await _client.PutAsJsonAsync($"Comments/{id}", request);
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }

    public async Task<CommentDto> GetSingleAsync(int id)
    {
        HttpResponseMessage httpResponse = await _client.GetAsync($"Comments/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<CommentDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<IEnumerable<CommentDto>> GetManyAsync(int? postId = null, int? userId = null, string? usernameContains = null)
    {
        List<string> queryParams = new();
        if (postId.HasValue)
            queryParams.Add($"postId={postId.Value}");
        if (userId.HasValue)
            queryParams.Add($"userId={userId.Value}");
        if (!string.IsNullOrWhiteSpace(usernameContains))
            queryParams.Add($"usernameContains={Uri.EscapeDataString(usernameContains)}");

        string query = queryParams.Count > 0 ? $"Comments?{string.Join("&", queryParams)}" : "Comments";
        
        HttpResponseMessage httpResponse = await _client.GetAsync(query);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<IEnumerable<CommentDto>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task DeleteAsync(int id)
    {
        HttpResponseMessage httpResponse = await _client.DeleteAsync($"Comments/{id}");
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
}

