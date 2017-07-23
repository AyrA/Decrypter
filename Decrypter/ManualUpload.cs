using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Decrypter
{
    public static class ManualUpload
    {
        private const string HEADER = @"POST /decrypt/upload HTTP/1.1
Host: dcrypt.it
Content-Length: {0}
Connection: close
Cache-Control: max-age=0
Origin: http://dcrypt.it
User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36
Content-Type: multipart/form-data; boundary=----SomeRandomBoundary
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp
Referer: http://dcrypt.it/
Accept-Language: en-US,en;q=0.8,de;q=0.6";

        private const string BODY = "------SomeRandomBoundary\r\n" +
            "Content-Disposition: form-data; name=\"dlcfile\"; filename=\"-.dlc\"\r\n"+
            "Content-Type: application/octet-stream\r\n"+
            "\r\n"+
            "{0}\r\n"+
            "------SomeRandomBoundary--\r\n";

        private struct Connection
        {
            public TcpClient Client;
            public IPEndPoint RemoteEnd;

            public Connection(IPAddress A, int Port = 80)
            {
                Client = new TcpClient();
                RemoteEnd = new IPEndPoint(A, Port);
            }

            public bool Connect(int Timeout = 3000)
            {
                Thread T = new Thread(Connect);
                T.IsBackground = true;
                T.Start();
                if (!T.Join(Timeout))
                {
                    try
                    {
                        T.Abort();
                    }
                    catch { }
                    return false;
                }
                return true;
            }

            private void Connect()
            {
                Client.Connect(RemoteEnd);
            }
        }

        public struct Result
        {
            public ResultContent success; 
        }

        public struct ResultContent
        {
            public string message;
            public string[] links;
        }

        private static string GetHeader(string BodyContent)
        {
            var Body = string.Format(BODY, BodyContent);
            var Header = string.Format(HEADER, Body.Length);
            return $"{Header}\r\n\r\n{Body}";
        }

        public static Result Upload(Stream SourceStream)
        {
            using (var MS = new MemoryStream())
            {
                SourceStream.CopyTo(MS);
                return Upload(MS.ToArray());
            }
        }

        public static Result Upload(byte[] FileContent)
        {
            var Connections = GetSocket("dcrypt.it");
            if (Connections == null || Connections.Length == 0)
            {
                return default(Result);
            }
            foreach (var C in Connections)
            {
                if (C.Connect())
                {
                    using (var NS = new NetworkStream(C.Client.Client, true))
                    {
                        byte[] Data = Encoding.UTF8.GetBytes(GetHeader(Encoding.UTF8.GetString(FileContent)));
                        //byte[] Data = Encoding.UTF8.GetBytes(GetHeader(File.ReadAllText(FileName)));
                        NS.Write(Data, 0, Data.Length);
                        NS.Flush();
                        using (var SR = new StreamReader(NS))
                        {
                            string s= SR.ReadToEnd();
                            s = s.Substring(s.IndexOf('{'));
                            s = s.Substring(0, s.LastIndexOf('}') + 1);
                            try
                            {
                                return Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(s);
                            }
                            catch
                            {
                                //Ignore
                            }
                        }
                    }
                }
            }
            return default(Result);
        }

        private static Connection[] GetSocket(string Domain, int Port = 80)
        {
            IPAddress[] Addr = null;
            try
            {
                Addr = Dns.GetHostAddresses(Domain);
            }
            catch
            {
                return null;
            }
            return Addr.Select(m => new Connection(m, Port)).ToArray();
        }
    }
}
