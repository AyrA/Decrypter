using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Decrypter.DecryptModules
{
    public class Decrypter
    {
        public struct WebResponse
        {
            public bool success;
            public string message;
            public string[] data;
        }

        public const string ENDPOINT = "https://cable.ayra.ch/decrypt/decrypt.php?mode=";

        public enum Mode : int
        {
            RSDF,
            CCF,
            DLC
        }

        public static WebResponse Decrypt(byte[] Content, Mode FileType)
        {
            var Req = WebRequest.CreateHttp(ENDPOINT + FileType.ToString().ToLower());

            Req.Method = "POST";

            using (var S = Req.GetRequestStream())
            {
                S.Write(Content, 0, Content.Length);
            }
            using (var Res = Req.GetResponse())
            {
                var Response = "";
                try
                {
                    using (var S = Res.GetResponseStream())
                    {
                        using (var SR = new StreamReader(S))
                        {
                            return JsonConvert.DeserializeObject<WebResponse>(Response = SR.ReadToEnd());
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new WebResponse()
                    {
                        data = new string[] { Response },
                        message = "Can't process Web response. Message: " + ex.Message,
                        success = false
                    };
                }
            }
        }
    }
}
