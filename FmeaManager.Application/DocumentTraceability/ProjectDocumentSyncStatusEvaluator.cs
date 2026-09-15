namespace FmeaManager.Application.DocumentTraceability;

public static class ProjectDocumentSyncStatusEvaluator
{
    public static DocumentSynchronizationState EvaluatePfmea(
        int sharedStepCount,
        int pfmeaDocumentCount,
        int linkedSharedStepCount)
    {
        if (sharedStepCount == 0 || pfmeaDocumentCount == 0)
        {
            return DocumentSynchronizationState.Missing;
        }

        if (linkedSharedStepCount < sharedStepCount)
        {
            return DocumentSynchronizationState.NeedsSync;
        }

        return DocumentSynchronizationState.Synchronized;
    }

    public static DocumentSynchronizationState EvaluateControlPlan(
        int sharedStepCount,
        bool hasControlPlan,
        int characteristicCount,
        bool hasInvalidSharedReferences)
    {
        if (sharedStepCount == 0 || !hasControlPlan)
        {
            return DocumentSynchronizationState.Missing;
        }

        if (hasInvalidSharedReferences)
        {
            return DocumentSynchronizationState.Attention;
        }

        if (characteristicCount == 0)
        {
            return DocumentSynchronizationState.Ready;
        }

        return DocumentSynchronizationState.Synchronized;
    }

    public static DocumentSynchronizationState EvaluateOverall(
        DocumentSynchronizationState asmf,
        DocumentSynchronizationState pfmea,
        DocumentSynchronizationState controlPlan)
    {
        if (asmf == DocumentSynchronizationState.Missing)
        {
            return DocumentSynchronizationState.Missing;
        }

        if (
            pfmea == DocumentSynchronizationState.Attention ||
            controlPlan == DocumentSynchronizationState.Attention)
        {
            return DocumentSynchronizationState.Attention;
        }

        if (
            pfmea == DocumentSynchronizationState.NeedsSync ||
            controlPlan == DocumentSynchronizationState.NeedsSync)
        {
            return DocumentSynchronizationState.NeedsSync;
        }

        if (
            pfmea == DocumentSynchronizationState.Synchronized &&
            controlPlan == DocumentSynchronizationState.Synchronized)
        {
            return DocumentSynchronizationState.Synchronized;
        }

        return DocumentSynchronizationState.Ready;
    }
}
