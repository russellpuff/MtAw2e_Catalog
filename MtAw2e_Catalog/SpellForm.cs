// SpellForm
// Allows the user to configure the specific settings of their spell. If they "cast" it, the results are calculated
// and the companion Discord bot will post it. 

using Newtonsoft.Json;
using CofDDice;
using System.Diagnostics.CodeAnalysis;
using Discord;
using Color = System.Drawing.Color;

namespace MtAw2e_Catalog
{
    public partial class SpellForm : Form
    {
        private readonly ulong server;
        private readonly ulong channel;
        private List<Character> profiles;
        private Character currentCharacter;
        private List<Yantra> defaultYantras;
        private readonly Spell spell;
        private bool disableEvents;
        private bool advDuration;
        private bool advScale;
        private bool roteSpell;
        private bool itsOkayYouCanRunThisEventWithoutWorryingAboutUnsavedChanges;
        // Bools to enable or disable the cast spell buttons.
        private bool withstandOK;
        private bool yantrasOK;
        private bool saveOK;
        // Variables for the final result
        private int freeReach;
        private int arcanaDots;
        private int reaches;
        private int mana;
        private int dice;
        private int paradoxTotal; // Before paradox penalties
        private int paradoxActual; // After paradox penalties
        #region Setup
        public SpellForm(Spell _spell, ulong _server, ulong _channel)
        {
            InitializeComponent();

            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Font;

            spell = _spell;
            server = _server;
            channel = _channel;
            string path = Application.StartupPath + "/defaultyantras.json";
            if (File.Exists(path))
            {
                using StreamReader sr = new(path);
#pragma warning disable CS8601
                var json = sr.ReadToEnd();
                defaultYantras = JsonConvert.DeserializeObject<List<Yantra>>(json);
#pragma warning restore CS8601
            }
            if (defaultYantras == null) { defaultYantras = new(); this.Close(); }
            SetupDefaults();
            profiles = new();
            LoadProfiles();
#nullable disable
            List<RadioButton> pfs = new() { sfDurationFactorRadio, sfScaleFactorRadio, sfPotencyFactorRadio };
            pfs.Find(r => r.Name.Contains(spell.PrimaryFactor)).Checked = true;
#nullable enable
            saveOK = true;
            sfCastNormalButton.Enabled = sfCastAimedButton.Enabled = false; // No yantras selected by default, force disable buttons on form load. 
        }

        // Method to set default selections when constructor loads. 
        private void SetupDefaults()
        {
            disableEvents = true;

            string desc = "";
            if (!string.IsNullOrEmpty(spell.Withstand))
            { desc += "Withstand: " + spell.Withstand + Environment.NewLine + Environment.NewLine; }
            desc += spell.Description;
            sfSpellDescriptionTextBox.Text = desc;

            reaches = mana = dice = paradoxTotal = paradoxActual = 0;
            sfTouchRangeRadio.Checked = true;
            sfInstantCastRadio.Checked = true;
            sfSDuration1Radio.Checked = true;
            sfSScale1Radio.Checked = true;
            sfCastImprovisedRadio.Checked = true;
            sfRollAgainComboBox.SelectedIndex = 0;
            sfSpellNameLabel.Text = spell.Name + " " + spell.Arcana;
            advDuration = advScale = roteSpell = false;

            if (spell.AddOns.Count > 0)
            {
                sfAddon1TextBox.Text = spell.AddOns[0].Requirement + ": " + spell.AddOns[0].Effect;
                sfIncludeAddon1Check.Enabled = true;
            }
            if (spell.AddOns.Count > 1)
            {
                sfAddon2TextBox.Text = spell.AddOns[1].Requirement + ": " + spell.AddOns[1].Effect;
                sfIncludeAddon2Check.Enabled = true;
            }
            if (spell.AddOns.Count > 2)
            {
                sfAddon3TextBox.Text = spell.AddOns[2].Requirement + ": " + spell.AddOns[2].Effect;
                sfIncludeAddon3Check.Enabled = true;
            }
            if (spell.AddOns.Count > 3)
            {
                sfAddon4TextBox.Text = spell.AddOns[3].Requirement + ": " + spell.AddOns[3].Effect;
                sfIncludeAddon4Check.Enabled = true;
            }

            disableEvents = false;
        }
        #endregion
        #region ProfileStuff
        private void NewProfileButton_Click(object sender, EventArgs e)
        {
            NewCharacterForm ncf = new();
            if (ncf.ShowDialog() == DialogResult.OK)
            {
#nullable disable
                profiles.Add(ncf.returnChar);
                sfProfileComboBox.Items.Add(ncf.returnChar.Name);
#nullable enable
            }
            sfProfileComboBox.SelectedIndex = sfProfileComboBox.Items.Count - 1;
            UnsavedChanges();
        }

        [MemberNotNull(nameof(currentCharacter))]
        private void LoadProfiles()
        {
            string path = Application.StartupPath + "/profiles/";
            string[] files = Directory.GetFiles(path, "*.json");

            foreach (string file in files)
            {
                try
                {
                    using StreamReader sr = new(file);
                    var json = sr.ReadToEnd();
                    Character? ch = JsonConvert.DeserializeObject<Character>(json);
                    if (ch == null) { throw new NullReferenceException(); }
                    profiles.Add(ch);
                    sfProfileComboBox.Items.Add(ch.Name);
                }
                catch
                {
                    string msg = file.ToString() + " was corrupted and could not be loaded.";
                    MessageBox.Show(msg, "Load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                } // Ignore broken file, try the rest. 

            }
            if (profiles.Count > 0)
            {
                currentCharacter = profiles[0];
                sfProfileComboBox.SelectedIndex = 0;
            }
            else
            { // No character files or all broken = default character. 
                currentCharacter ??= new("John Darkfall", 0, 0, 0);
            }
        }
        private void RefreshYantraList()
        {
            List<ComboBox> yan = new()
            {
                sfYantra1ComboBox, sfYantra2ComboBox, sfYantra3ComboBox,
                    sfYantra4ComboBox, sfYantra5ComboBox, sfYantra6ComboBox
            };
            foreach (ComboBox y in yan)
            {
                y.Items.Clear();
                foreach (Yantra yantra in currentCharacter.Yantras)
                {
                    y.Items.Add(yantra.DisplayName);
                }
                foreach (Yantra yantra in defaultYantras)
                {
                    y.Items.Add(yantra.DisplayName);
                }
            }
        }
        private void ProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            disableEvents = true;
            // Load Profile
            currentCharacter = profiles[sfProfileComboBox.SelectedIndex];
            sfDeathNumUpDown.Value = currentCharacter.DeathDots;
            sfFateNumUpDown.Value = currentCharacter.FateDots;
            sfForcesNumUpDown.Value = currentCharacter.ForcesDots;
            sfLifeNumUpDown.Value = currentCharacter.LifeDots;
            sfMatterNumUpDown.Value = currentCharacter.MatterDots;
            sfMindNumUpDown.Value = currentCharacter.MindDots;
            sfPrimeNumUpDown.Value = currentCharacter.PrimeDots;
            sfSpaceNumUpDown.Value = currentCharacter.SpaceDots;
            sfSpiritNumUpDown.Value = currentCharacter.SpiritDots;
            sfTimeNumUpDown.Value = currentCharacter.TimeDots;
            sfGnosisNumUpDown.Value = currentCharacter.Gnosis;
            sfWisdomNumUpDown.Value = currentCharacter.Wisdom;
            sfAimedOffNumUpDown.Value = currentCharacter.AimedOffense;
            arcanaDots = WhatArcana(spell.Arcana.Substring(1, 4), false);

            sfTimeBottleCheck.Checked = sfEverywhereCheck.Checked = sfPermanenceCheck.Checked = false; // Avoids exploit.

            List<CheckBox> rulearc = new()
            {
                sfDeathRulingCheck, sfFateRulingCheck, sfForcesRulingCheck, sfLifeRulingCheck, sfMatterRulingCheck,
                sfMindRulingCheck, sfPrimeRulingCheck, sfSpaceRulingCheck, sfSpiritRulingCheck, sfTimeRulingCheck
            };
            foreach (CheckBox cb in rulearc) { cb.Checked = false; }
            disableEvents = false;
            itsOkayYouCanRunThisEventWithoutWorryingAboutUnsavedChanges = true;
            for (int i = 0; i < 3; ++i) // I want the checkchanged event to fire without marking the form as unsaved...
            { if (currentCharacter.RulingArcana[i] > 0) { rulearc[currentCharacter.RulingArcana[i] - 1].Checked = true; } }
            itsOkayYouCanRunThisEventWithoutWorryingAboutUnsavedChanges = false;

            sfTimeBottleCheck.Enabled = currentCharacter.TimeDots >= 4;
            sfEverywhereCheck.Enabled = currentCharacter.SpaceDots >= 4;
            sfPermanenceCheck.Enabled = currentCharacter.MatterDots >= 4;

            List<ComboBox> yan = new()
            {
                sfYantra1ComboBox, sfYantra2ComboBox, sfYantra3ComboBox,
                    sfYantra4ComboBox, sfYantra5ComboBox, sfYantra6ComboBox
            };

            int yanmax = (int)Math.Ceiling(currentCharacter.Gnosis / 2F); // Yantra limits
            for (int i = 0; i < 6; ++i) { yan[i].Enabled = i <= yanmax; }

            CalculateFreeReach();
            RefreshYantraList();
            RefreshLabels();
        }
        private void SaveProfileButton_Click(object sender, EventArgs e)
        {
            string charName = currentCharacter.Name;

            currentCharacter.Gnosis = (int)sfGnosisNumUpDown.Value;
            currentCharacter.Wisdom = (int)sfWisdomNumUpDown.Value;
            currentCharacter.DeathDots = (int)sfDeathNumUpDown.Value;
            currentCharacter.FateDots = (int)sfFateNumUpDown.Value;
            currentCharacter.ForcesDots = (int)sfForcesNumUpDown.Value;
            currentCharacter.MatterDots = (int)sfMatterNumUpDown.Value;
            currentCharacter.MindDots = (int)sfMindNumUpDown.Value;
            currentCharacter.PrimeDots = (int)sfPrimeNumUpDown.Value;
            currentCharacter.LifeDots = (int)sfLifeNumUpDown.Value;
            currentCharacter.SpaceDots = (int)sfSpaceNumUpDown.Value;
            currentCharacter.SpiritDots = (int)sfSpiritNumUpDown.Value;
            currentCharacter.TimeDots = (int)sfTimeNumUpDown.Value;
            currentCharacter.AimedOffense = (int)sfAimedOffNumUpDown.Value;

            List<CheckBox> rulearc = new()
            {
                sfDeathRulingCheck, sfFateRulingCheck, sfForcesRulingCheck, sfLifeRulingCheck, sfMatterRulingCheck,
                sfMindRulingCheck, sfPrimeRulingCheck, sfSpaceRulingCheck, sfSpiritRulingCheck, sfTimeRulingCheck
            };

            var chek = rulearc.FindAll(r => r.Checked);
            for (int i = 0; i < 3; ++i)
            { // If fewer than 3 checked boxes, use the default return value (0) in WhatArcana.
                string bar = chek.Count > i ? chek[i].Name.Substring(2, 4) : "pepis lol";
                currentCharacter.RulingArcana[i] = WhatArcana(bar, true);
            }

            string path = Application.StartupPath + "/profiles/" + charName.Replace(' ', '_') + ".json";
            if (File.Exists(path)) { File.Delete(path); }
            using (StreamWriter sw = new(path))
            {
                var json = JsonConvert.SerializeObject(currentCharacter);
                sw.Write(json);
            }

            sfSaveProfileButton.ForeColor = Color.Black;
            saveOK = true;
            EnableDisableCastButtons();

            sfProfileComboBox.Items.Clear();
            LoadProfiles();
            int idx = sfProfileComboBox.Items.IndexOf(charName);
            sfProfileComboBox.SelectedIndex = idx;
        }
        private void CalculateFreeReach()
        {
            int spellDots = 0;
            string[] dots = spell.Arcana.Split(' ');
            foreach (char d in dots[1])
            { if (d == '•') { ++spellDots; } } // Extract dots from spell name. Only cares about leftmost (highest) Arcana. 
            arcanaDots = WhatArcana(spell.Arcana.Substring(1, 4), false);
            freeReach = roteSpell ? 6 - spellDots : arcanaDots - spellDots + 1;
            if (freeReach < 0) { freeReach = 0; }
        }

        private int WhatArcana(string s, bool id)
        {
            return s switch
            { // CalculateFreeReach needs the character's dots, Save_Click needs an id number. This method provides both.
                "Deat" => id ? 1 : currentCharacter.DeathDots,
                "Fate" => id ? 2 : currentCharacter.FateDots,
                "Forc" => id ? 3 : currentCharacter.ForcesDots,
                "Life" => id ? 4 : currentCharacter.LifeDots,
                "Matt" => id ? 5 : currentCharacter.MatterDots,
                "Mind" => id ? 6 : currentCharacter.MindDots,
                "Prim" => id ? 7 : currentCharacter.PrimeDots,
                "Spac" => id ? 8 : currentCharacter.SpaceDots,
                "Spir" => id ? 9 : currentCharacter.SpiritDots,
                "Time" => id ? 10 : currentCharacter.TimeDots,
                _ => 0
            };
        }
        #endregion
        #region LabelCalculations
        private void RefreshLabels()
        {
            CalculateDice();
            sfFinalDiceLabel.Text = dice.ToString() + " Dice";
            CalculateMana();
            sfFinalManaLabel.Text = mana.ToString() + " Mana";
            CalculateCurrentReaches();
            sfFinalReachLabel.Text = reaches.ToString() + "/" + freeReach.ToString() + " Reach";
            CalculateParadoxDice();
            sfFinalParadoxLabel.Text = "Paradox Dice: " + paradoxActual.ToString();
        }
        private void CalculateMana()
        {
            mana = 0;
            mana += (int)sfParadoxManaNumUpDown.Value;

            if (spell.Cost.Contains("Mana"))
            {
                bool m = Int32.TryParse(spell.Cost[0].ToString(), out int manacost);
                if (m) { mana += manacost; } // If it doesn't understand what the final cost is supposed to be, it ignores it. 
                // The player can correct that in the game itself. 
            }

            List<CheckBox> addons = new() { sfIncludeAddon1Check, sfIncludeAddon2Check, sfIncludeAddon3Check, sfIncludeAddon4Check };
            int i = 0;
            foreach (AddOn ao in spell.AddOns)
            {
                if (addons[i].Checked)
                {
                    int idx = ao.Requirement.IndexOf("Mana") - 2;
                    if (idx < 0) { ++i; continue; }
                    mana += ao.Requirement[idx] - '0';
                }
                ++i;
            }

            if (sfTimeBottleCheck.Checked && sfInstantCastRadio.Checked) { ++mana; }
            if (sfEverywhereCheck.Checked && advScale) { ++mana; }
            if (sfPermanenceCheck.Checked && advDuration) { ++mana; }

            // Mana penalty outside of ruling arcana. Rotes and praxis ignore. 
            if (sfCastImprovisedRadio.Checked)
            {
                List<CheckBox> rulearc = new()
                {
                    sfDeathRulingCheck, sfFateRulingCheck, sfForcesRulingCheck, sfLifeRulingCheck, sfMatterRulingCheck,
                    sfMindRulingCheck, sfPrimeRulingCheck, sfSpaceRulingCheck, sfSpiritRulingCheck, sfTimeRulingCheck
                };
                string arcana = spell.Arcana.Substring(1, 4);
#nullable disable
                if (!rulearc.Find(r => r.Name.Contains(arcana)).Checked) { ++mana; }
#nullable enable
            }
        }
        private void CalculateCurrentReaches()
        {
            reaches = 0;
            reaches += (int)sfExtraReachNumUpDown.Value;
            reaches += sfAdvancedPotencyCheck.Checked ? 1 : 0;
            reaches += sfSensoryRangeRadio.Checked ? 1 : 0;
            reaches += sfRemoteRangeRadio.Checked ? 2 : 0;
            reaches += sfSympatheticRangeRadio.Checked ? 1 : 0;
            reaches += sfTemporalRangeRadio.Checked ? 1 : 0;
            reaches += sfADuration6Radio.Checked ? 1 : 0; // Indefinite duration

            if (!sfTimeBottleCheck.Checked && sfInstantCastRadio.Checked) { ++reaches; }
            if (!sfEverywhereCheck.Checked && advScale) { ++reaches; }
            if (!sfPermanenceCheck.Checked && advDuration) { ++reaches; }

            // Spells over limit
            if (sfActiveSpellsNumUpDown.Value > currentCharacter.Gnosis)
            { reaches += (int)sfActiveSpellsNumUpDown.Value - currentCharacter.Gnosis; }
#nullable disable
            // Switching primary factor
            List<RadioButton> pfs = new() { sfDurationFactorRadio, sfScaleFactorRadio, sfPotencyFactorRadio };
            if (pfs.Find(r => r.Name.Contains(spell.PrimaryFactor)).Checked == false)
            { ++reaches; }
#nullable enable
            // Reaches in addons
            List<CheckBox> addons = new() { sfIncludeAddon1Check, sfIncludeAddon2Check, sfIncludeAddon3Check, sfIncludeAddon4Check };
            int i = 0;
            foreach (AddOn ao in spell.AddOns)
            {
                if (addons[i].Checked)
                {
                    int idx = ao.Requirement.IndexOf("Reach") - 2;
                    if (idx < 0) { ++i; continue; }
                    reaches += ao.Requirement[idx] - '0';
                }
                ++i;
            }
        }
        private void CalculateParadoxDice()
        {
            paradoxTotal = paradoxActual = (int)sfAddlParadoxNumUpDown.Value;
            int mult = (int)Math.Ceiling(currentCharacter.Gnosis / 2F); // Paradox dice per reach over limit.
            if (reaches > freeReach)
            { // Paradox total vs actual lets the application know when to skip rolling paradox and when to roll a chance die.
                paradoxTotal += (reaches - freeReach) * mult;
                paradoxActual += paradoxTotal - (int)sfParadoxManaNumUpDown.Value;

                List<ComboBox> yan = new()
                {
                sfYantra1ComboBox, sfYantra2ComboBox, sfYantra3ComboBox,
                    sfYantra4ComboBox, sfYantra5ComboBox, sfYantra6ComboBox
                };
                foreach (ComboBox y in yan)
                {
                    if (y.SelectedIndex >= 0)
                    {
                        Yantra get = currentCharacter.Yantras.Find(o => o.DisplayName == y.Text);
                        if (get.Description == null) { get = defaultYantras.Find(o => o.DisplayName == y.Text); }
                        paradoxActual += get.ParadoxPenalty;
                    }
                }
                if (paradoxActual < 0) { paradoxActual = 0; }
            }
        }
        private void CalculateDice()
        {
            dice = currentCharacter.Gnosis + arcanaDots + (int)sfExtraDiceNumUpDown.Value;
            int totalPenalty = 0;

            // -2 dice per potency. If p. factor is potency, grants a higher threshold before dice penalties. 
            if (sfPotencyFactorRadio.Checked)
            {
                int foo = 2 * ((int)sfPotencyNumUpDown.Value - arcanaDots);
                if (foo < 0) { foo = 0; }
                totalPenalty -= foo;
            }
            else
            { totalPenalty -= 2 * ((int)sfPotencyNumUpDown.Value - 1); }

            List<RadioButton> duration = new()
            {
                sfSDuration1Radio, sfSDuration2Radio, sfSDuration3Radio, sfSDuration4Radio, sfSDuration5Radio,
                sfADuration1Radio, sfADuration2Radio, sfADuration3Radio, sfADuration4Radio, sfADuration5Radio, sfADuration6Radio
            };
            List<RadioButton> scale = new() {
                sfSScale1Radio, sfSScale2Radio, sfSScale3Radio, sfSScale4Radio, sfSScale5Radio,
                sfAScale1Radio, sfAScale2Radio, sfAScale3Radio, sfAScale4Radio, sfAScale5Radio, sfAScale6Radio
            };
            List<Label> durlab = new() {
                sfSDDiceCount1Label, sfSDDiceCount2Label, sfSDDiceCount3Label, sfSDDiceCount4Label, sfSDDiceCount5Label,
                sfADDiceCount1Label,sfADDiceCount2Label,sfADDiceCount3Label,sfADDiceCount4Label, sfADDiceCount5Label, sfADDiceCount6Label
            };
            List<Label> scalab = new()
            {
                sfSSDiceCount1Label, sfSSDiceCount2Label, sfSSDiceCount3Label, sfSSDiceCount4Label, sfSSDiceCount5Label,
                sfASDiceCount1Label, sfASDiceCount2Label, sfASDiceCount3Label, sfASDiceCount4Label, sfASDiceCount5Label, sfASDiceCount6Label
            };

            int idx = duration.FindIndex(d => d.Checked);
            totalPenalty += Int32.Parse(durlab[idx].Text);
            idx = scale.FindIndex(d => d.Checked);
            totalPenalty += Int32.Parse(scalab[idx].Text);

            int yantraDice = 0;
            List<ComboBox> yan = new()
            {
                sfYantra1ComboBox, sfYantra2ComboBox, sfYantra3ComboBox,
                    sfYantra4ComboBox, sfYantra5ComboBox, sfYantra6ComboBox
            };
            foreach (ComboBox y in yan)
            {
                if (y.SelectedIndex >= 0)
                { // Looks for the value of the yantra based on which list it might be in (custom always first, then default)
                    if (y.SelectedIndex >= currentCharacter.Yantras.Count) { yantraDice += defaultYantras[y.SelectedIndex].DiceValue; }
                    else { yantraDice += currentCharacter.Yantras[y.SelectedIndex].DiceValue; }
                }
            }
            if (yantraDice + totalPenalty > 5) { yantraDice = 5; } // Yantra dice cannot be higher than +5 after dice penalties. 

            dice += yantraDice + totalPenalty;
        }
        #endregion
        #region CastSpell
        private void CastAimedButton_Click(object sender, EventArgs e)
        { CastSpell(true); }

        private void CastNormalButton_Click(object sender, EventArgs e)
        { CastSpell(false); }

        private async void CastSpell(bool aimed)
        {
            bool containParadox = false;
            DieRoller pairOfDocks = new(paradoxActual, false, 10);
            Tuple<string, int> paradoxResult = new(pairOfDocks.Roll(), pairOfDocks.successes);
            // Give user the opportunity to contain a paradox before posting results, since Paradox is always rolled first. 
            if (paradoxResult.Item2 > 0 && paradoxTotal > 0)
            {
                string msg = "Your paradox roll resulted in " + paradoxResult.Item2 + " successes, do you want to contain it?";
                var r = MessageBox.Show(msg, "Paradox", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                containParadox = r == DialogResult.Yes;
            }

            bool castSpell = true;
            if (aimed) { castSpell = AimSpell(); } // If you miss while aiming, don't bother actaully rolling for spellcasting.

            if (castSpell)
            {
                var spellResult = new EmbedBuilder();
                string title = currentCharacter.Name + " casts " + spell.Name + " " + spell.Arcana;
                spellResult.WithTitle(title);
                DieRoller spl = new(dice, sfRoteCheck.Checked, (10 - sfRollAgainComboBox.SelectedIndex));

                string result = spl.Roll();
                while (spl.successes == 0) { result = spl.Roll(); }
                if ((spl.successes > 3 && sfCastPraxisRadio.Checked) || spl.successes > 5)
                {
                    result += "\n\nThe spell was an exceptional success!";
                    result += sfCastPraxisRadio.Checked ? " (Praxis)" : "";
                }
                else if (spl.successes == 0) { result += "\n\nIt failed."; }

                spellResult.WithDescription(result);
                spellResult.WithColor(new Discord.Color(0, 0, 0));
                using (EmbedSender es = new(spellResult, server, channel))
                { es.SendEmbed(); }
                await Task.Delay(1000);

                if (spl.successes > 0)
                {
                    using (EmbedSender es = new(SendSpellToChat(), server, channel))
                    { es.SendEmbed(); }
                    await Task.Delay(1000);
                }
            }

            // Paradox result
            if (paradoxTotal == 0) { return; } // paradoxTotal being zero means no paradox is rolled.
            // If paradoxTotal is greater than zero but paradoxActual is zero, a chance die is rolled. 
            var embedParadox = new EmbedBuilder();
            embedParadox.WithTitle(spell.Name + " - Paradox Roll");
            string pResult = paradoxResult.Item1;

            if (containParadox)
            {
                DieRoller cont = new(currentCharacter.Wisdom, false, 10);
                pResult += "\n\n" + currentCharacter.Name + " attempts to contain the Paradox...\n";
                pResult += cont.Roll();

                if (cont.successes >= paradoxResult.Item2)
                {
                    pResult += "\n\nThe Paradox is fully contained! " + currentCharacter.Name + " takes " +
                        paradoxResult.Item2 + " points of bashing damage.";
                }
                else if (cont.successes == 0)
                {
                    pResult += "\n\nThe Paradox is not contained.";
                }
                else
                {
                    pResult += "\n\nThe Paradox is partially contained. " + currentCharacter.Name + " takes " +
                        cont.successes + " points of bashing damage and the Storyteller has " +
                        (paradoxResult.Item2 - cont.successes) +
                        " successes remaining for Paradox reach.";
                }
            }

            embedParadox.WithDescription(pResult);
            embedParadox.WithColor(Discord.Color.Default);
            using (EmbedSender es = new(embedParadox, server, channel))
            { es.SendEmbed(); }
        }

        private bool AimSpell()
        {
            string title = currentCharacter.Name + " aims a spell...";
            int aimDice = (int)sfAimedOffNumUpDown.Value - (int)sfAimedDefNumUpDown.Value + currentCharacter.Gnosis;
            DieRoller aim = new(aimDice, false, 10);
            string result = aim.Roll();
            result += (aim.successes > 0) ? "\n\nIt hit! Rolling to cast the spell..." : "\n\nIt missed...";

            EmbedBuilder aimSpell = new();
            aimSpell.WithTitle(title);
            aimSpell.WithDescription(result);
            aimSpell.WithColor(new Discord.Color(255, 255, 255));
            SendAimToChat(aimSpell);

            return aim.successes > 0;
        }

        private async void SendAimToChat(EmbedBuilder aimSpell)
        {
            using (EmbedSender es = new(aimSpell, server, channel))
            { es.SendEmbed(); }
            await Task.Delay(1000);
        }

        private EmbedBuilder SendSpellToChat()
        {
            var embedSend = new EmbedBuilder();
            embedSend.WithTitle(spell.Name + " " + spell.Arcana);

            string addField = "With these addons:";
            string addOns = "";
            if (sfIncludeAddon1Check.Checked)
            { addOns += "• " + spell.AddOns[0].Effect + "\n"; }
            if (sfIncludeAddon2Check.Checked)
            { addOns += "• " + spell.AddOns[1].Effect + "\n"; }
            if (sfIncludeAddon3Check.Checked)
            { addOns += "• " + spell.AddOns[2].Effect + "\n"; }
            if (sfIncludeAddon4Check.Checked)
            { addOns += "• " + spell.AddOns[3].Effect; }

            if (sfIncludeAddon1Check.Checked || sfIncludeAddon2Check.Checked || sfIncludeAddon3Check.Checked || sfIncludeAddon4Check.Checked)
            { embedSend.AddField(addField, addOns); }

            int potencyReal = (int)sfPotencyNumUpDown.Value - (int)sfWithstandNumUpDown.Value;
            embedSend.AddField("Potency", potencyReal);
            embedSend.AddField("Duration", GetDuration());
            embedSend.AddField("Scale", GetScale());
            if (mana > 0) { embedSend.AddField("Mana Cost", mana); }
            embedSend.AddField("Yantras", GetYantras());
            embedSend.WithDescription(spell.Description);
            embedSend.WithColor(ChooseEmbedColor(spell.Arcana));
            return embedSend;
        }

        private static Discord.Color ChooseEmbedColor(string arcana)
        {
            arcana = arcana.Substring(1, 4);
            return arcana switch
            {
                "Deat" => Discord.Color.DarkerGrey,
                "Fate" => Discord.Color.Gold,
                "Spac" => Discord.Color.Purple,
                "Mind" => Discord.Color.Magenta,
                "Time" => Discord.Color.DarkGreen,
                "Forc" => Discord.Color.Red,
                "Matt" => Discord.Color.Green,
                "Spir" => Discord.Color.Blue,
                "Prim" => Discord.Color.Orange,
                "Life" => Discord.Color.LightOrange,
                _ => Discord.Color.Teal
            };
        }

        private string GetDuration()
        {
            List<RadioButton> duration = new()
            {
                sfSDuration1Radio, sfSDuration2Radio, sfSDuration3Radio, sfSDuration4Radio, sfSDuration5Radio,
                sfADuration1Radio, sfADuration2Radio, sfADuration3Radio, sfADuration4Radio, sfADuration5Radio, sfADuration6Radio
            };

            RadioButton? ret = duration.Find(d => d.Checked);
            return (ret != null) ? ret.Text : "Error: Duration not found.";
        }

        private string GetScale()
        {
            List<RadioButton> scale = new() {
                sfSScale1Radio, sfSScale2Radio, sfSScale3Radio, sfSScale4Radio, sfSScale5Radio,
                sfAScale1Radio, sfAScale2Radio, sfAScale3Radio, sfAScale4Radio, sfAScale5Radio, sfAScale6Radio
            };
            RadioButton? ret = scale.Find(d => d.Checked);
            return (ret != null) ? ret.Text : "Error: Scale not found.";
        }

        private string GetYantras()
        {
            string ret = "";
            List<ComboBox> yan = new()
            {
                sfYantra1ComboBox, sfYantra2ComboBox, sfYantra3ComboBox,
                    sfYantra4ComboBox, sfYantra5ComboBox, sfYantra6ComboBox
            };
            foreach (ComboBox y in yan)
            {
                if (y.SelectedIndex != -1)
                { ret += "• " + y.Text + "\n"; }
            }
            ret = ret[0..^1]; // remove final newline
            return ret;
        }
        #endregion
        #region MiscControls
        private void AddonTextBox_DoubleClick(object sender, EventArgs e)
        { // Double click an addon textbox to view full text. 
            TextBox tb = (TextBox)sender;
            MessageBox.Show(tb.Text, "Info", MessageBoxButtons.OK);
        }

        private void DurationRadio_CheckChanged(object sender, EventArgs e)
        {
            if (disableEvents) { return; }
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked == false)
            {
                if (radio.Name.Contains("SDuration"))
                {
                    // Switching to advanced duration
                    if (sfPermanenceCheck.Checked) { ++mana; }
                }
                else
                {
                    // Switching to standard duration
                    if (sfPermanenceCheck.Checked) { --mana; }
                }
                return;
            }

            List<RadioButton> duration = new()
            {
                sfSDuration1Radio, sfSDuration2Radio, sfSDuration3Radio, sfSDuration4Radio, sfSDuration5Radio,
                sfADuration1Radio, sfADuration2Radio, sfADuration3Radio, sfADuration4Radio, sfADuration5Radio, sfADuration6Radio
            };

            if (radio.Name.Contains("SDuration"))
            { // Makings sure all adv duration radios are unchecked when switching to standard.
                for (int i = 5; i < 11; ++i) { duration[i].Checked = false; }
                advDuration = false;
            }
            else
            { // Same as above but the other way around. 
                for (int i = 0; i < 5; ++i) { duration[i].Checked = false; }
                advDuration = true;
            }
            RefreshLabels();
        }

        private void ScaleRadio_CheckChanged(object sender, EventArgs e)
        {
            if (disableEvents) { return; }
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked == false)
            {
                if (radio.Name.Contains("SScale"))
                {
                    // Switching to advanced scale
                    if (sfEverywhereCheck.Checked) { ++mana; }
                }
                else
                {
                    // Switching to standard scale
                    if (sfEverywhereCheck.Checked) { --mana; }
                }
                return;
            }

            List<RadioButton> scale = new() {
                sfSScale1Radio, sfSScale2Radio, sfSScale3Radio, sfSScale4Radio, sfSScale5Radio,
                sfAScale1Radio, sfAScale2Radio, sfAScale3Radio, sfAScale4Radio, sfAScale5Radio, sfAScale6Radio
            };

            if (radio.Name.Contains("SScale"))
            { // Makings sure all adv scale radios are unchecked when switching to standard.
                for (int i = 5; i < 11; ++i) { scale[i].Checked = false; }
                advScale = false;
            }
            else
            { // Same as above but the other way around. 
                for (int i = 0; i < 5; ++i) { scale[i].Checked = false; }
                advScale = true;
            }
            RefreshLabels();
        }

        private void FactorRadio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (!radio.Checked || disableEvents) { return; }
            disableEvents = true;
            List<Label> durlab = new() {
                sfSDDiceCount1Label, sfSDDiceCount2Label, sfSDDiceCount3Label, sfSDDiceCount4Label, sfSDDiceCount5Label,
                sfADDiceCount1Label,sfADDiceCount2Label,sfADDiceCount3Label,sfADDiceCount4Label, sfADDiceCount5Label, sfADDiceCount6Label
            };
            List<Label> scalab = new()
            {
                sfSSDiceCount1Label, sfSSDiceCount2Label, sfSSDiceCount3Label, sfSSDiceCount4Label, sfSSDiceCount5Label,
                sfASDiceCount1Label, sfASDiceCount2Label, sfASDiceCount3Label, sfASDiceCount4Label, sfASDiceCount5Label, sfASDiceCount6Label
            };
            bool[] marked = new bool[11];
            switch (radio.Text)
            {
                case "Potency": // The effects of primary factor potency are reflected in the calculate dots segment. 
                    sfPotencyNumUpDown.Value = arcanaDots > 0 ? arcanaDots : 1;
                    ResetDurationLabels(durlab);
                    ResetScaleLabels(scalab);
                    break;
                case "Duration": // Both duration and scale calculate the dice costs after applying primary factor bonuses. 
                    ResetScaleLabels(scalab);
                    sfPotencyNumUpDown.Value = 1;
                    durlab[0].Text = "-0";
                    marked[0] = true;
                    for (int i = 0; i <= arcanaDots - 1; ++i)
                    {
                        durlab[i].Text = "-0";
                        marked[i] = true;
                        durlab[i + 5].Text = "-0";
                        marked[i + 5] = true;
                    }
                    int k = -2;
                    for (int i = 1; i < 5; ++i)
                    {
                        if (!marked[i])
                        {
                            durlab[i].Text = k.ToString();
                            durlab[i + 5].Text = k.ToString();
                            k -= 2;
                            if (i == 4) { durlab[10].Text = k.ToString(); }
                        }
                    }
                    break;
                case "Scale":
                    ResetDurationLabels(durlab);
                    sfPotencyNumUpDown.Value = 1;
                    scalab[0].Text = "-0";
                    marked[0] = true;
                    for (int i = 1; i <= arcanaDots - 1; ++i)
                    {
                        scalab[i].Text = "-0";
                        marked[i] = true;
                        scalab[i + 5].Text = "-0";
                        marked[i + 5] = true;
                    }
                    int j = -2;
                    for (int i = 0; i < 5; ++i)
                    {
                        if (!marked[i])
                        {
                            scalab[i].Text = j.ToString();
                            scalab[i + 5].Text = j.ToString();
                            j -= 2;
                            if (i == 4) { scalab[10].Text = j.ToString(); }
                        }
                    }
                    break;
            }

            // Highlight primary factor modifications in blue. 
            sfPotencyLabel.ForeColor = sfPotencyFactorRadio.Checked ? Color.Blue : Color.Black;
            foreach (Label l in durlab) { l.ForeColor = sfDurationFactorRadio.Checked ? Color.Blue : Color.Black; }
            foreach (Label l in scalab) { l.ForeColor = sfScaleFactorRadio.Checked ? Color.Blue : Color.Black; }

            disableEvents = false;
            RefreshLabels();
        }
        // The following two methods set the defaults back to 0 at the basic level with -2 per level above it. 
        private static void ResetDurationLabels(List<Label> dur)
        {
            dur[0].Text = "-0";
            for (int i = 1; i < 5; ++i)
            {
                int k = i * -2;
                dur[i].Text = k.ToString();
                dur[i + 5].Text = k.ToString();
                if (i == 4) { dur[10].Text = (k - 2).ToString(); }
            }
        }
        private static void ResetScaleLabels(List<Label> sca)
        {
            sca[0].Text = "-0";
            for (int i = 1; i < 5; ++i)
            {
                int k = i * -2;
                sca[i].Text = k.ToString();
                sca[i + 5].Text = k.ToString();
                if (i == 4) { sca[10].Text = (k - 2).ToString(); }
            }
        }
        private void RulingArcana_CheckChanged(object sender, EventArgs e)
        { // Analyzes checked boxes to assure you never have more than 3 ruling arcana. 
            if (disableEvents) { return; }
            CheckBox box = (CheckBox)sender;
            List<CheckBox> rulearc = new()
            {
                sfDeathRulingCheck, sfFateRulingCheck, sfForcesRulingCheck, sfLifeRulingCheck, sfMatterRulingCheck,
                sfMindRulingCheck, sfPrimeRulingCheck, sfSpaceRulingCheck, sfSpiritRulingCheck, sfTimeRulingCheck
            };
            var chek = rulearc.FindAll(a => a.Checked);
            foreach (CheckBox cb in rulearc)
            {
                if (chek.Count == 3) { cb.Enabled = cb.Checked; }
                else { cb.Enabled = true; }
            }
            if (!itsOkayYouCanRunThisEventWithoutWorryingAboutUnsavedChanges) { UnsavedChanges(); }
            RefreshLabels();
        }
        #endregion
        #region AutoEvents
        private void IncludeAddonCheck_CheckedChanged(object sender, EventArgs e)
        { if (!disableEvents) RefreshLabels(); }

        private void LeftSideRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (!disableEvents)
                RefreshLabels();
            EnableDisableCastButtons();
        }

        private void LeftsideNumUpDown_ValueChanged(object sender, EventArgs e)
        { if (!disableEvents) RefreshLabels(); }

        private void Attainments_CheckedChanged(object sender, EventArgs e)
        { if (!disableEvents) RefreshLabels(); }

        private void PotencyWithstand_ValueChanged(object sender, EventArgs e)
        { // Alert user their potency doesn't surpass withstand, blocks casting until resolved. 
            int wmod = sfAdvancedPotencyCheck.Checked ? -2 : 0;
            sfWithstandLabel.ForeColor = sfPotencyNumUpDown.Value <= sfWithstandNumUpDown.Value + wmod ? Color.Red : Color.Black;
            EnableDisableCastButtons();
            RefreshLabels();
        }
        private void CastRoteRadio_CheckedChanged(object sender, EventArgs e)
        {
            roteSpell = sfCastRoteRadio.Checked;
            CalculateFreeReach();
            RefreshLabels();
        }
        private void ArcanaDots_ValueChanged(object sender, EventArgs e)
        { UnsavedChanges(); }
        private void UnsavedChanges()
        {
            if (disableEvents) { return; }
            sfSaveProfileButton.ForeColor = Color.Crimson;
            saveOK = false;
            EnableDisableCastButtons();
        }
        private void EditYantrasButton_Click(object sender, EventArgs e)
        {
            var set = new HashSet<string>(currentCharacter.Yantras.Select(y => y.DisplayName).ToList());
            YantraForm yf = new(ref currentCharacter.Yantras, ref defaultYantras);
            yf.ShowDialog();
            RefreshYantraList();
            if (!set.SetEquals(currentCharacter.Yantras.Select(y => y.DisplayName).ToList())) { UnsavedChanges(); }
        }

        private void YantraLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (disableEvents) { return; }
            ComboBox cb = (ComboBox)sender;
            if (cb.SelectedItem.ToString() == "-None-")
            {
                disableEvents = true;
                cb.SelectedIndex = -1;
                disableEvents = false;
            }
            EnableDisableCastButtons();
            RefreshLabels();
        }

        private void EnableDisableCastButtons()
        {
            List<ComboBox> yan = new()
            {
                sfYantra1ComboBox, sfYantra2ComboBox, sfYantra3ComboBox,
                    sfYantra4ComboBox, sfYantra5ComboBox, sfYantra6ComboBox
            };
            int count = yan.FindAll(y => y.SelectedIndex != -1).Count;
            yantrasOK = count > 0;

            withstandOK = sfWithstandLabel.ForeColor == Color.Black;

            sfCastNormalButton.Enabled = saveOK && withstandOK && yantrasOK;
            sfCastAimedButton.Enabled = saveOK && withstandOK && yantrasOK && sfTouchRangeRadio.Checked;
        }
        #endregion

        private void SpellDescription_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(sfSpellDescriptionTextBox.Text, "Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
