using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Auth.IntegrationTests;

public class AuthorizationControllerTest : IntegrationTestBase
{
    public AuthorizationControllerTest(WebApplicationFactory<Program> factory) : base(factory) { }
    
    [Fact]
    public async Task Exchange_WhenPasswordGrantType_AndRightCredentialsProvided_ShouldReturn200OkWithToken()
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", "ken@masters.com"),
            new KeyValuePair<string, string>("password", "123456"),
        });

        var response = await Client.PostAsync("/connect/token", formData);

        response.Should().HaveStatusCode(HttpStatusCode.OK);
        
        var result = await response.Content.ReadAsStringAsync(); 
        
        result.Should().Contain("access_token");
    }
    
    [Fact]
    public async Task Exchange_WhenPasswordGrantType_AndWrongCredentialsProvided_ShouldReturn400BadRequest()
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", "ken@masters.com"),
            new KeyValuePair<string, string>("password", "1234567"),
        });

        var response = await Client.PostAsync("/connect/token", formData);

        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Exchange_WhenRefreshTokenGrantType_AndRightTokenProvided_ShouldReturn200OkWithToken()
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", ""),
        });
        
        var response = await Client.PostAsync("/connect/token", formData);
        
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        
        var result = await response.Content.ReadAsStringAsync(); 
        
        result.Should().Contain("access_token");
    }
    
    
    [Fact]
    public async Task Exchange_WhenRefreshTokenGrantType_AndWrongTokenProvided_ShouldReturn400BadRequest()
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", "123456"),
        });

        var response = await Client.PostAsync("/connect/token", formData);

        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }
}