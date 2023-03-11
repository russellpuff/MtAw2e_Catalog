using Discord;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace MtAw2e_Catalog
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();
            ServerInfoForm sif = new();
            Application.Run(sif);
        }
    }

    // This class handles sending an EmbedBuilder object to Discord via Naomi Sakamoto.
    public class EmbedSender : IDisposable
    {
        private readonly EmbedBuilder embed;
        private readonly ulong guildid;
        private readonly ulong channid;

        public EmbedSender(EmbedBuilder _embed, ulong _gid, ulong _cid)
        {
            embed = _embed;
            guildid = _gid;
            channid = _cid;
        }

        public void SendEmbed()
        {
            TcpClient tclient = new();
            tclient.Connect(new IPEndPoint(IPAddress.Parse("34.139.87.153"), 8080));
            try
            {
                byte[] header = { 0xB0, 0x0B, 0xCA, 0xFE, 0xDE, 0xAD, 0xF1, 0x5F };
                byte[] g_bytes = BitConverter.GetBytes(guildid);
                byte[] c_bytes = BitConverter.GetBytes(channid);
                if (!BitConverter.IsLittleEndian) { Array.Reverse(g_bytes); Array.Reverse(c_bytes); }
                string json = JsonConvert.SerializeObject(embed);
                byte[] embed_bytes = Encoding.UTF8.GetBytes(json);
                byte[] package = new byte[24 + embed_bytes.Length];

                header.CopyTo(package, 0);
                g_bytes.CopyTo(package, 8);
                c_bytes.CopyTo(package, 16);
                embed_bytes.CopyTo(package, 24);

                NetworkStream stream = tclient.GetStream();
                stream.Write(package, 0, package.Length);
            }
            catch (Exception ex)
            {
                string msg = $"Unable to send data to Naomi.\nInfo: {ex.Message}";
                MessageBox.Show(msg, "Data send failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            tclient.Close();
        }

        public void Dispose() { GC.SuppressFinalize(this);  }
    }
}