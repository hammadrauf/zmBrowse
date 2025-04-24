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
    FlowLayoutPanel flowLayoutPanelThumbnails;
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

    private void BtnSelectFolder_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog dialog = new FolderBrowserDialog())
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                rFolder = dialog.SelectedPath;
                txtFolderPath.Text = rFolder;
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
        btnSelectFolder = new Button { Text = "Select Folder", Location = new Point(10, 10) };
        btnGenerateThumbnails = new Button { Text = "Generate Thumbnails", Location = new Point(10, 100) };
        txtFolderPath = new TextBox { Location = new Point(110, 15), Width = 500, ReadOnly = true, Text = "\\\\192.168.4.5\\ZM_Events\\1\\2025-04-22" };
        numericUpDownStart = new NumericUpDown { Location = new Point(20, 60), Minimum = 9999, Maximum = 999999, Value=76868 };
        numericUpDownEnd = new NumericUpDown { Location = new Point(220, 60), Minimum = 9999, Maximum = 999999, Value= 76894 };
        flowLayoutPanelThumbnails = new FlowLayoutPanel {
            Location = new Point(20, 140),
            Size = new Size(750, 450),
            AutoScroll = true,
            BorderStyle = BorderStyle.FixedSingle
        };

        this.Controls.Add(btnSelectFolder);
        this.Controls.Add(btnGenerateThumbnails);
        this.Controls.Add(txtFolderPath);
        this.Controls.Add(numericUpDownStart);
        this.Controls.Add(numericUpDownEnd);
        this.Controls.Add(flowLayoutPanelThumbnails);

        btnSelectFolder.Click += BtnSelectFolder_Click;
        btnGenerateThumbnails.Click += BtnGenerateThumbnails_Click;

    }

    #endregion
}


