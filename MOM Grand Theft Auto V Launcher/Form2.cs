using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace MOM_Grand_Theft_Auto_V_Launcher {
    public partial class Form2 : Form {
        public Form2() {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e) {
            Height = 196;
            System.Media.SystemSounds.Beep.Play();

            string[] updateInfo = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\updateInfo.temp");

            label3.Text = updateInfo[0];
            label5.Text = updateInfo[1];

            if (updateInfo[0] == updateInfo[1]) {
                downloadUpdate_btn.Enabled = false;
                label1.Left = 45;
                label1.Text = "You already have the last version...";
                label3.ForeColor = System.Drawing.Color.ForestGreen;
                label5.ForeColor = System.Drawing.Color.ForestGreen;
            }

            File.Delete(Directory.GetCurrentDirectory() + @"\updateInfo.temp");
        }

        private void toggleChangelog_btn_Click(object sender, EventArgs e) {
            if (toggleChangelog_btn.Text == "Show Changelog") {
                toggleChangelog_btn.Text = "Hide Changelog";
                Height = 438;
            } else {
                toggleChangelog_btn.Text = "Show Changelog";
                Height = 196;
            }

            label1.Focus();
        }

        private void downloadUpdate_btn_Click(object sender, EventArgs e) {
            label1.Focus();
            System.Diagnostics.Process.Start("https://www.gta5-mods.com/tools/mom-grand-theft-auto-v-launcher");
        }
    }
}
