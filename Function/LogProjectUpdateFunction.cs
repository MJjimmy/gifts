using System.Text.Json;
using GiftOfTheGivers.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GiftOfTheGivers.Functions;

/// <summary>
/// Logs employee project updates to Azure Blob Storage.
///
/// Called by the web app whenever an employee posts a project update:
///   POST /api/logprojectupdate
///
/// The payload is appended as a JSON blob in the "project-update-logs" container
/// (configured via the AzureWebJobsStorage connection string).
/// </summary>
public class LogProjectUpdateFunction
{
    private readonly ILogger<LogProjectUpdateFunction> _logger;

    public LogProjectUpdateFunction(ILogger<LogProjectUpdateFunction> logger)
    {
        _logger = logger;
    }

    [Function("LogProjectUpdate")]
    [BlobOutput("project-update-logs/{DateTime}.json", Connection = "AzureWebJobsStorage")]
    public async Task<string> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
    {
        _logger.LogInformation("LogProjectUpdate function received a request.");

        string body;
        try
        {
            body = await new StreamReader(request.Body).ReadToEndAsync();
        }
        catch
        {
            body = "{}";
        }

        // Normalise into a log record with a timestamp.
        var logRecord = new
        {
            loggedAtUtc = DateTime.UtcNow,
            payload = JsonSerializer.Deserialize<JsonElement>(string.IsNullOrWhiteSpace(body) ? "{}" : body)
        };

        _logger.LogInformation("Project update logged at {Time}.", logRecord.loggedAtUtc);

        // The returned string is written to blob storage by the [BlobOutput] binding.
        return JsonSerializer.Serialize(logRecord, new JsonSerializerOptions { WriteIndented = true });
    }
}
