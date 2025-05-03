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
using System.Timers;


namespace zmBrowse;

partial class mainForm
{

    private string rFolder;
    private int evStart, evEnd;
    private AppSettings settings;

    Button btnSelectFolder;
    Button btnGenerateThumbnails;
    Button btnCheckAllDates;
    Button btnUncheckAllDates;
    TextBox txtFolderPath;
    NumericUpDown numericUpDownStart;
    NumericUpDown numericUpDownEnd;
    ComboBox comboBoxCameraNameFolder;
    CheckedListBox clbDateFolders;
    FlowLayoutPanel flowLayoutPanelThumbnails;
    List<DateFolderStructure> selectedDateFolders = new List<DateFolderStructure>();
    private StatusStrip statusBar;
    private ToolStripStatusLabel statusArea;
    private ToolStripStatusLabel messageArea;
    private System.Timers.Timer messageClearTimer; // Timer to clear the message after 3 seconds
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
        SetStatus("Selecting folder...");
        using (FolderBrowserDialog dialog = new FolderBrowserDialog())
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ProcessRootFolder_Core(dialog);
                SetMessage($"Folder selected: {dialog.SelectedPath}");
            }
            else
            {
                SetMessage("Folder selection canceled.");
            }
        }
        SetStatus("Ready");
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

    private async void BtnGenerateThumbnails_Click(object sender, EventArgs e)
    {
        // Set the status to indicate the start of the process
        SetStatus("Generating thumbnails...");
        SetMessage("Initializing thumbnail generation...");

        evStart = (int)numericUpDownStart.Value;
        evEnd = (int)numericUpDownEnd.Value;

        flowLayoutPanelThumbnails.Controls.Clear();

        int totalThumbnails = 0;
        int skippedThumbnails = 0;
        List<PictureBox> tnList = null;

        await Task.Run(() =>
        {
            tnList = new List<PictureBox>();
            selectedDateFolders = selectedDateFolders.OrderBy(x => x.Name).ToList();
            foreach (var dateFolder in selectedDateFolders)
            {
                foreach (var eventID in dateFolder.sfEvents)
                {
                    if (int.TryParse(eventID, out int eventIDInt) && (eventIDInt < evStart || eventIDInt > evEnd))
                    {
                        SetMessage($"Skipping EventID: {eventIDInt}, is outside filter range");
                        skippedThumbnails++;
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
                            //flowLayoutPanelThumbnails.Controls.Add(pictureBox);
                            tnList.Add(pictureBox);
                            totalThumbnails++;
                            SetMessage($"Created thumbnail for EventID: {evInt}");
                        }
                        else
                        {
                            this.logger.LogWarning($"Thumbnail creation failed for {videoPath}");
                            SetMessage($"Failed to create thumbnail for EventID: {eventID}");
                            skippedThumbnails++;
                        }
                    }
                    else
                    {
                        this.logger.LogWarning($"For EventID: {eventID}, Video file was not found: {videoPath}");
                        SetMessage($"For EventID: {eventID}, Video file was not found: {videoPath}");
                        skippedThumbnails++;
                    }

                    // Yield control to allow other threads (like the timer) to execute
                    Task.Delay(10).Wait();
                }
            }
        });
        if (tnList != null)
        {
            foreach (var pictureBox in tnList)
            {
                flowLayoutPanelThumbnails.Controls.Add(pictureBox);
            }
        }
        // Update the status and message based on the results
        if (totalThumbnails > 0)
        {
            SetStatus("Ready.");
            SetMessage($"Generated {totalThumbnails} thumbnails. Skipped {skippedThumbnails} events.");
            this.logger.LogInformation($"Generated {totalThumbnails} thumbnails. Skipped {skippedThumbnails} events.");
        }
        else
        {
            SetStatus("Ready.");
            SetMessage($"No valid events found in the selected range. Skipped {skippedThumbnails} events.");
            this.logger.LogWarning($"No valid events found in the selected range. Skipped {skippedThumbnails} events.");
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

    private async void UpdateSelectedDateFolders_Core(List<string> updatedCheckedItems)
    {
        int evMin = 999999;
        int evMax = 1;

        await Task.Run(() =>
        {
            selectedDateFolders.Clear();
        });
        Task.Delay(5).Wait();
        foreach (var datefolder in updatedCheckedItems)
        {
            string dateFolderPath = Path.Combine(rFolder, comboBoxCameraNameFolder.SelectedItem?.ToString(), datefolder);
            string[] subFolders = null;
            await Task.Run(() =>
            {
                subFolders = Directory.GetDirectories(dateFolderPath);
            });
            Task.Delay(5).Wait();
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

    private void SetStatus(string status)
    {
        if (statusArea != null)
        {
            statusArea.Text = status;
        }
    }

    private void SetMessage(string message)
    {
        if (messageArea != null)
        {
            messageArea.Text = message;
            if (messageClearTimer != null)
            {
                messageClearTimer.Stop(); // Stop any existing timer
                messageClearTimer.Start(); // Start the timer for 3 seconds
            }
        }
    }

    private void ClearMessage()
    {
        if (messageArea != null)
        {
            messageArea.Text = "";
        }
        if (messageClearTimer != null)
            messageClearTimer.Stop();
    }

    private void OnClick_CheckAllDates(object sender, EventArgs e)
    {
        SetStatus("Checking all dates...");
        for (int i = 0; i < clbDateFolders.Items.Count; i++)
        {
            clbDateFolders.SetItemChecked(i, true);
        }
        UpdateSelectedDateFolders_Core(clbDateFolders.CheckedItems.Cast<string>().ToList());
        SetStatus("Ready.");
    }

    private void OnClick_UncheckAllDates(object sender, EventArgs e)
    {
        SetStatus("Unchecking all dates...");
        for (int i = 0; i < clbDateFolders.Items.Count; i++)
        {
            clbDateFolders.SetItemChecked(i, false);
        }
        UpdateSelectedDateFolders_Core(clbDateFolders.CheckedItems.Cast<string>().ToList());
        SetStatus("Ready.");
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

        btnCheckAllDates = new Button { Text = "Check All Dates", Location = new Point(360, 80), Size = new Size(160, 25) };
        btnUncheckAllDates = new Button { Text = "Uncheck All Dates", Location = new Point(360, 106), Size = new Size(160, 25) };

        btnCheckAllDates.Click += (s, e) => OnClick_CheckAllDates(s, e);

        btnUncheckAllDates.Click += (s, e) => OnClick_UncheckAllDates(s, e);

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

        // Initialize the StatusStrip
        statusBar = new StatusStrip
        {
            Dock = DockStyle.Bottom,
            SizingGrip = false, // Remove the resizing grip for a cleaner look
            Padding = new Padding(2), // Add padding around the StatusStrip
            BackColor = SystemColors.ControlLight // Set a light background color for contrast
        };
        statusBar.Dock = DockStyle.Bottom;

        // Initialize the StatusArea
        statusArea = new ToolStripStatusLabel
        {
            Text = "Ready",
            Width = (int)(this.Width * 0.3), // 30% of the form width
            TextAlign = ContentAlignment.MiddleLeft,
            Spring = false,
            BorderSides = ToolStripStatusLabelBorderSides.All, // Add borders on all sides
            BorderStyle = Border3DStyle.SunkenOuter, // Sunken border style
            Padding = new Padding(5), // Add padding inside the box
            Margin = new Padding(2) // Add margin between the box and the StatusStrip
        };

        // Initialize the MessageArea
        messageArea = new ToolStripStatusLabel
        {
            Text = "",
            Width = (int)(this.Width * 0.7), // 70% of the form width
            TextAlign = ContentAlignment.MiddleLeft,
            Spring = true,
            BorderSides = ToolStripStatusLabelBorderSides.All, // Add borders on all sides
            BorderStyle = Border3DStyle.SunkenOuter, // Sunken border style
            Padding = new Padding(5), // Add padding inside the box
            Margin = new Padding(2) // Add margin between the box and the StatusStrip
        };

        // Add the labels to the StatusStrip
        statusBar.Items.Add(statusArea);
        statusBar.Items.Add(messageArea);

        // Initialize the messageClearTimer
        messageClearTimer = new System.Timers.Timer(3000); // 3 seconds
        messageClearTimer.Elapsed += (s, e) =>
        {
            ClearMessage(); // Clear the message when the timer elapses
        };
        messageClearTimer.AutoReset = true; // Ensure the timer runs multiple times

        // Add labels to the form
        this.Controls.Add(lblFolderPath);
        this.Controls.Add(lblStartEventID);
        this.Controls.Add(lblEndEventID);
        this.Controls.Add(lblCameraName);
        this.Controls.Add(lblDateFolders);
        this.Controls.Add(btnCheckAllDates);
        this.Controls.Add(btnUncheckAllDates);

        this.Controls.Add(clbDateFolders);
        this.Controls.Add(btnSelectFolder);
        this.Controls.Add(btnGenerateThumbnails);
        this.Controls.Add(txtFolderPath);
        this.Controls.Add(numericUpDownStart);
        this.Controls.Add(numericUpDownEnd);
        this.Controls.Add(flowLayoutPanelThumbnails);
        this.Controls.Add(comboBoxCameraNameFolder);
        this.Controls.Add(statusBar);

        btnSelectFolder.Click += BtnSelectFolder_Click;
        btnGenerateThumbnails.Click += BtnGenerateThumbnails_Click;

        ProcessRootFolder_Core(null); // Call the method to initialize the folder structure
    }

    #endregion
}


