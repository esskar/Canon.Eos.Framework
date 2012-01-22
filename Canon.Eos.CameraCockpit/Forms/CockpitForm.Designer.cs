namespace Canon.Eos.CameraCockpit.Forms
{
    partial class CockpitForm
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
            if (disposing)
            {
                this.TearDown();
                if(components != null)
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
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this._controlPanel = new System.Windows.Forms.Panel();
            this._takePictureButton = new System.Windows.Forms.Button();
            this._storePicturesOnCameraRadioButton = new System.Windows.Forms.RadioButton();
            this._storePicturesOnHostRadioButton = new System.Windows.Forms.RadioButton();
            this._picturesOnHostLocationTextBox = new System.Windows.Forms.TextBox();
            this._browsePicturesOnHostLocationButton = new System.Windows.Forms.Button();
            this._cameraCollectionComboBox = new System.Windows.Forms.ComboBox();
            this._selectCameraLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this._controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pictureBox
            // 
            this._pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._pictureBox.Location = new System.Drawing.Point(12, 86);
            this._pictureBox.Name = "_pictureBox";
            this._pictureBox.Size = new System.Drawing.Size(596, 430);
            this._pictureBox.TabIndex = 0;
            this._pictureBox.TabStop = false;
            // 
            // _controlPanel
            // 
            this._controlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._controlPanel.Controls.Add(this._selectCameraLabel);
            this._controlPanel.Controls.Add(this._cameraCollectionComboBox);
            this._controlPanel.Controls.Add(this._takePictureButton);
            this._controlPanel.Location = new System.Drawing.Point(614, 12);
            this._controlPanel.Name = "_controlPanel";
            this._controlPanel.Size = new System.Drawing.Size(200, 504);
            this._controlPanel.TabIndex = 1;
            // 
            // _takePictureButton
            // 
            this._takePictureButton.Location = new System.Drawing.Point(-1, 48);
            this._takePictureButton.Name = "_takePictureButton";
            this._takePictureButton.Size = new System.Drawing.Size(197, 69);
            this._takePictureButton.TabIndex = 0;
            this._takePictureButton.Text = "Take Picture";
            this._takePictureButton.UseVisualStyleBackColor = true;
            this._takePictureButton.Click += new System.EventHandler(this.HandleTakePictureButtonClick);
            // 
            // _storePicturesOnCameraRadioButton
            // 
            this._storePicturesOnCameraRadioButton.AutoSize = true;
            this._storePicturesOnCameraRadioButton.Checked = true;
            this._storePicturesOnCameraRadioButton.Location = new System.Drawing.Point(12, 15);
            this._storePicturesOnCameraRadioButton.Name = "_storePicturesOnCameraRadioButton";
            this._storePicturesOnCameraRadioButton.Size = new System.Drawing.Size(145, 17);
            this._storePicturesOnCameraRadioButton.TabIndex = 2;
            this._storePicturesOnCameraRadioButton.TabStop = true;
            this._storePicturesOnCameraRadioButton.Text = "Store Pictures on Camera";
            this._storePicturesOnCameraRadioButton.UseVisualStyleBackColor = true;
            // 
            // _storePicturesOnHostRadioButton
            // 
            this._storePicturesOnHostRadioButton.AutoSize = true;
            this._storePicturesOnHostRadioButton.Location = new System.Drawing.Point(12, 38);
            this._storePicturesOnHostRadioButton.Name = "_storePicturesOnHostRadioButton";
            this._storePicturesOnHostRadioButton.Size = new System.Drawing.Size(131, 17);
            this._storePicturesOnHostRadioButton.TabIndex = 3;
            this._storePicturesOnHostRadioButton.Text = "Store Pictures on Host";
            this._storePicturesOnHostRadioButton.UseVisualStyleBackColor = true;
            // 
            // _picturesOnHostLocationTextBox
            // 
            this._picturesOnHostLocationTextBox.Location = new System.Drawing.Point(30, 60);
            this._picturesOnHostLocationTextBox.Name = "_picturesOnHostLocationTextBox";
            this._picturesOnHostLocationTextBox.Size = new System.Drawing.Size(525, 20);
            this._picturesOnHostLocationTextBox.TabIndex = 4;
            // 
            // _browsePicturesOnHostLocationButton
            // 
            this._browsePicturesOnHostLocationButton.Location = new System.Drawing.Point(561, 60);
            this._browsePicturesOnHostLocationButton.Name = "_browsePicturesOnHostLocationButton";
            this._browsePicturesOnHostLocationButton.Size = new System.Drawing.Size(46, 20);
            this._browsePicturesOnHostLocationButton.TabIndex = 5;
            this._browsePicturesOnHostLocationButton.Text = "...";
            this._browsePicturesOnHostLocationButton.UseVisualStyleBackColor = true;
            // 
            // _cameraCollectionComboBox
            // 
            this._cameraCollectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cameraCollectionComboBox.FormattingEnabled = true;
            this._cameraCollectionComboBox.Location = new System.Drawing.Point(2, 21);
            this._cameraCollectionComboBox.Name = "_cameraCollectionComboBox";
            this._cameraCollectionComboBox.Size = new System.Drawing.Size(194, 21);
            this._cameraCollectionComboBox.TabIndex = 1;
            // 
            // _selectCameraLabel
            // 
            this._selectCameraLabel.AutoSize = true;
            this._selectCameraLabel.Location = new System.Drawing.Point(3, 5);
            this._selectCameraLabel.Name = "_selectCameraLabel";
            this._selectCameraLabel.Size = new System.Drawing.Size(76, 13);
            this._selectCameraLabel.TabIndex = 2;
            this._selectCameraLabel.Text = "Select Camera";
            // 
            // CockpitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 528);
            this.Controls.Add(this._browsePicturesOnHostLocationButton);
            this.Controls.Add(this._picturesOnHostLocationTextBox);
            this.Controls.Add(this._storePicturesOnHostRadioButton);
            this.Controls.Add(this._storePicturesOnCameraRadioButton);
            this.Controls.Add(this._controlPanel);
            this.Controls.Add(this._pictureBox);
            this.Name = "CockpitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Camera Cockpit";
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this._controlPanel.ResumeLayout(false);
            this._controlPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.Panel _controlPanel;
        private System.Windows.Forms.Button _takePictureButton;
        private System.Windows.Forms.RadioButton _storePicturesOnCameraRadioButton;
        private System.Windows.Forms.RadioButton _storePicturesOnHostRadioButton;
        private System.Windows.Forms.TextBox _picturesOnHostLocationTextBox;
        private System.Windows.Forms.Button _browsePicturesOnHostLocationButton;
        private System.Windows.Forms.Label _selectCameraLabel;
        private System.Windows.Forms.ComboBox _cameraCollectionComboBox;
    }
}

