namespace GoogleDriveSync
{
    partial class MainSyncPage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainSyncPage));
            this.DGVFileInformation = new System.Windows.Forms.DataGridView();
            this.FName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DCreated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LModified = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HChanges = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.UFile = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TBCDocumentChanges = new System.Windows.Forms.TabControl();
            this.TBPDocumentChanges = new System.Windows.Forms.TabPage();
            this.TBLPath = new System.Windows.Forms.TextBox();
            this.LBLocalPath = new System.Windows.Forms.Label();
            this.TBModified = new System.Windows.Forms.TextBox();
            this.TBCreated = new System.Windows.Forms.TextBox();
            this.TBHidden = new System.Windows.Forms.TextBox();
            this.TBGDPath = new System.Windows.Forms.TextBox();
            this.TBPathIds = new System.Windows.Forms.TextBox();
            this.TBType = new System.Windows.Forms.TextBox();
            this.TBName = new System.Windows.Forms.TextBox();
            this.TBId = new System.Windows.Forms.TextBox();
            this.DGVChanges = new System.Windows.Forms.DataGridView();
            this.Field = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OldValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Stream = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LBModified = new System.Windows.Forms.Label();
            this.LBCreated = new System.Windows.Forms.Label();
            this.LBHidden = new System.Windows.Forms.Label();
            this.LBGDPath = new System.Windows.Forms.Label();
            this.LBPathIds = new System.Windows.Forms.Label();
            this.LBType = new System.Windows.Forms.Label();
            this.LBName = new System.Windows.Forms.Label();
            this.LBId = new System.Windows.Forms.Label();
            this.PRBLoading = new System.Windows.Forms.ProgressBar();
            this.BTNCompare = new System.Windows.Forms.Button();
            this.BTNSync = new System.Windows.Forms.Button();
            this.PBDown = new System.Windows.Forms.PictureBox();
            this.PBUp = new System.Windows.Forms.PictureBox();
            this.PBLoading = new System.Windows.Forms.PictureBox();
            this.CBAutoSync = new System.Windows.Forms.CheckBox();
            this.TMAutoSync = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DGVFileInformation)).BeginInit();
            this.TBCDocumentChanges.SuspendLayout();
            this.TBPDocumentChanges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVChanges)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // DGVFileInformation
            // 
            this.DGVFileInformation.AllowUserToAddRows = false;
            this.DGVFileInformation.AllowUserToDeleteRows = false;
            this.DGVFileInformation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVFileInformation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FName,
            this.FType,
            this.FPath,
            this.DCreated,
            this.LModified,
            this.HChanges,
            this.MType,
            this.UFile});
            this.DGVFileInformation.Location = new System.Drawing.Point(0, 23);
            this.DGVFileInformation.Name = "DGVFileInformation";
            this.DGVFileInformation.Size = new System.Drawing.Size(844, 450);
            this.DGVFileInformation.TabIndex = 0;
            this.DGVFileInformation.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGVFileInformationCellValue);
            this.DGVFileInformation.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.DGVFileInformationRowState);
            // 
            // FName
            // 
            this.FName.HeaderText = "File Name";
            this.FName.Name = "FName";
            this.FName.ReadOnly = true;
            // 
            // FType
            // 
            this.FType.HeaderText = "File Type";
            this.FType.Name = "FType";
            this.FType.ReadOnly = true;
            // 
            // FPath
            // 
            this.FPath.HeaderText = "File Path";
            this.FPath.Name = "FPath";
            this.FPath.ReadOnly = true;
            // 
            // DCreated
            // 
            this.DCreated.HeaderText = "Date Created";
            this.DCreated.Name = "DCreated";
            this.DCreated.ReadOnly = true;
            // 
            // LModified
            // 
            this.LModified.HeaderText = "Last Modified";
            this.LModified.Name = "LModified";
            this.LModified.ReadOnly = true;
            // 
            // HChanges
            // 
            this.HChanges.HeaderText = "Has Changes";
            this.HChanges.Name = "HChanges";
            this.HChanges.ReadOnly = true;
            // 
            // MType
            // 
            this.MType.HeaderText = "Merge Type";
            this.MType.Items.AddRange(new object[] {
            "Up Stream",
            "Down Stream"});
            this.MType.Name = "MType";
            // 
            // UFile
            // 
            this.UFile.HeaderText = "Update";
            this.UFile.Name = "UFile";
            // 
            // TBCDocumentChanges
            // 
            this.TBCDocumentChanges.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.TBCDocumentChanges.Controls.Add(this.TBPDocumentChanges);
            this.TBCDocumentChanges.ItemSize = new System.Drawing.Size(0, 1);
            this.TBCDocumentChanges.Location = new System.Drawing.Point(847, 23);
            this.TBCDocumentChanges.Margin = new System.Windows.Forms.Padding(0);
            this.TBCDocumentChanges.Name = "TBCDocumentChanges";
            this.TBCDocumentChanges.Padding = new System.Drawing.Point(0, 0);
            this.TBCDocumentChanges.SelectedIndex = 0;
            this.TBCDocumentChanges.Size = new System.Drawing.Size(400, 450);
            this.TBCDocumentChanges.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TBCDocumentChanges.TabIndex = 1;
            this.TBCDocumentChanges.Visible = false;
            // 
            // TBPDocumentChanges
            // 
            this.TBPDocumentChanges.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TBPDocumentChanges.Controls.Add(this.TBLPath);
            this.TBPDocumentChanges.Controls.Add(this.LBLocalPath);
            this.TBPDocumentChanges.Controls.Add(this.TBModified);
            this.TBPDocumentChanges.Controls.Add(this.TBCreated);
            this.TBPDocumentChanges.Controls.Add(this.TBHidden);
            this.TBPDocumentChanges.Controls.Add(this.TBGDPath);
            this.TBPDocumentChanges.Controls.Add(this.TBPathIds);
            this.TBPDocumentChanges.Controls.Add(this.TBType);
            this.TBPDocumentChanges.Controls.Add(this.TBName);
            this.TBPDocumentChanges.Controls.Add(this.TBId);
            this.TBPDocumentChanges.Controls.Add(this.DGVChanges);
            this.TBPDocumentChanges.Controls.Add(this.LBModified);
            this.TBPDocumentChanges.Controls.Add(this.LBCreated);
            this.TBPDocumentChanges.Controls.Add(this.LBHidden);
            this.TBPDocumentChanges.Controls.Add(this.LBGDPath);
            this.TBPDocumentChanges.Controls.Add(this.LBPathIds);
            this.TBPDocumentChanges.Controls.Add(this.LBType);
            this.TBPDocumentChanges.Controls.Add(this.LBName);
            this.TBPDocumentChanges.Controls.Add(this.LBId);
            this.TBPDocumentChanges.Location = new System.Drawing.Point(4, 5);
            this.TBPDocumentChanges.Margin = new System.Windows.Forms.Padding(0);
            this.TBPDocumentChanges.Name = "TBPDocumentChanges";
            this.TBPDocumentChanges.Size = new System.Drawing.Size(392, 441);
            this.TBPDocumentChanges.TabIndex = 0;
            this.TBPDocumentChanges.UseVisualStyleBackColor = true;
            // 
            // TBLPath
            // 
            this.TBLPath.Location = new System.Drawing.Point(123, 198);
            this.TBLPath.Name = "TBLPath";
            this.TBLPath.ReadOnly = true;
            this.TBLPath.Size = new System.Drawing.Size(202, 20);
            this.TBLPath.TabIndex = 18;
            // 
            // LBLocalPath
            // 
            this.LBLocalPath.AutoSize = true;
            this.LBLocalPath.Location = new System.Drawing.Point(67, 201);
            this.LBLocalPath.Name = "LBLocalPath";
            this.LBLocalPath.Size = new System.Drawing.Size(41, 13);
            this.LBLocalPath.TabIndex = 17;
            this.LBLocalPath.Text = "L Path:";
            // 
            // TBModified
            // 
            this.TBModified.Location = new System.Drawing.Point(123, 273);
            this.TBModified.Name = "TBModified";
            this.TBModified.ReadOnly = true;
            this.TBModified.Size = new System.Drawing.Size(202, 20);
            this.TBModified.TabIndex = 16;
            // 
            // TBCreated
            // 
            this.TBCreated.Location = new System.Drawing.Point(123, 248);
            this.TBCreated.Name = "TBCreated";
            this.TBCreated.ReadOnly = true;
            this.TBCreated.Size = new System.Drawing.Size(202, 20);
            this.TBCreated.TabIndex = 15;
            // 
            // TBHidden
            // 
            this.TBHidden.Location = new System.Drawing.Point(123, 223);
            this.TBHidden.Name = "TBHidden";
            this.TBHidden.ReadOnly = true;
            this.TBHidden.Size = new System.Drawing.Size(202, 20);
            this.TBHidden.TabIndex = 14;
            // 
            // TBGDPath
            // 
            this.TBGDPath.Location = new System.Drawing.Point(123, 173);
            this.TBGDPath.Name = "TBGDPath";
            this.TBGDPath.ReadOnly = true;
            this.TBGDPath.Size = new System.Drawing.Size(202, 20);
            this.TBGDPath.TabIndex = 13;
            // 
            // TBPathIds
            // 
            this.TBPathIds.Location = new System.Drawing.Point(123, 148);
            this.TBPathIds.Name = "TBPathIds";
            this.TBPathIds.ReadOnly = true;
            this.TBPathIds.Size = new System.Drawing.Size(202, 20);
            this.TBPathIds.TabIndex = 12;
            // 
            // TBType
            // 
            this.TBType.Location = new System.Drawing.Point(123, 123);
            this.TBType.Name = "TBType";
            this.TBType.ReadOnly = true;
            this.TBType.Size = new System.Drawing.Size(202, 20);
            this.TBType.TabIndex = 11;
            // 
            // TBName
            // 
            this.TBName.Location = new System.Drawing.Point(123, 98);
            this.TBName.Name = "TBName";
            this.TBName.ReadOnly = true;
            this.TBName.Size = new System.Drawing.Size(202, 20);
            this.TBName.TabIndex = 10;
            // 
            // TBId
            // 
            this.TBId.Location = new System.Drawing.Point(123, 73);
            this.TBId.Name = "TBId";
            this.TBId.ReadOnly = true;
            this.TBId.Size = new System.Drawing.Size(202, 20);
            this.TBId.TabIndex = 9;
            // 
            // DGVChanges
            // 
            this.DGVChanges.AllowUserToAddRows = false;
            this.DGVChanges.AllowUserToDeleteRows = false;
            this.DGVChanges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVChanges.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Field,
            this.OldValue,
            this.NewValue,
            this.Stream});
            this.DGVChanges.Location = new System.Drawing.Point(3, 298);
            this.DGVChanges.Name = "DGVChanges";
            this.DGVChanges.ReadOnly = true;
            this.DGVChanges.Size = new System.Drawing.Size(384, 70);
            this.DGVChanges.TabIndex = 8;
            // 
            // Field
            // 
            this.Field.HeaderText = "Field";
            this.Field.Name = "Field";
            this.Field.ReadOnly = true;
            this.Field.Width = 70;
            // 
            // OldValue
            // 
            this.OldValue.HeaderText = "Old Value";
            this.OldValue.Name = "OldValue";
            this.OldValue.ReadOnly = true;
            // 
            // NewValue
            // 
            this.NewValue.HeaderText = "NewValue";
            this.NewValue.Name = "NewValue";
            this.NewValue.ReadOnly = true;
            // 
            // Stream
            // 
            this.Stream.HeaderText = "Stream";
            this.Stream.Name = "Stream";
            this.Stream.ReadOnly = true;
            this.Stream.Width = 70;
            // 
            // LBModified
            // 
            this.LBModified.AutoSize = true;
            this.LBModified.Location = new System.Drawing.Point(67, 276);
            this.LBModified.Name = "LBModified";
            this.LBModified.Size = new System.Drawing.Size(50, 13);
            this.LBModified.TabIndex = 7;
            this.LBModified.Text = "Modified:";
            // 
            // LBCreated
            // 
            this.LBCreated.AutoSize = true;
            this.LBCreated.Location = new System.Drawing.Point(67, 251);
            this.LBCreated.Name = "LBCreated";
            this.LBCreated.Size = new System.Drawing.Size(47, 13);
            this.LBCreated.TabIndex = 6;
            this.LBCreated.Text = "Created:";
            // 
            // LBHidden
            // 
            this.LBHidden.AutoSize = true;
            this.LBHidden.Location = new System.Drawing.Point(67, 226);
            this.LBHidden.Name = "LBHidden";
            this.LBHidden.Size = new System.Drawing.Size(44, 13);
            this.LBHidden.TabIndex = 5;
            this.LBHidden.Text = "Hidden:";
            // 
            // LBGDPath
            // 
            this.LBGDPath.AutoSize = true;
            this.LBGDPath.Location = new System.Drawing.Point(67, 176);
            this.LBGDPath.Name = "LBGDPath";
            this.LBGDPath.Size = new System.Drawing.Size(51, 13);
            this.LBGDPath.TabIndex = 4;
            this.LBGDPath.Text = "GD Path:";
            // 
            // LBPathIds
            // 
            this.LBPathIds.AutoSize = true;
            this.LBPathIds.Location = new System.Drawing.Point(67, 151);
            this.LBPathIds.Name = "LBPathIds";
            this.LBPathIds.Size = new System.Drawing.Size(49, 13);
            this.LBPathIds.TabIndex = 3;
            this.LBPathIds.Text = "Path Ids:";
            // 
            // LBType
            // 
            this.LBType.AutoSize = true;
            this.LBType.Location = new System.Drawing.Point(67, 126);
            this.LBType.Name = "LBType";
            this.LBType.Size = new System.Drawing.Size(34, 13);
            this.LBType.TabIndex = 2;
            this.LBType.Text = "Type:";
            // 
            // LBName
            // 
            this.LBName.AutoSize = true;
            this.LBName.Location = new System.Drawing.Point(67, 101);
            this.LBName.Name = "LBName";
            this.LBName.Size = new System.Drawing.Size(38, 13);
            this.LBName.TabIndex = 1;
            this.LBName.Text = "Name:";
            // 
            // LBId
            // 
            this.LBId.AutoSize = true;
            this.LBId.Location = new System.Drawing.Point(67, 76);
            this.LBId.Name = "LBId";
            this.LBId.Size = new System.Drawing.Size(19, 13);
            this.LBId.TabIndex = 0;
            this.LBId.Text = "Id:";
            // 
            // PRBLoading
            // 
            this.PRBLoading.Location = new System.Drawing.Point(432, 0);
            this.PRBLoading.Name = "PRBLoading";
            this.PRBLoading.Size = new System.Drawing.Size(400, 23);
            this.PRBLoading.TabIndex = 0;
            // 
            // BTNCompare
            // 
            this.BTNCompare.Location = new System.Drawing.Point(544, 473);
            this.BTNCompare.Name = "BTNCompare";
            this.BTNCompare.Size = new System.Drawing.Size(86, 23);
            this.BTNCompare.TabIndex = 3;
            this.BTNCompare.Text = "Compare";
            this.BTNCompare.UseVisualStyleBackColor = true;
            this.BTNCompare.Click += new System.EventHandler(this.BTNCompareClick);
            // 
            // BTNSync
            // 
            this.BTNSync.Enabled = false;
            this.BTNSync.Location = new System.Drawing.Point(635, 473);
            this.BTNSync.Name = "BTNSync";
            this.BTNSync.Size = new System.Drawing.Size(86, 23);
            this.BTNSync.TabIndex = 4;
            this.BTNSync.Text = "Sync 0 File(s)";
            this.BTNSync.UseVisualStyleBackColor = true;
            this.BTNSync.Click += new System.EventHandler(this.BTNSyncClick);
            // 
            // PBDown
            // 
            this.PBDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PBDown.Enabled = false;
            this.PBDown.Image = global::GoogleDriveSync.Properties.Resources.Down_Arrow;
            this.PBDown.Location = new System.Drawing.Point(727, 473);
            this.PBDown.Name = "PBDown";
            this.PBDown.Size = new System.Drawing.Size(23, 23);
            this.PBDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PBDown.TabIndex = 6;
            this.PBDown.TabStop = false;
            this.PBDown.Click += new System.EventHandler(this.SyncDown);
            // 
            // PBUp
            // 
            this.PBUp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PBUp.Enabled = false;
            this.PBUp.Image = global::GoogleDriveSync.Properties.Resources.Up_Arrow;
            this.PBUp.Location = new System.Drawing.Point(515, 473);
            this.PBUp.Name = "PBUp";
            this.PBUp.Size = new System.Drawing.Size(23, 23);
            this.PBUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PBUp.TabIndex = 5;
            this.PBUp.TabStop = false;
            this.PBUp.Click += new System.EventHandler(this.SyncUp);
            // 
            // PBLoading
            // 
            this.PBLoading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PBLoading.Location = new System.Drawing.Point(838, 0);
            this.PBLoading.Name = "PBLoading";
            this.PBLoading.Size = new System.Drawing.Size(23, 23);
            this.PBLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PBLoading.TabIndex = 2;
            this.PBLoading.TabStop = false;
            // 
            // CBAutoSync
            // 
            this.CBAutoSync.AutoSize = true;
            this.CBAutoSync.Location = new System.Drawing.Point(351, 3);
            this.CBAutoSync.Name = "CBAutoSync";
            this.CBAutoSync.Size = new System.Drawing.Size(75, 17);
            this.CBAutoSync.TabIndex = 7;
            this.CBAutoSync.Text = "Auto Sync";
            this.CBAutoSync.UseVisualStyleBackColor = true;
            this.CBAutoSync.CheckedChanged += new System.EventHandler(this.CBAutoSyncChecked);
            // 
            // TMAutoSync
            // 
            this.TMAutoSync.Interval = 15000;
            this.TMAutoSync.Tick += new System.EventHandler(this.TMAutoSyncElapsedAsync);
            // 
            // MainSyncPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 496);
            this.Controls.Add(this.CBAutoSync);
            this.Controls.Add(this.PBDown);
            this.Controls.Add(this.PBUp);
            this.Controls.Add(this.BTNSync);
            this.Controls.Add(this.BTNCompare);
            this.Controls.Add(this.PBLoading);
            this.Controls.Add(this.PRBLoading);
            this.Controls.Add(this.DGVFileInformation);
            this.Controls.Add(this.TBCDocumentChanges);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1265, 535);
            this.MinimumSize = new System.Drawing.Size(1265, 535);
            this.Name = "MainSyncPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Google Drive Sync";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Exit);
            ((System.ComponentModel.ISupportInitialize)(this.DGVFileInformation)).EndInit();
            this.TBCDocumentChanges.ResumeLayout(false);
            this.TBPDocumentChanges.ResumeLayout(false);
            this.TBPDocumentChanges.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVChanges)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DGVFileInformation;
        private System.Windows.Forms.TabControl TBCDocumentChanges;
        private System.Windows.Forms.TabPage TBPDocumentChanges;
        private System.Windows.Forms.PictureBox PBLoading;
        private System.Windows.Forms.Button BTNCompare;
        private System.Windows.Forms.Button BTNSync;
        private System.Windows.Forms.ProgressBar PRBLoading;
        private System.Windows.Forms.Label LBId;
        private System.Windows.Forms.Label LBModified;
        private System.Windows.Forms.Label LBCreated;
        private System.Windows.Forms.Label LBHidden;
        private System.Windows.Forms.Label LBGDPath;
        private System.Windows.Forms.Label LBPathIds;
        private System.Windows.Forms.Label LBType;
        private System.Windows.Forms.Label LBName;
        private System.Windows.Forms.DataGridView DGVChanges;
        private System.Windows.Forms.TextBox TBModified;
        private System.Windows.Forms.TextBox TBCreated;
        private System.Windows.Forms.TextBox TBHidden;
        private System.Windows.Forms.TextBox TBGDPath;
        private System.Windows.Forms.TextBox TBPathIds;
        private System.Windows.Forms.TextBox TBType;
        private System.Windows.Forms.TextBox TBName;
        private System.Windows.Forms.TextBox TBId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field;
        private System.Windows.Forms.DataGridViewTextBoxColumn OldValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn NewValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Stream;
        private System.Windows.Forms.DataGridViewTextBoxColumn FName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FType;
        private System.Windows.Forms.DataGridViewTextBoxColumn FPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn DCreated;
        private System.Windows.Forms.DataGridViewTextBoxColumn LModified;
        private System.Windows.Forms.DataGridViewTextBoxColumn HChanges;
        private System.Windows.Forms.DataGridViewComboBoxColumn MType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UFile;
        private System.Windows.Forms.TextBox TBLPath;
        private System.Windows.Forms.Label LBLocalPath;
        private System.Windows.Forms.PictureBox PBUp;
        private System.Windows.Forms.PictureBox PBDown;
        private System.Windows.Forms.CheckBox CBAutoSync;
        private System.Windows.Forms.Timer TMAutoSync;
    }
}

