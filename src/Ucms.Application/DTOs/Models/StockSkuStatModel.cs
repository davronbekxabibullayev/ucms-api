namespace Ucms.Application.DTOs.Models;

public record StockSkuStatModel(
    decimal CarStockSkuAmount,
    decimal CaseStockSkuAmount,
    decimal OtherStockSkuAmount
);
