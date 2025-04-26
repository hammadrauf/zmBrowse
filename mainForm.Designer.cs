using System.Diagnostics;
using System.Drawing.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NReco.VideoConverter;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;


namespace zmBrowse;

partial class mainForm
{

    private string rFolder;
    private int evStart, evEnd;

    Button btnSelectFolder;
    Button btnGenerateThumbnails;
    TextBox txtFolderPath;
    NumericUpDown numericUpDownStart;
    NumericUpDown numericUpDownEnd;
    ComboBox comboBoxCameraNameFolder;
    CheckedListBox clbDateFolders;
    FlowLayoutPanel flowLayoutPanelThumbnails;
    //List<string> selectedDateFolders;
    List<DateFolderStructure> selectedDateFolders = new List<DateFolderStructure>();
    ILoggerFactory factory;
    ILogger logger;

    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;


    public mainForm(ILogger logger)
    {
        this.logger = logger;
        InitializeComponent();
    }

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }


    public struct DateFolderStructure
    {
        public string Name;
        public string Path;
        public List<string> sfEvents;

        public DateFolderStructure(string name, string path, List<string> sfevents)
        {
            Name = name;
            Path = path;
            sfEvents = sfevents;
        }

        public override string ToString()
        {
            return $"Name: {Name}, Path: {Path}, sfEvents: {sfEvents.ToString()}";
        }
    }


    private void BtnSelectFolder_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog dialog = new FolderBrowserDialog())
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                rFolder = dialog.SelectedPath;
                txtFolderPath.Text = rFolder;

                // Get all sub-folder names, sort them, and populate the ComboBox
                string[] subFolders = Directory.GetDirectories(rFolder);
                List<string> folderNames = subFolders.Select(Path.GetFileName).OrderBy(name => name).ToList();

                comboBoxCameraNameFolder.Items.Clear(); // Clear existing items
                comboBoxCameraNameFolder.Items.AddRange(folderNames.ToArray());

                if (folderNames.Count > 0)
                {
                    comboBoxCameraNameFolder.SelectedIndex = 0; // Set the first sub-folder as the default selection
                }

                // Get the selected cameraNameFolder
                string cameraNameFolder = comboBoxCameraNameFolder.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(cameraNameFolder))
                {
                    string cameraFolderPath = Path.Combine(rFolder, cameraNameFolder);

                    // Get all sub-sub-folder names, sort them, and populate the CheckedListBox
                    string[] dateFolders = Directory.GetDirectories(cameraFolderPath);
                    List<string> dateFolderNames = dateFolders.Select(Path.GetFileName).OrderBy(name => name).ToList();

                    clbDateFolders.Items.Clear(); // Clear existing items
                    clbDateFolders.Items.AddRange(dateFolderNames.ToArray());

                    // Select all items by default
                    for (int i = 0; i < clbDateFolders.Items.Count; i++)
                    {
                        clbDateFolders.SetItemChecked(i, true);
                    }

                    // Store selected folder names in a collection for further processing
                    UpdateSelectedDateFolders();
                }

            }
        }
    }

    private void BtnGenerateThumbnails_Click(object sender, EventArgs e)
    {
        evStart = (int)numericUpDownStart.Value;
        evEnd = (int)numericUpDownEnd.Value;

        flowLayoutPanelThumbnails.Controls.Clear();

        for (int i = evStart; i <= evEnd; i++)
        {
            string eventFolder = Path.Combine(rFolder, i.ToString());
            string videoPath = Path.Combine(eventFolder, $"{i}-video.mp4");


            if (File.Exists(videoPath))
            {
                PictureBox pictureBox = CreateThumbnail(videoPath, eventFolder, $"{i}.png");

                if (pictureBox != null)
                {
                    flowLayoutPanelThumbnails.Controls.Add(pictureBox);
                }
            }
        }
    }

    private PictureBox CreateThumbnail(string videoPath, string eventFolder, string tnailName)
    {
        try
        {
            //string thumbnailPath = Path.Combine(eventFolder, "alarm.jpg");
            //string thumbnailPath = Path.Combine(eventFolder, "snapshot-48x64.jpg");
            //string thumbnailPath = Path.Combine(eventFolder, "snapshot.jpg");
            string thumbnailPath = Path.Combine(System.IO.Path.GetTempPath(), tnailName);
            var ffMpeg = new FFMpegConverter();
            ffMpeg.GetVideoThumbnail(videoPath, thumbnailPath, 1.0f);

            PictureBox pictureBox = new PictureBox
            {
                Image = Image.FromFile(thumbnailPath),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width = 90,
                Height = 120,
                Tag = videoPath
            };

            pictureBox.Click += (s, e) => PlayVideo((string)pictureBox.Tag);
            return pictureBox;
        }
        catch (Exception ex) {
            logger.LogError("Exception when creating thumbnail.");
            logger.LogDebug(ex.ToString());
            return null;
        }

    }

    private void PlayVideo(string videoPath)
    {
        Process.Start(new ProcessStartInfo(videoPath) { UseShellExecute = true });
    }

    private void UpdateSelectedDateFolders()
    {
        selectedDateFolders.Clear();
        foreach (var datefolder in clbDateFolders.CheckedItems.Cast<string>().ToList())
        {
            string dateFolderPath = Path.Combine(rFolder, comboBoxCameraNameFolder.SelectedItem?.ToString(), datefolder);
            string[] subFolders = Directory.GetDirectories(dateFolderPath);
            List<string> folderNames = subFolders.Select(Path.GetFileName).OrderBy(name => name).ToList();

            selectedDateFolders.Add(new DateFolderStructure(datefolder, dateFolderPath, folderNames));
        }
    }


    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Text = "ZoneMinder Thumbnail Browser";
        this.Size = new Size(800, 800);

        // Initialize buttons manually
        btnSelectFolder = new Button { Text = "Select Zone-Minder Folder", Location = new Point(10, 10) };
        btnGenerateThumbnails = new Button { Text = "Generate Thumbnails", Location = new Point(10, 110) };
        txtFolderPath = new TextBox { Location = new Point(110, 12), Width = 500, ReadOnly = true, Text = "\\\\192.168.4.5\\ZM_Events\\1\\2025-04-22" };
        numericUpDownStart = new NumericUpDown { Location = new Point(20, 80), Size = new Size(120, 30), Minimum = 1, Maximum = 999999, Value = 76868 };
        numericUpDownEnd = new NumericUpDown { Location = new Point(220, 80), Size = new Size(120, 30), Minimum = 1, Maximum = 999999, Value = 76894 };
        flowLayoutPanelThumbnails = new FlowLayoutPanel
        {
            Location = new Point(20, 140),
            Size = new Size(750, 450),
            AutoScroll = true,
            BorderStyle = BorderStyle.FixedSingle
        };
        comboBoxCameraNameFolder = new ComboBox
        {
            Location = new Point(20, 50), 
            Width = 120,                  
            DropDownStyle = ComboBoxStyle.DropDownList // Prevent manual text entry
        };
        comboBoxCameraNameFolder.Items.Add("1");
        comboBoxCameraNameFolder.SelectedIndex = 0;

        clbDateFolders = new CheckedListBox
        {
            Location = new Point(150, 50),
            Size = new Size(200, 80),
            CheckOnClick = true 
        };

        clbDateFolders.ItemCheck += (s, e) => UpdateSelectedDateFolders();

        this.Controls.Add(clbDateFolders);
        this.Controls.Add(btnSelectFolder);
        this.Controls.Add(btnGenerateThumbnails);
        this.Controls.Add(txtFolderPath);
        this.Controls.Add(numericUpDownStart);
        this.Controls.Add(numericUpDownEnd);
        this.Controls.Add(flowLayoutPanelThumbnails);
        this.Controls.Add(comboBoxCameraNameFolder);

        btnSelectFolder.Click += BtnSelectFolder_Click;
        btnGenerateThumbnails.Click += BtnGenerateThumbnails_Click;


    }

    #endregion
}


