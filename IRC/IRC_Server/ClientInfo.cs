using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IRC_Server
{
    public class ClientInfo
    {
        private readonly Socket _clientSocket;
        private readonly SslStream _clientSslStream;
        private readonly NetworkStream _clientNetworkStream;
        private string _username;
        private const int BufferSize = 1024;
        private readonly byte[] _dataBuffer = new byte[BufferSize];
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly bool _usingSsl;
        private readonly Semaphore _semaphorePool = new Semaphore(0, 1);

        /// <summary>
        /// Default constructor for ClientInfo, sets values to null
        /// </summary>
        public ClientInfo()
        {
            _clientSocket = null;
            _clientSslStream = null;
            _clientNetworkStream = null;
            _username = "";
            _semaphorePool.Release(1);// release all so we can start using the semaphore
        }

        /// <summary>
        /// Constructor for ClientInfo
        /// </summary>
        /// <param name="clientSocket">The clients socket</param>
        /// <param name="clientSslStream">The clients SslStream</param>
        /// <param name="clientNetworkStream">The clients NetworkStream</param>
        public ClientInfo(Socket clientSocket, SslStream clientSslStream, NetworkStream clientNetworkStream)
        {
            _clientSocket = clientSocket;
            _clientSslStream = clientSslStream;
            _clientNetworkStream = clientNetworkStream;
            _username = "";
            _usingSsl = true;
            _semaphorePool.Release(1);// release all so we can start using the semaphore
        }

        /// <summary>
        /// Stops the client
        /// </summary>
        public void StopClient()
        {
            if (_clientSocket != null)
            {
                if (_clientSocket.Connected)
                {
                    _clientSocket.Shutdown(SocketShutdown.Both);
                }
                _clientSocket.Close();
            }

            _clientSslStream?.Close();
            _clientNetworkStream?.Close();
        }

        /// <summary>
        /// Sends a message to the client
        /// </summary>
        /// <param name="message">The formatted message to send</param>
        public void MessageClient(string message)
        {
            byte[] sendBuffer = Encoding.ASCII.GetBytes(message);
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

        /// <summary>
        /// Set the clients username
        /// </summary>
        /// <param name="username">New client username</param>
        public void SetUsername(string username)
        {
            _username = username;
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
        /// Get the client socket
        /// </summary>
        /// <returns>Clients socket</returns>
        public Socket ClientSocket()
        {
            return _clientSocket;
        }

        /// <summary>
        /// Check if client is sending data
        /// </summary>
        /// <returns>True if sending and false otherwise</returns>
        public bool IsSending()
        {
            // Just wait a millisecond, if true then isn't sending so we flip it
            return !_semaphorePool.WaitOne(1);
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
        /// Get the current data buffer
        /// </summary>
        /// <returns>An array of bytes</returns>
        public byte[] DataBuffer()
        {
            return _dataBuffer;
        }

        /// <summary>
        /// Get the string builder
        /// </summary>
        /// <returns>Clients StringBuilder</returns>
        public StringBuilder ClientStringBuilder()
        {
            return _stringBuilder;
        }

        /// <summary>
        /// Runs when data is received from a client. Parses the information and sends it off
        /// to a method to handle the information.
        /// </summary>
        /// <param name="asyncResult">Result containing the client info</param>
        public void DataReceivedCallback(IAsyncResult asyncResult)
        {
            ClientInfo client = (ClientInfo)asyncResult.AsyncState;
            try
            {
                var readBytes = _usingSsl ? client._clientSslStream.EndRead(asyncResult) : client.ClientSocket().EndReceive(asyncResult);

                client.ClientStringBuilder().Append(Encoding.ASCII.GetString(client.DataBuffer(), 0, readBytes));// Add on the new data
                string currentData = client.ClientStringBuilder().ToString();
                int position;
                while ((position = currentData.IndexOf("<EOF>", StringComparison.Ordinal)) != -1)// Keep going until we don't have any more commands
                {
                    string command = currentData.Substring(0, position);
                    Console.WriteLine($"<{client._username}> {command}");
                    currentData = currentData.Remove(0, position + 5);// Remove the command from the currentData string
                    HelperMethods.CommandHandler(command, client);// Handle the command
                }

                client.ClientStringBuilder().Clear();// We used the data in the string builder so now we can clear it
                client.ClientStringBuilder().Append(currentData);// There still might be some data left so we add it back in
                client.BeginReceiveData();// Continue receiving data
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("DataReceivedCallback(): Socket was closed before finishing receive");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054)// Connection reset by peer.
                {
                    Console.WriteLine($"DataReceivedCallback(): Client went offline");
                }
                else
                {
                    Console.WriteLine($"DataReceivedCallback(): {se.Message} Error Code: {se.ErrorCode}");
                }
                HelperMethods.RemoveClientFromAll(client);
            }
            catch (IOException ioe)
            {
                if (ioe.InnerException is SocketException se)
                {
                    if (se.ErrorCode == 10054)// Connection reset by peer.
                    {
                        Console.WriteLine($"DataReceivedCallback(): Client went offline");
                    }
                }
                else
                {
                    Console.WriteLine($"DataReceivedCallback(): {ioe.Message}");
                }
                HelperMethods.RemoveClientFromAll(client);
            }
            catch (Exception e)
            {
                Console.WriteLine($"DataReceivedCallbackSsl(): {e.Message}");
                if (e.InnerException != null)
                {
                    Console.WriteLine($"DataReceivedCallbackSsl(): {e.InnerException}");
                }
                HelperMethods.RemoveClientFromAll(client);
            }
        }

        /// <summary>
        /// Runs when data is sent to the client
        /// </summary>
        /// <param name="asyncResult">Result containing the client info</param>
        public void DataSentCallback(IAsyncResult asyncResult)
        {
            ClientInfo client = (ClientInfo)asyncResult.AsyncState;
            try
            {
                if (_usingSsl)
                {
                    client._clientSslStream.EndWrite(asyncResult);
                    Console.WriteLine($"DataSentCallbackSsl(): Successfully sent message to {client.GetUsername()}");
                    _semaphorePool.Release();
                }
                else
                {
                    int result = client.ClientSocket().EndSend(asyncResult, out var errorCode);
                    Console.WriteLine(errorCode == SocketError.Success ?
                            $"Successfully sent message with size of {result} bytes to {client.GetUsername()}" :
                            $"Error sending. code: {errorCode}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Unhandled DataSentCallback() Exception! " + e);
            }
        }
    }
}
