using Decrypter.DecryptModules;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
                #pragma warning disable CS4014
                DecryptLinks();
                #pragma warning restore CS4014
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

        private async void btnDecrypt_Click(object sender, EventArgs e)
        {
            await DecryptLinks();
        }

        private async Task DecryptLinks()
        {
            if (!string.IsNullOrEmpty(tbInput.Text) && File.Exists(tbInput.Text))
            {
                Enabled = false;
                tbLinks.Text = "Decrypting...";
                byte[] Data = null;
                try
                {
                    Data = File.ReadAllBytes(tbInput.Text);
                }
                catch (Exception ex)
                {
                    ShowError($"Can't read source file. Close other applications that might be using the file.\r\n\r\nMessage from system: {ex.Message}", "Source not readable");
                    Enabled = true;
                    return;
                }

                var Hash = GenericDecrypter.GetHash(Encoding.Default.GetBytes(Encoding.Default.GetString(Data).ToUpper()));
                string Name = null;
                if (!await GenericDecrypter.Hash(Hash))
                {
                    using (var f = new frmName())
                    {
                        f.ShowDialog();
                        Name = f.Data;
                    }
                }
                else
                {
                    Name = string.Empty;
                }

                var Result = await GenericDecrypter.Decrypt(Data, Name, GenericDecrypter.ModeFromFileName(tbInput.Text));
                if (Result.success)
                {
                    lblName.Text = Result.data.name;
                    tbLinks.Lines = (string[])Result.data.links.Clone();
                    SaveList();
                }
                else
                {
                    ShowError($"The Decryptor returned an error: {Result.message}", "Decryptor API Error");
                    tbLinks.Text = "Raw Response:\r\n" + JsonConvert.SerializeObject(Result.data);
                }
            }
            else
            {
                ShowError("The source file could not be found", "Source not found");
            }
            Enabled = true;
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbInput.Text))
            {
                if (MessageBox.Show("Delete Container File?", "Delete File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(tbInput.Text);
                    }
                    catch(Exception ex)
                    {
                        ShowError($"Can't delete file. Reason given: {ex.Message}", "Delete File");
                    }
                }
            }
        }
    }
}
