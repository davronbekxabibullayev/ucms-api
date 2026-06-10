namespace Ucms.Stock.Api.Application.Consumers.Income;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Stock.Infrastructure.Persistance;
public record FindIncomeMessage(string Name) : IRequest<IncomeModel>;

public class FindIncomeConsumer : RequestHandler<FindIncomeMessage, IncomeModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindIncomeConsumer(IStockDbContext dbContext, IMapper mapper)
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
