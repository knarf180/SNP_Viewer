using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SNP_Viewer
{
    class Snap
    {
        public static string ConvertSNP(string file)
        {
            //File does not exist
            if (!File.Exists(file))
                return null;

            //  string name = Path.GetFileName(file);
            //   string dir = Path.GetDirectoryName(file);

            //Uncompress - its a cab file
            string uncompressedSnapshot = CreateTempFile();

            //Set path - used for the super old StrStorage.dll calls
            string tempDir = Path.GetDirectoryName(uncompressedSnapshot);
            Directory.SetCurrentDirectory(tempDir);


            SetupDecompressOrCopyFile(file, uncompressedSnapshot, 0);

            // Check and repair uncompressed Snapshot if necessary prior to conversion
            RepairSnapshot.ProcessFile(uncompressedSnapshot);

            // Convert uncompressed Snapshot to PDF
            string outputPDF = Path.GetFileNameWithoutExtension(uncompressedSnapshot) + ".pdf";

            /*
             * The following string contains a path to a PDF file which doesn't exist
             * This is used in the MergePDFDocuments function below
             */
            string dummyPDF = Path.GetFileNameWithoutExtension(uncompressedSnapshot) + "_dummy.pdf";

            bool r = false;
            r = ConvertUncompressedSnapshot(uncompressedSnapshot, outputPDF, 0, "", "", 0, 0, 0);
            /*
             * ConvertUncompressedSnapshot function will produce a Secured PDF with random restrictions
             * 
             * This is a known issue.  A workaround is to call the MergePDFDocuments function using a
             * PDF that doesn't exist, as this will remove the security from the created PDF
             * 
             */
            r = MergePDFDocuments(outputPDF, dummyPDF);


            DeleteTempFile(uncompressedSnapshot);


            return Path.Combine(tempDir, outputPDF);
        }

        private static string CreateTempFile()
        {
            string fileName = string.Empty;

            try
            {
                fileName = Path.GetTempFileName();

                FileInfo fileInfo = new FileInfo(fileName)
                {
                    Attributes = FileAttributes.Temporary
                };

            }
            catch
            {
                fileName = null;
            }

            return fileName;
        }

        public static void DeleteTempFile(string tmpFile)
        {
            try
            {
                if (File.Exists(tmpFile))
                {
                    File.Delete(tmpFile);
                }
            }
            catch
            {

            }
        }

        [DllImport("setupapi.dll")]
        public static extern int SetupDecompressOrCopyFile(string SourFileName, string TargetFileName, uint CompressionType);


        /*
         * In .Net 4.0 Calling the external library causes a stack imbalance error even though
         * the specified parameters are correct for the unmanaged code function that we're calling. 
         * 
         * Use .Net 3.5 to get around this issue since in that runtime version, the framework will 
         * auto-correct and resolve this imbalance error. 
         * 
         * See: http://codenition.blogspot.com/2010/05/pinvokestackimbalance-in-net-40i-beg.html
         * 
         */
        //[DllImport("StrStorage.dll", CallingConvention = CallingConvention.Cdecl)]
        [DllImport("StrStorage.dll")]
        public static extern bool ConvertUncompressedSnapshot(String uncompressedSnapshotName,
                                                               String outputPDF,
                                                               long compressionLevel,
                                                               String passwordOpen,
                                                               String passwordOwner,
                                                               long passwordRestrictions,
                                                               long PDFNoFontEmbedding,
                                                               long PDFUnicodeFlags);

        [DllImport("StrStorage.dll")]
        public static extern bool MergePDFDocuments(String firstPDF, String secondPDF);

    }

}

