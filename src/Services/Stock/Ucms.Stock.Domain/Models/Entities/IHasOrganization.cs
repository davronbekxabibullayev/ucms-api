namespace Ucms.Stock.Domain.Models.Entities;

using System;

public interface IHasOrganization
{
    Guid OrganizationId { get; set; }
}
