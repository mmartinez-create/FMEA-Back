namespace FMEA_Api.Contracts.ControlPlans;

public sealed record CreateControlPlanRequest(
    string Number,
    string Name);
