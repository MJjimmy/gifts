using GiftOfTheGivers.Helpers;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GiftOfTheGivers.Tests;

/// <summary>
/// Integration-style tests for the web app's Azure Functions client, mapped to
/// realistic donation cases (see docs/AZURE-SETUP.md - Function test matrix).
/// </summary>
public class FunctionClientTests
{
    private static FunctionClient CreateClient(HttpClient httpClient)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Functions:BaseUrl"] = "http://localhost:7071"
            })
            .Build();

        return new FunctionClient(
            new StubHttpClientFactory(httpClient),
            configuration,
            NullLogger<FunctionClient>.Instance);
    }

    private static HttpClient CreateHttpClient(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        return new HttpClient(new StubHandler(responder));
    }

    private static Donation RealisticDonation() => new()
    {
        Id = 12,
        Reference = "GTG-20260924-K7QX2M",
        DonationType = "OneTime",
        Amount = 750m,
        Currency = "ZAR",
        DonorName = "Thabo Mokoena",
        IsAnonymous = false,
        CreatedAt = new DateTime(2026, 9, 24, 10, 0, 0, DateTimeKind.Utc)
    };

    [Fact]
    public async Task ReturnsCertificate_WhenFunctionRespondsToDonation()
    {
        HttpResponseMessage Responder(HttpRequestMessage request)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                      "certificateNumber": "GOTG-2026-0012",
                      "donationReference": "GTG-20260924-K7QX2M",
                      "donorName": "Thabo Mokoena",
                      "amount": 750,
                      "currency": "ZAR",
                      "donationType": "OneTime",
                      "issuedAtUtc": "2026-09-24T10:00:01Z"
                    }
                    """,
                    System.Text.Encoding.UTF8,
                    "application/json")
            };
            return response;
        }

        var client = CreateClient(CreateHttpClient(Responder));
        var certificate = await client.RequestTaxCertificateAsync(RealisticDonation());

        Assert.NotNull(certificate);
        Assert.Equal("GOTG-2026-0012", certificate!.CertificateNumber);
        Assert.Equal("GTG-20260924-K7QX2M", certificate.DonationReference);
        Assert.Equal("Thabo Mokoena", certificate.DonorName);
    }

    [Fact]
    public async Task SendsDonationDetails_WhenRequestingCertificate()
    {
        string? capturedBody = null;

        HttpResponseMessage Responder(HttpRequestMessage request)
        {
            capturedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
            };
        }

        var client = CreateClient(CreateHttpClient(Responder));
        await client.RequestTaxCertificateAsync(RealisticDonation());

        Assert.NotNull(capturedBody);
        // PostAsJsonAsync uses JsonSerializerDefaults.Web -> camelCase property names.
        Assert.Contains("\"reference\":\"GTG-20260924-K7QX2M\"", capturedBody);
        Assert.Contains("\"amount\":750", capturedBody);
        Assert.Contains("\"currency\":\"ZAR\"", capturedBody);
    }

    [Fact]
    public async Task FallsBackToNull_WhenFunctionReturnsServerError()
    {
        HttpResponseMessage Responder(HttpRequestMessage request)
            => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);

        var client = CreateClient(CreateHttpClient(Responder));
        var certificate = await client.RequestTaxCertificateAsync(RealisticDonation());

        Assert.Null(certificate); // Web app falls back to local certificate generation.
    }

    [Fact]
    public async Task FallsBackToNull_WhenFunctionIsUnreachable()
    {
        HttpResponseMessage Responder(HttpRequestMessage request)
            => throw new HttpRequestException("No connection could be made (function app not running).");

        var client = CreateClient(CreateHttpClient(Responder));
        var certificate = await client.RequestTaxCertificateAsync(RealisticDonation());

        Assert.Null(certificate);
    }

    [Fact]
    public async Task FallsBackToNull_WhenFunctionIsSlow()
    {
        HttpResponseMessage Responder(HttpRequestMessage request)
        {
            Thread.Sleep(TimeSpan.FromSeconds(4)); // Client timeout is 3 seconds.
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        var client = CreateClient(CreateHttpClient(Responder));
        var certificate = await client.RequestTaxCertificateAsync(RealisticDonation());

        Assert.Null(certificate);
    }

    [Fact]
    public async Task PostsProjectUpdateDetails_WhenEmployeePostsUpdate()
    {
        string? capturedBody = null;

        HttpResponseMessage Responder(HttpRequestMessage request)
        {
            capturedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        var client = CreateClient(CreateHttpClient(Responder));

        var project = new Project { Id = 1, Title = "Gauteng Flood Relief" };
        var update = new ProjectUpdate
        {
            Id = 5,
            ProjectId = 1,
            Title = "Relief distribution completed",
            Body = "Food parcels and clean water delivered to affected families.",
            PostedBy = "Relief Coordinator",
            CreatedAt = new DateTime(2026, 9, 24, 9, 0, 0, DateTimeKind.Utc)
        };

        await client.LogProjectUpdateAsync(project, update);

        Assert.NotNull(capturedBody);
        Assert.Contains("\"projectTitle\":\"Gauteng Flood Relief\"", capturedBody);
        Assert.Contains("\"title\":\"Relief distribution completed\"", capturedBody);
    }

    [Fact]
    public async Task LogProjectUpdate_SwallowsConnectionErrors()
    {
        HttpResponseMessage Responder(HttpRequestMessage request)
            => throw new HttpRequestException("No connection could be made.");

        var client = CreateClient(CreateHttpClient(Responder));

        var project = new Project { Id = 1, Title = "Gauteng Flood Relief" };
        var update = new ProjectUpdate { Id = 5, ProjectId = 1, Title = "t", Body = "b", PostedBy = "p" };

        // Must not throw - posting updates works even without the function app.
        await client.LogProjectUpdateAsync(project, update);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responder(request));
        }
    }

    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public StubHttpClientFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient CreateClient(string name) => _httpClient;
    }
}
