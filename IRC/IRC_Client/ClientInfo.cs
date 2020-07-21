using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using IRC_Server;


namespace IRC_Client
{
    public class ClientInfo
    {
        private Socket _clientSocket;
        private SslStream _clientSslStream;
        private NetworkStream _clientNetworkStream;
        private string _username;
        private const int BufferSize = 1024;
        private readonly byte[] _dataBuffer = new byte[BufferSize];
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private string _serverIp;
        private int _serverPort;
        private string _hostName;
        private readonly List<Room> _rooms = new List<Room>();
        private readonly Room _debugRoom = new Room("DEBUG", true, false);
        private readonly bool _usingSsl;
        private readonly Semaphore _semaphorePool;

        /// <summary>
        /// Default constructor for ClientInfo, sets values to null
        /// </summary>
        public ClientInfo()
        {
            _clientSocket = null;
            _username = "";
            _serverIp = "";
            _serverPort = -1;
            _hostName = "";
            _usingSsl = false;

            _semaphorePool = new Semaphore(0, 1);
            _semaphorePool.Release(1);// Release all so we can start using the semaphore
        }

        /// <summary>
        /// Constructor for ClientInfo
        /// </summary>
        /// <param name="serverIp">The IP address of the server</param>
        /// <param name="serverPort">The port of the server</param>
        /// <param name="hostName">The host name of the server</param>
        /// <param name="username">Clients username</param>
        /// <param name="ssl">Whether we're using ssl or not</param>
        public ClientInfo(string serverIp, int serverPort, string hostName, string username, bool ssl)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
            _hostName = hostName;
            _username = username;
            _usingSsl = ssl;

            _semaphorePool = new Semaphore(0, 1);
            _semaphorePool.Release(1);// Release all so we can start using the semaphore
        }

        /// <summary>
        /// Sends a message to the client
        /// </summary>
        /// <param name="message">The message to send already formatted</param>
        public void MessageClient(string message)
        {
            byte[] sendBuffer = Encoding.ASCII.GetBytes(message);
            try
            {
                if (_usingSsl)
                {
                    _semaphorePool.WaitOne();
                    _clientSslStream.BeginWrite(sendBuffer, 0, sendBuffer.Length, DataSentCallback, this);
                }
                else
                {
                    _clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, DataSentCallback, this);
                }
            }
            catch (Exception e)
            {
                _debugRoom.AddMessage(new Message(DateTime.Now, "MessageClient()", $"Uncaught exception, stopping client: {e.Message}"));
                StopClient(true);// forced doesn't try to send any messages to the server
            }

        }

        /// <summary>
        /// Sets the IP address of the server
        /// </summary>
        /// <param name="serverIp">The IP address of the server</param>
        public void SetServerIp(string serverIp)
        {
            _serverIp = serverIp;
        }

        /// <summary>
        /// Sets the port of the server
        /// </summary>
        /// <param name="serverPort">The port of the server</param>
        public void SetServerPort(int serverPort)
        {
            _serverPort = serverPort;
        }

        /// <summary>
        /// Sets the clients username
        /// </summary>
        /// <param name="username">New client username</param>
        public void SetUsername(string username)
        {
            _username = username;
        }

        /// <summary>
        /// Sets the host name of the server
        /// </summary>
        /// <param name="hostname">The host name of the server</param>
        public void SetHostName(string hostname)
        {
            _hostName = hostname;
        }

        /// <summary>
        /// Get the client username
        /// </summary>
        /// <returns>The current username of the client</returns>
        public string GetUsername()
        {
            return _username;
        }

        /// <summary>
        /// Check if the socket is connected
        /// </summary>
        /// <returns>True if connected and false otherwise</returns>
        public bool Connected()
        {
            return _clientSocket.Connected;
        }

        /// <summary>
        /// Get the client socket
        /// </summary>
        /// <returns>Clients socket</returns>
        public Socket ClientSocket()
        {
            return _clientSocket;
        }

        /// <summary>
        /// Begins receiving data from the client socket
        /// </summary>
        public void BeginReceiveData()
        {
            if (_usingSsl)
            {
                _clientSslStream.BeginRead(_dataBuffer, 0, BufferSize, DataReceivedCallback, this);
            }
            else
            {
                _clientSocket.BeginReceive(_dataBuffer, 0, BufferSize, SocketFlags.None, DataReceivedCallback, this);
            }
        }

        /// <summary>
        /// Get the string builder
        /// </summary>
        /// <returns>Clients string builder</returns>
        public StringBuilder ClientStringBuilder()
        {
            return _stringBuilder;
        }

        /// <summary>
        /// Checks if the server certificate is valid
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="certificate">The server certificate</param>
        /// <param name="chain">The X509Chain</param>
        /// <param name="sslPolicyErrors">Any ssl policy errors with the certificate</param>
        /// <returns>True if the certificate is valid and false otherwise</returns>
        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            _debugRoom.AddMessage(new Message(DateTime.Now, "ValidateServerCertificate()", $"Certificate error: {sslPolicyErrors}"));

            // Server certificate had problems
            return false;
        }

        /// <summary>
        /// Starts the client
        /// </summary>
        public void StartClient()
        {
            try
            {
                StopClient(false);// Stop in case still running
                Room serverChat = new Room("SERVER", true, false);
                serverChat.AddMember("SERVER");
                serverChat.AddMember(_username);
                AddRoom(serverChat);

                // Set up the remote endpoint
                IPAddress ipAddress = IPAddress.Parse(_serverIp);
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, _serverPort);

                // Create the socket
                _clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint. 
                _clientSocket.BeginConnect(remoteEndPoint, ClientConnectCallback, _clientSocket);
            }
            catch (Exception e)
            {
                _debugRoom.AddMessage(new Message(DateTime.Now, "StartClient()", e.ToString()));
                StopClient(false);// Error starting, stop everything
            }
        }

        /// <summary>
        /// Stops the client
        /// </summary>
        /// <param name="forced">If forced client won't notify server that its disconnecting</param>
        public void StopClient(bool forced)
        {
            if (_clientSocket != null)
            {
                if (_clientSocket.Connected)
                {
                    if (forced == false)
                    {
                        MessageClient(HelperMethods.FormatMessage(Headers.ClientDisconnecting));
                        if (_usingSsl)
                        {
                            _semaphorePool.WaitOne();
                            _semaphorePool.Release();
                            // If we're using ssl wait for message to finish sending
                        }
                    }
                    _clientSocket.Shutdown(SocketShutdown.Both);
                }
                _clientSocket.Close();
            }

            _clientSslStream?.Close();
            _clientNetworkStream?.Close();

            _stringBuilder.Clear();
            _rooms.Clear();
            Globals.ClientForm.ReloadRooms();
            Globals.ClientForm.ClearMembers();
            Globals.ClientForm.ClearMessages();
        }

        /// <summary>
        /// Runs on a new client connection, and sets a client id and adds to list of clients.
        /// </summary>
        /// <param name="asyncResult">Result from connection containing socket to client</param>
        public void ClientConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket clientSocket = (Socket)asyncResult.AsyncState;
                clientSocket.EndConnect(asyncResult);
                _debugRoom.AddMessage(new Message(DateTime.Now, "ClientConnectCallback()", $"Socket connected to {clientSocket.RemoteEndPoint}"));


                _clientNetworkStream = new NetworkStream(_clientSocket);
                _clientSslStream = new SslStream(_clientNetworkStream, true, ValidateServerCertificate);

                try
                {
                    _clientSslStream.AuthenticateAsClient(_hostName);
                }
                catch (AuthenticationException e)
                {
                    _debugRoom.AddMessage(new Message(DateTime.Now, "ClientConnectCallback()", $"Exception: {e.Message}"));
                    if (e.InnerException != null)
                    {
                        _debugRoom.AddMessage(new Message(DateTime.Now, "ClientConnectCallback()", $"Inner exception: {e.InnerException.Message}"));
                    }

                    _debugRoom.AddMessage(new Message(DateTime.Now, "ClientConnectCallback()", "Authentication failed - closing the connection."));
                    StopClient(false);
                    return;
                }
                catch (Exception e)
                {
                    _debugRoom.AddMessage(new Message(DateTime.Now, "ClientConnectCallback()", $"Exception: {e.Message}"));
                    if (e.InnerException != null)
                    {
                        _debugRoom.AddMessage(new Message(DateTime.Now, "ClientConnectCallback()", $"Inner exception: {e.InnerException.Message}"));
                    }
                    StopClient(false);
                    return;
                }


                // Connected so save settings
                // Username is saved when server responds with confirmation
                Globals.Settings.ServerIp = _serverIp;
                Globals.Settings.ServerPort = _serverPort;
                Globals.Settings.HostName = _hostName;
                Globals.Settings.UseSsl = _usingSsl;

                BeginReceiveData();
                MessageClient(HelperMethods.FormatMessage(Headers.Username, _username));
                MessageClient(HelperMethods.FormatMessage(Headers.GetRooms));

            }
            catch (ObjectDisposedException)
            {
                _debugRoom.AddMessage(new Message(DateTime.Now, "ClientConnectCallback()", "Object already disposed"));
            }
            catch (SocketException se)
            {
                _debugRoom.AddMessage(new Message(DateTime.Now, "ClientConnectCallback()", se.Message));
                StopClient(false);// Error connecting, just stop everything
            }
        }

        /// <summary>
        /// Runs when data is received from a client. Parses the information and sends it off
        /// to a method to handle the information.
        /// </summary>
        /// <param name="asyncResult">Result from the client containing the client info allowing us to get the data.</param>
        public void DataReceivedCallback(IAsyncResult asyncResult)
        {
            ClientInfo client = (ClientInfo)asyncResult.AsyncState;
            try
            {
                int readBytes = _usingSsl ? client._clientSslStream.EndRead(asyncResult) : client._clientSocket.EndReceive(asyncResult);

                client.ClientStringBuilder().Append(Encoding.ASCII.GetString(client._dataBuffer, 0, readBytes));
                string currentData = client.ClientStringBuilder().ToString();
                int position;
                while ((position = currentData.IndexOf("<EOF>", StringComparison.Ordinal)) != -1)
                {
                    string command = currentData.Substring(0, position);
                    currentData = currentData.Remove(0, position + 5);
                    HelperMethods.CommandHandler(command, client);

                    GetRoom("SERVER")?.AddMessage(new Message(DateTime.Now, "SERVER", command));
                }

                client.ClientStringBuilder().Clear();
                client.ClientStringBuilder().Append(currentData);
                client.BeginReceiveData();
            }
            catch (ObjectDisposedException)
            {
                _debugRoom.AddMessage(new Message(DateTime.Now, "DataReceivedCallback()",
                    "Socket was closed before finishing receive"));
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054)// Connection reset by peer.
                {
                    _debugRoom.AddMessage(new Message(DateTime.Now, "DataReceivedCallback()", "Server went offline"));
                }
                else
                {
                    _debugRoom.AddMessage(new Message(DateTime.Now, "DataReceivedCallback()", se.Message));
                }

                StopClient(false);
            }
            catch (IOException ioe)
            {
                if (ioe.InnerException is SocketException se)
                {
                    if (se.ErrorCode == 10054)// Connection reset by peer
                    {
                        _debugRoom.AddMessage(new Message(DateTime.Now, "DataReceivedCallback()", "Server went offline"));
                    }
                }
                else
                {
                    _debugRoom.AddMessage(new Message(DateTime.Now, "DataReceivedCallback()", $"Unhandled exception, stopping client: {ioe.Message}"));
                }
                StopClient(false);
            }
            catch (Exception e)
            {
               
                _debugRoom.AddMessage(new Message(DateTime.Now, "DataReceivedCallback()", $"Unhandled exception, stopping client: {e.Message}"));
                if (e.InnerException != null)
                {
                    _debugRoom.AddMessage(new Message(DateTime.Now, "DataReceivedCallback()", $"Unhandled exception, stopping client: {e.InnerException}"));
                }
                StopClient(false);
            }
        }

        /// <summary>
        /// Runs when data is sent to the client, and handles errors if they arise. NOT COMPLETE THOUGH
        /// </summary>
        /// <param name="asyncResult">Contains our client info to get the needed information.</param>
        public void DataSentCallback(IAsyncResult asyncResult)
        {
            ClientInfo client = (ClientInfo)asyncResult.AsyncState;
            try
            {
                if (_usingSsl)
                {
                    client._clientSslStream.EndWrite(asyncResult);
                    _debugRoom.AddMessage(new Message(DateTime.Now, "ClientConnectCallback()", "Successfully sent message to server"));
                    _semaphorePool.Release();
                }
                else
                {
                    int result = client._clientSocket.EndSend(asyncResult, out var errorCode);
                    _debugRoom.AddMessage(new Message(DateTime.Now, "DataSentCallback()", errorCode == SocketError.Success ?
                            $"Successfully sent message with size of {result} bytes to server" :
                            $"Error sending. code: {errorCode}"));
                }
            }
            catch (Exception e)
            {
                _debugRoom.AddMessage(new Message(DateTime.Now, "DataSentCallback()", $"Exception {e}"));
            }
        }

        /// <summary>
        /// Get the room list
        /// </summary>
        /// <returns>List of rooms</returns>
        public List<Room> RoomList()
        {
            return _rooms;
        }

        /// <summary>
        /// Check if a room exists
        /// </summary>
        /// <param name="room">Room name to query</param>
        /// <returns>True if room exists and false otherwise</returns>
        public bool ContainsRoom(string room)
        {
            return _rooms.Any(curRoom => curRoom.RoomName() == room);
        }

        /// <summary>
        /// Adds a room
        /// </summary>
        /// <param name="room">Room to add</param>
        public void AddRoom(Room room)
        {
            _rooms.Add(room);
            Globals.ClientForm.ReloadRooms();
        }

        /// <summary>
        /// Removes a room
        /// </summary>
        /// <param name="room">Name of room to remove</param>
        /// <returns>True if room gets removed and false otherwise</returns>
        public bool RemoveRoom(string room)
        {
            if (GetRoom(room) != null && _rooms.Remove(GetRoom(room)))
            {
                Globals.ClientForm.ReloadRooms();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get a room by name
        /// </summary>
        /// <param name="room">Name of room to get</param>
        /// <returns>The room if it exists and null otherwise</returns>
        public Room GetRoom(string room)
        {
            foreach (var curRoom in _rooms)
            {
                if (curRoom.RoomName() == room)
                {
                    return curRoom;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the debug room
        /// </summary>
        /// <returns>The debug room</returns>
        public Room GetDebugRoom()
        {
            return _debugRoom;
        }
    }
}
