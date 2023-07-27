using DotNetBB.PushNotification.Abstraction;
using DotNetBB.PushNotification.Model;

namespace DotNetBB.PushNotification.Implemention;

public class FCMClient : IFCMClient
{
    protected readonly IFCMConfiguration _fcmConfiguration;
    public FCMClient(IFCMConfiguration fcmConfiguration)
    {
        _fcmConfiguration = fcmConfiguration;
    }

    public bool SendNotification(FCMData data, out string responseText)
    {
        responseText = string.Empty;

        ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);
        var postData = JsonSerializer.Serialize(data);

        byte[] byteArray = Encoding.UTF8.GetBytes(postData);

        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        Request.Method = "POST";
        Request.KeepAlive = false;
        Request.ContentType = "application/json";
        Request.Headers.Add(string.Format("Authorization: key={0}", _fcmConfiguration.AuthKey));
        Request.ContentLength = byteArray.Length;

        Stream dataStream = Request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();

        WebResponse Response = Request.GetResponse();
        HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
        if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
        {
            responseText = "Unauthorized - need new token";
            return false;

        }
        else if (!ResponseCode.Equals(HttpStatusCode.OK))
        {
            responseText = "Response from web service isn't OK";
            return false;
        }

        StreamReader Reader = new StreamReader(Response.GetResponseStream());
        responseText = Reader.ReadToEnd();
        Reader.Close();

        dynamic jObj = JsonSerializer.Deserialize<dynamic>(responseText);

        int successCount = 0;
        if (jObj["success"] != null)
        {
            successCount = (int)jObj["success"];
        }

        if (successCount == 0)
        {
            return false;
        }

        return true;
    }

    private static bool ValidateServerCertificate(
                                                object sender,
                                                X509Certificate certificate,
                                                X509Chain chain,
                                                SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}