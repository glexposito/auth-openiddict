using Auth.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Client = factory.CreateClient();

        factory.Services.CreateScope().ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
    }
}