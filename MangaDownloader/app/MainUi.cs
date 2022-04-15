using MangaDownloader.app.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MangaDownloader.WebsitePacks.Default;

namespace MangaDownloader.app
{
    public partial class MainUi : Form
    {
        Driver general;
        Driver downloader;
        public MainUi()
        {
            InitializeComponent();
        }

        private void MainUi_Load(object sender, EventArgs e)
        {
            // Set a Loding... text.
            comboBox1.Text = "Loading";

            // Load default theme or the selected theme from settings.
            if (Properties.Settings.Default.Theme == "Dark") makedark();
            else makelight();

            // Prepare the selenium web browser (There's 2, 1 for search, 1 for download.)
            this.general = new Driver(true);
            this.downloader = new Driver(true);

            // Load website packs
            WebsitesList WebsitesList = new WebsitesList();
            foreach (IWebsite Website in WebsitesList.Websites)
            {
                comboBox1.Items.Add(Website.Name);
            }
        }

        private void aboutGhostoPettoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox newMDIChild = new AboutBox();
            newMDIChild.ShowDialog();
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            makelight();
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            makedark();
        }

        private void makedark()
        {
            lightToolStripMenuItem.Checked = false;
            darkToolStripMenuItem.Checked = true;
            this.BackColor = SystemColors.ControlDark;
            MenuToolbar.BackColor = SystemColors.ControlDarkDark;
            dataGridView1.BackgroundColor = SystemColors.ControlDark;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.ControlDarkDark;
            dataGridView1.DefaultCellStyle.BackColor = SystemColors.ControlDark;
            statusStrip1.BackColor = SystemColors.ControlDark;
            Properties.Settings.Default.Theme = "Dark";
            Properties.Settings.Default.Save();
        }

        private void makelight()
        {
            lightToolStripMenuItem.Checked = true;
            darkToolStripMenuItem.Checked = false;
            this.BackColor = SystemColors.Control;
            MenuToolbar.BackColor = SystemColors.Control;
            dataGridView1.BackgroundColor = SystemColors.Control;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dataGridView1.DefaultCellStyle.BackColor = SystemColors.Window;
            statusStrip1.BackColor = SystemColors.Control;
            Properties.Settings.Default.Theme = "Light";
            Properties.Settings.Default.Save();
        }
    }
}
