using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Config.Net;

namespace IRC_Server
{
    public static class Globals
    {
        public static IMySettings Settings = new ConfigurationBuilder<IMySettings>().UseJsonFile("settings.json").Build();
        public static X509Certificate2 ServerCertificate;
        public static Socket Listener;
        public static SynchronizedCollection<ClientInfo> ClientList = new SynchronizedCollection<ClientInfo>();
        public static SynchronizedCollection<ServerRoom> ServerRooms = new SynchronizedCollection<ServerRoom>();
        public static Random Random = new Random();
    }

    public interface IMySettings
    {
        [Option(DefaultValue = "127.0.0.1")]
        string ServerIp { get; set; }

        [Option(DefaultValue = 6697)]
        int ServerPort { get; set; }

        [Option(DefaultValue = "IRCert.pfx")]
        string ServerCertificatePath { get; set; }

        [Option(DefaultValue = "password")]
        string ServerCertificatePassword { get; set; }
    }
}