namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Payments;
using Ucms.Domain.Enums;

[ApiController]
[Route("api/projects/{projectId:guid}/payments")]
[Tags("Payment")]
[Authorize]
public class PaymentController(
    GetClientPayments.Handler    getClientPayments,
    CreateClientPayment.Handler  createClientPayment,
    GetBrigadePayments.Handler   getBrigadePayments,
    CreateBrigadePayment.Handler createBrigadePayment,
    GetFinancialSummary.Handler  getFinancialSummary) : ControllerBase
{
    public record CreateClientPaymentRequest(
        Guid? ActId, DateTimeOffset Date, decimal Amount,
        PaymentMethod PaymentMethod, string? Note);

    public record CreateBrigadePaymentRequest(
        Guid BrigadeId, DateTimeOffset Date, decimal Amount,
        PaymentMethod PaymentMethod, Guid[] WorkLogIds, string? Note);

    [HttpGet("client")]
    public async Task<IActionResult> GetClientPayments(
        Guid projectId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        CancellationToken ct = default)
    {
        var (data, notFound, forbidden) = await getClientPayments.HandleAsync(new(projectId, from, to), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return Ok(data);
    }

    [HttpPost("client")]
    [Authorize(Roles = "Admin,Manager,Accountant")]
    public async Task<IActionResult> CreateClientPayment(
        Guid projectId, [FromBody] CreateClientPaymentRequest req, CancellationToken ct)
    {
        var (data, notFound, forbidden) = await createClientPayment.HandleAsync(
            new(projectId, req.ActId, req.Date, req.Amount, req.PaymentMethod, req.Note), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return Ok(data);
    }

    [HttpGet("brigade")]
    public async Task<IActionResult> GetBrigadePayments(
        Guid projectId,
        [FromQuery] Guid? brigadeId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        CancellationToken ct = default)
    {
        var (data, notFound, forbidden) = await getBrigadePayments.HandleAsync(
            new(projectId, brigadeId, from, to), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return Ok(data);
    }

    [HttpPost("brigade")]
    [Authorize(Roles = "Admin,Manager,Accountant")]
    public async Task<IActionResult> CreateBrigadePayment(
        Guid projectId, [FromBody] CreateBrigadePaymentRequest req, CancellationToken ct)
    {
        var (data, notFound, forbidden) = await createBrigadePayment.HandleAsync(
            new(projectId, req.BrigadeId, req.Date, req.Amount, req.PaymentMethod, req.WorkLogIds, req.Note), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return Ok(data);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetFinancialSummary(Guid projectId, CancellationToken ct)
    {
        var (data, notFound, forbidden) = await getFinancialSummary.HandleAsync(new(projectId), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return Ok(data);
    }
}
