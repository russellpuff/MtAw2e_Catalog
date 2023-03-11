namespace MtAw2e_Catalog
{
    public partial class ServerInfoForm : Form
    {
        public ServerInfoForm()
        {
            InitializeComponent();
            TryLoadDefaultGIDCID();
        }

        private void TryLoadDefaultGIDCID()
        {
            try
            {
                if (!File.Exists("cfg.bin")) { return; }
                using var reader = new BinaryReader(File.OpenRead("cfg.bin"));
                ulong gid = reader.ReadUInt64();
                ulong cid = reader.ReadUInt64();
                sifGuildIDTextBox.Text = gid.ToString();
                sifChannelIDTextBox.Text = cid.ToString();
            } catch { }
        }

        private async void StartButton_Click(object sender, EventArgs e)
        {

            bool s_check = ulong.TryParse(sifGuildIDTextBox.Text, out ulong server);
            bool c_check = ulong.TryParse(sifChannelIDTextBox.Text, out ulong channel);

            if (s_check && c_check)
            {
                if (!File.Exists("cfg.bin")) { var f = File.Create("cfg.bin"); f.Dispose(); }
                using var writer = new BinaryWriter(File.Open("cfg.bin", FileMode.Truncate, FileAccess.Write));
                writer.Write(server);
                writer.Write(channel);
                try
                {
                    
                } catch { }

                MainForm mf = new(server, channel);
                this.Hide();
                mf.ShowDialog();
                while (mf.DialogResult != DialogResult.OK)
                {
                    await Task.Delay(Timeout.Infinite);
                }
            } else
            {
                string msg = "Invalid guild id or channel id.";
                MessageBox.Show(msg, "Invalid ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
