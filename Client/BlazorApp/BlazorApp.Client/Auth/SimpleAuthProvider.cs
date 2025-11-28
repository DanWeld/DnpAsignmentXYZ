using System.Security.Claims;
using System.Text.Json;
using System.Net.Http.Json;
using ApiContracts.Auth;
using ApiContracts.Users;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp.Client.Auth;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task LoginAsync(string userName, string password)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "auth/login",
            new LoginRequest(userName, password));

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        // Cache user in browser session storage
        string serialisedData = JsonSerializer.Serialize(userDto);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userDto.Username),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
            new Claim("Id", userDto.Id.ToString()),
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(claimsPrincipal))
        );
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string userAsJson = "";
        try
        {
            userAsJson = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
        }
        catch (InvalidOperationException)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        if (string.IsNullOrEmpty(userAsJson))
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(userAsJson)!;
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userDto.Username),
            new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
            new Claim("Id", userDto.Id.ToString()),
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
        return new AuthenticationState(claimsPrincipal);
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", "");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }
}

