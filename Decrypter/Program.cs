using System;
using System.Linq;
using System.Windows.Forms;

namespace Decrypter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            RSDF.Decrypt(
                "6f59654632532f656f7a33312f485752615336515242303dda63"+
                "47502f4d4a4c614f775979715355306369782f51334b62464b6b"+
                "674e4d62545838682f6161646dda436d494f4c6b7a5161534a56"+
                "656c65654b49386733422f397759656771585144306c2b39794d"+
                "4f52da6f6c58445a793657557537506568324245324b57515031"+
                "523835596344682b395a38646d67773479da"
                /*
                "6f59654632532f656f7a33312f485752615336515242303d0d0a"+
                "6347502f4d4a4c614f775979715355306369782f51334b62464b"+
                "6b674e4d62545838682f6161646d0d0a436d494f4c6b7a516153"+
                "4a56656c65654b49386733422f397759656771585144306c2b39"+
                "794d4f520d0a6f6c58445a793657557537506568324245324b57"+
                "515031523835596344682b395a38646d677734790d0a"
                //*/
                );
            //return;

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
