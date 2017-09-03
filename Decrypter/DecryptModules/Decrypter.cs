using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Decrypter.DecryptModules
{
    public class GenericDecrypter
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

        public static Mode ModeFromFileName(string NameOrExtension)
        {
            string ext;
            switch (ext=NameOrExtension.Split('.').Last().ToLower())
            {
                case "rsdf":
                    return Mode.RSDF;
                case "ccf":
                    return Mode.CCF;
                case "dlc":
                    return Mode.DLC;
                default:
                    throw new Exception($"Can't convert {ext} to a supported value");
            }
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
