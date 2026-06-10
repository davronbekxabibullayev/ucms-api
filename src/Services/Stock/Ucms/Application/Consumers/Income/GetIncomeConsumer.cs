namespace Ucms.Stock.Api.Application.Consumers.Income;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;
public record GetIncomeMessage(Guid Id) : IRequest<IncomeModel>;

public class GetIncomeConsumer : RequestHandler<GetIncomeMessage, IncomeModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetIncomeConsumer(IStockDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<IncomeModel> Handle(GetIncomeMessage message, CancellationToken cancellationToken)
    {
        var income = await _dbContext.Incomes
            .Include(i => i.IncomeItems)
            .ThenInclude(th => th.Sku!.MeasurementUnit)
            .Include(i => i.IncomeItems)
            .ThenInclude(th => th.MeasurementUnit)
            .FirstOrDefaultAsync(f => f.Id == message.Id, cancellationToken)
            ?? throw new NotFoundException($"Income with ID: {message.Id}, not found!");

        return _mapper.Map<IncomeModel>(income);
    }
}
