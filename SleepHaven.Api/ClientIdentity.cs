using SleepHaven;

namespace SleepHaven.Api;

internal static class ClientIdentity
{
    public static bool TryRead(HttpRequest request, out string clientId, out IResult error)
    {
        clientId = request.Headers[BackendProductStore.ClientHeaderName].ToString().Trim();
        if (clientId.Length is > 0 and <= 128)
        {
            error = Results.Empty;
            return true;
        }

        error = Results.BadRequest(new
        {
            Error = $"A 1-128 character {BackendProductStore.ClientHeaderName} header is required."
        });
        return false;
    }
}
