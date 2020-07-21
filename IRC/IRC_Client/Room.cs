using System;
using System.Collections.Generic;

namespace IRC_Client
{
    public class Message
    {
        public DateTime Time;
        public string Username;
        public string Msg;

        /// <summary>
        /// Constructor for creating a message
        /// </summary>
        /// <param name="time">Date and time of the message</param>
        /// <param name="username">Username of who sent the message</param>
        /// <param name="message">The message</param>
        public Message(DateTime time, string username, string message)
        {
            Time = time;
            Username = username;
            Msg = message;
        }

        /// <summary>
        /// Formats the message into a string as
        /// </summary>
        /// <returns>Message as a string</returns>
        public override string ToString()
        {
            return $"{Time.ToLongTimeString()} <{Username}> {Msg}";
        }
    }

    public class Room
    {
        private string _roomName;
        private readonly List<Message> _messageList = new List<Message>();
        private readonly List<string> _memberList = new List<string>();
        private readonly bool _privateRoom;
        private bool _isOwner;

        /// <summary>
        /// Constructor for creating a room
        /// </summary>
        /// <param name="roomName">Name of the room</param>
        /// <param name="privateRoom">Whether the room is private</param>
        /// <param name="isOwner">If the client is the owner of the room</param>
        public Room(string roomName, bool privateRoom, bool isOwner)
        {
            _roomName = roomName;
            _privateRoom = privateRoom;
            _isOwner = isOwner;
        }

        /// <summary>
        /// Adds a message to the room
        /// </summary>
        /// <param name="message">Message to add</param>
        public void AddMessage(Message message)
        {
            Globals.ClientForm.AddMessage(_roomName, message.ToString());// Also try to add the message to the message list box
            _messageList.Add(message);
        }

        /// <summary>
        /// Get the list of messages
        /// </summary>
        /// <returns>A list of messages</returns>
        public List<Message> MessageList()
        {
            return _messageList;
        }

        /// <summary>
        /// Adds a member to the room
        /// </summary>
        /// <param name="username">Username of member to add</param>
        public void AddMember(string username)
        {
            Globals.ClientForm.AddMember(_roomName, username);
            _memberList.Add(username);
        }

        /// <summary>
        /// Get the list of members
        /// </summary>
        /// <returns>A list of members</returns>
        public List<string> MemberList()
        {
            return _memberList;
        }

        /// <summary>
        /// Check if a member is in the room
        /// </summary>
        /// <param name="member">The member to query</param>
        /// <returns>True if the member is in the room and false otherwise</returns>
        public bool ContainsMember(string member)
        {
            return _memberList.Contains(member);
        }

        /// <summary>
        /// Sets the name of the room
        /// </summary>
        /// <param name="roomName">The name to set the room to</param>
        public void SetRoomName(string roomName)
        {
            _roomName = roomName;
        }

        /// <summary>
        /// Get the name of the room
        /// </summary>
        /// <returns>The name of the room</returns>
        public string RoomName()
        {
            return _roomName;
        }

        /// <summary>
        /// Changes the name of a member in the room
        /// </summary>
        /// <param name="oldUsername">The old username of the member</param>
        /// <param name="newUsername">The new username of the member</param>
        public void ChangeUsername(string oldUsername, string newUsername)
        {
            //if we remove the old username then it existed in this room so we can add in the new one
            if (_memberList.Remove(oldUsername))
            {
                _memberList.Add(newUsername);
            }

            //change username in the messages
            foreach (var message in _messageList)
            {
                if (message.Username == oldUsername)
                {
                    message.Username = newUsername;
                }
            }
        }

        /// <summary>
        /// Returns whether the room is private or not
        /// </summary>
        /// <returns>True if the room is private and false otherwise</returns>
        public bool PrivateRoom()
        {
            return _privateRoom;
        }

        /// <summary>
        /// Returns whether the client is the owner of the room
        /// </summary>
        /// <returns>True if the client is the owner and false otherwise</returns>
        public bool IsOwner()
        {
            return _isOwner;
        }

        /// <summary>
        /// Sets the client as the owner of the room
        /// </summary>
        public void SetAsOwner()
        {
            _isOwner = true;
        }
    }
}
