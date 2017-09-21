using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Decrypter.DecryptModules
{
    public class GenericDecrypter
    {
        public struct WebResponse
        {
            public bool success;
            public string message;
            public ContainerData data;
        }

        public struct ContainerData
        {
            public string name;
            public string[] links;
        }

        public const string ENDPOINT = "https://cable.ayra.ch/decrypt/decrypt.php?mode={0}&name={1}";

        public enum Mode : int
        {
            RSDF,
            CCF,
            DLC,
            Check
        }

        public static string GetHash(byte[] Content)
        {
            using (var Hasher = new SHA1Managed())
            {
                return string.Concat(Hasher.ComputeHash(Content).Select(m => m.ToString("X2"))).Replace("-", "").ToLower();
            }
        }

        public static Mode ModeFromFileName(string NameOrExtension)
        {
            string ext;
            switch (ext = NameOrExtension.Split('.').Last().ToLower())
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

        public static async Task<WebResponse> Decrypt(byte[] Content, string Name, Mode FileType)
        {
            if (FileType == Mode.Check)
            {
                throw new ArgumentException("FileType can't be 'Check'");
            }
            var Req = WebRequest.CreateHttp(string.Format(ENDPOINT, FileType.ToString().ToLower(), Name));

            Req.Method = "POST";

            using (var S = await Req.GetRequestStreamAsync())
            {
                S.Write(Content, 0, Content.Length);
            }
            using (var Res = await Req.GetResponseAsync())
            {
                var Response = "";
                try
                {
                    using (var S = Res.GetResponseStream())
                    {
                        using (var SR = new StreamReader(S))
                        {
                            return JsonConvert.DeserializeObject<WebResponse>(Response = await SR.ReadToEndAsync());
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new WebResponse()
                    {
                        data = new ContainerData() { name = null, links = new string[] { Response } },
                        message = "Can't process Web response. Message: " + ex.Message,
                        success = false
                    };
                }
            }
        }

        public static async Task<bool> Hash(string Hash)
        {
            var Req = WebRequest.CreateHttp(string.Format(ENDPOINT, Mode.Check.ToString().ToLower(), Hash));

            Req.Method = "GET";

            using (var Res = await Req.GetResponseAsync())
            {
                var Response = "";
                try
                {
                    using (var S = Res.GetResponseStream())
                    {
                        using (var SR = new StreamReader(S))
                        {
                            var Result = JsonConvert.DeserializeObject<WebResponse>(Response = await SR.ReadToEndAsync());
                            return Result.success && string.IsNullOrEmpty(Result.message);
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
