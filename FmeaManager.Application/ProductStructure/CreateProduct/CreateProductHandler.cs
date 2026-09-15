using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.ProductStructure.CreateProduct;

public sealed class CreateProductHandler
{
    private readonly IProductionLineRepository _productionLineRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductHandler(
        IProductionLineRepository productionLineRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productionLineRepository = productionLineRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateProductResult> HandleAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var line = await _productionLineRepository.GetByIdAsync(
            command.ProductionLineId,
            cancellationToken);

        if (line is null)
        {
            throw new NotFoundException(
                $"Production line '{command.ProductionLineId}' was not found.");
        }

        if (!line.IsActive)
        {
            throw new ConflictException(
                $"Production line '{line.Code}' is inactive.");
        }

        var product = Product.Create(
            line.Id,
            command.Code,
            command.Name,
            command.PartNumber);

        if (await _productRepository.ExistsByCodeAsync(
                line.Id,
                product.Code,
                cancellationToken))
        {
            throw new ConflictException(
                $"Product code '{product.Code}' already exists in production line '{line.Code}'.");
        }

        _productRepository.Add(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(
            product.Id,
            product.ProductionLineId,
            product.Code,
            product.Name,
            product.PartNumber,
            product.IsActive);
    }
}
