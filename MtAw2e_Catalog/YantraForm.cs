// YantraForm
// Allows the user to select from or create a list of yantras for spellcasting. 

namespace MtAw2e_Catalog
{
    public partial class YantraForm : Form
    {
        private List<Yantra> customYantras;
        public YantraForm(ref List<Yantra> _c, ref List<Yantra> _d)
        {
            InitializeComponent();
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Font;
            customYantras = _c;

            foreach (Yantra yantra in _d)
            { yfDefaultYantrasListBox.Items.Add(yantra.DisplayName); }
            RefreshList();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string disp = yfYantraNameTextBox.Text + " (+" + yfDiceBonusNumUpDown.Value.ToString() + ")";
            customYantras.Add(new(yfYantraNameTextBox.Text, disp, (int)yfDiceBonusNumUpDown.Value, (int)yfParadoxPenaltyNumUpDown.Value));
            yfYantraNameTextBox.Text = "";
            yfDiceBonusNumUpDown.Value = yfParadoxPenaltyNumUpDown.Value = 0;
            RefreshList();
        }

        private void RefreshList()
        {
            yfCustomYantrasListBox.Items.Clear();
            foreach (Yantra yantra in customYantras)
            { yfCustomYantrasListBox.Items.Add(yantra.DisplayName); }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            int idx = yfCustomYantrasListBox.SelectedIndex;
            if (idx >= 0)
            {
                customYantras.RemoveAt(idx);
                RefreshList();
            }
            
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            string msg = "The default list contains all Yantras that any character can use except Soul Stones " +
                "which are elaborated on below.\nThe dice bonus for most Yantras scales from +1 to +3, all Tool Yantras " +
                "have a default bonus of +1 unless they've been enhanced.\nIf a Tool is a Dedicated Tool, it penalize the Paradox dice pool " +
                "by -2.\n\nSoul Stones have different properties depending on circumstance.\nIf a Soul Stone is used by its creator, " +
                "it provides a +2 dice bonus and serves as a Dedicated Tool, stacking with the mage's normal Dedicated Tool for -2 Paradox dice." +
                "\nIf a mage makes their own stone into a Dedicated Tool, the Paradox penalty increases to -3, but Paradoxes incurred using" +
                "this cannot be contained.\nIf a Soul Stone is used by a mage other than the creator, the bonus is +2 or +3 if the creator's " +
                "Gnosis is higher than the user.\n\nGive custom yantras unique names, some yantras can only be used in certain applications, " +
                "consider either noting in the name what those applications are, or in another document for quick reference.\n\nYantras that " +
                "provide special effects like free Reach or free factor levels (see: Storytelling Yantras) are not reflected in this interface, " +
                "Instead, any free bonuses should be reflected by using the Extra Dice number spinner on the spellcasting sheet to " +
                "\"refund\" the dice spent on these free bonuses.";
            MessageBox.Show(msg, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
