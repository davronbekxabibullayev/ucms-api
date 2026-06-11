namespace Ucms.Application.Handlers.Outcome;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Domain.Entities;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Storage;
using Ucms.Application.Abstractions.Mediator;

public record UploadOutcomeFileMessage(Guid OutcomeId, IFormFile File) : IRequest<FileEntryModel?>;

public class UploadOutcomeFileConsumer : RequestHandler<UploadOutcomeFileMessage, FileEntryModel?>
{
    private readonly IFileStorageClient _storageClient;
    private readonly IUcmsDbContext _dbContext;

    public UploadOutcomeFileConsumer(IFileStorageClient storageClient, IUcmsDbContext dbContext)
    {
        _storageClient = storageClient;
        _dbContext = dbContext;
    }

    protected override async Task<FileEntryModel?> Handle(UploadOutcomeFileMessage message, CancellationToken cancellationToken)
    {
        var outcomeId = message.OutcomeId;
        var file = message.File;

        var outcome = await _dbContext.Outcomes.AsTracking()
            .FirstOrDefaultAsync(a => a.Id == message.OutcomeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Outcome), outcomeId);

        if (message.File.ContentType != "application/pdf")
            throw new AppException("Загружаемый файл не является PDF-файлом.");

        var path1 = "outcomes";
        var response = await _storageClient.UploadAsync(path1, $"{outcomeId}.pdf", file.OpenReadStream())
            ?? throw new AppException("Ошибка при загрузке файла.");

        outcome.FilePath = response.FilePath;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return response;
    }
}
