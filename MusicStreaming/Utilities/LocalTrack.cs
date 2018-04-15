using System.Xml.Serialization;

namespace Utilities
{
    public class LocalTrack
    {
        /// <summary>
        /// liste des properties (vocabulaire du langage C#)
        /// </summary>
        public string Id { get; set; }
        public string Name { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        [XmlIgnore]
        public string Image { get; set; }
        public double Duration { get; set; }
        public string DurationMin { get { return Duration + " Min"; } }
        public StreamingSystemType Type { get; set; }
    }
}
