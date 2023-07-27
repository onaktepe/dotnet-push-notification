using DotNetBB.PushNotification.Model;

namespace DotNetBB.PushNotification.Abstraction;

public interface IFCMClient
{
    bool SendNotification(FCMData data, out string responseText);
}