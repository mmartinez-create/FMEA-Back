using FmeaManager.Application.Abstractions.Persistence;
using FmeaManager.Application.Common.Exceptions;
using FmeaManager.Application.Fmeas.Common;
using FmeaManager.Application.Fmeas.GetRiskAssessment;
using FmeaManager.Application.Fmeas.UpsertRiskAssessment;
using FmeaManager.Domain.Fmeas;

namespace FmeaManager.Application.Tests.Fmeas.RiskAssessment;

public sealed class RiskAssessmentHandlersTests
{
    [Fact]
    public async Task Upsert_WhenAssessmentDoesNotExist_CreatesAndCalculatesRpn()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        var step = ProcessStep.Create(revision.Id, 10, "Assembly");
        var mode = FailureMode.Create(step.Id, "Wrong polarity");
        var cause = FailureCause.Create(mode.Id, "Terminal reversed");

        var context = new FakeContext(revision, step, mode, cause);

        var handler = new UpsertRiskAssessmentHandler(
            context.Guard,
            context.RiskAssessments,
            context.UnitOfWork);

        var result = await handler.HandleAsync(
            new UpsertRiskAssessmentCommand(
                cause.Id,
                9,
                4,
                3,
                "cris"));

        Assert.Equal(108, result.Rpn);
        Assert.NotNull(context.RiskAssessments.Assessment);
        Assert.Equal(1, context.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Upsert_WhenAssessmentExists_UpdatesSameAssessment()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        var step = ProcessStep.Create(revision.Id, 10, "Assembly");
        var mode = FailureMode.Create(step.Id, "Wrong polarity");
        var cause = FailureCause.Create(mode.Id, "Terminal reversed");

        var existing = FmeaManager.Domain.Fmeas.RiskAssessment.Create(
            cause.Id,
            5,
            5,
            5,
            "creator");

        var context = new FakeContext(revision, step, mode, cause, existing);

        var handler = new UpsertRiskAssessmentHandler(
            context.Guard,
            context.RiskAssessments,
            context.UnitOfWork);

        var result = await handler.HandleAsync(
            new UpsertRiskAssessmentCommand(
                cause.Id,
                10,
                2,
                4,
                "reviewer"));

        Assert.Equal(existing.Id, result.Id);
        Assert.Equal(80, result.Rpn);
        Assert.Equal("reviewer", result.UpdatedBy);
    }

    [Fact]
    public async Task Upsert_WhenRevisionIsUnderReview_ThrowsConflict()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        revision.SubmitForReview();

        var step = ProcessStep.Create(revision.Id, 10, "Assembly");
        var mode = FailureMode.Create(step.Id, "Wrong polarity");
        var cause = FailureCause.Create(mode.Id, "Terminal reversed");

        var context = new FakeContext(revision, step, mode, cause);

        var handler = new UpsertRiskAssessmentHandler(
            context.Guard,
            context.RiskAssessments,
            context.UnitOfWork);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(
                new UpsertRiskAssessmentCommand(
                    cause.Id,
                    9,
                    4,
                    3,
                    "cris")));
    }

    [Fact]
    public async Task Get_WhenAssessmentExists_ReturnsAssessment()
    {
        var revision = FmeaRevision.CreateInitial(Guid.NewGuid(), "cris");
        var step = ProcessStep.Create(revision.Id, 10, "Assembly");
        var mode = FailureMode.Create(step.Id, "Wrong polarity");
        var cause = FailureCause.Create(mode.Id, "Terminal reversed");

        var existing = FmeaManager.Domain.Fmeas.RiskAssessment.Create(
            cause.Id,
            9,
            4,
            3,
            "cris");

        var causeRepository = new FakeFailureCauseRepository(cause);
        var riskRepository = new FakeRiskAssessmentRepository(existing);

        var handler = new GetRiskAssessmentHandler(
            causeRepository,
            riskRepository);

        var result = await handler.HandleAsync(cause.Id);

        Assert.NotNull(result);
        Assert.Equal(108, result!.Rpn);
    }

    [Fact]
    public async Task Get_WhenCauseExistsButAssessmentDoesNot_ReturnsNull()
    {
        var cause = FailureCause.Create(
            Guid.NewGuid(),
            "Terminal reversed");

        var handler = new GetRiskAssessmentHandler(
            new FakeFailureCauseRepository(cause),
            new FakeRiskAssessmentRepository());

        var result = await handler.HandleAsync(cause.Id);

        Assert.Null(result);
    }

    private sealed class FakeContext
    {
        public FakeContext(
            FmeaRevision revision,
            ProcessStep step,
            FailureMode mode,
            FailureCause cause,
            FmeaManager.Domain.Fmeas.RiskAssessment? assessment = null)
        {
            Revisions = new FakeRevisionRepository(revision);
            ProcessSteps = new FakeProcessStepRepository(step);
            FailureModes = new FakeFailureModeRepository(mode);
            FailureCauses = new FakeFailureCauseRepository(cause);
            RiskAssessments = new FakeRiskAssessmentRepository(assessment);
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
        public FakeFailureCauseRepository FailureCauses { get; }
        public FakeRiskAssessmentRepository RiskAssessments { get; }
        public FakeUnitOfWork UnitOfWork { get; }
        public FmeaEditabilityGuard Guard { get; }
    }

    private sealed class FakeRevisionRepository : IFmeaRevisionRepository
    {
        private readonly FmeaRevision _revision;

        public FakeRevisionRepository(FmeaRevision revision)
        {
            _revision = revision;
        }

        public Task<FmeaRevision?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<FmeaRevision?>(
                _revision.Id == id ? _revision : null);

        public Task<IReadOnlyList<FmeaRevision>> ListByFmeaIdAsync(
            Guid fmeaId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FmeaRevision>>(
                _revision.FmeaId == fmeaId
                    ? new[] { _revision }
                    : Array.Empty<FmeaRevision>());

        public void Add(FmeaRevision revision) =>
            throw new NotSupportedException();
    }

    private sealed class FakeProcessStepRepository : IProcessStepRepository
    {
        private readonly ProcessStep _step;

        public FakeProcessStepRepository(ProcessStep step)
        {
            _step = step;
        }

        public Task<bool> ExistsBySequenceAsync(
            Guid fmeaRevisionId,
            int sequence,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<ProcessStep?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<ProcessStep?>(
                _step.Id == id ? _step : null);

        public Task<IReadOnlyList<ProcessStep>> ListByRevisionIdAsync(
            Guid revisionId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProcessStep>>(
                _step.FmeaRevisionId == revisionId
                    ? new[] { _step }
                    : Array.Empty<ProcessStep>());

        public void Add(ProcessStep processStep) =>
            throw new NotSupportedException();
    }

    private sealed class FakeFailureModeRepository : IFailureModeRepository
    {
        private readonly FailureMode _mode;

        public FakeFailureModeRepository(FailureMode mode)
        {
            _mode = mode;
        }

        public Task<FailureMode?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<FailureMode?>(
                _mode.Id == id ? _mode : null);

        public Task<IReadOnlyList<FailureMode>> ListByProcessStepIdAsync(
            Guid processStepId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FailureMode>>(
                _mode.ProcessStepId == processStepId
                    ? new[] { _mode }
                    : Array.Empty<FailureMode>());

        public void Add(FailureMode failureMode) =>
            throw new NotSupportedException();
    }

    private sealed class FakeFailureCauseRepository : IFailureCauseRepository
    {
        private readonly FailureCause _cause;

        public FakeFailureCauseRepository(FailureCause cause)
        {
            _cause = cause;
        }

        public Task<FailureCause?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<FailureCause?>(
                _cause.Id == id ? _cause : null);

        public Task<IReadOnlyList<FailureCause>> ListByFailureModeIdAsync(
            Guid failureModeId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FailureCause>>(
                _cause.FailureModeId == failureModeId
                    ? new[] { _cause }
                    : Array.Empty<FailureCause>());

        public void Add(FailureCause failureCause) =>
            throw new NotSupportedException();
    }

    private sealed class FakeRiskAssessmentRepository : IRiskAssessmentRepository
    {
        public FakeRiskAssessmentRepository(
            FmeaManager.Domain.Fmeas.RiskAssessment? assessment = null)
        {
            Assessment = assessment;
        }

        public FmeaManager.Domain.Fmeas.RiskAssessment? Assessment { get; private set; }

        public Task<FmeaManager.Domain.Fmeas.RiskAssessment?> GetByFailureCauseIdAsync(
            Guid failureCauseId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(
                Assessment is not null &&
                Assessment.FailureCauseId == failureCauseId
                    ? Assessment
                    : null);

        public void Add(FmeaManager.Domain.Fmeas.RiskAssessment riskAssessment)
        {
            Assessment = riskAssessment;
        }
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
