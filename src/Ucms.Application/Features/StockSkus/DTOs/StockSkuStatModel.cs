namespace Ucms.Application.Features.StockSkus;

public record StockSkuStatModel(
    decimal CarStockSkuAmount,
    decimal CaseStockSkuAmount,
    decimal OtherStockSkuAmount
);
