using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Auth.IntegrationTests;

public class ExistentialControllerTest : IntegrationTestBase
{
    public ExistentialControllerTest(WebApplicationFactory<Program> factory) : base(factory) { }
    
    [Fact]
    public async Task WhoAmI_WhenValidTokenProvided_ShouldReturn200Ok()
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", "ken@masters.com"),
            new KeyValuePair<string, string>("password", "123456"),
        });

        var tokenResponse = await Client.PostAsync("/connect/token", formData);

        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<JsonObject>(tokenContent);

        var token = data!["access_token"]!.GetValue<string>();
        
        var tokenType = data["token_type"]!.GetValue<string>();

        var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, "/whoami");
        requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue(tokenType, token);
          
        var response = await Client.SendAsync(requestMessage);
        
        var content = await response.Content.ReadAsStringAsync();
        
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        content.Should().Contain("Ken Masters");
    }
    
    [Fact]
    public async Task WhoAmI_WhenInvalidTokenProvided_ShouldReturn401Unauthorized()
    {
        var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, "/whoami");
        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", "eyJhbGc");
          
        var response = await Client.SendAsync(requestMessage);
        
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task WhoAmI_WhenNoTokenProvided_ShouldReturn401Unauthorized()
    {
        var response = await Client.GetAsync("/whoami");
        
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}