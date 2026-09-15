using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.GetDetectionControlsByFailureCause;
using FmeaManager.Application.Fmeas.GetFailureCausesByFailureMode;
using FmeaManager.Application.Fmeas.GetFailureEffectsByFailureMode;
using FmeaManager.Application.Fmeas.GetFailureModesByProcessStep;
using FmeaManager.Application.Fmeas.GetPreventionControlsByFailureCause;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Tests.Fmeas.FailureAnalysis;

public sealed class GetFailureAnalysisHandlersTests
{
    [Fact]
    public async Task GetFailureModes_WhenProcessStepExists_ReturnsModes()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        var step = ProcessStep.Create(revision.Id, 10, "Assembly");
        var mode = FailureMode.Create(step.Id, "Wrong polarity");

        var handler = new GetFailureModesByProcessStepHandler(
            new FakeProcessStepRepository(step),
            new FakeFailureModeRepository(mode));

        var result = await handler.HandleAsync(step.Id);

        var item = Assert.Single(result);
        Assert.Equal(mode.Id, item.Id);
        Assert.Equal(step.Id, item.ProcessStepId);
        Assert.Equal("Wrong polarity", item.Description);
    }

    [Fact]
    public async Task GetFailureModes_WhenProcessStepDoesNotExist_ThrowsNotFound()
    {
        var handler = new GetFailureModesByProcessStepHandler(
            new FakeProcessStepRepository(null),
            new FakeFailureModeRepository(null));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetEffectsAndCauses_WhenFailureModeExists_ReturnsCollections()
    {
        var stepId = Guid.NewGuid();
        var mode = FailureMode.Create(stepId, "Incorrect terminal connection");
        var effect = FailureEffect.Create(mode.Id, "Vehicle cannot start");
        var cause = FailureCause.Create(mode.Id, "Terminal in wrong position");

        var modeRepo = new FakeFailureModeRepository(mode);

        var effectsHandler = new GetFailureEffectsByFailureModeHandler(
            modeRepo,
            new FakeFailureEffectRepository(effect));

        var causesHandler = new GetFailureCausesByFailureModeHandler(
            modeRepo,
            new FakeFailureCauseRepository(cause));

        var effects = await effectsHandler.HandleAsync(mode.Id);
        var causes = await causesHandler.HandleAsync(mode.Id);

        Assert.Equal("Vehicle cannot start", Assert.Single(effects).Description);
        Assert.Equal("Terminal in wrong position", Assert.Single(causes).Description);
    }

    [Fact]
    public async Task GetControls_WhenFailureCauseExists_ReturnsBothControlTypes()
    {
        var mode = FailureMode.Create(Guid.NewGuid(), "Incorrect connection");
        var cause = FailureCause.Create(mode.Id, "Wrong position");
        var prevention = PreventionControl.Create(cause.Id, "Poka-yoke fixture");
        var detection = DetectionControl.Create(cause.Id, "Vision inspection");

        var causeRepo = new FakeFailureCauseRepository(cause);

        var preventionHandler = new GetPreventionControlsByFailureCauseHandler(
            causeRepo,
            new FakePreventionControlRepository(prevention));

        var detectionHandler = new GetDetectionControlsByFailureCauseHandler(
            causeRepo,
            new FakeDetectionControlRepository(detection));

        var preventionResult = await preventionHandler.HandleAsync(cause.Id);
        var detectionResult = await detectionHandler.HandleAsync(cause.Id);

        Assert.Equal("Poka-yoke fixture", Assert.Single(preventionResult).Description);
        Assert.Equal("Vision inspection", Assert.Single(detectionResult).Description);
    }

    private sealed class FakeProcessStepRepository : IProcessStepRepository
    {
        private readonly ProcessStep? _step;

        public FakeProcessStepRepository(ProcessStep? step) => _step = step;

        public Task<bool> ExistsBySequenceAsync(
            Guid fmeaRevisionId,
            int sequence,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<ProcessStep?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_step is not null && _step.Id == id ? _step : null);

        public Task<IReadOnlyList<ProcessStep>> ListByRevisionIdAsync(
            Guid revisionId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProcessStep>>(
                _step is not null && _step.FmeaRevisionId == revisionId
                    ? new[] { _step }
                    : Array.Empty<ProcessStep>());

        public void Add(ProcessStep processStep) =>
            throw new NotSupportedException();
    }

    private sealed class FakeFailureModeRepository : IFailureModeRepository
    {
        private readonly FailureMode? _mode;

        public FakeFailureModeRepository(FailureMode? mode) => _mode = mode;

        public Task<FailureMode?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_mode is not null && _mode.Id == id ? _mode : null);

        public Task<IReadOnlyList<FailureMode>> ListByProcessStepIdAsync(
            Guid processStepId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FailureMode>>(
                _mode is not null && _mode.ProcessStepId == processStepId
                    ? new[] { _mode }
                    : Array.Empty<FailureMode>());

        public void Add(FailureMode failureMode) =>
            throw new NotSupportedException();
    }

    private sealed class FakeFailureEffectRepository : IFailureEffectRepository
    {
        private readonly FailureEffect? _effect;

        public FakeFailureEffectRepository(FailureEffect? effect) => _effect = effect;

        public Task<FailureEffect?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_effect is not null && _effect.Id == id ? _effect : null);

        public Task<IReadOnlyList<FailureEffect>> ListByFailureModeIdAsync(
            Guid failureModeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FailureEffect>>(
                _effect is not null && _effect.FailureModeId == failureModeId
                    ? new[] { _effect }
                    : Array.Empty<FailureEffect>());

        public void Add(FailureEffect failureEffect) =>
            throw new NotSupportedException();
    }

    private sealed class FakeFailureCauseRepository : IFailureCauseRepository
    {
        private readonly FailureCause? _cause;

        public FakeFailureCauseRepository(FailureCause? cause) => _cause = cause;

        public Task<FailureCause?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_cause is not null && _cause.Id == id ? _cause : null);

        public Task<IReadOnlyList<FailureCause>> ListByFailureModeIdAsync(
            Guid failureModeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FailureCause>>(
                _cause is not null && _cause.FailureModeId == failureModeId
                    ? new[] { _cause }
                    : Array.Empty<FailureCause>());

        public void Add(FailureCause failureCause) =>
            throw new NotSupportedException();
    }

    private sealed class FakePreventionControlRepository : IPreventionControlRepository
    {
        private readonly PreventionControl? _control;

        public FakePreventionControlRepository(PreventionControl? control) =>
            _control = control;

        public Task<PreventionControl?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_control is not null && _control.Id == id ? _control : null);

        public Task<IReadOnlyList<PreventionControl>> ListByFailureCauseIdAsync(
            Guid failureCauseId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PreventionControl>>(
                _control is not null && _control.FailureCauseId == failureCauseId
                    ? new[] { _control }
                    : Array.Empty<PreventionControl>());

        public void Add(PreventionControl preventionControl) =>
            throw new NotSupportedException();
    }

    private sealed class FakeDetectionControlRepository : IDetectionControlRepository
    {
        private readonly DetectionControl? _control;

        public FakeDetectionControlRepository(DetectionControl? control) =>
            _control = control;

        public Task<DetectionControl?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_control is not null && _control.Id == id ? _control : null);

        public Task<IReadOnlyList<DetectionControl>> ListByFailureCauseIdAsync(
            Guid failureCauseId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DetectionControl>>(
                _control is not null && _control.FailureCauseId == failureCauseId
                    ? new[] { _control }
                    : Array.Empty<DetectionControl>());

        public void Add(DetectionControl detectionControl) =>
            throw new NotSupportedException();
    }
}
