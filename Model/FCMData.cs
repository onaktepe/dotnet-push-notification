using System;

namespace DotNetBB.PushNotification.Model;

public class FCMData
{
    public string to { get; set; }
    public FCMNotificationData notification { get; set; }
    public FCMCustomData data { get; set; }

    public FCMData()
    {
        notification = new FCMNotificationData();
        data = new FCMCustomData();
    }
}

public class FCMNotificationData
{
    public string title { get; set; }
    public string body { get; set; }
    public string image { get; set; }
}

public class FCMCustomData
{
    public int notificationId { get; set; }
    public string title { get; set; }
    public string content { get; set; }
    public string type { get; set; }
    public string itemType { get; set; }
    public int itemId { get; set; }
    public int userId { get; set; }
    public string photoUrl { get; set; }
    public string sname { get; set; }
    public string sparam { get; set; }
}
