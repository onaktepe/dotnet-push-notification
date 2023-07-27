using DotNetBB.PushNotification.Model;

namespace DotNetBB.PushNotification.Abstraction;

public interface IAPNSClient
{
    bool SendNotification(string pushRegistrationId, string topicName, APNSData data, out string responseText);
}