namespace Ucms.Stock.Api.Application.Consumers.MeasurementUnit;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record FindMeasurementUnitMessage(string Code) : IRequest<MeasurementUnitModel>;

public class FindMeasurementUnitConsumer : RequestHandler<FindMeasurementUnitMessage, MeasurementUnitModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public FindMeasurementUnitConsumer(
        IStockDbContext dbContext,
        IMapper mapper,
        IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }
    protected override async Task<MeasurementUnitModel> Handle(FindMeasurementUnitMessage message, CancellationToken cancellationToken)
    {
        var measurementUnit = await _dbContext.MeasurementUnits
            .FirstOrDefaultAsync(f => f.Code == message.Code && f.EmergencyType == _workContext.EmergencyType, cancellationToken)
            ?? throw new NotFoundException($"Measurement unit with code: {message.Code}, not found!");

        return _mapper.Map<MeasurementUnitModel>(measurementUnit);
    }
}
