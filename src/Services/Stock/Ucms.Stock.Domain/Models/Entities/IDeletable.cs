namespace Ucms.Stock.Domain.Models.Entities;

public interface IDeletable
{
    bool IsDeleted { get; set; }
}
