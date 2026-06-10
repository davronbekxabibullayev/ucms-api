namespace Ucms.Stock.Api.Application.Consumers.Product;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Stock.Domain.Exceptions;
using Ucms.Core.Services;
using Ucms.Core.Services.Mediator;
using Ucms.Stock.Contracts.Models;
using Ucms.Stock.Infrastructure.Persistance;

public record FindNameProductMessage(string Name) : IRequest<ProductModel>;

public class FindNameProductConsumer : RequestHandler<FindNameProductMessage, ProductModel>
{
    private readonly IStockDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public FindNameProductConsumer(IStockDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }

    protected override async Task<ProductModel> Handle(FindNameProductMessage message,
        CancellationToken cancellationToken)
    {
        var query = message.Name.ToLower();

        var product = await _dbContext.Products
                          .FirstOrDefaultAsync(f =>
                                  f.EmergencyType == _workContext.EmergencyType
                                  && (f.Name.ToLower().Contains(query)
                                  || f.NameKa!.ToLower().Contains(query)
                                  || f.NameRu.ToLower().Contains(query)
                                  || f.NameEn!.ToLower().Contains(query))
                              , cancellationToken)
                      ?? throw new NotFoundException();

        var result = _mapper.Map<ProductModel>(product);
        return result;
    }
}
