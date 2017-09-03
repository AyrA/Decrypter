/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Decrypter
{
    public static class RSDF
    {
        public static string[] Decrypt(string FileContent)
        {
            var Content =
                //Get individual link segments
                GetParts(FileContent.ToUpper())
                //Make byte arrays from hex strings
                .Select(m => m.ToByteArray())
                //Get string representations of said byte arrays
                .Select(m => Encoding.Default.GetString(m))
                //Convert these strings from b64 to byte[]
                .Select(m => Convert.FromBase64String(m.Trim()))
                //Decrypt byte arrays
                .Select(m => DecryptPart(m))
                //Make Array
                .ToArray();
            return Content;
        }

        private static string DecryptPart(byte[] Part)
        {
            var Key = "8C35192D964DC3182C6F84F3252239EB4A320D2500000000".ToByteArray();
            var IV = "A3D5A33CB95AC1F5CBDB1AD25CB0A7AA".ToByteArray();
            var BlockSize = IV.Length;

            using (var Decrypter = new RijndaelManaged())
            {
                Decrypter.Mode = CipherMode.CFB;
                Decrypter.Padding = PaddingMode.None;
                Decrypter.BlockSize = BlockSize * 8;
                Decrypter.IV = IV;
                Decrypter.Key = Key;

                var Data = Part;
                //Extend Data with nullbytes to match block size if needed
                if (Data.Length % BlockSize != 0)
                {
                    Data = Data
                        .Concat(new byte[BlockSize - (Data.Length % BlockSize)])
                        .ToArray();
                }

                //Encrypted Source data
                using (var MS = new MemoryStream(Data, false))
                {
                    //Decryption component
                    using (var Dec = Decrypter.CreateDecryptor(Key,IV))
                    {
                        //Decryptor Stream
                        using (var CS = new CryptoStream(MS, Dec, CryptoStreamMode.Read))
                        {
                            //Output data
                            using (var OUT = new MemoryStream())
                            {
                                //Decrypt everything
                                CS.CopyTo(OUT);
                                //Return Result
                                var Result = OUT.ToArray();
                                return Encoding.Default.GetString(Result, 0, Result.Length);
                            }
                        }
                    }
                }
            }
        }

        private static string[] GetParts(string S)
        {
            string data = "";
            List<string> DataList = new List<string>();
            for (int i = 0; i < S.Length; i += 2)
            {
                if (S.Substring(i, 2) == "0A")
                {
                    //Encoded Line break
                    if (!string.IsNullOrEmpty(data))
                    {
                        DataList.Add(data);
                    }
                    data = "";
                }
                else if (S.Substring(i, 2) == "DA")
                {
                    //Encoded separator
                    if (!string.IsNullOrEmpty(data))
                    {
                        DataList.Add(data);
                    }
                    data = "";
                }
                else
                {
                    //Just add this symbol
                    data += S.Substring(i, 2);
                }
            }
            return DataList.ToArray();
        }
    }
}
//*/
