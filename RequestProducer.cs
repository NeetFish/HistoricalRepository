using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;


namespace WebApplication2
{
    public static class RequestProducer
    {
        public static string GetEarhquakeRequestString(Earthquake.DataSource datasource, string startdate, string enddate, string min_magnitude, string max_magnitude)
        {
            string requestString = "";
            string prefix;
            string latLng;
            string time;
            string magnitude;

            switch (datasource)
            {
                case Earthquake.DataSource.CKAN:

                    //
                    // ****************************
                    // *                          *
                    // *   CKAN                   *
                    // *                          *
                    // ****************************
                    //
                    //  The format of the link : 
                    //
                    //      Domain                  :  ConstCkan.CkanAPIPrefix
                    //
                    //      Organization            :  organization_show?id=earthquake&
                    //
                    //      Need to show dataset    :  include_datasets =true
                    //
                    //      The response data type is JSON.
                    //

                    prefix = ConstCkan.CkanAPIPrefix + "organization_show?id=earthquake&include_datasets=true";
                    latLng = "";
                    time = startdate + "&" + enddate;
                    magnitude = "";
                    requestString = prefix;
                    break;

                case Earthquake.DataSource.CWB:

                    //
                    // ****************************
                    // *                          *
                    // *   CWB                    *
                    // *                          *
                    // ****************************
                    //
                    //  The format of the link : Domain + Time
                    //
                    //      Prefix      :   http://scweb.cwb.gov.tw/GraphicContent.aspx?ItemId=20&
                    //      Time        :   Date=201603
                    //                          (All the earthquakes in 2016/03)
                    //
                    //  The response data is HTML text.
                    //  The data of the earthqukes are in each row except the first row of the table datalist4 
                    //
                    prefix = "http://scweb.cwb.gov.tw/GraphicContent.aspx?ItemId=20&Date=";
                    DateTime date_i = DateTime.Parse(startdate);
                    requestString = prefix + date_i.ToString("yyyyMM");
                    break;

                case Earthquake.DataSource.NOAA:
                    break;

                case Earthquake.DataSource.PALERT:

                    //
                    // ****************************
                    // *                          *
                    // *   P-ALERT                *
                    // *                          *
                    // ****************************
                    //
                    //  The format of the link : Domain + LatLng + Time + Intensity
                    //  * The "Intensity" of P-ALERT actually means "Magnitude".
                    //
                    //      Prefix      :   http://palert.earth.sinica.edu.tw/db/querydb.php?
                    //      LatLng      :   lat%5B%5D=21&lat%5B%5D=26&lng%5B%5D=119&lng%5B%5D=123&
                    //                          (from latitude 21~26, longitude 119~123)
                    //      Time        :   time%5B%5D=2016-03-01&time%5B%5D=2016-03-31&
                    //                          (from 2016/03/01 to 2016/03/31)
                    //      Intensity   :   intensity%5B%5D=1&intensity%5B%5D=7
                    //                          (from intensity 1 to intensity 7)
                    //
                    //      The response data type is JSON.
                    //
                    //      If there is no earthquake data, P-ALERT will response "false".
                    //

                    prefix = "http://palert.earth.sinica.edu.tw/db/querydb.php?";
                    latLng = "lat%5B%5D=21&lat%5B%5D=26&lng%5B%5D=119&lng%5B%5D=123&";
                    time = "time%5B%5D=" + startdate + "&time%5B%5D=" + enddate + "&";
                    magnitude = "intensity%5B%5D=" + min_magnitude + "&intensity%5B%5D=" + max_magnitude;

                    requestString = prefix + latLng + time + magnitude;
                    break;
            }

            return requestString;
        }
    }
}