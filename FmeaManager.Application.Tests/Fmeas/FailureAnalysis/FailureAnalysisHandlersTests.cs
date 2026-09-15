using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.Common;
using FmeaManager.Application.Fmeas.CreateDetectionControl;
using FmeaManager.Application.Fmeas.CreateFailureCause;
using FmeaManager.Application.Fmeas.CreateFailureEffect;
using FmeaManager.Application.Fmeas.CreateFailureMode;
using FmeaManager.Application.Fmeas.CreatePreventionControl;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Tests.Fmeas.FailureAnalysis;

public sealed class FailureAnalysisHandlersTests
{
    [Fact]
    public async Task CreateFailureMode_OnDraftRevision_PersistsEntity()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        var step = ProcessStep.Create(revision.Id, 10, "Assembly");
        var context = new FakeContext(revision, step);

        var handler = new CreateFailureModeHandler(
            context.Guard,
            context.FailureModes,
            context.UnitOfWork);

        var result = await handler.HandleAsync(
            new CreateFailureModeCommand(step.Id, " Wrong polarity "));

        Assert.Equal("Wrong polarity", result.Description);
        Assert.NotNull(context.FailureModes.Added);
        Assert.Equal(1, context.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateFailureMode_OnRevisionUnderReview_ThrowsConflict()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        revision.SubmitForReview();

        var step = ProcessStep.Create(revision.Id, 10, "Assembly");
        var context = new FakeContext(revision, step);

        var handler = new CreateFailureModeHandler(
            context.Guard,
            context.FailureModes,
            context.UnitOfWork);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(
                new CreateFailureModeCommand(step.Id, "Wrong polarity")));
    }

    [Fact]
    public async Task NestedFailureAnalysis_CanBeCreatedInOrder()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        var step = ProcessStep.Create(revision.Id, 20, "Terminal assembly");
        var context = new FakeContext(revision, step);

        var modeHandler = new CreateFailureModeHandler(
            context.Guard,
            context.FailureModes,
            context.UnitOfWork);

        var modeResult = await modeHandler.HandleAsync(
            new CreateFailureModeCommand(step.Id, "Incorrect terminal connection"));

        var effectHandler = new CreateFailureEffectHandler(
            context.Guard,
            context.FailureEffects,
            context.UnitOfWork);

        var effectResult = await effectHandler.HandleAsync(
            new CreateFailureEffectCommand(modeResult.Id, "Vehicle cannot start"));

        var causeHandler = new CreateFailureCauseHandler(
            context.Guard,
            context.FailureCauses,
            context.UnitOfWork);

        var causeResult = await causeHandler.HandleAsync(
            new CreateFailureCauseCommand(modeResult.Id, "Terminal in wrong position"));

        var preventionHandler = new CreatePreventionControlHandler(
            context.Guard,
            context.PreventionControls,
            context.UnitOfWork);

        var preventionResult = await preventionHandler.HandleAsync(
            new CreatePreventionControlCommand(causeResult.Id, "Assembly fixture"));

        var detectionHandler = new CreateDetectionControlHandler(
            context.Guard,
            context.DetectionControls,
            context.UnitOfWork);

        var detectionResult = await detectionHandler.HandleAsync(
            new CreateDetectionControlCommand(causeResult.Id, "Vision inspection"));

        Assert.Equal("Vehicle cannot start", effectResult.Description);
        Assert.Equal("Terminal in wrong position", causeResult.Description);
        Assert.Equal("Assembly fixture", preventionResult.Description);
        Assert.Equal("Vision inspection", detectionResult.Description);
        Assert.Equal(5, context.UnitOfWork.SaveChangesCallCount);
    }

    private sealed class FakeContext
    {
        public FakeContext(FmeaRevision revision, ProcessStep step)
        {
            Revisions = new FakeRevisionRepository(revision);
            ProcessSteps = new FakeProcessStepRepository(step);
            FailureModes = new FakeFailureModeRepository();
            FailureEffects = new FakeFailureEffectRepository();
            FailureCauses = new FakeFailureCauseRepository();
            PreventionControls = new FakePreventionControlRepository();
            DetectionControls = new FakeDetectionControlRepository();
            UnitOfWork = new FakeUnitOfWork();

            Guard = new FmeaEditabilityGuard(
                ProcessSteps,
                FailureModes,
                FailureCauses,
                Revisions);
        }

        public FakeRevisionRepository Revisions { get; }
        public FakeProcessStepRepository ProcessSteps { get; }
        public FakeFailureModeRepository FailureModes { get; }
        public FakeFailureEffectRepository FailureEffects { get; }
        public FakeFailureCauseRepository FailureCauses { get; }
        public FakePreventionControlRepository PreventionControls { get; }
        public FakeDetectionControlRepository DetectionControls { get; }
        public FakeUnitOfWork UnitOfWork { get; }
        public FmeaEditabilityGuard Guard { get; }
    }

    private sealed class FakeRevisionRepository : IFmeaRevisionRepository
    {
        private readonly FmeaRevision _revision;

        public FakeRevisionRepository(FmeaRevision revision) => _revision = revision;

        public Task<FmeaRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<FmeaRevision?>(_revision.Id == id ? _revision : null);

        public Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(Guid fmeaId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FmeaRevision>>(new[] { _revision });

        public void Add(FmeaRevision revision) => throw new NotSupportedException();
    }

    private sealed class FakeProcessStepRepository : IProcessStepRepository
    {
        private readonly ProcessStep _step;

        public FakeProcessStepRepository(ProcessStep step) => _step = step;

        public Task<bool> ExistsBySequenceAsync(Guid fmeaRevisionId, int sequence, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<ProcessStep?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<ProcessStep?>(_step.Id == id ? _step : null);

        public Task<IReadOnlyList<ProcessStep>> ListByRevisionIdAsync(Guid fmeaRevisionId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProcessStep>>(new[] { _step });

        public void Add(ProcessStep processStep) => throw new NotSupportedException();
    }

    private sealed class FakeFailureModeRepository : IFailureModeRepository
    {
        public FailureMode? Added { get; private set; }

        public Task<FailureMode?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<FailureMode?>(Added is not null && Added.Id == id ? Added : null);

        public Task<IReadOnlyList<FailureMode>> ListByProcessStepIdAsync(Guid processStepId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FailureMode>>(
                Added is null ? Array.Empty<FailureMode>() : new[] { Added });

        public void Add(FailureMode failureMode) => Added = failureMode;
    }

    private sealed class FakeFailureEffectRepository : IFailureEffectRepository
    {
        public FailureEffect? Added { get; private set; }

        public Task<FailureEffect?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<FailureEffect?>(Added is not null && Added.Id == id ? Added : null);

        public Task<IReadOnlyList<FailureEffect>> ListByFailureModeIdAsync(Guid failureModeId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FailureEffect>>(
                Added is null ? Array.Empty<FailureEffect>() : new[] { Added });

        public void Add(FailureEffect failureEffect) => Added = failureEffect;
    }

    private sealed class FakeFailureCauseRepository : IFailureCauseRepository
    {
        public FailureCause? Added { get; private set; }

        public Task<FailureCause?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<FailureCause?>(Added is not null && Added.Id == id ? Added : null);

        public Task<IReadOnlyList<FailureCause>> ListByFailureModeIdAsync(Guid failureModeId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FailureCause>>(
                Added is null ? Array.Empty<FailureCause>() : new[] { Added });

        public void Add(FailureCause failureCause) => Added = failureCause;
    }

    private sealed class FakePreventionControlRepository : IPreventionControlRepository
    {
        public PreventionControl? Added { get; private set; }

        public Task<PreventionControl?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<PreventionControl?>(Added is not null && Added.Id == id ? Added : null);

        public Task<IReadOnlyList<PreventionControl>> ListByFailureCauseIdAsync(Guid failureCauseId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PreventionControl>>(
                Added is null ? Array.Empty<PreventionControl>() : new[] { Added });

        public void Add(PreventionControl preventionControl) => Added = preventionControl;
    }

    private sealed class FakeDetectionControlRepository : IDetectionControlRepository
    {
        public DetectionControl? Added { get; private set; }

        public Task<DetectionControl?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<DetectionControl?>(Added is not null && Added.Id == id ? Added : null);

        public Task<IReadOnlyList<DetectionControl>> ListByFailureCauseIdAsync(Guid failureCauseId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DetectionControl>>(
                Added is null ? Array.Empty<DetectionControl>() : new[] { Added });

        public void Add(DetectionControl detectionControl) => Added = detectionControl;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
