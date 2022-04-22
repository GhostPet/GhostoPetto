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
using Newtonsoft.Json;

namespace MangaDownloader.app
{
    public partial class MainUi : Form
    {
        IWebsite selectedWebsite;
        Dictionary<string, string> mangas = new Dictionary<string, string>();
        KeyValuePair<string, string> selectedManga;
        Dictionary<string, string> episodes = new Dictionary<string, string>();
        List<KeyValuePair<string, string>> downloadQueque = new List<KeyValuePair<string, string>>();
        List<KeyValuePair<string, string>> downloadFinished = new List<KeyValuePair<string, string>>();
        KeyValuePair<string, string> downloadNow = new KeyValuePair<string, string>();

        public MainUi()
        {
            InitializeComponent();
        }

        private void MainUi_Load(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;

            // Load settings.
            toolStripStatusLabel1.Text = "Loading theme...";
            if (Properties.Settings.Default.Theme == "Dark") makedark();
            else makelight();
            toolStripStatusLabel1.Text = "Loading settings...";
            textBox2.Text = Properties.Settings.Default.DownloadPath;

            // Load website packs
            toolStripStatusLabel1.Text = "Loading website packs...";
            foreach (IWebsite Website in WebsitesList.Websites)
            {
                comboBox1.Items.Add(Website);
            }

            // Ready
            toolStripStatusLabel1.Text = "Ready to fly!";
            comboBox1.Enabled = true;
        }

        // ToolStrip Options
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Coming Soon...");
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

        // Save button
        private void button6_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DownloadPath = textBox2.Text;
            Properties.Settings.Default.Save();
            toolStripStatusLabel1.Text = "Settings are saved.";
        }

        // Loading Websites
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "Loading...";
            comboBox2.Enabled = false;
            this.selectedWebsite = (IWebsite)comboBox1.SelectedItem;

            var MangaLists = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string> > >(Properties.Settings.Default.WebsiteMangaList);
            if (MangaLists != null && this.selectedWebsite != null)
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> MangaList in MangaLists)
                {
                    if (MangaList.Key == this.selectedWebsite.Name)
                    {
                        this.mangas = MangaList.Value;
                        combobox1_Update();
                        button2.Enabled = true;
                        return;
                    }
                }
            }
            backgroundWorker1.RunWorkerAsync();
            return;
        }

        // Loading Mangas
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = "Cancel";
            comboBox2.Enabled = false;
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
                button2.Text = "Refresh";
                toolStripStatusLabel1.Text = "Cancelling in progress.";
            }
            else
            {
                this.mangas = new Dictionary<string, string>();
                toolStripStatusLabel1.Text = "Refreshing...";
                backgroundWorker1.RunWorkerAsync();
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this.selectedWebsite.GetMangas(ref this.mangas, ref backgroundWorker1);
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel1.Text = this.mangas.Count.ToString() + " manga(s) loaded.";
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(Properties.Settings.Default.WebsiteMangaList);
            if (deserialized == null) deserialized = new Dictionary<string, Dictionary<string, string>>();
            if (deserialized.ContainsKey(this.selectedWebsite.Name)) deserialized.Remove(this.selectedWebsite.Name);
            deserialized.Add(this.selectedWebsite.Name, this.mangas);
            Properties.Settings.Default.WebsiteMangaList = JsonConvert.SerializeObject(deserialized);
            Properties.Settings.Default.Save();
            button2.Text = "Refresh";
            toolStripStatusLabel1.Text = this.mangas.Count.ToString() + " manga(s) loaded.";
            combobox1_Update();
        }
        private void combobox1_Update()
        {
            comboBox2.Items.Clear();
            foreach (KeyValuePair<string, string> Manga in this.mangas)
                comboBox2.Items.Add(Manga.Key);
            comboBox2.Text = "";
            comboBox2.Enabled = true;
        }

    // Loading Episodes
      // Via Custom URL
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            this.episodes = new Dictionary<string, string>();
            toolStripStatusLabel1.Text = "Loading...";

            if (backgroundWorker2.IsBusy)
            {
                backgroundWorker2.CancelAsync();
                button3.Text = "Load";
                toolStripStatusLabel1.Text = "Cancelling in progress.";
            }
            else
            {
                this.selectedWebsite = null;
                if (textBox1.Text != "")
                {
                    foreach (IWebsite Website in comboBox1.Items)
                    {
                        foreach (string Url in Website.Url)
                        {
                            if (textBox1.Text.StartsWith(Url))
                            {
                                this.selectedWebsite = Website;
                                this.selectedManga = new KeyValuePair<string, string>("Custom Url", textBox1.Text);
                                backgroundWorker2.RunWorkerAsync();
                                break;
                            }
                        }
                        if (this.selectedWebsite != null)
                        {
                            break;
                        }
                    }
                }
            }
        }
      // Via Manga List Item
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.episodes = new Dictionary<string, string>();
            toolStripStatusLabel1.Text = "Loading...";

            foreach (KeyValuePair<string, string> Manga in this.mangas)
            {
                if (Manga.Key == comboBox2.SelectedItem.ToString())
                {
                    this.selectedManga = Manga;
                    dataGridView1.Rows.Clear();
                    backgroundWorker2.RunWorkerAsync();
                }
            }
        }
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            this.selectedWebsite.GetEpisodes(this.selectedManga, ref this.episodes, ref backgroundWorker2);
        }
        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel1.Text = this.episodes.Count.ToString() + " episode(s) loaded.";
        }
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button3.Enabled = true;
            toolStripStatusLabel1.Text = "Loading Complete.";
            combobox2_Update();
        }
        private void combobox2_Update()
        {
            dataGridView1.Rows.Clear();
            int Counter = this.episodes.Count;
            foreach (KeyValuePair<string, string> Episode in this.episodes)
                dataGridView1.Rows.Insert(0,
                    Counter--,
                    Episode.Key,
                    "Download",
                    Episode.Value
                );
        }

        // Select & Download the Episode
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                this.downloadQueque.Add(new KeyValuePair<string, string>((string)dataGridView1.Rows[e.RowIndex].Cells[1].Value, (string)dataGridView1.Rows[e.RowIndex].Cells[3].Value) );
                if (!backgroundWorker3.IsBusy || !backgroundWorker4.IsBusy) backgroundWorker3.RunWorkerAsync();
            }
        }

        // Bottom buttons control
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            button1.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button7.Enabled = true;
        }
        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                button1.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button7.Enabled = false;
            }
        }
        // Bottom buttons functions
        private void button1_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow Row in dataGridView1.Rows)
            {
                this.downloadQueque.Add(new KeyValuePair<string, string>((string)Row.Cells[1].Value, (string)Row.Cells[3].Value));
            }
            if (!backgroundWorker3.IsBusy || !backgroundWorker4.IsBusy) backgroundWorker3.RunWorkerAsync();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow Row in dataGridView1.SelectedRows)
            {
                this.downloadQueque.Add(new KeyValuePair<string, string>((string)Row.Cells[1].Value, (string)Row.Cells[3].Value));
            }
            if (!backgroundWorker3.IsBusy || !backgroundWorker4.IsBusy) backgroundWorker3.RunWorkerAsync();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            backgroundWorker3.CancelAsync();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            this.downloadFinished.Clear();
        }


        /* QUEQUE:
         * When you add a new episode to the download list; bgworker3 will be triggered and start bgworker4 for downloading the first episode on the list.
         * When download completed, first item on the list will be deleted (or moved to finished list), and
         * Bgworker3 will be triggered again and worker start a new process that will start download the first episode on the list.
        */

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            if (backgroundWorker3.CancellationPending || this.downloadQueque.Count == 0) return;
            this.downloadNow = this.downloadQueque[0];
            toolStripStatusLabel1.Text = (this.downloadFinished.Count).ToString() + "/" + (this.downloadFinished.Count + this.downloadQueque.Count).ToString() + ": " + this.downloadNow.Key + " is downloading...";
            backgroundWorker4.RunWorkerAsync();
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            this.selectedWebsite.Download(textBox2.Text, this.downloadNow);
        }

        private void backgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.downloadQueque.Remove(this.downloadNow);
            this.downloadFinished.Add(this.downloadNow);
            toolStripStatusLabel1.Text = (this.downloadFinished.Count).ToString() + "/" + (this.downloadFinished.Count + this.downloadQueque.Count).ToString() + ": " + this.downloadNow.Key + " downloaded.";
            backgroundWorker3.RunWorkerAsync();
        }
    }
}
