/*
A Snapshot viewer to replace the one created by microsoft in 1997ish.

Based on the work of:
Stephen Lebans: 
StrStorage.dll - Converts snapshot files to PDFs
                https://www.lebans.com/reporttopdf.htm

Andrew McCarthy:
RepairSnapshot.cs - Attempts to repair broken Snapshot files prior to conversion, specifically 
                    those created using Access 2007 onwards
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace SNP_Viewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Make sure that the required DLL files exist 
            if (!DLLExist())
            {
                string msg = "A required dll was not found. Please make sure that \n";
                msg += "StrStorage.dll, dynapdf.dll, and pdfium.dll are in the same folder\n";
                msg += "as this program and try again.\n";

                MessageBox.Show(msg, "A Required DLL Was Not Found");

                return; // Don't Launch the application
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            
            // Check args to open associated files
            if (args != null && args.Length > 0)
            {
                //When /attrib arg is given setup file associations then quit
                if (args[0] == "/attrib")
                {
                    FileAssociations.EnsureAssociationsSet();
                    return;

                }

                //A filename arg is set, open it

                string fileName = args[0];
                Form1 MainFrom = new Form1();

                //Check file exists
                if (File.Exists(fileName))
                {
                    MainFrom.LoadFileIntoViewer(fileName);
                }

                Application.Run(MainFrom);
            }
            //without args
            else
            {
                Application.Run(new Form1());
            }

        }

        /// <summary>
        /// Determines if one of the required DLLs is not in the current directory.
        /// The dlls in quesiton are StrStorage.dll , dynapdf.dll , and pdfium.dll
        /// </summary>
        /// <returns></returns>
        static bool DLLExist()
        {
            string ExePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            FileInfo strstor_dll = new FileInfo(Path.Combine(ExePath, "StrStorage.dll"));
            FileInfo dynapdf_dll = new FileInfo(Path.Combine(ExePath, "dynapdf.dll"));
            FileInfo pdfium_dll = new FileInfo(Path.Combine(ExePath, "pdfium.dll"));

            if (!strstor_dll.Exists || !dynapdf_dll.Exists || !pdfium_dll.Exists)
            { // if either required DLL is missing
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
