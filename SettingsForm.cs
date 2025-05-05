using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using YamlDotNet.Serialization;

namespace zmBrowse
{
    public partial class SettingsForm : Form
    {
        private AppSettings appSettings;
        private string settingsFilePath;

        public SettingsForm(AppSettings appSettings)
        {
            InitializeComponent();
            this.appSettings = appSettings;
            settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings-zmbrowse.yml");
            LoadSettings();
        }

        private void LoadSettings()
        {
            txtDefaultFolderPath.Text = appSettings.DefaultFolderPath;
            txtDefaultCameraName.Text = appSettings.DefaultCameraName;
            cmbThumbnailTextColor.SelectedItem = appSettings.ThumbnailTextColor;
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDefaultFolderPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            appSettings.DefaultFolderPath = txtDefaultFolderPath.Text;
            appSettings.DefaultCameraName = txtDefaultCameraName.Text;
            appSettings.ThumbnailTextColor = cmbThumbnailTextColor.SelectedItem?.ToString() ?? "White";

            SaveSettingsToFile();
            MessageBox.Show("Settings saved successfully. The Application must be restarted for settings to take affect.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void SaveSettingsToFile()
        {
            var serializer = new Serializer();
            var settingsDictionary = new Dictionary<string, AppSettings>
            {
                { "zmbrowse", appSettings }
            };

            using (var writer = new StreamWriter(settingsFilePath))
            {
                serializer.Serialize(writer, settingsDictionary);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }

}




