namespace FMEA_Api.Security;

public sealed class CurrentUserAccessor
{
    public const string UserHeader = "X-Fmea-User";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserKey
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;

            var headerUser =
                context?.Request.Headers[UserHeader]
                    .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(headerUser))
            {
                return headerUser.Trim().ToLowerInvariant();
            }

            var authenticated =
                context?.User?.Identity?.Name;

            return string.IsNullOrWhiteSpace(authenticated)
                ? "local-development"
                : authenticated.Trim().ToLowerInvariant();
        }
    }

    public string DisplayName
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;

            var claimName =
                context?.User?.Claims
                    .FirstOrDefault(claim =>
                        claim.Type == "name" ||
                        claim.Type.EndsWith("/name", StringComparison.OrdinalIgnoreCase))
                    ?.Value;

            return string.IsNullOrWhiteSpace(claimName)
                ? UserKey
                : claimName.Trim();
        }
    }

}
