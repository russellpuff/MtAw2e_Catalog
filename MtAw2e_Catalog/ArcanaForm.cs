// ArcanaForm
// File defines a small form that appears when choosing the Arcana for a spell. 
namespace MtAw2e_Catalog
{
    public partial class ArcanaForm : Form
    {
        public string arcanaReturnValues = "";
        public ArcanaForm()
        {
            InitializeComponent();
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Font;
            afHighestArcanaComboBox.SelectedIndex = afOtherArcanaComboBox.SelectedIndex = 0;
        }

        // Interprets the values of the controls to construct a string to return to the caller. 
        private void ArcanaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool includeOther = afOtherArcanaComboBox.SelectedIndex != 0 && afOtherDotsNumUpDown.Value > 0;

            arcanaReturnValues = "(" + afHighestArcanaComboBox.Text + " ";
            for (int i = 1; i <= afHighestDotsNumUpDown.Value; ++i) {  arcanaReturnValues += "•"; }
            if (includeOther)
            {
                arcanaReturnValues += " + " + afOtherArcanaComboBox.Text + " ";
                for (int i = 1; i <= afOtherDotsNumUpDown.Value; ++i) { arcanaReturnValues += "•"; }
            }
            arcanaReturnValues += ")";
        }
    }
}
