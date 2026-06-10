namespace Ucms.Application.DTOs.Requests.Outcomes;

public record GetOutcomeStatsRequest(
    Guid OrganizationId,
    DateTime From,
    DateTime To,
    DateTime PreviousFrom,
    DateTime PreviousTo);
