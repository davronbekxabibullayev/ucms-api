namespace Ucms.Application.Handlers.Income;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindIncomeMessage(string Name) : IRequest<IncomeModel>;

public class FindIncomeConsumer : RequestHandler<FindIncomeMessage, IncomeModel>
{
    private readonly IUcmsDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindIncomeConsumer(IUcmsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    protected override async Task<IncomeModel> Handle(FindIncomeMessage message, CancellationToken cancellationToken)
    {
        var income = await _dbContext.Incomes
            .Include(i => i.IncomeItems)
            .Include(a => a.Stock)
            .FirstOrDefaultAsync(f => f.Name == message.Name, cancellationToken)
            ?? throw new NotFoundException($"Income with Name: {message.Name}, not found!");

        return _mapper.Map<IncomeModel>(income);
    }
}
