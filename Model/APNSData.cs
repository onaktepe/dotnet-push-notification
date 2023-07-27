using System;

namespace DotNetBB.PushNotification.Model;

public class APNSData
{
    public ApsData aps { get; set; }
    public AdditionalData additionalData { get; set; }

    public APNSData()
    {
        aps = new ApsData();
        additionalData = new AdditionalData();
    }

}

public class ApsData
{
    public Alert alert {get; set;}
    public string category {get; set;}
    public ApsData()
    {   
        alert = new Alert();
    }
}

public class Alert 
{
    public string title { get; set; }
    public string subtitle { get; set; }
    public string body { get; set; }
    public string launchImage { get; set; }
}

public class AdditionalData
{
    public string notificationId { get; set; }
    public string type { get; set; }
    public string extradata { get; set; }
    public string sname { get; set; }
    public string sparam { get; set; }
}
