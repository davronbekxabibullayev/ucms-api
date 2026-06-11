namespace Ucms.Application.Handlers.Income;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Storage;
using Ucms.Application.Abstractions.Mediator;

public record UploadIncomeFileMessage(Guid IncomeId, IFormFile File) : IRequest<FileEntryModel?>;

public class UploadIncomeFileConsumer : RequestHandler<UploadIncomeFileMessage, FileEntryModel?>
{
    private readonly IFileStorageClient _storageClient;
    private readonly IUcmsDbContext _dbContext;

    public UploadIncomeFileConsumer(IFileStorageClient storageClient, IUcmsDbContext dbContext)
    {
        _storageClient = storageClient;
        _dbContext = dbContext;
    }

    protected override async Task<FileEntryModel?> Handle(UploadIncomeFileMessage message, CancellationToken cancellationToken)
    {
        var incomeId = message.IncomeId;
        var file = message.File;

        var income = await _dbContext.Incomes.AsTracking()
            .FirstOrDefaultAsync(a => a.Id == message.IncomeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Income), incomeId);

        if (message.File.ContentType != "application/pdf")
            throw new AppException("Загружаемый файл не является PDF-файлом.");

        var path1 = "incomes";
        var response = await _storageClient.UploadAsync(path1, $"{incomeId}.pdf", file.OpenReadStream())
            ?? throw new AppException("Ошибка при загрузке файла.");

        income.FilePath = response.FilePath;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return response;
    }
}
