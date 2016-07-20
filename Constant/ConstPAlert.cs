using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Constant
{
    public class ConstPAlert
    {
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
        //      Domain      :   http://palert.earth.sinica.edu.tw/db/querydb.php?
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

        public const string PAlertDomain = "http://palert.earth.sinica.edu.tw/db/querydb.php?";

    }
}