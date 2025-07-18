using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Net;
using System.Diagnostics;

namespace MCPluger
{
    public partial class Form1 : Form
    {
        SoundPlayer player;
        public Form1()
        {
            InitializeComponent();
            CenterLabel();
            this.Resize += Form1_Resize;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            //music
            string url = "https://github.com/bartekZygor/MCPluger/raw/refs/heads/main/space.wav"; // <-- zmień na swój URL
            PlayWavFromUrl(url);


            string folder = Application.StartupPath;

            string sciezka = Path.Combine(folder, "background.png");

            if (File.Exists(sciezka))
            {
                this.BackgroundImage = Image.FromFile(sciezka);
                this.BackgroundImageLayout = ImageLayout.Stretch; // dopasowanie rozmiaru
            }
        }

        private void CenterLabel()
        {
            int centerX1 = (this.ClientSize.Width - label1.Width) / 2;
            int centerX2 = (this.ClientSize.Width - label2.Width) / 2;
            int centerX3 = (this.ClientSize.Width - label3.Width) / 2;
            int centerX6 = (this.ClientSize.Width - button3.Width) / 2;
            int centerX7 = (this.ClientSize.Width - button4.Width) / 2;

            label1.Location = new Point(centerX1, label1.Location.Y);
            label2.Location = new Point(centerX2, label2.Location.Y);
            label3.Location = new Point(centerX3, label3.Location.Y);
            button3.Location = new Point(centerX6, button3.Location.Y);
            button4.Location = new Point(centerX7, button4.Location.Y);
        }

        private void button2_Click(object sender, EventArgs e) // PLUG IN
        {
            string gtaPath = label4.Text;
            string modsSourcePath = label5.Text;
            string modsTargetPath = Path.Combine(gtaPath, "mods");

            if (!Directory.Exists(gtaPath) || !Directory.Exists(modsSourcePath))
            {
                MessageBox.Show("Invalid paths specified.");
                return;
            }

            if (!Directory.Exists(modsTargetPath))
            {
                Directory.CreateDirectory(modsTargetPath);
            }

            string[] files = Directory.GetFiles(modsSourcePath);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destination = Path.Combine(modsTargetPath, fileName);
                File.Copy(file, destination, true);
            }

            MessageBox.Show("Files have been added to GTA (mods).");
        }

        private void button1_Click(object sender, EventArgs e) // PLUG OUT
        {
            string gtaPath = label4.Text;
            string modsTargetPath = label5.Text;
            string modsSourcePath = Path.Combine(gtaPath, "mods");

            if (!Directory.Exists(modsSourcePath) || !Directory.Exists(modsTargetPath))
            {
                MessageBox.Show("Missing mods folder or destination folder.");
                return;
            }

            string[] files = Directory.GetFiles(modsSourcePath);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destination = Path.Combine(modsTargetPath, fileName);

                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }

                File.Move(file, destination);
            }

            MessageBox.Show("Mod(s) removed from GTA and migrated back. No data was lost.");
        }


        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select GTA 5 FOLDER (path1)";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    label4.Text = dialog.SelectedPath;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select Folder with mods (path2)";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    label5.Text = dialog.SelectedPath;
                }
            }
        }

        private void PlayWavFromUrl(string url)
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "temp_music.wav");

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(url, tempPath);
                }

                player = new SoundPlayer(tempPath);
                player.PlayLooping();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (player != null)
            {
                player.Stop();
                player.Dispose();
            }

            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "temp_music.wav");
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch { }
        }



        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterLabel();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            linkLabel1.Links.Add(0, linkLabel1.Text.Length, "https://apoki.site");
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = e.Link.LinkData as string;
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nie udało się otworzyć linku: " + ex.Message);
                }
            }
        }

    }
}
