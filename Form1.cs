using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SNP_Viewer
{
    public partial class Form1 : Form
    {
        private string LastFileName;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new OpenFileDialog())
            {
                form.Filter = "Snapshot Files (*.snp)|*.snp|All Files (*.*)|*.*";
                form.RestoreDirectory = true;
                form.Title = "Open SNP File";

                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    Dispose();
                    return;
                }

                LoadFileIntoViewer(form.FileName);


            }
        }
        public void LoadFileIntoViewer(string filename)
        {

            string tfile = Snap.ConvertSNP(filename);
            if (tfile == null)
            {
                MessageBox.Show("Error loading file", "Error");
                return;
            }

            pdfViewer1.Document?.Dispose();
                                 
            //A file was previously opened.  Clean temp file before opening a new one
            if (File.Exists(LastFileName))
            {
                Snap.DeleteTempFile(LastFileName);
            }
            LastFileName = tfile;

            pdfViewer1.Document = OpenDocument(tfile);

            FitWidth();
        }
        private void FitWidth()
        {
            //Force the viewer to fit the display to best width
            int page = pdfViewer1.Renderer.Page;
            pdfViewer1.ZoomMode = PdfViewerZoomMode.FitWidth;
            pdfViewer1.Renderer.Zoom = 1;
            pdfViewer1.Renderer.Page = page;
        }

        private PdfDocument OpenDocument(string fileName)
        {
            try
            {
                return PdfDocument.Load(this, fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
      
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Cleanup temp files before exiting
            if (File.Exists(LastFileName))
            {
                Snap.DeleteTempFile(LastFileName);
            }
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            //Cleanup temp files before exiting
            if (File.Exists(LastFileName))
            {
                Snap.DeleteTempFile(LastFileName);
            }
        }

         private void AboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
                using (AboutBox1 box = new AboutBox1())
                {
                    box.ShowDialog(this);
                }
        }
    }
}
