using System.Net.Security;

namespace CollectorApp.Platforms.Android;

public static class HttpsConfig
{
    public static HttpClientHandler GetDevelopmentHandler()
    {
        var handler = new HttpClientHandler();

#if DEBUG
        // Tylko w DEBUG – ignorujemy błędy certyfikatu dla lokalnego API
        handler.ServerCertificateCustomValidationCallback =
            (message, cert, chain, errors) =>
            {
                if (cert?.Issuer?.Contains("localhost") == true)
                    return true;

                return errors == SslPolicyErrors.None;
            };
#endif

        return handler;
    }
}