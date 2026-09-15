using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.ProductStructure.CreatePlant;
using FmeaManager.Application.ProductStructure.CreateProduct;
using FmeaManager.Application.ProductStructure.CreateProductionLine;
using FmeaManager.Domain.ProductStructure;

namespace FmeaManager.Application.Tests.ProductStructure;

public sealed class ProductStructureHandlersTests
{
    [Fact]
    public async Task CreatePlant_WithValidData_PersistsPlant()
    {
        var repository = new FakePlantRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreatePlantHandler(
            repository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new CreatePlantCommand(
                "MX-TOL",
                "Toluca Plant"));

        Assert.Equal("MX-TOL", result.Code);
        Assert.NotNull(repository.Added);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateLine_WhenPlantDoesNotExist_ThrowsNotFound()
    {
        var handler = new CreateProductionLineHandler(
            new FakePlantRepository(),
            new FakeProductionLineRepository(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(
                new CreateProductionLineCommand(
                    Guid.NewGuid(),
                    "ASSY-01",
                    "Assembly Line 1")));
    }

    [Fact]
    public async Task CreateProduct_WhenLineExists_PersistsProduct()
    {
        var plant = Plant.Create("MX-TOL", "Toluca");
        var line = ProductionLine.Create(
            plant.Id,
            "ASSY-01",
            "Assembly Line 1");

        var productRepository = new FakeProductRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateProductHandler(
            new FakeProductionLineRepository(line),
            productRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new CreateProductCommand(
                line.Id,
                "BAT-001",
                "12V Battery",
                "PN-001"));

        Assert.Equal("BAT-001", result.Code);
        Assert.NotNull(productRepository.Added);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    private sealed class FakePlantRepository : IPlantRepository
    {
        private readonly Plant? _plant;

        public FakePlantRepository(Plant? plant = null)
        {
            _plant = plant;
        }

        public Plant? Added { get; private set; }

        public Task<bool> ExistsByCodeAsync(
            string code,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<Plant?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<Plant?>(
                _plant is not null && _plant.Id == id
                    ? _plant
                    : Added is not null && Added.Id == id
                        ? Added
                        : null);

        public Task<IReadOnlyList<Plant>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Plant>>(
                Added is not null
                    ? new[] { Added }
                    : _plant is not null
                        ? new[] { _plant }
                        : Array.Empty<Plant>());

        public void Add(Plant plant) => Added = plant;
    }

    private sealed class FakeProductionLineRepository
        : IProductionLineRepository
    {
        private readonly ProductionLine? _line;

        public FakeProductionLineRepository(
            ProductionLine? line = null)
        {
            _line = line;
        }

        public ProductionLine? Added { get; private set; }

        public Task<bool> ExistsByCodeAsync(
            Guid plantId,
            string code,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<ProductionLine?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ProductionLine?>(
                _line is not null && _line.Id == id
                    ? _line
                    : Added is not null && Added.Id == id
                        ? Added
                        : null);

        public Task<IReadOnlyList<ProductionLine>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProductionLine>>(
                _line is null
                    ? Array.Empty<ProductionLine>()
                    : new[] { _line });

        public Task<IReadOnlyList<ProductionLine>> ListByPlantIdAsync(
            Guid plantId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProductionLine>>(
                _line is not null && _line.PlantId == plantId
                    ? new[] { _line }
                    : Array.Empty<ProductionLine>());

        public void Add(ProductionLine productionLine) =>
            Added = productionLine;
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public Product? Added { get; private set; }

        public Task<bool> ExistsByCodeAsync(
            Guid productionLineId,
            string code,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<Product?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<Product?>(
                Added is not null && Added.Id == id
                    ? Added
                    : null);

        public Task<IReadOnlyList<Product>> ListAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Product>>(
                Added is null
                    ? Array.Empty<Product>()
                    : new[] { Added });

        public Task<IReadOnlyList<Product>> ListByProductionLineIdAsync(
            Guid productionLineId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Product>>(
                Added is not null &&
                Added.ProductionLineId == productionLineId
                    ? new[] { Added }
                    : Array.Empty<Product>());

        public void Add(Product product) => Added = product;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
