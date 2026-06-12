namespace Ucms.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ucms.Application.Features.Salaries.Commands;
using Ucms.Application.Features.Salaries.Queries;

/// <summary>
/// Xodimlar maoshlarini boshqarish.
/// Управление зарплатами сотрудников.
/// </summary>
[ApiController]
[Route("api/salaries")]
[Tags("Salary")]
[Authorize]
public class SalaryController(
    GetSalaries.Handler    getAll,
    GetSalaryById.Handler  getById,
    CreateSalary.Handler   create,
    UpdateSalary.Handler   update,
    DeleteSalary.Handler   delete) : ControllerBase
{
    public record CreateSalaryRequest(
        string EmployeeName, string? Position,
        string Month, decimal Amount, string? Notes);

    public record UpdateSalaryRequest(
        string EmployeeName, string? Position,
        string Month, decimal Amount, string? Notes);

    /// <summary>
    /// Maoshlar ro'yxati (oy filtri va sahifalash bilan).
    /// Список зарплат (с фильтром по месяцу и пагинацией).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? month,
        [FromQuery] int page = 1,
        [FromQuery] int size = 50,
        CancellationToken ct = default)
    {
        var (data, forbidden) = await getAll.HandleAsync(new(month, page, size), ct);
        if (forbidden) return Forbid();
        return Ok(data);
    }

    /// <summary>
    /// ID bo'yicha maosh yozuvini olish.
    /// Получить запись о зарплате по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var (data, forbidden) = await getById.HandleAsync(new(id), ct);
        if (forbidden) return Forbid();
        return data is null ? NotFound() : Ok(data);
    }

    /// <summary>
    /// Yangi maosh yozuvi qo'shish. Admin yoki Manager uchun.
    /// Добавить новую запись о зарплате. Для Admin или Manager.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateSalaryRequest req, CancellationToken ct)
    {
        var result = await create.HandleAsync(
            new(req.EmployeeName, req.Position, req.Month, req.Amount, req.Notes), ct);
        if (result is null) return BadRequest(new { message = "Foydalanuvchiga tashkilot biriktirilmagan. / Пользователю не привязана организация." });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Maosh yozuvini yangilash. Admin yoki Manager uchun.
    /// Обновить запись о зарплате. Для Admin или Manager.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSalaryRequest req, CancellationToken ct)
    {
        var (notFound, forbidden) = await update.HandleAsync(
            new(id, req.EmployeeName, req.Position, req.Month, req.Amount, req.Notes), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }

    /// <summary>
    /// Maosh yozuvini o'chirish. Admin yoki Manager uchun.
    /// Удалить запись о зарплате. Для Admin или Manager.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var (notFound, forbidden) = await delete.HandleAsync(new(id), ct);
        if (notFound)  return NotFound();
        if (forbidden) return Forbid();
        return NoContent();
    }
}
