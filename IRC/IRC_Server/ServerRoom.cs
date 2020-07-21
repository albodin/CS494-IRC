using System.Collections.Generic;

namespace IRC_Server
{
    public class ServerRoom
    {
        private readonly string _roomName;
        private readonly ClientInfo _roomOwner;
        private readonly SynchronizedCollection<ClientInfo> _clientList = new SynchronizedCollection<ClientInfo>();


        /// <summary>
        /// Default constructor for ServerRoom, sets values to null
        /// </summary>
        public ServerRoom()
        {
            _roomName = "";
            _roomOwner = null;
        }

        /// <summary>
        /// ServerRoom constructor
        /// </summary>
        /// <param name="roomName">Unique room name</param>
        /// <param name="roomOwner">The rooms owner</param>
        public ServerRoom(string roomName, ClientInfo roomOwner)
        {
            _roomName = roomName;
            _roomOwner = roomOwner;
        }

        /// <summary>
        /// Messages all of the clients in the room
        /// </summary>
        /// <param name="message">The message to send already formatted</param>
        public void MessageClients(string message)
        {
            foreach (var client in _clientList)
            {
                client.MessageClient(message);
            }
        }

        /// <summary>
        /// Add a client to the server room
        /// </summary>
        /// <param name="client">Client to add</param>
        /// <returns>true if added and false if client already in the room</returns>
        public bool AddClient(ClientInfo client)
        {
            if (_clientList.Contains(client))
            {
                return false;
            }
            else
            {
                _clientList.Add(client);
                return true;
            }
        }

        /// <summary>
        /// Removes a client from the server room
        /// </summary>
        /// <param name="client">Client to remove</param>
        /// <returns>true if removed and false if failure to remove</returns>
        public bool RemoveClient(ClientInfo client)
        {
            return _clientList.Remove(client);
        }

        /// <summary>
        /// Removes all clients from the room
        /// </summary>
        public void RemoveAllClients()
        {
            _clientList.Clear();
        }

        /// <summary>
        /// Gets all room clients usernames
        /// </summary>
        /// <returns>list of strings</returns>
        public List<string> GetAllClientsNames()
        {
            List<string> memberNames = new List<string>();
            foreach (var client in _clientList)
            {
                memberNames.Add(client.GetUsername());
            }

            return memberNames;
        }

        /// <summary>
        /// Get the rooms name
        /// </summary>
        /// <returns>Room name</returns>
        public string RoomName()
        {
            return _roomName;
        }

        /// <summary>
        /// Check if client is owner of the room
        /// </summary>
        /// <param name="client">Client to check</param>
        /// <returns>true if client is the owner and false if not</returns>
        public bool IsOwner(ClientInfo client)
        {
            return client == _roomOwner;
        }

        /// <summary>
        /// Check if client is in room
        /// </summary>
        /// <param name="client">Client to check</param>
        /// <returns>True if client is in the room and false if not</returns>
        public bool Contains(ClientInfo client)
        {
            return _clientList.Contains(client);
        }
    }
}
