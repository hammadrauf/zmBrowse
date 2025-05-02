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
using YamlDotNet.Serialization;


namespace zmBrowse;

partial class mainForm
{

    private string rFolder;
    private int evStart, evEnd;
    private AppSettings settings;

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
    //ILoggerFactory factory;
    ILogger logger;

    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;


    public mainForm(ILogger logger)
    {
        this.logger = logger;
        settings = new AppSettings(logger);
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
                ProcessRootFolder_Core(dialog);
            }
        }
    }

    private void ProcessRootFolder_Core(FolderBrowserDialog dialog)
    {
        if (dialog == null)
            rFolder = settings.DefaultFolderPath;
        else if (string.IsNullOrEmpty(dialog.SelectedPath))
            rFolder = System.IO.Path.GetTempPath();
        else
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

            // Select today folder by default
            string todayFolder = DateTime.Now.ToString("yyyy-MM-dd");
            if (clbDateFolders.Items.Contains(todayFolder))
            {
                // Select the today folder
                clbDateFolders.SetItemChecked(clbDateFolders.Items.IndexOf(todayFolder), true);
            }
            else if (clbDateFolders.Items.Count == 0)
            {
                // Select the first folder
                clbDateFolders.SetItemChecked(0, true);
            }
            else if (clbDateFolders.Items.Count > 1)
            {
                // Select the last folder
                clbDateFolders.SetItemChecked(clbDateFolders.Items.Count - 1, true);
            }

            // Get the current checked items
            var checkedItems = clbDateFolders.CheckedItems.Cast<string>().ToList();
            // Call the core method
            UpdateSelectedDateFolders_Core(checkedItems);
        }
    }

    private void BtnGenerateThumbnails_Click(object sender, EventArgs e)
    {
        evStart = (int)numericUpDownStart.Value;
        evEnd = (int)numericUpDownEnd.Value;

        flowLayoutPanelThumbnails.Controls.Clear();

        foreach(var dateFolder in selectedDateFolders)
        {
            foreach (var eventID in dateFolder.sfEvents)
            {
                if (int.TryParse(eventID, out int eventIDInt) && (eventIDInt < evStart || eventIDInt > evEnd))
                {
                    continue; // Skip this event ID if it's outside the range
                }
                string eventFolder = Path.Combine(dateFolder.Path, eventID);
                string videoPath = Path.Combine(eventFolder, $"{eventID}-video.mp4");
                if (File.Exists(videoPath))
                {
                    int evInt = int.TryParse(eventID, out int eventIDInt2) ? eventIDInt2 : 0;
                    PictureBox pictureBox = CreateThumbnail(videoPath, eventFolder, evInt);
                    if (pictureBox != null)
                    {
                        flowLayoutPanelThumbnails.Controls.Add(pictureBox);
                    }
                }
            }
        }
            }

    private PictureBox CreateThumbnail(string videoPath, string eventFolder, int eventID)
    {
        try
        {
            string tnailName = $"{eventID}.png";
            string thumbnailPath = Path.Combine(System.IO.Path.GetTempPath(), tnailName);
            var ffMpeg = new FFMpegConverter();
            ffMpeg.GetVideoThumbnail(videoPath, thumbnailPath, 1.0f);

            // Load the thumbnail image
            using (Image thumbnailImage = Image.FromFile(thumbnailPath))
            {
                // Create a new bitmap to draw on
                Bitmap bitmapWithText = new Bitmap(thumbnailImage);

                using (Graphics graphics = Graphics.FromImage(bitmapWithText))
                {
                    // Set up the font and brush for the text
                    Font font = new Font("Arial", 72, FontStyle.Bold);
                    Color tColor = Color.FromName(settings.ThumbnailTextColor);
                    Brush brush = new SolidBrush(tColor);

                    // Measure the size of the text
                    SizeF textSize = graphics.MeasureString(eventID.ToString(), font);

                    // Calculate the position for the text (lower-right corner)
                    float x = bitmapWithText.Width - textSize.Width - 5; // 5px padding from the right
                    float y = bitmapWithText.Height - textSize.Height - 5; // 5px padding from the bottom

                    // Draw the text on the image
                    graphics.DrawString(eventID.ToString(), font, brush, x, y);
                }

                // Save the modified image back to the temporary path
                string modifiedThumbnailPath = Path.Combine(System.IO.Path.GetTempPath(), $"modified_{tnailName}");
                bitmapWithText.Save(modifiedThumbnailPath, ImageFormat.Png);

                // Create the PictureBox with the modified image
                PictureBox pictureBox = new PictureBox
                {
                    Image = Image.FromFile(modifiedThumbnailPath),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = 90,
                    Height = 120,
                    Tag = videoPath // Store the VideoPath in the Tag property for playback
                };

                // Add a context menu for right-click
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Copy Event Folder path to clipboard and open");
                //copyMenuItem.Click += (s, e) => Clipboard.SetText(eventFolder); // Copy eventFolder to clipboard
                copyMenuItem.Click += (s, e) =>
                {
                    Clipboard.SetText(eventFolder);
                    if (Directory.Exists(eventFolder))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = eventFolder,
                            UseShellExecute = true,
                            Verb = "open"
                        });
                    }
                };
                contextMenu.Items.Add(copyMenuItem);

                // Show the context menu on right-click
                pictureBox.MouseUp += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        contextMenu.Show(pictureBox, e.Location);
                    }
                };

                // Add a click event to play the video on left-click
                pictureBox.MouseClick += (s, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        PlayVideo((string)pictureBox.Tag); // Use the videoPath stored in the Tag
                    }
                };

                return pictureBox;
            }
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

    private void UpdateSelectedDateFolders_Handler(object sender, ItemCheckEventArgs e)
    {
        // Create a temporary list to reflect the updated checked state
        var updatedCheckedItems = clbDateFolders.CheckedItems.Cast<string>().ToList();

        // Add or remove the item being checked/unchecked
        string currentItem = clbDateFolders.Items[e.Index].ToString();
        if (e.NewValue == CheckState.Checked && !updatedCheckedItems.Contains(currentItem))
        {
            updatedCheckedItems.Add(currentItem);
        }
        else if (e.NewValue == CheckState.Unchecked && updatedCheckedItems.Contains(currentItem))
        {
            updatedCheckedItems.Remove(currentItem);
        }

        // Call the core method
        UpdateSelectedDateFolders_Core(updatedCheckedItems);
    }

    private void UpdateSelectedDateFolders_Core(List<string> updatedCheckedItems)
    {
        int evMin = 999999;
        int evMax = 1;

        selectedDateFolders.Clear();
        foreach (var datefolder in updatedCheckedItems)
        {
            string dateFolderPath = Path.Combine(rFolder, comboBoxCameraNameFolder.SelectedItem?.ToString(), datefolder);
            string[] subFolders = Directory.GetDirectories(dateFolderPath);
            List<string> folderNames = subFolders.Select(Path.GetFileName).OrderBy(name => name).ToList();

            if (folderNames.Count > 0)
            {
                // Update evMin and evMax based on folder names
                if (int.TryParse(folderNames[0], out int firstEventID) && firstEventID < evMin)
                    evMin = firstEventID;
                if (int.TryParse(folderNames[folderNames.Count - 1], out int lastEventID) && lastEventID > evMax)
                    evMax = lastEventID;
            }

            selectedDateFolders.Add(new DateFolderStructure(datefolder, dateFolderPath, folderNames));
        }

        // Update the values for numericUpDownStart and numericUpDownEnd
        if (selectedDateFolders.Count > 0)
        {
            numericUpDownStart.Value = evMin;
            numericUpDownEnd.Value = evMax;
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
        btnSelectFolder = new Button { Text = "Zone-Minder Folder", Location = new Point(10, 30), Size = new Size(160,25) };
        btnGenerateThumbnails = new Button { Text = "Generate Thumbnails", Location = new Point(10, 210), Size = new Size(160, 25) };
        txtFolderPath = new TextBox { Location = new Point(220, 32), Width = 400, ReadOnly = true, Text = settings.DefaultFolderPath };
        numericUpDownStart = new NumericUpDown { Location = new Point(20, 180), Size = new Size(120, 30), Minimum = 1, Maximum = 999999, Value = 76868 };
        numericUpDownEnd = new NumericUpDown { Location = new Point(220, 180), Size = new Size(120, 30), Minimum = 1, Maximum = 999999, Value = 76894 };
        flowLayoutPanelThumbnails = new FlowLayoutPanel
        {
            Location = new Point(20, 250),
            Size = new Size(750, 450),
            AutoScroll = true,
            BorderStyle = BorderStyle.FixedSingle
        };
        comboBoxCameraNameFolder = new ComboBox
        {
            Location = new Point(20, 80), 
            Width = 120,                  
            DropDownStyle = ComboBoxStyle.DropDownList // Prevent manual text entry
        };
        comboBoxCameraNameFolder.Items.Add(settings.DefaultCameraName);
        comboBoxCameraNameFolder.SelectedIndex = 0;

        clbDateFolders = new CheckedListBox
        {
            Location = new Point(150, 80),
            Size = new Size(200, 80),
            CheckOnClick = true 
        };

        clbDateFolders.ItemCheck += (s, e) => UpdateSelectedDateFolders_Handler(s, e);

        // Label for txtFolderPath
        Label lblFolderPath = new Label
        {
            Text = "Folder Path:",
            Location = new Point(220, 15), // Position above txtFolderPath
            AutoSize = true
        };

        // Label for numericUpDownStart
        Label lblStartEventID = new Label
        {
            Text = "Start Event ID:",
            Location = new Point(20, 160), // Position above numericUpDownStart
            AutoSize = true
        };

        // Label for numericUpDownEnd
        Label lblEndEventID = new Label
        {
            Text = "End Event ID:",
            Location = new Point(220, 160), // Position above numericUpDownEnd
            AutoSize = true
        };

        // Label for comboBoxCameraNameFolder
        Label lblCameraName = new Label
        {
            Text = "Camera Name:",
            Location = new Point(20, 60), // Position above comboBoxCameraNameFolder
            AutoSize = true
        };

        // Label for clbDateFolders
        Label lblDateFolders = new Label
        {
            Text = "Date Folders:",
            Location = new Point(150, 60), // Position above clbDateFolders
            AutoSize = true
        };

        // Add labels to the form
        this.Controls.Add(lblFolderPath);
        this.Controls.Add(lblStartEventID);
        this.Controls.Add(lblEndEventID);
        this.Controls.Add(lblCameraName);
        this.Controls.Add(lblDateFolders);


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

        ProcessRootFolder_Core(null); // Call the method to initialize the folder structure
    }

    #endregion
}


