namespace Ucms.Stock.Contracts.Models;

public record StockSkuStatModel(
    decimal CarStockSkuAmount,
    decimal CaseStockSkuAmount,
    decimal OtherStockSkuAmount
);
