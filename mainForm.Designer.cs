using Microsoft.Extensions.Logging;

namespace zmBrowse
{

partial class mainForm
{

    private string rFolder;
    private int evStart, evEnd;
    private AppSettings settings;
    List<DateFolderStructure> selectedDateFolders = new List<DateFolderStructure>();
    ILogger logger;


    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;


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


    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
		btnEditSettings = new Button();
        lblFolderPath = new Label();
        lblStartEventID = new Label();
        lblEndEventID = new Label();
        lblCameraName = new Label();
        lblDateFolders = new Label();
        btnCheckAllDates = new Button();
        btnUncheckAllDates = new Button();
        clbDateFolders = new CheckedListBox();
        btnSelectFolder = new Button();
        btnGenerateThumbnails = new Button();
        txtFolderPath = new TextBox();
        numericUpDownStart = new NumericUpDown();
        numericUpDownEnd = new NumericUpDown();
        flowLayoutPanelThumbnails = new FlowLayoutPanel();
        comboBoxCameraNameFolder = new ComboBox();
		statusArea = new ToolStripStatusLabel();
		messageArea = new ToolStripStatusLabel();
        statusBar = new StatusStrip();
        SuspendLayout();	
		//
		// btnEditSettings
		//
        btnEditSettings.Location = new Point(521, 80);
		btnEditSettings.Name = "btnEditSettings";
        btnEditSettings.Text = "Edit Settings";
        btnEditSettings.Size = new Size(160, 25);
        btnEditSettings.Click += BtnEditSettings_Click;		
		//
		// lblFolderPath
		//
        lblFolderPath.Location = new Point(220, 15);
        lblFolderPath.Name = "lblFolderPath";
        lblFolderPath.Text = "Folder Path:";
        lblFolderPath.AutoSize = true;
		//
		// lblStartEventID
		//
        lblStartEventID.Location = new Point(20, 160);
		lblStartEventID.Name = "lblStartEventID";
        lblStartEventID.Text = "Start Event ID:";
        lblStartEventID.AutoSize = true;
		//
		// lblEndEventID
		//
        lblEndEventID.Location = new Point(220, 160);
		lblEndEventID.Name = "lblEndEventID";
        lblEndEventID.Text = "End Event ID:";
        lblEndEventID.AutoSize = true;
		//
		// lblCameraName
		//
        lblCameraName.Location = new Point(20, 60);
		lblCameraName.Name = "lblCameraName";
        lblCameraName.Text = "Camera Name:";
        lblCameraName.AutoSize = true;		
		//
		// lblDateFolders
		//
        lblDateFolders.Location = new Point(150, 60);
        lblDateFolders.Name = "lblDateFolders";
        lblDateFolders.Text = "Date Folders:";
		lblDateFolders.AutoSize = true;		
		//
		// btnCheckAllDates
		//
		btnCheckAllDates.Location = new Point(360, 80);
		btnCheckAllDates.Name = "btnCheckAllDates";
        btnCheckAllDates.Text = "Check All Dates";
		btnCheckAllDates.Size = new Size(160, 25);
		btnCheckAllDates.Click += OnClick_CheckAllDates;
		//
		// btnUncheckAllDates
		//
		btnUncheckAllDates.Location = new Point(360, 106);
		btnUncheckAllDates.Name = "btnUncheckAllDates";
        btnUncheckAllDates.Text = "Uncheck All Dates";
		btnUncheckAllDates.Size = new Size(160, 25);
        btnUncheckAllDates.Click += OnClick_UncheckAllDates;		
		//
		// clbDateFolders
		//
        clbDateFolders.Location = new Point(150, 80);
		clbDateFolders.Name = "clbDateFolders";
		clbDateFolders.Size = new Size(200, 80);
        clbDateFolders.CheckOnClick = true;
        clbDateFolders.ItemCheck += UpdateSelectedDateFolders_Handler;
		//
		// btnSelectFolder
		//
		btnSelectFolder.Location = new Point(10, 30);
		btnSelectFolder.Name= "btnSelectFolder";		
        btnSelectFolder.Text = "Zone-Minder Folder";
		btnSelectFolder.Size = new Size(160,25);
		btnSelectFolder.Click += BtnSelectFolder_Click;		
		//
		// btnGenerateThumbnails
		//
		btnGenerateThumbnails.Location = new Point(10, 210);
		btnGenerateThumbnails.Name = "btnGenerateThumbnails";
        btnGenerateThumbnails.Text = "Generate Thumbnails";
		btnGenerateThumbnails.Size = new Size(160, 25);
		btnGenerateThumbnails.Click += BtnGenerateThumbnails_Click;		
		//
		// txtFolderPath
		//
        txtFolderPath.Location = new Point(220, 32);
		txtFolderPath.Name = "txtFolderPath";
		txtFolderPath.Width = 400;
		txtFolderPath.ReadOnly = true;
		//
		// numericUpDownStart
		//
        numericUpDownStart.Location = new Point(20, 180);
		numericUpDownStart.Name = "numericUpDownStart";
		numericUpDownStart.Size = new Size(120, 30);
		numericUpDownStart.Minimum = 1;
		numericUpDownStart.Maximum = 999999;
		numericUpDownStart.Value = 76868;
		//
		// numericUpDownEnd
		//
        numericUpDownEnd.Location = new Point(220, 180);
		numericUpDownEnd.Name= "numericUpDownEnd";
		numericUpDownEnd.Size = new Size(120, 30);
		numericUpDownEnd.Minimum = 1;
		numericUpDownEnd.Maximum = 999999;
		numericUpDownEnd.Value = 76894;		
		//
		// flowLayoutPanelThumbnails
		//
        flowLayoutPanelThumbnails.Location = new Point(20, 250);
		flowLayoutPanelThumbnails.Name = "flowLayoutPanelThumbnails";
		flowLayoutPanelThumbnails.Size = new Size(750, 450);
		flowLayoutPanelThumbnails.AutoScroll = true;
        flowLayoutPanelThumbnails.BorderStyle = BorderStyle.FixedSingle;
		//
		// comboBoxCameraNameFolder
		//
        comboBoxCameraNameFolder.Location = new Point(20, 80); 
		comboBoxCameraNameFolder.Name = "comboBoxCameraNameFolder";
		comboBoxCameraNameFolder.Width = 120;
		comboBoxCameraNameFolder.DropDownStyle = ComboBoxStyle.DropDownList;
		//
		// statusArea
		//
        statusArea.Name= "statusArea";
        statusArea.Text = "Ready";
        statusArea.Width = (int)(this.Width * 0.3);
        statusArea.TextAlign = ContentAlignment.MiddleLeft;
        statusArea.Spring = false;
        statusArea.BorderSides = ToolStripStatusLabelBorderSides.All;
        statusArea.BorderStyle = Border3DStyle.SunkenOuter;
        statusArea.Padding = new Padding(5);
        statusArea.Margin = new Padding(2);		
		//
		// messageArea
		//
        messageArea.Name= "messageArea";
        messageArea.Text = "";
        messageArea.Width = (int)(this.Width * 0.7);
        messageArea.TextAlign = ContentAlignment.MiddleLeft;
        messageArea.Spring = true;
        messageArea.BorderSides = ToolStripStatusLabelBorderSides.All;
        messageArea.BorderStyle = Border3DStyle.SunkenOuter;
        messageArea.Padding = new Padding(5);
        messageArea.Margin = new Padding(2);		
		//
		// statusBar
		//
		statusBar.Name = "statusBar";
		statusBar.SizingGrip = false;
        statusBar.Padding = new Padding(2);
        statusBar.BackColor = SystemColors.ControlLight;
        statusBar.Dock = DockStyle.Bottom;
        statusBar.Items.Add(statusArea);
        statusBar.Items.Add(messageArea);		
		//
		// mainForm
		//
        Name = "mainForm";
		components = new System.ComponentModel.Container();
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        Text = "ZoneMinder Thumbnail Browser";
        Size = new Size(800, 800);
		Controls.Add(btnEditSettings);
        Controls.Add(lblFolderPath);
        Controls.Add(lblStartEventID);
        Controls.Add(lblEndEventID);
        Controls.Add(lblCameraName);
        Controls.Add(lblDateFolders);
        Controls.Add(btnCheckAllDates);
        Controls.Add(btnUncheckAllDates);
        Controls.Add(clbDateFolders);
        Controls.Add(btnSelectFolder);
        Controls.Add(btnGenerateThumbnails);
        Controls.Add(txtFolderPath);
        Controls.Add(numericUpDownStart);
        Controls.Add(numericUpDownEnd);
        Controls.Add(flowLayoutPanelThumbnails);
        Controls.Add(comboBoxCameraNameFolder);
        Controls.Add(statusBar);
		ResumeLayout(false);
		PerformLayout();	
    }

    #endregion
	
    private Button btnSelectFolder;
    private Button btnGenerateThumbnails;
    private Button btnCheckAllDates;
    private Button btnUncheckAllDates;
    private Button btnEditSettings;
    private TextBox txtFolderPath;
    private NumericUpDown numericUpDownStart;
    private NumericUpDown numericUpDownEnd;
    private ComboBox comboBoxCameraNameFolder;
    private CheckedListBox clbDateFolders;
    private FlowLayoutPanel flowLayoutPanelThumbnails;
    private StatusStrip statusBar;
    private ToolStripStatusLabel statusArea;
    private ToolStripStatusLabel messageArea;
	private Label lblFolderPath;
	private Label lblStartEventID;
	private Label lblEndEventID;
	private Label lblCameraName;
	private Label lblDateFolders;
    private System.Timers.Timer messageClearTimer;
	
}


}
