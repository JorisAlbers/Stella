using System.Xml;
using System.Xml.Serialization;

namespace StellaServer
{
    public class UserSettings
    {
        public ServerSetupSettings ServerSetup { get; set; }
    }

    [XmlType]
    public class ServerSetupSettings
    {
        [XmlAttribute]
        public string ServerIp { get; set; }
        [XmlAttribute]
        public int ServerTcpPort { get; set; }
        [XmlAttribute]
        public int ServerUdpPort { get; set; }
        [XmlElement]
        public string MappingFilePath { get; set; }
        [XmlElement]
        public string BitmapFolder { get; set; }
        [XmlElement]
        public string StoryboardFolder { get; set; }
        [XmlElement]
        public int MaximumFrameRate { get; set; }
    }
}