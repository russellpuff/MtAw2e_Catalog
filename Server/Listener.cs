/* This is an abstract for the public GitHub repo.
 * You will need to modify it to suit your needs, such as creating a console messagewriter. 
 * Also you need a functioning discord bot. The discordClient object in this code references a working bot.
*/


private async Task MageCatalogListener()
{
    Dictionary<string, Queue<Socket>> ipQueueDict = new();
    const string msg_source = "MageCatalogListener";
    IPEndPoint endpoint = new(IPAddress.Any, 8080);
    Socket listener = new(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    listener.Bind(endpoint);
    listener.Listen();
    MessageWriter(msg_source, "Waiting for a connection.");

    while (true)
    {
        // Wait for connections. If 30 minutes with no connections passes, restarts listener.
        Task<Socket> acceptSocketTask = listener.AcceptAsync();
        Task timerTask = Task.Delay(TimeSpan.FromMinutes(30));
        await Task.WhenAny(acceptSocketTask, timerTask);

        if (acceptSocketTask.IsCompletedSuccessfully)
        {
            string ip = acceptSocketTask.Result.RemoteEndPoint!.ToString()!.Split(':')[0];
            // Multiple connections from the same IP go into a queue where they're processed sequentially on the same thread. 
            if (ipQueueDict.ContainsKey(ip))
                { ipQueueDict[ip].Enqueue(acceptSocketTask.Result); } 
            else
            {
                ipQueueDict[ip] = new();
                ipQueueDict[ip].Enqueue(acceptSocketTask.Result);
                Thread clientThread = new(() => HandleMageCatalogClient(ref ipQueueDict, ip));
                clientThread.Start(); // Handle in separate thread. 
            }
        } else
        {
            MessageWriter(msg_source, $"No connection in 30 minutes, restarting Mage Catalog listener.");
            listener.Close();
            listener = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endpoint);
            listener.Listen();
            MessageWriter(msg_source, "Waiting for a connection...");
        }
    }
}

private void HandleMageCatalogClient(ref Dictionary<string, Queue<Socket>> ipQueueDict, string ip)
{
    const string msg_source = "MageCatalogListener";
    bool validated = false;
    while (ipQueueDict[ip].Count > 0)
    {
        Socket socketClient = ipQueueDict[ip].Dequeue();
        string? endpoint_string = socketClient.RemoteEndPoint is not null ? socketClient.RemoteEndPoint.ToString() : "NO_ENDPOINT?";
        MessageWriter(msg_source, $"Socket connected from: {endpoint_string}");
        try
        {
            Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(60)); // Security feature, kill client after 1 minute of idle behavior.
            var completedTask = Task.WhenAny(Task.Run(async () =>
            {
                // Begin receive client data. 
                int bytesRead = 0, totalBytesRead = 0;
                byte[] l_bytes = new byte[4]; // Byte array containing the length of the package after header/gid/cid. 
                socketClient.ReceiveTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
                socketClient.Receive(l_bytes, 0, 4, SocketFlags.None);

                // Collect header / guild id / channel id.
                byte[] head = new byte[24];
                totalBytesRead = 0;
                do
                {
                    socketClient.ReceiveTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
                    bytesRead = socketClient.Receive(
                        head, totalBytesRead, 24 - totalBytesRead, SocketFlags.None);
                    totalBytesRead += bytesRead;
                } while (totalBytesRead < 24);
                // End collect.

                if (VerifyMageCatalogData(head, out ulong serverid, out ulong channelid))
                {
                    // Collect remaining data if verified.
                    int package_length = BitConverter.ToInt32(l_bytes, 0);
                    byte[] buffer = new byte[package_length];
                    totalBytesRead = 0;
                    do
                    {
                        socketClient.ReceiveTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
                        bytesRead = socketClient.Receive(
                            buffer, totalBytesRead, package_length - totalBytesRead, SocketFlags.None);
                        totalBytesRead += bytesRead;
                    } while (totalBytesRead < package_length);
                    byte[] data = buffer;
                    // End collection of remaining data.

                    // Reconstruct EmbedBuilder object from data stream.
                    string json = Encoding.UTF8.GetString(data, 0, data.Length);
                    EmbedBuilder? embed = JsonConvert.DeserializeObject<EmbedBuilder>(json);

                    if (embed is null)
                    {
                        string s = "Encountered a problem where the inccoming json data couldn't be deserialized to EmbedBuilder, aborting operation.";
                        MessageWriter(msg_source, s);
                        return;
                    }

                    // Restore color lost during deserialize due to bug.
                    var obj = JObject.Parse(json);
                    embed.WithColor(new Color((uint)obj["Color"]!["RawValue"]!));
                    MessageWriter(msg_source, "Received EmbedBuilder data from client.");
                    // End reconstruction.

                    // Clunky diagnosis segment to try to figure out what went wrong if this can't send.
                    if (discordClient.ConnectionState == ConnectionState.Disconnected)
                    { throw new DirectoryNotFoundException("DiscordSocketClient was not connected to Discord."); }
                    if (discordClient.GetGuild(serverid) == null)
                    { throw new DirectoryNotFoundException("Guild ID was invalid."); }
                    if (discordClient.GetGuild(serverid).GetTextChannel(channelid) == null)
                    { throw new DirectoryNotFoundException("Channel ID was invalid."); }
                    // End diagnosis segment.

                    // Send to Discord server. 
                    await discordClient.GetGuild(serverid).GetTextChannel(channelid).SendMessageAsync(embed: embed.Build());
                    MessageWriter(msg_source, $"Successfully sent embed to Discord with GID: {serverid}, CID: {channelid}");
                    validated = true;
                }
                else
                { MessageWriter(msg_source, $"Failed to validate incoming data: {endpoint_string}"); }
            }), timeoutTask).Result;

            // Check if the completed task was a timeout. If so, log. The socket is killed regardless.
            if (completedTask == timeoutTask)
            { MessageWriter(msg_source, $"Maximum lifespan reached, killing client: {endpoint_string}"); }
        }
        catch (SocketException ex)
        { MessageWriter(msg_source, $"Timeout receiving data from client {endpoint_string}: {ex.Message}"); }
        catch (DirectoryNotFoundException ex)
        { MessageWriter(msg_source, $"Unable to send data to Discord: {ex.Message}"); }
        catch (Exception ex)
        { MessageWriter(msg_source, $"Error receiving data from client {endpoint_string}: {ex.Message}"); }
        finally
        {
            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();
            socketClient.Dispose(); // Redundant maybe.
            MessageWriter(msg_source, $"Closing client: {endpoint_string}");
            if (!validated)
            {
                // Server security code removed. 
                // This is where you would handle a case where your connection wasn't validated. 
            }
        }
    }
}