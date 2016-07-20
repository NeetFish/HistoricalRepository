using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2
{
    // This is the class of query data of earthquake
    //
    //          The parameters :
    //                          startdate
    //                              -> the start date of the search range
    //                                 Format  : YYYY-MM-DD
    //                                 DEFAULT : 1900-01-01
    //                          enddate
    //                              -> the end date of the search range
    //                                 Format  : YYYY-MM-DD
    //                                 DEFAULT : today
    //                          min_magnitude
    //                              -> the minimum search magnitude of the earthquake
    //                                 DEFAULT : 0.0
    //                          max_magnitude
    //                              -> the maximum search magnitude of the earthquake
    //                                 DEFAULT : 10.0

    public class QueryEarthquake
    {
        // The default value
        private string STARTDATE_DEFAULT = "1900-01-01";
        private string ENDDATE_DEFAULT = DateTime.Today.ToString("yyyy-MM-dd");
        private string MIN_MAG_DEFAULT = "0.0";
        private string MAX_MAG_DEFAULT = "10.0";

        //
        private string _startdate;
        private string _enddate;
        private string _min_magnitude;
        private string _max_magnitude;

        public string startdate
        {
            get { return _startdate; }
            set
            {  
                // Format : YYYY-MM-DD
                // Format check
                try
                {
                    string[] dateChk = value.Split('-');
                    DateTime tryout = new DateTime(int.Parse(dateChk[0]), int.Parse(dateChk[1]), int.Parse(dateChk[2]));
                    _startdate = value;                  
                }
                catch
                {
                    _startdate = STARTDATE_DEFAULT;
                }
            }
        }

        public string enddate
        {
            get { return _enddate; }
            set
            {   
                // Format : YYYY-MM-DD
                // Format check
                try
                {
                    string[] dateChk = value.Split('-');
                    DateTime tryout = new DateTime(int.Parse(dateChk[0]), int.Parse(dateChk[1]), int.Parse(dateChk[2]));
                    _enddate = value;
                }
                catch
                {
                    _enddate = ENDDATE_DEFAULT;
                }
            }
        }

        public string min_magnitude
        {
            get { return _min_magnitude; }
            set
            {
                // Format : must be a float
                // Format check
                float tryout;

                if (!(float.TryParse(value, out tryout)))
                {
                    _min_magnitude = MIN_MAG_DEFAULT;
                }
                else
                    _min_magnitude = value;
            }
        }

        public string max_magnitude
        {
            get { return _max_magnitude; }
            set
            {
                // Format : must be a float
                // Format check
                float tryout;

                if (!(float.TryParse(value, out tryout)))
                {
                    _max_magnitude = MAX_MAG_DEFAULT;
                }
                else
                    _max_magnitude = value;
            }
        }

        public QueryEarthquake()
        {
            // Set to DEFAULT
            _startdate = STARTDATE_DEFAULT;
            _enddate = ENDDATE_DEFAULT;
            _min_magnitude = MIN_MAG_DEFAULT;
            _max_magnitude = MAX_MAG_DEFAULT;
        }

    }
}
 