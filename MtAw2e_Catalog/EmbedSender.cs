// EmbedSender
// A class that handles sending spell data to a Discord bot.
using Discord;
using System.Net.Sockets;
using System.Text;

namespace MtAw2e_Catalog
{
    // This class handles sending an EmbedBuilder object to Discord via a companion bot.
    // You must set up the bot yourself as the code is (mostly) not provided here. A relevant file in the Server folder should be helpful.
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
            var endpoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8080); // Replace with an IP that points to a discord bot listening on port 8080.
            var socketClient = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socketClient.Connect(endpoint);
            try
            {
                byte[] header = { 0xB0, 0x0B, 0xCA, 0xFE, 0xDE, 0xAD, 0xF1, 0x5F };
                byte[] g_bytes = BitConverter.GetBytes(guildid);
                byte[] c_bytes = BitConverter.GetBytes(channid);
                if (BitConverter.IsLittleEndian) { Array.Reverse(g_bytes); Array.Reverse(c_bytes); }
                string json = JsonConvert.SerializeObject(embed);
                byte[] embed_bytes = Encoding.UTF8.GetBytes(json);
                int length = embed_bytes.Length;
                byte[] package = new byte[28 + length];
                byte[] l_bytes = BitConverter.GetBytes(length);

                l_bytes.CopyTo(package, 0);
                header.CopyTo(package, 4);
                g_bytes.CopyTo(package, 12);
                c_bytes.CopyTo(package, 20);
                embed_bytes.CopyTo(package, 28);

                socketClient.Send(package, 0, package.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {
                string msg = $"Unable to send data to Discord.\nInfo: {ex.Message}";
                MessageBox.Show(msg, "Data send failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();
            socketClient.Dispose();
        }

        public void Dispose() { GC.SuppressFinalize(this); }
    }
}
