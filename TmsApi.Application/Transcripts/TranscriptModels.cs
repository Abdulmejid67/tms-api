namespace TmsApi.Application.Transcripts;

public enum TranscriptState { Queued, Processing, Ready, Failed }

public record TranscriptRequest(int StudentId, string? ReportId = null)
{
    public TranscriptRequest WithReportId(string id) => this with { ReportId = id };
}

public record TranscriptStatus
{
    public string ReportId { get; init; } = string.Empty;
    public int StudentId { get; init; }
    public TranscriptState State { get; init; }
    public DateTimeOffset RequestedAt { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? DownloadUrl { get; init; }
    public string? ErrorMessage { get; init; }

    public TranscriptStatus() { }

    public TranscriptStatus(
        string reportId,
        int studentId,
        TranscriptState state,
        DateTimeOffset requestedAt,
        DateTimeOffset? startedAt = null,
        DateTimeOffset? completedAt = null,
        string? downloadUrl = null,
        string? errorMessage = null)
    {
        ReportId = reportId;
        StudentId = studentId;
        State = state;
        RequestedAt = requestedAt;
        StartedAt = startedAt;
        CompletedAt = completedAt;
        DownloadUrl = downloadUrl;
        ErrorMessage = errorMessage;
    }
}