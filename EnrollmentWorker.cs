using TmsApi.Services;

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EnrollmentWorker> _logger;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory, ILogger<EnrollmentWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public void ProcessBatch()
    {
        // Create a short-lived scope
        using var scope = _scopeFactory.CreateScope();

        // Resolve scoped service from the scope's provider
        // Use IEnrollmentService interface
        var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        // Use the service
        _logger.LogInformation("Processing enrollment batch...");

        // The 'using' block will dispose the scope and its scoped services
    }
}