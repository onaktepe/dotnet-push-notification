namespace DotNetBB.PushNotification.Abstraction;

public interface IAPNSConfiguration
{
    string AuthKey { get; }
    string AuthKeySecret { get; }
    string TeamId { get; }
    bool IsTest { get; }
}