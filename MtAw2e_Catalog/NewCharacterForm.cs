// NewCharacterForm
// A simple form that appears when creating a new player character. 
// Allows configuring the basic info of the character. 

namespace MtAw2e_Catalog
{
    public partial class NewCharacterForm : Form
    {
        public Character? returnChar;
        public NewCharacterForm()
        {
            InitializeComponent();
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Font;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ncfNameTextBox.Text)) { return; }
            returnChar = new(ncfNameTextBox.Text,
                ncfFirstRulingComboBox.SelectedIndex == -1 ? 0 : ncfFirstRulingComboBox.SelectedIndex,
                ncfSecondRulingComboBox.SelectedIndex == -1 ? 0 : ncfSecondRulingComboBox.SelectedIndex,
                ncfThirdRulingComboBox.SelectedIndex == -1 ? 0 : ncfThirdRulingComboBox.SelectedIndex
                )
            {
                Gnosis = (int)ncfGnosisNumUpDown.Value,
                Wisdom = (int)ncfWisdomNumUpDown.Value,
                AimedOffense = (int)ncfOffenseNumUpDown.Value
            };
            this.Close();
        }

        private void NewCharacterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = returnChar == null ? DialogResult.Cancel : DialogResult.OK;
        }
    }
}
