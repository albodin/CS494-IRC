using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace IRC_Server
{
    class HelperMethods
    {
        /// <summary>
        /// Displays the security level of the SslStream
        /// </summary>
        /// <param name="stream">SslStream to check</param>
        public static void DisplaySecurityLevel(SslStream stream)
        {
            Console.WriteLine($"Cipher: {stream.CipherAlgorithm} strength {stream.CipherStrength}");
            Console.WriteLine($"Hash: {stream.HashAlgorithm} strength {stream.HashStrength}");
            Console.WriteLine($"Key exchange: {stream.KeyExchangeAlgorithm} strength {stream.KeyExchangeStrength}");
            Console.WriteLine($"Protocol: {stream.SslProtocol}");
        }
        
        /// <summary>
        /// Displays the security services of the SslStream
        /// </summary>
        /// <param name="stream">SslStream to check</param>
        public static void DisplaySecurityServices(SslStream stream)
        {
            Console.WriteLine($"Is authenticated: {stream.IsAuthenticated} as server? {stream.IsServer}");
            Console.WriteLine($"IsSigned: {stream.IsSigned}");
            Console.WriteLine($"Is Encrypted: {stream.IsEncrypted}");
        }

        /// <summary>
        /// Displays the SslStreams properties
        /// </summary>
        /// <param name="stream">SslStream to check</param>
        public static void DisplayStreamProperties(SslStream stream)
        {
            Console.WriteLine($"Can read: {stream.CanRead}, write {stream.CanWrite}");
            Console.WriteLine($"Can timeout: {stream.CanTimeout}");
        }

        /// <summary>
        /// Displays the certificate information of the SslStream
        /// </summary>
        /// <param name="stream">SslStream to check</param>
        public static void DisplayCertificateInformation(SslStream stream)
        {
            Console.WriteLine($"Certificate revocation list checked: {stream.CheckCertRevocationStatus}");

            X509Certificate localCertificate = stream.LocalCertificate;
            Console.WriteLine(localCertificate != null
                ? $"Local cert was issued to {localCertificate.Subject} and is valid from {localCertificate.GetEffectiveDateString()} until {localCertificate.GetExpirationDateString()}."
                : "Local certificate is null.");

            // Display the properties of the client's certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            Console.WriteLine(remoteCertificate != null
                ? $"Remote cert was issued to {remoteCertificate.Subject} and is valid from {remoteCertificate.GetEffectiveDateString()} until {remoteCertificate.GetExpirationDateString()}."
                : "Remote certificate is null.");
        }

        /// <summary>
        /// Get the local IPv4 addresses
        /// </summary>
        /// <returns>List of strings</returns>
        public static List<string> GetIpAddresses()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).ToList();
        }

        /// <summary>
        /// Get the room names
        /// </summary>
        /// <returns>List of strings</returns>
        public static List<string> GetRoomNameList()
        {
            return Globals.ServerRooms.Select(room => room.RoomName()).ToList();
        }

        /// <summary>
        /// Sends a message to all clients in the list
        /// </summary>
        /// <param name="message">The formatted message to send</param>
        public static void MessageAllClients(string message)
        {
            foreach (var client in Globals.ClientList)
            {
                client.MessageClient(message);
            }
        }

        /// <summary>
        /// Checks if a username is taken
        /// </summary>
        /// <param name="username">The wanted username</param>
        /// <returns>True if the username is taken and false otherwise</returns>
        public static bool UsernameTaken(string username)
        {
            return Globals.ClientList.Any(client => client.GetUsername() == username);
        }

        /// <summary>
        /// Changes the clients username
        /// </summary>
        /// <param name="wantedUsername">The new username</param>
        /// <param name="client">Our client</param>
        /// <returns>True if username was changed and false otherwise</returns>
        public static bool ChangeClientUsername(string wantedUsername, ClientInfo client)
        {
            if (wantedUsername == "")
            {
                return false;
            }

            if (!UsernameTaken(wantedUsername) && client != null)
            {
                Console.Write($"Client changed usernames. Previous username: {client.GetUsername()}\tNew username: ");
                client.SetUsername(wantedUsername);
                Console.WriteLine(client.GetUsername());
                return true;
            }

            if (client != null)
            {
                return false;
            }

            Console.WriteLine("ERROR: Client wasn't in client list");
            return false;
        }

        /// <summary>
        /// Formats the message to send to the client
        /// </summary>
        /// <param name="header">The header of the message</param>
        /// <param name="parameters">Array of parameters for the message</param>
        /// <returns>Formatted message</returns>
        public static string FormatMessage(string header, params string[] parameters)
        {
            StringBuilder messageStringBuilder = new StringBuilder(header);
            foreach (var parameter in parameters)
            {
                messageStringBuilder.Append("<EOL>");
                messageStringBuilder.Append(parameter);
            }

            messageStringBuilder.Append("<EOF>");
            return messageStringBuilder.ToString();
        }

        /// <summary>
        /// Removes the client from  the client list and all of the server rooms
        /// </summary>
        /// <param name="client">Our client</param>
        public static void RemoveClientFromAll(ClientInfo client)
        {
            Console.WriteLine($"Client {client.GetUsername()} disconnected");

            // Remove client from global list
            Console.WriteLine(Globals.ClientList.Remove(client)
                ? $"Removed client {client.GetUsername()} from client list"
                : $"Error removing client {client.GetUsername()} from client list");

            // remove from rooms too
            int removedFrom = 0;
            foreach (var room in Globals.ServerRooms)
            {
                if (room.RemoveClient(client))
                {
                    removedFrom++;
                    // Notify all clients of new member list
                    List<string> toSendList = new List<string> { room.RoomName() };
                    toSendList.AddRange(room.GetAllClientsNames());
                    MessageAllClients(FormatMessage(Headers.RoomMembersResult, toSendList.ToArray()));
                }
            }

            MessageAllClients(FormatMessage(Headers.ClientDisconnected, client.GetUsername()));// Notify all clients that this client left
            Console.WriteLine($"Removed client {client.GetUsername()} from {removedFrom} rooms");
            client.StopClient();
        }

        /// <summary>
        /// Create a new room
        /// </summary>
        /// <param name="roomName">New room name</param>
        /// <param name="client">Client creating the room</param>
        /// <returns>True if room creation is successful and false otherwise</returns>
        public static bool CreateRoom(string roomName, ClientInfo client)
        {
            // Check for special room names
            if (roomName == "" || roomName.ToLower() == "rooms" || roomName.ToLower() == "debug" || roomName.ToLower() == "server")
            {
                return false;
            }

            // Check if room already exists
            foreach (var room in Globals.ServerRooms)
            {
                if (room.RoomName() == roomName)
                {
                    return false;// room name already exists
                }
            }

            ServerRoom newRoom = new ServerRoom(roomName, client);
            if (!newRoom.AddClient(client))
            {
                // failure to add owner as a client so cancel room creation
                return false;
            }

            Globals.ServerRooms.Add(newRoom);
            return true;
        }

        /// <summary>
        /// Removes an existing room
        /// </summary>
        /// <param name="roomName">Room name to remove</param>
        /// <param name="client">Client trying to remove the room</param>
        /// <returns>True if room was removed and false otherwise</returns>
        public static bool RemoveRoom(string roomName, ClientInfo client)
        {
            foreach (var room in Globals.ServerRooms)
            {
                if (room.RoomName() == roomName && room.IsOwner(client))
                {
                    // Send a message to the room that it's getting removed
                    room.MessageClients(FormatMessage(Headers.RoomMsgData, roomName, "SERVER", $"Room owner {client.GetUsername()} has deleted the room."));
                    room.RemoveAllClients();

                    // Remove the room from the room list
                    Console.WriteLine(Globals.ServerRooms.Remove(room)
                        ? $"Successfully deleted room {roomName}"
                        : $"Error removing room {roomName} from room list");

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a client to a room
        /// </summary>
        /// <param name="roomName">Room to add to</param>
        /// <param name="client">Client to add to room</param>
        /// <returns>True if client was successfully added and false otherwise</returns>
        public static bool AddClientToRoom(string roomName, ClientInfo client)
        {
            foreach (var room in Globals.ServerRooms)
            {
                if (room.RoomName() == roomName)
                {
                    if (room.AddClient(client))
                    {
                        // Notify all clients of new member list
                        List<string> tosendList = new List<string> {roomName};
                        tosendList.AddRange(room.GetAllClientsNames());
                        MessageAllClients(FormatMessage(Headers.RoomMembersResult, tosendList.ToArray()));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a client from a room
        /// </summary>
        /// <param name="roomName">Room to remove from</param>
        /// <param name="client">Client to remove from room</param>
        /// <returns>True if client was successfully removed and false otherwise</returns>
        public static bool RemoveClientFromRoom(string roomName, ClientInfo client)
        {
            foreach (var room in Globals.ServerRooms)
            {
                if (room.RoomName() == roomName)
                {
                    if (room.RemoveClient(client))
                    {
                        // if we remove the client we notify the rest
                        // of the members with the new room member list
                        List<string> toSendList = new List<string> { roomName };
                        toSendList.AddRange(room.GetAllClientsNames());
                        MessageAllClients(FormatMessage(Headers.RoomMembersResult, toSendList.ToArray()));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Get the members of a room
        /// </summary>
        /// <param name="roomName">Room to get members of</param>
        /// <param name="membersList">Reference to list of strings</param>
        /// <returns>True if room was found and false otherwise</returns>
        public static bool GetRoomMembers(string roomName, ref List<string> membersList)
        {
            foreach (var room in Globals.ServerRooms)
            {
                if (room.RoomName() == roomName)
                {
                    membersList.AddRange(room.GetAllClientsNames());
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sends a message to all members in a room
        /// </summary>
        /// <param name="roomName">Name of room to message</param>
        /// <param name="message">Unformatted message to send</param>
        /// <param name="client">Client sending the message</param>
        /// <returns>True if room exists and false if it doesn't</returns>
        public static bool MessageRoomMembers(string roomName, string message, ClientInfo client)
        {
            foreach (var room in Globals.ServerRooms)
            {
                // if room name is invalid or client isn't a member we continue looping
                if (room.RoomName() != roomName || !room.Contains(client)) continue;

                room.MessageClients(FormatMessage(Headers.RoomMsgData, roomName, client.GetUsername(), message));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends a private message to another member
        /// </summary>
        /// <param name="memberName">Member to message</param>
        /// <param name="message">Unformatted message to send</param>
        /// <param name="client">Client sending the message</param>
        /// <returns>True if member was found and false otherwise</returns>
        public static bool MessageMember(string memberName, string message, ClientInfo client)
        {
            foreach (var curClient in Globals.ClientList)
            {
                if (curClient.GetUsername() == memberName)
                {
                    curClient.MessageClient(FormatMessage(Headers.PrivMsgData, client.GetUsername(), curClient.GetUsername(), message));
                    client.MessageClient(FormatMessage(Headers.PrivMsgData, client.GetUsername(), curClient.GetUsername(), message));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handles the commands sent from the client to the server
        /// </summary>
        /// <param name="command">The command received from the client</param>
        /// <param name="client">Client that sent the command</param>
        public static void CommandHandler(string command, ClientInfo client)
        {
            // Our format is
            // COMMAND<EOL>COMMAND DATA<EOF>

            string[] commandStrings = command.Split(new[] { "<EOL>" }, StringSplitOptions.None);
            switch (commandStrings[0])
            {
                // Set username
                // USERNAME<EOL>username<EOF>
                // Will return
                // USERNAMERESULT<EOL>TRUE?FALSE?USERNAME<EOL>NOTE<EOF>
                // USERNAMECHANGE<EOL>OLDUSERNAME<EOL>NEWUSERNAME<EOF>
                case Headers.Username:
                    string oldUsername = client.GetUsername();
                    // trim to remove white space from begging and end
                    if (ChangeClientUsername(commandStrings[1].Trim(), client))
                    {
                        client.MessageClient(FormatMessage(Headers.UsernameResult, "TRUE", client.GetUsername(), "Change successful"));
                        MessageAllClients(FormatMessage(Headers.UsernameChange, oldUsername, client.GetUsername()));// Message all clients of username change
                    }
                    else
                    {
                        client.MessageClient(FormatMessage(Headers.UsernameResult, "FALSE", client.GetUsername(), "Change unsuccessful; username taken"));
                    }
                    break;


                // Make room
                // MAKEROOM<EOL>ROOMNANE<EOF>
                // Will return
                // MAKEROOMRESULT<EOL>TRUE?FALSE<EOL>NOTE<EOL>ROOMCREATEDNAME<EOF>
                case Headers.MakeRoom:
                    if (CreateRoom(commandStrings[1].Trim(), client))
                    {
                        client.MessageClient(FormatMessage(Headers.MakeRoomResult, "TRUE", "Created room successfully", commandStrings[1].Trim()));
                        MessageAllClients(FormatMessage(Headers.GetRoomsResult, GetRoomNameList().ToArray()));// Send all clients all of the rooms
                    }
                    else
                    {
                        client.MessageClient(FormatMessage(Headers.MakeRoomResult, "FALSE", "Error creating room. Room name could be taken"));
                    }
                    break;


                // Delete room(can only be done by server or client who made it)
                // REMOVEROOM<EOL>ROOMNAME<EOF>
                // Will return
                // REMOVEROOMRESULT<EOL>TRUE?FALSE<EOL>NOTE<EOF>
                // ROOMREMOVED<EOL>ROOMNAME<EOF>
                case Headers.RemoveRoom:
                    if (RemoveRoom(commandStrings[1].Trim(), client))
                    {
                        client.MessageClient(FormatMessage(Headers.RemoveRoomResult, "TRUE", "Removed room successfully"));
                        MessageAllClients(FormatMessage(Headers.RoomRemoved, commandStrings[1].Trim()));// Notify clients that the room was removed
                    }
                    else
                    {
                        client.MessageClient(FormatMessage(Headers.RemoveRoomResult, "FALSE", "Error removing room. Not owner or invalid room name"));
                    }
                    break;


                // Get all rooms
                // GETROOMS<EOF>
                // Will return rooms in format
                // GETROOMSRESULT<EOL>RM1<EOL>RM2<EOF>
                case Headers.GetRooms:
                    client.MessageClient(FormatMessage(Headers.GetRoomsResult, GetRoomNameList().ToArray()));
                    break;


                // Join room
                // JOINROOM<EOL>ROOMNAME<EOF>
                // Will return
                // JOINROOMRESULT<EOL>TRUE?FALSE<EOF>
                case Headers.JoinRoom:
                    client.MessageClient(AddClientToRoom(commandStrings[1].Trim(), client)
                        ? FormatMessage(Headers.JoinRoomResult, "TRUE")
                        : FormatMessage(Headers.JoinRoomResult, "FALSE"));
                    break;


                // Leave room
                // LEAVEROOM<EOL>ROOMNAME<EOF>
                // Will return
                // LEAVEROOMRESULT<EOL>TRUE?FALSE<EOF>
                case Headers.LeaveRoom:
                    client.MessageClient(RemoveClientFromRoom(commandStrings[1].Trim(), client)
                        ? FormatMessage(Headers.LeaveRoomResult, "TRUE")
                        : FormatMessage(Headers.LeaveRoomResult, "FALSE"));
                    break;


                // Get room members
                // ROOMMEMBERS<EOL>ROOMNAME<EOF>
                // Will return
                // ROOMMEMBERSRESULT<EOL>ROOMNAME<EOL>MEM1<EOL>MEM2<EOF>
                case Headers.RoomMembers:
                    List<string> roomMembers = new List<string> { commandStrings[1].Trim() };
                    if (GetRoomMembers(commandStrings[1].Trim(), ref roomMembers))
                    {
                        client.MessageClient(FormatMessage(Headers.RoomMembersResult, roomMembers.ToArray()));
                    }
                    break;


                // Message to room
                // ROOMMSG<EOL>ROOMNAME<EOL>MSG<EOF>
                // Will return to client
                // ROOMMSGRETURN<EOL>TRUE?FALSE<EOF>
                // Will send to all room members
                // ROOMMSGDATA<EOL>ROOMNAME<EOL>SENDERNAME<EOL>MSG<EOF>
                // msg will be formatted in <username> CLIENTMSG
                case Headers.RoomMsg:
                    client.MessageClient(MessageRoomMembers(commandStrings[1].Trim(), commandStrings[2].Trim(), client)
                        ? FormatMessage(Headers.RoomMsgReturn, "TRUE")
                        : FormatMessage(Headers.RoomMsgReturn, "FALSE"));
                    break;


                // Message from client to client
                // PRIVMSG<EOL>DESTUSERNAME<EOL>MSG<EOF>
                // Will return to client
                // PRIVMSGRETURN<EOL>TRUE?FALSE<EOF>
                // Will send to dest client
                // PRIVMSGDATA<EOL>FROMUSERNAME<EOL>TOUSERNAME<EOL>MSG<EOF>
                case Headers.PrivMsg:
                    client.MessageClient(MessageMember(commandStrings[1].Trim(), commandStrings[2].Trim(), client)
                        ? FormatMessage(Headers.PrivMsgReturn, "TRUE")
                        : FormatMessage(Headers.PrivMsgReturn, "FALSE"));
                    break;


                // CLIENTDISCONNECTING<EOF>
                // Will return to all
                // CLIENTDISCONNECTED<EOL>USERNAME<EOF>
                case Headers.ClientDisconnecting:
                    RemoveClientFromAll(client);
                    break;


                // Ping pong
                // PING<EOF>
                // Will return
                // PONG<EOF>
                case Headers.Ping:
                    client.MessageClient(FormatMessage(Headers.Pong));
                    break;


                default:
                    // We don't know the command
                    break;
            }
        }
    }
}