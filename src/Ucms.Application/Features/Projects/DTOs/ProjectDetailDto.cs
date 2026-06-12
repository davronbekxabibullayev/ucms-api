namespace Ucms.Application.Features.Projects.DTOs;

using Ucms.Domain.Enums;

public record ProjectDetailDto(
    Guid                          Id,
    string                        Name,
    string?                       Address,
    string?                       Description,
    string?                       ContractNumber,
    DateTimeOffset?               ContractDate,
    DateTimeOffset?               StartDate,
    DateTimeOffset?               EndDate,
    ProjectStatus                 Status,
    Guid                          OrganizationId,
    DateTimeOffset                CreatedAt,
    DateTimeOffset                UpdatedAt,
    IEnumerable<ProjectSectionDto> Sections);
