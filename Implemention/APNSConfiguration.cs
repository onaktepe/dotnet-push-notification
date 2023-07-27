using DotNetBB.PushNotification.Abstraction;

namespace DotNetBB.PushNotification.Implemention;

public class APNSConfiguration : IAPNSConfiguration
{
    private string _authKey;
    private string _authKeySecret;
    private string _teamId;
    private bool _isTest;

    public APNSConfiguration(string authKey, string authKeySecret, string teamId, bool isTest=false)
    {
        _authKey = authKey;
        _authKeySecret = authKeySecret;
        _teamId = teamId;
        _isTest = isTest;
    }

    public string AuthKey => _authKey;

    public string AuthKeySecret => _authKeySecret;

    public string TeamId => _teamId;

    public bool IsTest => _isTest;
}