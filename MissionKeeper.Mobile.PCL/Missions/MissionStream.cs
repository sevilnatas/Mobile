using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionKeeper
{

    public class MissionStream
    {

        public Guid StreamID { get; set; }

        public Guid MissionID { get; set; }

        public bool IsDefault { get; set; }

        public string Name { get; set; }

        public string Platform { get; set; }

        public string VideoURI { get; set; }

        public string ThumbnailURI { get; set; }

        public int Color { get; set; }
       
    }

}
