namespace Ucms.Stock.Contracts.Requests.Incomes;

using Ucms.Stock.Domain.Models.Enums;

public record CreateIncomeRequest
{
    public CreateIncomeRequest()
    {
        IncomeItems = [];
    }

    /// <summary>
    /// Наименование прихода
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Примечание
    /// </summary>
    public string? Note { get; init; }

    /// <summary>
    /// Дата прихода
    /// </summary>
    public DateTimeOffset IncomeDate { get; init; }

    /// <summary>
    /// Тип прихода
    /// </summary>
    public IncomeType IncomeType { get; init; }

    /// <summary>
    /// Статус прихода
    /// </summary>
    public IncomeStatus IncomeStatus { get; init; }

    /// <summary>
    /// Тип оплаты
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Идентификатор склада
    /// </summary>
    public Guid StockId { get; init; }

    public ICollection<CreateIncomeItemModel> IncomeItems { get; init; }
}

public record CreateIncomeItemModel(Guid SkuId, Guid MeasurementUnitId, int Amount);
