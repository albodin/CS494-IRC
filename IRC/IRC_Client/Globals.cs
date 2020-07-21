using Config.Net;

namespace IRC_Client
{
    class Globals
    {
        public static IMySettings Settings = new ConfigurationBuilder<IMySettings>().UseJsonFile("settings.json").Build();
        public static ClientForm ClientForm;
        public static ClientInfo Client = null;
    }

    public interface IMySettings
    {
        [Option(DefaultValue = "127.0.0.1")]
        string ServerIp { get; set; }

        [Option(DefaultValue = 6697)]
        int ServerPort { get; set; }

        [Option(DefaultValue = "IRC Project")]
        string HostName { get; set; }

        [Option(DefaultValue = "user")]
        string Username { get; set; }

        [Option(DefaultValue = true)]
        bool UseSsl { get; set; }
    }
}
