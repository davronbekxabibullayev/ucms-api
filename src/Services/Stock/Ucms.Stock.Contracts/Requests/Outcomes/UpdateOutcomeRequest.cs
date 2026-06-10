namespace Ucms.Stock.Contracts.Requests.Incomes;

using Ucms.Stock.Domain.Models.Enums;

public record UpdateOutcomeRequest
{
    public UpdateOutcomeRequest()
    {
        OutcomeItems = [];
    }

    /// <summary>
    /// Наименование расхода
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Примечание
    /// </summary>
    public string? Note { get; init; }

    /// <summary>
    /// Дата расхода
    /// </summary>
    public DateTimeOffset OutcomeDate { get; init; }

    /// <summary>
    /// Тип расхода
    /// </summary>
    public OutcomeType OutcomeType { get; init; }

    /// <summary>
    /// Статус расхода
    /// </summary>
    public OutcomeStatus OutcomeStatus { get; init; }

    /// <summary>
    /// Тип оплаты
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Идентификатор склада
    /// </summary>
    public Guid StockId { get; init; }

    /// <summary>
    /// Идентификатор принимающего склада
    /// </summary>
    public Guid? IncomeStockId { get; init; }

    public ICollection<UpdateOutcomeItemModel> OutcomeItems { get; init; }
}

public record UpdateOutcomeItemModel(Guid SkuId, Guid MeasurementUnitId, decimal Amount, decimal ActualAmount);
