using System;
using System.Linq;
using System.Text;
using IRC_Server;

namespace IRC_Client
{
    class HelperMethods
    {
        /// <summary>
        /// Formats the message to send to the client from a header and the parameters
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
        /// Changes a members username
        /// </summary>
        /// <param name="oldUsername">The old username</param>
        /// <param name="newUsername">The new username</param>
        /// <param name="client">ClientInfo that we're working with</param>
        public static void ChangeUsername(string oldUsername, string newUsername, ClientInfo client)
        {
            foreach (var room in client.RoomList())
            {
                room.ChangeUsername(oldUsername, newUsername);
            }
            Globals.ClientForm.ReloadMembers();
        }

        /// <summary>
        /// Handles the commands sent from the client to the server and responds.
        /// </summary>
        /// <param name="command">The command received from the client</param>
        /// <param name="client">Client that sent the command</param>
        public static void CommandHandler(string command, ClientInfo client)
        {
            // Our format is
            // COMMAND<EOL>COMMAND DATA<EOF>
            Room curRoom;
            string[] commandStrings = command.Split(new[] { "<EOL>" }, StringSplitOptions.None);
            switch (commandStrings[0])
            {
                // USERNAMECHANGE<EOL>OLDUSERNAME<EOL>NEWUSERNAME<EOF>
                case Headers.UsernameChange:
                    ChangeUsername(commandStrings[1].Trim(), commandStrings[2].Trim(), client);
                    break;

                // USERNAMERESULT<EOL>TRUE?FALSE?USERNAME<EOL>NOTE<EOF>
                case Headers.UsernameResult:
                    if (commandStrings[1].Trim() == "FALSE")
                    {
                        // Username change error. Set name to local one
                        string newUsername = commandStrings[2].Trim();
                        ChangeUsername(client.GetUsername(), newUsername, client);
                        client.SetUsername(newUsername);
                        client.GetDebugRoom().AddMessage(new Message(DateTime.Now, "CommandHandler() Username Result", commandStrings[3].Trim()));
                    }
                    else if (commandStrings[1].Trim() == "TRUE")
                    {
                        client.GetDebugRoom().AddMessage(new Message(DateTime.Now, "CommandHandler() Username Result", "Username Changed"));
                        Globals.Settings.Username = commandStrings[2].Trim();
                    }
                    break;

                // MAKEROOMRESULT<EOL>TRUE?FALSE<EOL>NOTE<EOL>ROOMCREATEDNAME<EOF>
                case Headers.MakeRoomResult:
                    client.GetDebugRoom().AddMessage(new Message(DateTime.Now, "CommandHandler() Room Creation Result", commandStrings[2].Trim()));
                    if (commandStrings[1].Trim() == "TRUE")
                    {
                        if (client.ContainsRoom(commandStrings[3].Trim()))
                        {
                            // Room got added before result came in so we find it and set it that we are the owner
                            client.GetRoom(commandStrings[3].Trim()).SetAsOwner();
                        }
                        else
                        {
                            // Otherwise create the room and set it as owner from the beginning
                            Room newRoom = new Room(commandStrings[3].Trim(), false, true);
                            client.AddRoom(newRoom);

                            // Now get the members of this new room
                            client.MessageClient(FormatMessage(Headers.RoomMembers, commandStrings[3].Trim()));
                        }
                    }
                    break;

                // GETROOMSRESULT<EOL>RM1<EOL>RM2<EOF>
                case Headers.GetRoomsResult:
                    foreach (var room in commandStrings.Skip(1))
                    {
                        if (!client.ContainsRoom(room))
                        {
                            Room newRoom = new Room(room, false, false);
                            client.AddRoom(newRoom);

                            // Now get the members of this new room
                            client.MessageClient(FormatMessage(Headers.RoomMembers, room));
                        }
                    }
                    break;

                // REMOVEROOMRESULT<EOL>TRUE?FALSE<EOL>NOTE<EOF>
                case Headers.RemoveRoomResult:
                    client.GetDebugRoom().AddMessage(new Message(DateTime.Now, "CommandHandler() Room Removal Result", commandStrings[2].Trim()));
                    break;

                // ROOMREMOVED<EOL>ROOMNAME<EOF>
                case Headers.RoomRemoved:
                    if (client.RemoveRoom(commandStrings[1].Trim()))
                    {
                        client.GetDebugRoom().AddMessage(new Message(DateTime.Now, "CommandHandler() Room Removed", $"Successfully removed room {commandStrings[1].Trim()}"));
                    }
                    else
                    {
                        client.GetDebugRoom().AddMessage(new Message(DateTime.Now, "CommandHandler() Room Removal Error", $"Error removing room {commandStrings[1].Trim()}"));
                    }
                    break;

                // ROOMMSGDATA<EOL>ROOMNAME<EOL>SENDERNAME<EOL>MSG<EOF>
                case Headers.RoomMsgData:
                    curRoom = client.GetRoom(commandStrings[1].Trim());
                    if (curRoom != null)
                    {
                        Message message = new Message(DateTime.Now, commandStrings[2].Trim(), commandStrings[3].Trim());
                        curRoom.AddMessage(message);
                    }
                    break;

                // ROOMMEMBERSRESULT<EOL>ROOMNAME<EOL>MEM1<EOL>MEM2<EOF>
                case Headers.RoomMembersResult:
                    curRoom = client.GetRoom(commandStrings[1].Trim());
                    if (curRoom != null)
                    {
                        curRoom.MemberList().Clear();
                        Globals.ClientForm.ClearMembers(curRoom.RoomName());
                        foreach (var member in commandStrings.Skip(2))// skip command and roomname
                        {
                            curRoom.AddMember(member);
                        }
                    }
                    break;

                // JOINROOMRESULT<EOL>TRUE?FALSE<EOF>
                case Headers.JoinRoomResult:
                    if (commandStrings[1].Trim() == "TRUE")
                    {
                        Globals.ClientForm.SetChatBox(true);
                    }
                    break;

                // PRIVMSGDATA<EOL>FROMUSERNAME<EOL>TOUSERNAME<EOL>MSG<EOF>
                case Headers.PrivMsgData:
                    if (!client.ContainsRoom(commandStrings[1].Trim()) && client.GetUsername() != commandStrings[1].Trim())
                    {
                        Room newRoom = new Room(commandStrings[1].Trim(), true, false);
                        newRoom.AddMember(client.GetUsername());// Add yourself
                        newRoom.AddMember(commandStrings[1].Trim());// Add other user
                        client.AddRoom(newRoom);
                    }

                    if (client.GetUsername() == commandStrings[1].Trim() && client.GetRoom(commandStrings[2].Trim()) != null)
                    {
                        client.GetRoom(commandStrings[2].Trim()).AddMessage(new Message(DateTime.Now, commandStrings[1].Trim(), commandStrings[3].Trim()));
                    }
                    else
                    {
                        client.GetRoom(commandStrings[1].Trim()).AddMessage(new Message(DateTime.Now, commandStrings[1].Trim(), commandStrings[3].Trim()));
                    }
                    
                    break;

                // CLIENTDISCONNECTED<EOL>USERNAME<EOF>
                case Headers.ClientDisconnected:
                    if (client.RemoveRoom(commandStrings[1].Trim()))
                    {
                        client.GetDebugRoom().AddMessage(new Message(DateTime.Now, "CommandHandler() Member Left", $"Member {commandStrings[1].Trim()} removed from PM's"));
                    }
                    break;

                // SERVERDISCONNECTING<EOF>
                // Server is closing so we stop
                // Forced because we don't need
                // to notify the server about anything
                case Headers.ServerDisconnecting:
                    client.GetDebugRoom().AddMessage(new Message(DateTime.Now, "CommandHandler()", $"Server is disconnecting, stopping client"));
                    client.StopClient(true);
                    break;

                default:
                    // We don't know the command
                    break;
            }
        }
    }
}
