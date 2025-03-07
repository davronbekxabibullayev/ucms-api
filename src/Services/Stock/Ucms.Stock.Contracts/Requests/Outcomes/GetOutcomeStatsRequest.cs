namespace Ucms.Stock.Contracts.Requests.Incomes;

public record GetOutcomeStatsRequest(
    Guid OrganizationId,
    DateTime From,
    DateTime To,
    DateTime PreviousFrom,
    DateTime PreviousTo);
