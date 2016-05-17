using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionKeeper
{


    public enum MissionType
    {
        Sea,
        Air,
        Land
    }

    public class Mission
    {

        public Mission()
        {

            moVideoStreams = new List<MissionStream>();
        }

        public MissionType Type { get; set; }

        public bool IsHidden { get; set; }

        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public DateTime Comex { get; set; }

        public DateTime Finex { get; set; }

        public int ApiKey { get; set; }

        public string SessionID { get; set; }

        public string Owner { get; set; }

        public string[] Roles { get; set; }

        private List<MissionStream> moVideoStreams;
        public IEnumerable<MissionStream> VideoStreams
        {
            get
            {
                return moVideoStreams;
            }
            set
            {
                moVideoStreams = value.ToList();
            }
        }

        internal void SetupStream(Guid streamID, string name, string uri, string thumbnail, bool isDefault = false, string telemetryFile = "")
        {

            var nuStream = new MissionStream()
            {
                MissionID = this.ID,
                StreamID = streamID,
                Name = name,
                VideoURI = uri,
                ThumbnailURI = thumbnail,
                IsDefault = isDefault,

            };

            this.moVideoStreams.Add(nuStream);
        }
    }

}
