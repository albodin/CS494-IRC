namespace IRC_Server
{
    class Headers
    {
        public const string Username = "USERNAME";
        public const string UsernameResult = "USERNAMERESULT";
        public const string UsernameChange = "USERNAMECHANGE";

        public const string MakeRoom = "MAKEROOM";
        public const string MakeRoomResult = "MAKEROOMRESULT";

        public const string RemoveRoom = "REMOVEROOM";
        public const string RemoveRoomResult = "REMOVEROOMRESULT";
        public const string RoomRemoved = "ROOMREMOVED";

        public const string GetRooms = "GETROOMS";
        public const string GetRoomsResult = "GETROOMSRESULT";

        public const string JoinRoom = "JOINROOM";
        public const string JoinRoomResult = "JOINROOMRESULT";

        public const string LeaveRoom = "LEAVEROOM";
        public const string LeaveRoomResult = "LEAVEROOMRESULT";

        public const string RoomMembers = "ROOMMEMBERS";
        public const string RoomMembersResult = "ROOMMEMBERSRESULT";

        public const string RoomMsg = "ROOMMSG";
        public const string RoomMsgReturn = "ROOMMSGRETURN";
        public const string RoomMsgData = "ROOMMSGDATA";

        public const string PrivMsg = "PRIVMSG";
        public const string PrivMsgReturn = "PRIVMSGRETURN";
        public const string PrivMsgData = "PRIVMSGDATA";

        public const string Ping = "PING";
        public const string Pong = "PONG";

        public const string ClientDisconnecting = "CLIENTDISCONNECTING";
        public const string ClientDisconnected = "CLIENTDISCONNECTED";

        public const string ServerDisconnecting = "SERVERDISCONNECTING";
    }
}
