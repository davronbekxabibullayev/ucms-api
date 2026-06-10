namespace Ucms.Application.Handlers.Product;

using System.Threading;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ucms.Domain.Exceptions;
using Ucms.Application.DTOs.Models;
using Ucms.Application.Persistence;
using Ucms.Application.Abstractions;
using Ucms.Application.Abstractions.Mediator;

public record GetProductMessage(Guid Id) : IRequest<ProductModel>;

public class GetProductConsumer : RequestHandler<GetProductMessage, ProductModel>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWorkContext _workContext;

    public GetProductConsumer(IAppDbContext dbContext, IMapper mapper, IWorkContext workContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _workContext = workContext;
    }
    protected override async Task<ProductModel> Handle(GetProductMessage message, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
           .FirstOrDefaultAsync(f => f.Id == message.Id && f.EmergencyType == _workContext.EmergencyType, cancellationToken)
           ?? throw new NotFoundException($"product with ID: {message.Id}, not found");

        var result = _mapper.Map<ProductModel>(product);

        return result;
    }
}

