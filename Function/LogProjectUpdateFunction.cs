using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GiftOfTheGivers.Functions;

/// <summary>
/// Logs employee project updates to Azure Blob Storage.
/// POST /api/logprojectupdate
/// </summary>
public class LogProjectUpdateFunction
{
    private readonly ILogger<LogProjectUpdateFunction> _logger;

    public LogProjectUpdateFunction(ILogger<LogProjectUpdateFunction> logger)
    {
        _logger = logger;
    }

    [Function("LogProjectUpdate")]
    public async Task<LogProjectUpdateResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
    {
        _logger.LogInformation("LogProjectUpdate function received a request.");

        JsonElement payload;

        try
        {
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            payload = JsonSerializer.Deserialize<JsonElement>(string.IsNullOrWhiteSpace(body) ? "{}" : body);

            if (payload.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException("Payload must be a JSON object.");
            }
        }
        catch
        {
            _logger.LogWarning("LogProjectUpdate: request rejected - payload is not a valid JSON object.");
            return new LogProjectUpdateResult
            {
                HttpResponse = await BadRequestAsync(request, "Request body must be a JSON object.")
            };
        }

        if (!payload.TryGetProperty("projectId", out var projectIdElement)
            || !projectIdElement.TryGetInt32(out var projectId)
            || projectId <= 0)
        {
            _logger.LogWarning("LogProjectUpdate: request rejected - projectId is missing or invalid.");
            return new LogProjectUpdateResult
            {
                HttpResponse = await BadRequestAsync(request, "projectId is required and must be a positive number.")
            };
        }

        var logRecord = new
        {
            loggedAtUtc = DateTime.UtcNow,
            projectId,
            payload
        };

        _logger.LogInformation("Project update for project {ProjectId} accepted for logging.", projectId);

        return new LogProjectUpdateResult
        {
            BlobContent = JsonSerializer.Serialize(logRecord, new JsonSerializerOptions { WriteIndented = true }),
            HttpResponse = await OkAsync(request, $"Project update for project {projectId} logged.")
        };
    }

    private static async Task<HttpResponseData> BadRequestAsync(HttpRequestData request, string message)
    {
        var response = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(new { error = message });
        return response;
    }

    private static async Task<HttpResponseData> OkAsync(HttpRequestData request, string message)
    {
        var response = request.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { status = message });
        return response;
    }
}

/// <summary>Multiple output bindings: HTTP response + blob content.</summary>
public class LogProjectUpdateResult
{
    [BlobOutput("project-update-logs/{DateTime}.json", Connection = "AzureWebJobsStorage")]
    public string? BlobContent { get; set; }

    [HttpResult]
    public HttpResponseData? HttpResponse { get; set; }
}
