namespace MtAw2e_Catalog
{
    partial class ArcanaForm
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
            this.afHighestArcanaComboBox = new System.Windows.Forms.ComboBox();
            this.afHighestDotsNumUpDown = new System.Windows.Forms.NumericUpDown();
            this.afOtherDotsNumUpDown = new System.Windows.Forms.NumericUpDown();
            this.afHighArcanaLabel = new System.Windows.Forms.Label();
            this.afOtherArcanaLabel = new System.Windows.Forms.Label();
            this.afDotsLabel1 = new System.Windows.Forms.Label();
            this.afDotsLabel2 = new System.Windows.Forms.Label();
            this.afOtherArcanaComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.afHighestDotsNumUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.afOtherDotsNumUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // afHighestArcanaComboBox
            // 
            this.afHighestArcanaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.afHighestArcanaComboBox.FormattingEnabled = true;
            this.afHighestArcanaComboBox.Items.AddRange(new object[] {
            "Death",
            "Fate",
            "Forces",
            "Life",
            "Matter",
            "Mind",
            "Prime",
            "Space",
            "Spirit",
            "Time"});
            this.afHighestArcanaComboBox.Location = new System.Drawing.Point(12, 28);
            this.afHighestArcanaComboBox.Name = "afHighestArcanaComboBox";
            this.afHighestArcanaComboBox.Size = new System.Drawing.Size(121, 23);
            this.afHighestArcanaComboBox.Sorted = true;
            this.afHighestArcanaComboBox.TabIndex = 0;
            // 
            // afHighestDotsNumUpDown
            // 
            this.afHighestDotsNumUpDown.Location = new System.Drawing.Point(139, 28);
            this.afHighestDotsNumUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.afHighestDotsNumUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.afHighestDotsNumUpDown.Name = "afHighestDotsNumUpDown";
            this.afHighestDotsNumUpDown.Size = new System.Drawing.Size(42, 23);
            this.afHighestDotsNumUpDown.TabIndex = 1;
            this.afHighestDotsNumUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // afOtherDotsNumUpDown
            // 
            this.afOtherDotsNumUpDown.Location = new System.Drawing.Point(314, 28);
            this.afOtherDotsNumUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.afOtherDotsNumUpDown.Name = "afOtherDotsNumUpDown";
            this.afOtherDotsNumUpDown.Size = new System.Drawing.Size(42, 23);
            this.afOtherDotsNumUpDown.TabIndex = 3;
            // 
            // afHighArcanaLabel
            // 
            this.afHighArcanaLabel.AutoSize = true;
            this.afHighArcanaLabel.Location = new System.Drawing.Point(12, 9);
            this.afHighArcanaLabel.Name = "afHighArcanaLabel";
            this.afHighArcanaLabel.Size = new System.Drawing.Size(88, 15);
            this.afHighArcanaLabel.TabIndex = 4;
            this.afHighArcanaLabel.Text = "Highest Arcana";
            // 
            // afOtherArcanaLabel
            // 
            this.afOtherArcanaLabel.AutoSize = true;
            this.afOtherArcanaLabel.Location = new System.Drawing.Point(187, 10);
            this.afOtherArcanaLabel.Name = "afOtherArcanaLabel";
            this.afOtherArcanaLabel.Size = new System.Drawing.Size(77, 15);
            this.afOtherArcanaLabel.TabIndex = 5;
            this.afOtherArcanaLabel.Text = "Other Arcana";
            // 
            // afDotsLabel1
            // 
            this.afDotsLabel1.AutoSize = true;
            this.afDotsLabel1.Location = new System.Drawing.Point(139, 10);
            this.afDotsLabel1.Name = "afDotsLabel1";
            this.afDotsLabel1.Size = new System.Drawing.Size(31, 15);
            this.afDotsLabel1.TabIndex = 6;
            this.afDotsLabel1.Text = "Dots";
            // 
            // afDotsLabel2
            // 
            this.afDotsLabel2.AutoSize = true;
            this.afDotsLabel2.Location = new System.Drawing.Point(314, 9);
            this.afDotsLabel2.Name = "afDotsLabel2";
            this.afDotsLabel2.Size = new System.Drawing.Size(31, 15);
            this.afDotsLabel2.TabIndex = 7;
            this.afDotsLabel2.Text = "Dots";
            // 
            // afOtherArcanaComboBox
            // 
            this.afOtherArcanaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.afOtherArcanaComboBox.FormattingEnabled = true;
            this.afOtherArcanaComboBox.Items.AddRange(new object[] {
            "-none-",
            "Death",
            "Fate",
            "Forces",
            "Life",
            "Matter",
            "Mind",
            "Prime",
            "Space",
            "Spirit",
            "Time"});
            this.afOtherArcanaComboBox.Location = new System.Drawing.Point(187, 28);
            this.afOtherArcanaComboBox.Name = "afOtherArcanaComboBox";
            this.afOtherArcanaComboBox.Size = new System.Drawing.Size(121, 23);
            this.afOtherArcanaComboBox.Sorted = true;
            this.afOtherArcanaComboBox.TabIndex = 8;
            // 
            // ArcanaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(367, 56);
            this.Controls.Add(this.afOtherArcanaComboBox);
            this.Controls.Add(this.afDotsLabel2);
            this.Controls.Add(this.afDotsLabel1);
            this.Controls.Add(this.afOtherArcanaLabel);
            this.Controls.Add(this.afHighArcanaLabel);
            this.Controls.Add(this.afOtherDotsNumUpDown);
            this.Controls.Add(this.afHighestDotsNumUpDown);
            this.Controls.Add(this.afHighestArcanaComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArcanaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Arcana Selector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ArcanaForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.afHighestDotsNumUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.afOtherDotsNumUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox afHighestArcanaComboBox;
        private NumericUpDown afHighestDotsNumUpDown;
        private NumericUpDown afOtherDotsNumUpDown;
        private Label afHighArcanaLabel;
        private Label afOtherArcanaLabel;
        private Label afDotsLabel1;
        private Label afDotsLabel2;
        private ComboBox afOtherArcanaComboBox;
    }
}