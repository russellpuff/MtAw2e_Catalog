using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MtAw2e_Catalog
{
    public partial class ServerInfoForm : Form
    {
        DiscordSocketClient client;
        public ServerInfoForm(ref DiscordSocketClient _client)
        {
            client = _client;
            InitializeComponent();
            sifServerComboBox.SelectedIndex = 0;
        }

        private async void StartButton_Click(object sender, EventArgs e)
        {
            ulong server, channel;
            if (sifServerComboBox.SelectedIndex == 0)
            {
                server = 404042069259321345;
                channel = 995154537692270684;
                
            } else
            {
                server = 124894979536584704;
                channel = 695713954277884057;
            }

            MainForm mf = new(ref client, server, channel);
            this.Hide();
            mf.ShowDialog();
            while (mf.DialogResult != DialogResult.OK)
            {
                await Task.Delay(-1);
            }
        }
    }
}
