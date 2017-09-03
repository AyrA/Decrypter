using Decrypter.DecryptModules;
using System;
using System.IO;
using System.Windows.Forms;

namespace Decrypter
{
    public partial class frmMain : Form
    {
        public frmMain(string Input = null, string Output = null)
        {
            InitializeComponent();
            tbInput.Text = Input;
            tbOutput.Text = Output;
            if (!string.IsNullOrEmpty(tbInput.Text))
            {
                DecryptLinks();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                tbInput.Text = OFD.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SFD.ShowDialog() == DialogResult.OK)
            {
                tbOutput.Text = SFD.FileName;
                if (!string.IsNullOrEmpty(tbLinks.Text) && MessageBox.Show("Save the existing list?", "Save list", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveList();
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbLinks.Text))
            {
                Clipboard.Clear();
                Clipboard.SetText(tbLinks.Text);
                MessageBox.Show("Links copied to clipboard", "Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            DecryptLinks();
        }

        private void DecryptLinks()
        {
            if (!string.IsNullOrEmpty(tbInput.Text) && File.Exists(tbInput.Text))
            {
                byte[] Data = null;
                try
                {
                    Data = File.ReadAllBytes(tbInput.Text);
                }
                catch (Exception ex)
                {
                    ShowError($"Can't read source file. Close other applications that might be using the file.\r\n\r\nMessage from system: {ex.Message}", "Source not readable");
                    return;
                }
                var Result = GenericDecrypter.Decrypt(Data, GenericDecrypter.ModeFromFileName(tbInput.Text));
                if (Result.success)
                {
                    tbLinks.Lines = (string[])Result.data.Clone();
                    SaveList();
                }
                else
                {
                    ShowError($"The Decryptor returned an error: {Result.message}", "Decryptor API Error");
                }
                /*
                var Result = ManualUpload.Upload(Data);
                if (Result.success.links != null)
                {
                    if (Result.success.links.Length > 0)
                    {
                        tbLinks.Lines = (string[])Result.success.links.Clone();
                        SaveList();
                    }
                    else
                    {
                        MessageBox.Show("The file contains no links", "Empty link list", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    ShowError($"The API returned an empty result. Ensure the source file is really a link container. Message : {Result.success.message}", "API error");
                }
                //*/
            }
            else
            {
                ShowError("The source file could not be found", "Source not found");
            }
        }

        private void SaveList()
        {
            if (!string.IsNullOrEmpty(tbOutput.Text) && !string.IsNullOrEmpty(tbLinks.Text))
            {
                if (!File.Exists(tbOutput.Text) || MessageBox.Show("A file with this name already exists. Owerwrite it?", "Overwrite file", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllLines(tbOutput.Text, tbLinks.Lines);
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Can't write file. Close other applications that might be using the file and ensure the media is not full or write protected.\r\n\r\nMessage from system: {ex.Message}", "Destination not writable");
                    }
                }
            }
        }

        private void ShowError(string Message, string Title)
        {
            MessageBox.Show(Message, Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void tbLinks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control && !e.Alt && !e.Shift)
            {
                tbLinks.SelectAll();
            }
        }
    }
}
