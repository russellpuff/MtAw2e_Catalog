using Discord;
using Newtonsoft.Json;
using Color = Discord.Color;

namespace MtAw2e_Catalog
{
    public partial class MainForm : Form
    {
        private bool unsaved, deactivateEvent;
        private readonly ulong server;
        private readonly ulong channel;
        private List<Spell> spellList;
        private int listIndex = -1;
        public MainForm(ulong _server, ulong _channel)
        {
            server = _server;
            channel = _channel;
            InitializeComponent();

            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Font;

            spellList = new List<Spell>();
            LoadSpells();
            mfPracticeComboBox.SelectedIndex = mfFactorComboBox.SelectedIndex = 
                mfArcanaSelectorComboBox.SelectedIndex = mfDotsSelectorComboBox.SelectedIndex = 0;
            unsaved = deactivateEvent = false;
        }

        private void LoadSpells()
        {
            string path = Application.StartupPath + "/spells.json";
            if (File.Exists(path))
            {
#pragma warning disable CS8601, CS8604
                using (StreamReader sr = new(path))
                {
                    var json = sr.ReadToEnd();
                    spellList = JsonConvert.DeserializeObject<List<Spell>>(json);
                }
                if (spellList.Any()) { RefreshList(); }
#pragma warning restore CS8601, CS8604
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string path = Application.StartupPath + "/spells.json";
            // Write list to file.
            if (File.Exists(path)) { File.Delete(path); }
            using (StreamWriter sw = new(path))
            {
                var json = JsonConvert.SerializeObject(spellList);
                sw.Write(json);
            }
            // Program.cs has an indefinite task delay to keep the bot running.
            // This forces the program to terminate when the form closes, which otherwise is stopped by the delay. 
            this.DialogResult = DialogResult.OK;
            Application.Exit();
        }

        private void ArcanaTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            ArcanaForm af = new();
            af.ShowDialog();
            mfArcanaTextBox.Text = af.arcanaReturnValues;
        }

        private void ClearForm_Click(object? sender, EventArgs? e)
        {
            if (unsaved)
            {
                var result = MessageBox.Show("Warning: Unsaved changes, clear anyways?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) { return; }
            }

            mfNameTextBox.Text = mfArcanaTextBox.Text = mfSpellDescriptionTextBox.Text =
                 mfAddonReqTextBox1.Text = mfAddonReqTextBox2.Text = mfAddonReqTextBox3.Text =
                 mfAddonReqTextBox4.Text = mfAddonEffectTextBox1.Text = mfAddonEffectTextBox2.Text =
                 mfAddonEffectTextBox3.Text = mfAddonEffectTextBox4.Text = mfWithstandComboBox.Text = mfCostTextBox.Text = "";

            mfPracticeComboBox.SelectedIndex = mfFactorComboBox.SelectedIndex = 0;
            unsaved = false;
            deactivateEvent = true;
            mfSpellListBox.SelectedIndex = -1;
        }

        private void Textbox_TextChanged(object sender, EventArgs e)
        {
            unsaved = true;
        }

        private void SaveSpellButton_Click(object sender, EventArgs e)
        {
            if (!VerifyOkToSave())
            {
                MessageBox.Show("Error: Not all fields filled.", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Spell spell = new()
            {
                Name = mfNameTextBox.Text,
                Arcana = mfArcanaTextBox.Text,
                Description = mfSpellDescriptionTextBox.Text,
                Practice = mfPracticeComboBox.Text,
                PrimaryFactor = mfFactorComboBox.Text
            };

            if (!string.IsNullOrEmpty(mfAddonReqTextBox1.Text) && !string.IsNullOrEmpty(mfAddonEffectTextBox1.Text))
            {
                AddOn first = new()
                {
                    Requirement = mfAddonReqTextBox1.Text,
                    Effect = mfAddonEffectTextBox1.Text
                };
                spell.AddOns.Add(first);
            }
            if (!string.IsNullOrEmpty(mfAddonReqTextBox2.Text) && !string.IsNullOrEmpty(mfAddonEffectTextBox2.Text)) {
                AddOn second = new()
                {
                    Requirement = mfAddonReqTextBox2.Text,
                    Effect = mfAddonEffectTextBox2.Text
                };
                spell.AddOns.Add(second);
            }
            if (!string.IsNullOrEmpty(mfAddonReqTextBox3.Text) && !string.IsNullOrEmpty(mfAddonEffectTextBox3.Text))
            {
                AddOn third = new()
                {
                    Requirement = mfAddonReqTextBox3.Text,
                    Effect = mfAddonEffectTextBox3.Text
                };
                spell.AddOns.Add(third);
            }
            if (!string.IsNullOrEmpty(mfAddonReqTextBox4.Text) && !string.IsNullOrEmpty(mfAddonEffectTextBox4.Text))
            {
                AddOn forth = new()
                {
                    Requirement = mfAddonReqTextBox4.Text,
                    Effect = mfAddonEffectTextBox4.Text
                };
                spell.AddOns.Add(forth);
            }

            if (!string.IsNullOrEmpty(mfWithstandComboBox.Text)) { spell.Withstand = mfWithstandComboBox.Text; }
            if (!string.IsNullOrEmpty(mfCostTextBox.Text)) { spell.Cost = mfCostTextBox.Text; }

            spellList.Add(spell);
            spellList = spellList.OrderBy(s => s.Arcana).ThenBy(s=>s.Name).ToList();
            RefreshList();

            unsaved = false;
            ClearForm_Click(null, null);
        }

        private bool VerifyOkToSave()
        {
            return !string.IsNullOrEmpty(mfNameTextBox.Text) && !string.IsNullOrEmpty(mfArcanaTextBox.Text) &&
                !string.IsNullOrEmpty(mfSpellDescriptionTextBox.Text);
        }

        private void RefreshList()
        {
            mfSpellListBox.Items.Clear();
            foreach (Spell s in spellList)
            {
                string[] arcana = s.Arcana.Split('+');
                bool validateArcana = arcana[0].Contains(mfArcanaSelectorComboBox.Text) || mfArcanaSelectorComboBox.SelectedIndex == 0;
                bool validateDots = arcana[0].Count(c => c == '•') == mfDotsSelectorComboBox.Text.Count(c => c == '•') || mfDotsSelectorComboBox.SelectedIndex == 0;
                if (validateArcana && validateDots)   
                {
                    string info = s.Name + " " + s.Arcana;
                    mfSpellListBox.Items.Add(info);
                }
            }
        }

        private void SendSpellButton_Click(object sender, EventArgs e)
        {
            var embedSend = new EmbedBuilder();
            embedSend.WithTitle(mfNameTextBox.Text + " " + mfArcanaTextBox.Text);

            string send = "**Practice**: " + mfPracticeComboBox.Text +
                "\n**Primary Factor**: " + mfFactorComboBox.Text;

            if (!string.IsNullOrEmpty(mfCostTextBox.Text)) { send += "\n**Cost**: " + mfCostTextBox.Text; }
            if (!string.IsNullOrEmpty(mfWithstandComboBox.Text)) { send += "\n**Withstand**: " + mfWithstandComboBox.Text; }

            send += "\n\n" + mfSpellDescriptionTextBox.Text;

            if (!string.IsNullOrEmpty(mfAddonReqTextBox1.Text) && !string.IsNullOrEmpty(mfAddonEffectTextBox1.Text))
            {
                embedSend.AddField(mfAddonReqTextBox1.Text, mfAddonEffectTextBox1.Text);

                if (!string.IsNullOrEmpty(mfAddonReqTextBox2.Text) && !string.IsNullOrEmpty(mfAddonEffectTextBox2.Text))
                {
                    embedSend.AddField(mfAddonReqTextBox2.Text, mfAddonEffectTextBox2.Text);
                }
                if (!string.IsNullOrEmpty(mfAddonReqTextBox3.Text) && !string.IsNullOrEmpty(mfAddonEffectTextBox3.Text))
                {
                    embedSend.AddField(mfAddonReqTextBox3.Text, mfAddonEffectTextBox3.Text);
                }
                if (!string.IsNullOrEmpty(mfAddonReqTextBox4.Text) && !string.IsNullOrEmpty(mfAddonEffectTextBox4.Text))
                {
                    embedSend.AddField(mfAddonReqTextBox4.Text, mfAddonEffectTextBox4.Text);
                }
            }

            embedSend.WithDescription(send);
            embedSend.WithColor(ChooseEmbedColor(mfArcanaTextBox.Text));

            using (EmbedSender es = new(embedSend, server, channel))
            { es.SendEmbed(); }
            
        }

        private static Color ChooseEmbedColor(string arcana)
        {
            arcana = arcana.Substring(1, 4);
            return arcana switch
            {
                "Deat" => Color.DarkerGrey,
                "Fate" => Color.Gold,
                "Spac" => Color.Purple,
                "Mind" => Color.Magenta,
                "Time" => Color.Teal,
                "Forc" => Color.DarkRed,
                "Matt" => Color.DarkGreen,
                "Spir" => Color.Blue,
                "Prim" => Color.Orange,
                "Life" => new Color(255,255,158),
                _ => new Color(255,255,255)
            };
        }

        private void DeleteSpellButton_Click(object sender, EventArgs e)
        {
            spellList.RemoveAt(listIndex);
            RefreshList();
        }

        private void CastSpellButton_Click(object sender, EventArgs e)
        {
            Spell? s = spellList.Find(s => s.Name == mfNameTextBox.Text);
            if (s == null)
            {
                string msg = "Error casting this spell. It's likely due to a mismatch between the name in the text box " +
                    "and the name in the spell list. Make sure this spell is saved before you try to cast it.";
                MessageBox.Show(msg, "Spell error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SpellForm sf = new(s, server, channel);
            sf.ShowDialog();
        }

        private void SpellSearchComboBoxes_SelectedIndexChanged(object sender, EventArgs e)
        { RefreshList(); }

        private void SpellListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deactivateEvent) { deactivateEvent = false; return; }
            if (unsaved)
            {
                var result = MessageBox.Show("You have unsaved changes. Continue?", "Unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    if (listIndex != -1)
                    {
                        deactivateEvent = true;
                        mfSpellListBox.SelectedIndex = listIndex;
                    }
                    return;
                }
            }

            string sp = mfSpellListBox.SelectedItems[0].ToString() ?? "";
            Spell? selectedSpell = spellList.Find(s => sp.Contains(s.Name));
            if (selectedSpell == null) 
            {

                string msg = "Error loading spell. A match wasn't found in spells.json.";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            mfNameTextBox.Text = selectedSpell.Name;
            mfArcanaTextBox.Text = selectedSpell.Arcana;
            mfSpellDescriptionTextBox.Text = selectedSpell.Description;
            mfPracticeComboBox.Text = selectedSpell.Practice;
            mfFactorComboBox.Text = selectedSpell.PrimaryFactor;
            mfWithstandComboBox.Text = selectedSpell.Withstand;
            mfCostTextBox.Text = selectedSpell.Cost;

            mfAddonReqTextBox1.Text = mfAddonReqTextBox2.Text = mfAddonReqTextBox3.Text = mfAddonReqTextBox4.Text =
               mfAddonEffectTextBox1.Text = mfAddonEffectTextBox2.Text = mfAddonEffectTextBox3.Text = mfAddonEffectTextBox4.Text = "";

            if (selectedSpell.AddOns.Any())
            {
                mfAddonReqTextBox1.Text = selectedSpell.AddOns[0].Requirement;
                mfAddonEffectTextBox1.Text = selectedSpell.AddOns[0].Effect;

                if (selectedSpell.AddOns.Count > 1)
                {
                    mfAddonReqTextBox2.Text = selectedSpell.AddOns[1].Requirement;
                    mfAddonEffectTextBox2.Text = selectedSpell.AddOns[1].Effect;
                }
                if (selectedSpell.AddOns.Count > 2)
                {
                    mfAddonReqTextBox3.Text = selectedSpell.AddOns[2].Requirement;
                    mfAddonEffectTextBox3.Text = selectedSpell.AddOns[2].Effect;
                }
                if (selectedSpell.AddOns.Count > 3)
                {
                    mfAddonReqTextBox4.Text = selectedSpell.AddOns[3].Requirement;
                    mfAddonEffectTextBox4.Text = selectedSpell.AddOns[3].Effect;
                }
            }

            unsaved = false;
        }
    }
}