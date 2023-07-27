using DotNetBB.PushNotification.Abstraction;
using DotNetBB.PushNotification.Model;

namespace DotNetBB.PushNotification.Implemention;

public class APNSClient : IAPNSClient
{
    protected const string _apnsProdUrl =  "https://api.push.apple.com";
    protected const string _apnsSandboxUrl = "https://api.sandbox.push.apple.com";
    protected IAPNSConfiguration _apnsCofiguration;
    protected string _apnsUrl => _apnsCofiguration.IsTest ? _apnsSandboxUrl : _apnsProdUrl;
    
    protected string? _lastGeneratedJWT;
    protected DateTime? _lastJWTGenerationDate;

    public APNSClient(IAPNSConfiguration apnsCofiguration)
    {
        _apnsCofiguration = apnsCofiguration;
    }

    public bool SendNotification(string pushRegistrationId, string topicName, APNSData data, out string responseText)
    {
        responseText = "";
        
        if(string.IsNullOrWhiteSpace(_lastGeneratedJWT) 
            || _lastJWTGenerationDate == null 
            || ((DateTime)_lastJWTGenerationDate).AddMinutes(15) < DateTime.UtcNow)
        {
            _lastGeneratedJWT = CreateJwt();
        }

        string jwt = _lastGeneratedJWT?? throw new ArgumentNullException(nameof(_lastGeneratedJWT));

        return SendNotificationInternal(jwt, pushRegistrationId, topicName, data, out responseText);
    }

    private bool SendNotificationInternal(string apnsJwt, string pushRegistrationId, string topicName, APNSData data, out string responseText)
    {
        responseText = "";
        ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);
        var postData = JsonSerializer.Serialize(data);

        var request = new HttpRequestMessage(HttpMethod.Post, "/3/device/" + pushRegistrationId)
        {
            Version = new System.Version(2, 0),
            Content = new StringContent(postData, Encoding.UTF8, "application/json"),
        };
        
        request.Headers.Add("Authorization", string.Format("Bearer {0}", apnsJwt));
        request.Headers.Add("apns-push-type", "alert");
        request.Headers.Add("apns-expiration", "0");
        request.Headers.Add("apns-priority", "10");
        request.Headers.Add("apns-topic", topicName);

        var client = new HttpClient() 
        {
            BaseAddress = new Uri(_apnsUrl),
            DefaultRequestVersion = new Version(2, 0)
        };

        var response = client.SendAsync(request).GetAwaiter().GetResult();
        HttpStatusCode ResponseCode = response.StatusCode;
        if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
        {
            responseText = "Unauthorized - need new token";
            return false;
        }
        else if (!ResponseCode.Equals(HttpStatusCode.OK))
        {
            responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if(string.IsNullOrWhiteSpace(responseText))
            {
                responseText = "Response from web service isn't OK";
            }
            
            return false;
        }

        return true;

    }

    
    private string CreateJwt()
    {
        var dsa = GetECDsa();
        var securityKey = new ECDsaSecurityKey(dsa) { KeyId = _apnsCofiguration.AuthKey };
        var credentials = new SigningCredentials(securityKey, "ES256");

        var descriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.Now,
            Issuer = _apnsCofiguration.TeamId,
            SigningCredentials = credentials,
            
        };

        var handler = new JwtSecurityTokenHandler();
        var encodedToken = handler.CreateEncodedJwt(descriptor);
        return encodedToken;
    }

    private ECDsa GetECDsa()
    {
        using (TextReader reader = new StringReader(_apnsCofiguration.AuthKeySecret))
        {
            var ecPrivateKeyParameters = (ECPrivateKeyParameters)new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
            Org.BouncyCastle.Math.EC.ECPoint q = ecPrivateKeyParameters.Parameters.G.Multiply(ecPrivateKeyParameters.D).Normalize();

            var msEcp = new ECParameters();
            msEcp.Curve = ECCurve.NamedCurves.nistP256;
            msEcp.Q.X = q.AffineXCoord.GetEncoded();
            msEcp.Q.Y = q.AffineYCoord.GetEncoded();
            msEcp.D =  ecPrivateKeyParameters.D.ToByteArrayUnsigned();

            return ECDsa.Create(msEcp);
        }
    }

    public bool ValidateServerCertificate(
                                                object sender,
                                                X509Certificate certificate,
                                                X509Chain chain,
                                                SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}