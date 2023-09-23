namespace MtAw2e_Catalog
{
    partial class ServerInfoForm
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
            this.sifStartButton = new System.Windows.Forms.Button();
            this.sifGuildIDLabel = new System.Windows.Forms.Label();
            this.sifChannelIDLabel = new System.Windows.Forms.Label();
            this.sifGuildIDTextBox = new System.Windows.Forms.TextBox();
            this.sifChannelIDTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sifStartButton
            // 
            this.sifStartButton.Location = new System.Drawing.Point(229, 63);
            this.sifStartButton.Name = "sifStartButton";
            this.sifStartButton.Size = new System.Drawing.Size(75, 23);
            this.sifStartButton.TabIndex = 1;
            this.sifStartButton.Text = "Start";
            this.sifStartButton.UseVisualStyleBackColor = true;
            this.sifStartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // sifGuildIDLabel
            // 
            this.sifGuildIDLabel.AutoSize = true;
            this.sifGuildIDLabel.Location = new System.Drawing.Point(12, 9);
            this.sifGuildIDLabel.Name = "sifGuildIDLabel";
            this.sifGuildIDLabel.Size = new System.Drawing.Size(49, 15);
            this.sifGuildIDLabel.TabIndex = 2;
            this.sifGuildIDLabel.Text = "Guild ID";
            // 
            // sifChannelIDLabel
            // 
            this.sifChannelIDLabel.AutoSize = true;
            this.sifChannelIDLabel.Location = new System.Drawing.Point(12, 37);
            this.sifChannelIDLabel.Name = "sifChannelIDLabel";
            this.sifChannelIDLabel.Size = new System.Drawing.Size(65, 15);
            this.sifChannelIDLabel.TabIndex = 3;
            this.sifChannelIDLabel.Text = "Channel ID";
            // 
            // sifGuildIDTextBox
            // 
            this.sifGuildIDTextBox.Location = new System.Drawing.Point(83, 5);
            this.sifGuildIDTextBox.Name = "sifGuildIDTextBox";
            this.sifGuildIDTextBox.Size = new System.Drawing.Size(221, 23);
            this.sifGuildIDTextBox.TabIndex = 4;
            // 
            // sifChannelIDTextBox
            // 
            this.sifChannelIDTextBox.Location = new System.Drawing.Point(83, 34);
            this.sifChannelIDTextBox.Name = "sifChannelIDTextBox";
            this.sifChannelIDTextBox.Size = new System.Drawing.Size(221, 23);
            this.sifChannelIDTextBox.TabIndex = 5;
            // 
            // ServerInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(311, 94);
            this.Controls.Add(this.sifChannelIDTextBox);
            this.Controls.Add(this.sifGuildIDTextBox);
            this.Controls.Add(this.sifChannelIDLabel);
            this.Controls.Add(this.sifGuildIDLabel);
            this.Controls.Add(this.sifStartButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server Information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button sifStartButton;
        private Label sifGuildIDLabel;
        private Label sifChannelIDLabel;
        private TextBox sifGuildIDTextBox;
        private TextBox sifChannelIDTextBox;
    }
}