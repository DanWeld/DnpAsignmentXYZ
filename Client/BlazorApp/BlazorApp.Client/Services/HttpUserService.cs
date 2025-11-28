using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts.Users;

namespace BlazorApp.Client.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient _client;

    public HttpUserService(HttpClient client)
    {
        _client = client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage httpResponse = await _client.PostAsJsonAsync("Users", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<UserDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto request)
    {
        HttpResponseMessage httpResponse = await _client.PutAsJsonAsync($"Users/{id}", request);
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }

    public async Task<UserDto> GetSingleAsync(int id)
    {
        HttpResponseMessage httpResponse = await _client.GetAsync($"Users/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<UserDto>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<IEnumerable<UserDto>> GetManyAsync(string? usernameContains = null)
    {
        string query = string.IsNullOrWhiteSpace(usernameContains) 
            ? "Users" 
            : $"Users?usernameContains={Uri.EscapeDataString(usernameContains)}";
        
        HttpResponseMessage httpResponse = await _client.GetAsync(query);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<IEnumerable<UserDto>>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task DeleteAsync(int id)
    {
        HttpResponseMessage httpResponse = await _client.DeleteAsync($"Users/{id}");
        if (!httpResponse.IsSuccessStatusCode)
        {
            string response = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception(response);
        }
    }
}

