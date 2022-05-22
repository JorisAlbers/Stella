namespace StellaServerLib.Animation.Mapping
{
    public class ClientMapping
    {
        public int Index { get; }
        public string Mac { get; }

        public ClientMapping(int index, string mac)
        {
            Index = index;
            Mac = mac;
        }
    }
}