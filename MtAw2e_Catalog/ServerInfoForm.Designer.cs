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
            this.sifServerComboBox = new System.Windows.Forms.ComboBox();
            this.sifStartButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sifServerComboBox
            // 
            this.sifServerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sifServerComboBox.FormattingEnabled = true;
            this.sifServerComboBox.Items.AddRange(new object[] {
            "Bork",
            "FWSP"});
            this.sifServerComboBox.Location = new System.Drawing.Point(12, 11);
            this.sifServerComboBox.Name = "sifServerComboBox";
            this.sifServerComboBox.Size = new System.Drawing.Size(121, 23);
            this.sifServerComboBox.TabIndex = 0;
            // 
            // sifStartButton
            // 
            this.sifStartButton.Location = new System.Drawing.Point(161, 11);
            this.sifStartButton.Name = "sifStartButton";
            this.sifStartButton.Size = new System.Drawing.Size(75, 23);
            this.sifStartButton.TabIndex = 1;
            this.sifStartButton.Text = "Start";
            this.sifStartButton.UseVisualStyleBackColor = true;
            this.sifStartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // ServerInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 51);
            this.Controls.Add(this.sifStartButton);
            this.Controls.Add(this.sifServerComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ServerInfoForm";
            this.ResumeLayout(false);

        }

        #endregion

        private ComboBox sifServerComboBox;
        private Button sifStartButton;
    }
}