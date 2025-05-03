namespace zmBrowse
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtDefaultFolderPath = new TextBox();
            txtDefaultCameraName = new TextBox();
            cmbThumbnailTextColor = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            btnBrowseFolder = new Button();
            btnSave = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // txtDefaultFolderPath
            // 
            txtDefaultFolderPath.Location = new Point(268, 53);
            txtDefaultFolderPath.Name = "txtDefaultFolderPath";
            txtDefaultFolderPath.Size = new Size(171, 23);
            txtDefaultFolderPath.TabIndex = 0;
            // 
            // txtDefaultCameraName
            // 
            txtDefaultCameraName.Location = new Point(267, 82);
            txtDefaultCameraName.Name = "txtDefaultCameraName";
            txtDefaultCameraName.Size = new Size(100, 23);
            txtDefaultCameraName.TabIndex = 1;
            // 
            // cmbThumbnailTextColor
            // 
            cmbThumbnailTextColor.FormattingEnabled = true;
            cmbThumbnailTextColor.Location = new Point(268, 111);
            cmbThumbnailTextColor.Name = "cmbThumbnailTextColor";
            cmbThumbnailTextColor.Size = new Size(121, 23);
            cmbThumbnailTextColor.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(80, 56);
            label1.Name = "label1";
            label1.Size = new Size(181, 15);
            label1.TabIndex = 3;
            label1.Text = "Default Zone-Minder Folder Path";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(137, 82);
            label2.Name = "label2";
            label2.Size = new Size(124, 15);
            label2.TabIndex = 4;
            label2.Text = "Default Camera Name";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(141, 114);
            label3.Name = "label3";
            label3.Size = new Size(121, 15);
            label3.TabIndex = 5;
            label3.Text = "Thumbnail Text Color";
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Location = new Point(445, 56);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(104, 23);
            btnBrowseFolder.TabIndex = 6;
            btnBrowseFolder.Text = "Select Folder";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(713, 386);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 7;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(713, 415);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(btnBrowseFolder);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cmbThumbnailTextColor);
            Controls.Add(txtDefaultCameraName);
            Controls.Add(txtDefaultFolderPath);
            Name = "SettingsForm";
            Text = "SettingsForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtDefaultFolderPath;
        private TextBox txtDefaultCameraName;
        private ComboBox cmbThumbnailTextColor;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btnBrowseFolder;
        private Button btnSave;
        private Button btnCancel;
    }
}