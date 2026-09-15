using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.AccessControl;

namespace FmeaManager.Application.ProductStructure.GetProductStructure;

public sealed class GetProductStructureHandler
{
    private readonly IPlantRepository _plantRepository;
    private readonly IProductionLineRepository _productionLineRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly AccessControlService _accessControl;

    public GetProductStructureHandler(
        IPlantRepository plantRepository,
        IProductionLineRepository productionLineRepository,
        IProductRepository productRepository,
        IProjectRepository projectRepository,
        AccessControlService accessControl)
    {
        _plantRepository = plantRepository;
        _productionLineRepository = productionLineRepository;
        _productRepository = productRepository;
        _projectRepository = projectRepository;
        _accessControl = accessControl;
    }

    public async Task<ProductStructureTree> HandleAsync(
        string userKey,
        CancellationToken cancellationToken = default)
    {
        var plants = await _plantRepository.ListAsync(cancellationToken);
        var lines = await _productionLineRepository.ListAsync(cancellationToken);
        var products = await _productRepository.ListAsync(cancellationToken);
        var projects = await _projectRepository.ListAsync(cancellationToken);

        var visiblePlants = new List<PlantNode>();

        foreach (var plant in plants.OrderBy(x => x.Name))
        {
            var plantAccess = await _accessControl.GetPlantAccessAsync(
                userKey,
                plant.Id,
                cancellationToken);

            var visibleLines = new List<ProductionLineNode>();

            foreach (var line in lines
                .Where(x => x.PlantId == plant.Id)
                .OrderBy(x => x.Name))
            {
                var lineAccess =
                    await _accessControl.GetProductionLineAccessAsync(
                        userKey,
                        line.Id,
                        cancellationToken);

                var visibleProducts = new List<ProductNode>();

                foreach (var product in products
                    .Where(x => x.ProductionLineId == line.Id)
                    .OrderBy(x => x.Name))
                {
                    var productAccess =
                        await _accessControl.GetProductAccessAsync(
                            userKey,
                            product.Id,
                            cancellationToken);

                    var visibleProjects =
                        new List<ProductProjectNode>();

                    foreach (var project in projects
                        .Where(x => x.ProductId == product.Id)
                        .OrderByDescending(x =>
                            x.UpdatedAt ?? x.CreatedAt))
                    {
                        var projectAccess =
                            await _accessControl.GetProjectAccessAsync(
                                userKey,
                                project.Id,
                                cancellationToken);

                        if (!projectAccess.CanRead)
                        {
                            continue;
                        }

                        visibleProjects.Add(
                            new ProductProjectNode(
                                project.Id,
                                project.Code,
                                project.Name,
                                project.Owner,
                                project.Status,
                                project.CreatedAt,
                                project.UpdatedAt,
                                ToSummary(projectAccess)));
                    }

                    if (!productAccess.CanRead &&
                        visibleProjects.Count == 0)
                    {
                        continue;
                    }

                    visibleProducts.Add(
                        new ProductNode(
                            product.Id,
                            product.ProductionLineId,
                            product.Code,
                            product.Name,
                            product.PartNumber,
                            product.IsActive,
                            ToSummary(productAccess),
                            visibleProjects));
                }

                if (!lineAccess.CanRead &&
                    visibleProducts.Count == 0)
                {
                    continue;
                }

                visibleLines.Add(
                    new ProductionLineNode(
                        line.Id,
                        line.PlantId,
                        line.Code,
                        line.Name,
                        line.IsActive,
                        ToSummary(lineAccess),
                        visibleProducts));
            }

            if (!plantAccess.CanRead &&
                visibleLines.Count == 0)
            {
                continue;
            }

            visiblePlants.Add(
                new PlantNode(
                    plant.Id,
                    plant.Code,
                    plant.Name,
                    plant.IsActive,
                    ToSummary(plantAccess),
                    visibleLines));
        }

        return new ProductStructureTree(visiblePlants);
    }

    private static ScopeAccessSummary ToSummary(
        EffectiveAccess access)
    {
        return new ScopeAccessSummary(
            access.CanRead,
            access.CanEdit,
            access.CanApprove);
    }
}
