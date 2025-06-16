using CxIntegrator.Services;
using CxIntegrator.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;

await MainAsync();

static async Task MainAsync()
{
    Console.WriteLine("Initialization...");

    var configuration = BuildConfiguration();

    var services = ConfigureServices(configuration);
    var serviceProvider = services.BuildServiceProvider();

    ConfigureSerilog(configuration);
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        Console.WriteLine("Press any key to start...");
        Console.ReadKey(true);

        var crmService = serviceProvider.GetRequiredService<ICrmService>();
        var cxoneService = serviceProvider.GetRequiredService<ICxoneService>();
        var mapper = serviceProvider.GetRequiredService<IMapperService>();

        logger.LogInformation("Starting getting tickets from CRM...");
        var tickets = await crmService.GetTicketsAsync();
        if (tickets == null || tickets.Count == 0)
        {
            logger.LogInformation("No tickets found in CRM.");
            return;
        }
        logger.LogInformation("Got {TicketCount} tickets.", tickets.Count);
        await SendTicketsParallelAsync(logger, cxoneService, mapper, tickets);

        logger.LogInformation("Finished.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during integration.");
    }
}

static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();
}

static ServiceCollection ConfigureServices(IConfiguration configuration)
{
    // Validate CRM and CXone API URLs
    var crmApiUrl = configuration["CrmApi"];
    if (string.IsNullOrEmpty(crmApiUrl))
        throw new ArgumentException("The CRM API URL is not configured properly in appsettings.json.", nameof(configuration));

    var cxoneApiUrl = configuration["CxoneApi"];
    if (string.IsNullOrEmpty(cxoneApiUrl))
        throw new ArgumentException("The CXone API URL is not configured properly in appsettings.json.", nameof(configuration));

    var services = new ServiceCollection();

    services.AddSingleton<ICrmService>(sp =>
        new CrmService(new HttpClient(), crmApiUrl));
    services.AddSingleton<ICxoneService>(sp =>
        new CxoneService(new HttpClient(), cxoneApiUrl));
    services.AddSingleton<IMapperService, MapperService>();
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddSerilog();
    });

    return services;
}

static void ConfigureSerilog(IConfiguration configuration)
{
    // Validate log file path
    var logFilePath = configuration["LogFilePath"];
    if (string.IsNullOrEmpty(logFilePath))
        throw new ArgumentException("The logFilePath is not configured properly in appsettings.json.", nameof(configuration));

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
        .CreateLogger();
}

static async Task SendTicketsParallelAsync(ILogger<Program> logger, ICxoneService cxoneService, IMapperService mapper, List<CxIntegrator.Models.CrmTicket> tickets)
{
    int maxDegreeOfParallelism = Environment.ProcessorCount;
    var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

    var tasks = tickets.Select(async crmTicket =>
    {
        await semaphore.WaitAsync();
        try
        {
            const int maxRetries = 3;
            int retryCount = 0;
            HttpStatusCode responseCode = HttpStatusCode.Continue;
            var cxoneTicket = mapper.Map(crmTicket);
            logger.LogInformation("Sending ticket with ID {TicketId} to CXone...", cxoneTicket.TicketId);
            while (!(responseCode == HttpStatusCode.OK) && retryCount < maxRetries)
            {
                var (ResponseCode, ErrorMessage) = await cxoneService.SendTicketAsync(cxoneTicket);
                responseCode = ResponseCode;
                if (responseCode == HttpStatusCode.OK)
                {
                    logger.LogInformation("Ticket with ID {TicketId} has been successfully sent with code {Code}.", 
                        cxoneTicket.TicketId, responseCode);
                }
                else
                {
                    retryCount++;
                    logger.LogWarning("Attempt {RetryCount} failed for ticket ID {TicketId}; Error message: {ErrorMessage}; Code: {Code}; Retrying...", 
                        retryCount, cxoneTicket.TicketId, ErrorMessage, ResponseCode);

                    if (retryCount == maxRetries)
                    {
                        logger.LogError("Failed to send ticket ID {TicketId} after {MaxRetries} attempts; Error message: {ErrorMessage}; Code: {Code}.", 
                            cxoneTicket.TicketId, maxRetries, ErrorMessage, ResponseCode);
                    }
                    else
                    {
                        // Exponential backoff strategy: wait longer with each retry
                        await Task.Delay((int)Math.Pow(10, retryCount));
                    }
                }
            }
        }
        finally
        {
            semaphore.Release();
        }
    });

    await Task.WhenAll(tasks);
}