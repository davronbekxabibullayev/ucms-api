namespace Ucms.Application.Handlers.Income;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record GetIncomeMessage(Guid Id) : IRequest<IncomeModel>;

public class GetIncomeConsumer : RequestHandler<GetIncomeMessage, IncomeModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetIncomeConsumer(IAppDbContext dbContext, IMapper mapper)
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
