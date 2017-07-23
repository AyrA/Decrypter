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
