using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Models
{
    public class Earthquake
    {

        public enum DataSource { CKAN, PALERT, NOAA, CWB };

        // timestring
        // Time zone : UTC/GMT+8 
        // Format : YYYYMMDDhhmm
        // Example : 2016.03.01 14:36 (GMT+8) -> 201603011436
        public string timeString { get; private set; }
        public string lat { get; private set; }
        public string lng { get; private set; }

        // Depth
        // Unit : KM
        public string depth { get; private set; }
        public string magnitude { get; private set; }
        public DataSource dataSource { get; private set; }
        public string data { get; private set; }

        public string dataLink { get; private set; }

        public Earthquake(string timeString, string lat, string lng, string depth, string magnitude, DataSource dataSource, string data, string dataLink)
        {
            this.timeString = timeString;
            this.lat = lat;
            this.lng = lng;
            this.depth = depth;
            this.magnitude = magnitude;
            this.dataSource = dataSource;
            this.data = data;
            this.dataLink = dataLink;
        }

        public Earthquake(string timeString, string lat, string lng, string depth, string magnitude, DataSource dataSource, string dataLink)
        {
            this.timeString = timeString;
            this.lat = lat;
            this.lng = lng;
            this.depth = depth;
            this.magnitude = magnitude;
            this.dataSource = dataSource;
            this.dataLink = dataLink;
        }

    }
}