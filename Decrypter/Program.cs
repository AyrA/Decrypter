using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Decrypter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Any(m => m.ToLower() == "--help" || m == "/?" || m == "-?"))
            {
                ShowHelp();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain(args.Length > 0 ? args[0] : null, args.Length > 1 ? args[1] : null));
            }
            /*
            //How to use
            if (args.Length == 2)
            {
                if (File.Exists(args[0]))
                {
                    Console.Error.Write("Processing file...");
                    var Result = ManualUpload.Upload(args[0]);
                    Console.Error.WriteLine("[DONE]");
                    Console.Error.Write("Saving...");
                    if (args.Length == 1)
                    {
                        var FI = new FileInfo(args[0]);
                        //add/swap extension for .txt
                        if (args[0].Contains("."))
                        {
                            File.WriteAllLines(args[0].Substring(0, args[0].LastIndexOf('.')) + ".txt", Result.success.links);
                        }
                        else
                        {
                            File.WriteAllLines(args[0] + ".txt", Result.success.links);
                        }
                    }
                    else
                    {
                        if (args[1] == "-")
                        {
                            Console.WriteLine(string.Join("\r\n", Result.success.links));
                        }
                        else
                        {
                            File.WriteAllLines(args[1], Result.success.links);
                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine("Can't find input file");
                }
                Console.Error.WriteLine("[DONE]");
                Console.ReadKey(true);
            }
            else
            {
                ShowHelp();
            }
            //*/
        }

        static void ShowHelp()
        {
            Console.Error.WriteLine(@"Decrypter <infile> [outfile]

Decrypts DLC files

infile   - File name of the DLC file
outfile  - File name to get all links. Use '-' for terminal
           If not specified, it will be the dlc file with txt extension.");
        }
    }
}
