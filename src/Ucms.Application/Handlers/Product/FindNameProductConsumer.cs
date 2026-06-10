namespace Ucms.Application.Handlers.Product;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions.Mediator;

public record FindNameProductMessage(string Name) : IRequest<ProductModel>;

public class FindNameProductConsumer : RequestHandler<FindNameProductMessage, ProductModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public FindNameProductConsumer(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override async Task<ProductModel> Handle(FindNameProductMessage message,
        CancellationToken cancellationToken)
    {
        var query = message.Name.ToLower();

        var product = await _dbContext.Products
                          .FirstOrDefaultAsync(f =>
                                  f.Name.ToLower().Contains(query)
                                  || f.NameKa!.ToLower().Contains(query)
                                  || f.NameRu.ToLower().Contains(query)
                                  || f.NameEn!.ToLower().Contains(query)
                              , cancellationToken)
                      ?? throw new NotFoundException();

        var result = _mapper.Map<ProductModel>(product);
        return result;
    }
}
