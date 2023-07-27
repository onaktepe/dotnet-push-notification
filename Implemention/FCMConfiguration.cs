using DotNetBB.PushNotification.Abstraction;

namespace DotNetBB.PushNotification.Implemention;

public class FCMConfiguration : IFCMConfiguration
{
    private string _authKey;

    public FCMConfiguration(string authKey)
    {
        _authKey = authKey;
    }

    public string AuthKey => _authKey;
}