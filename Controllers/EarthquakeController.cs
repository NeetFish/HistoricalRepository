using WebApplication2.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace WebApplication2.Controllers
{
    public class EarthquakeController : ApiController
    {

        List<Earthquake> EARTHQUAKE_LIST = new List<Earthquake>();

 /*       public IEnumerable<Palert> GetAllProducts()
        {
            return "";
        }
 */
        public IHttpActionResult GetProduct(string id)
        {

            EARTHQUAKE_LIST.Clear();

            // The format of id : YYYY-MM-DD$yyyy-mm-dd
            // Upper case : start date; Lower case : end date
            string[] dateTemp = id.Split('$');

            string startDate_s = dateTemp[0];
            string endDate_s = dateTemp[1];

            // [0] = year, [1] = month, [2] = day
            string[] startDate_s_ = startDate_s.Split('-');
            string[] endDate_s_ = endDate_s.Split('-');

            DateTime startDate_dt = new DateTime(int.Parse(startDate_s_[0]), int.Parse(startDate_s_[1]), int.Parse(startDate_s_[2]));
            DateTime endDate_dt = new DateTime(int.Parse(endDate_s_[0]), int.Parse(endDate_s_[1]), int.Parse(endDate_s_[2]));

            string data = null;

            EarthquakeObjConverter eqConverter = new EarthquakeObjConverter();
            List<Earthquake> tempEqList = new List<Earthquake>();

            //
            // ****************************
            // *                          *
            // *   P-ALERT                *
            // *                          *
            // ****************************
            //
            //  The format of the link : Domain + LatLng + Time + Intensity
            //
            //      Domain      :   http://palert.earth.sinica.edu.tw/db/querydb.php?
            //      LatLng     :   lat%5B%5D=21&lat%5B%5D=26&lng%5B%5D=119&lng%5B%5D=123&
            //                          (from latitude 21~26, longitude 119~123)
            //      Time        :   time%5B%5D=2016-03-01&time%5B%5D=2016-03-31&
            //                          (from 2016/03/01 to 2016/03/31)
            //      Intensity   :   intensity%5B%5D=1&intensity%5B%5D=7
            //                          (from intensity 1 to intensity 7)
            //
            //      The response data type is JSON.
            //

            string Domain = "http://palert.earth.sinica.edu.tw/db/querydb.php?";
            string LatLng = "lat%5B%5D=21&lat%5B%5D=26&lng%5B%5D=119&lng%5B%5D=123&";
            string Time = "time%5B%5D=" + startDate_s + "&time%5B%5D=" + endDate_s + "&";
            string Intensity = "intensity%5B%5D=1&intensity%5B%5D=7";


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Domain + LatLng + Time + Intensity);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json";
            
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        
                        data = reader.ReadToEnd();

                        tempEqList = eqConverter.EarthquakeTrans(data, Earthquake.DataSource.PALERT, Domain, LatLng, Time, Intensity);
                        EARTHQUAKE_LIST.AddRange(tempEqList);

                        reader.Close();
                        reader.Dispose();
                        
                    }
                }
            }

            //
            // ****************************
            // *                          *
            // *   CWB                    *
            // *                          *
            // ****************************
            //
            //  The format of the link : Domain + Time
            //
            //      Domain      :   http://scweb.cwb.gov.tw/GraphicContent.aspx?ItemId=20&
            //      Time        :   Date=201603
            //                          (All the earthquakes in 2016/03)
            //
            //  The response data is HTML text.
            //  The data of the earthqukes are in each row except the first row of the table datalist4 
            //

            Domain = "http://scweb.cwb.gov.tw/GraphicContent.aspx?ItemId=20&Date=";

            for (DateTime date_i = new DateTime(startDate_dt.Year, startDate_dt.Month, 1); date_i <= endDate_dt; date_i = date_i.AddMonths(1))
            {
                request = (HttpWebRequest)WebRequest.Create(Domain + date_i.ToString("yyyyMM"));
                request.Method = WebRequestMethods.Http.Get;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {

                            data = reader.ReadToEnd();

                            tempEqList = eqConverter.EarthquakeTrans(data, Earthquake.DataSource.CWB, Domain, LatLng, Time, Intensity);
                            EARTHQUAKE_LIST.AddRange(tempEqList);

                            reader.Close();
                            reader.Dispose();
                        }

                    }
                }
            }




            //
            // ****************************
            // *                          *
            // *   CKAN                   *
            // *                          *
            // ****************************
            //
            //  The format of the link : Domain + LatLng + Time + Intensity
            //
            //      Domain      :   http://palert.earth.sinica.edu.tw/db/querydb.php?
            //      LatLng     :   lat%5B%5D=21&lat%5B%5D=26&lng%5B%5D=119&lng%5B%5D=123&
            //                          (from latitude 21~26, longitude 119~123)
            //      Time        :   time%5B%5D=2016-03-01&time%5B%5D=2016-03-31&
            //                          (from 2016/03/01 to 2016/03/31)
            //      Intensity   :   intensity%5B%5D=1&intensity%5B%5D=7
            //                          (from intensity 1 to intensity 7)
            //
            //      The response data type is JSON.
            //

            Domain = "http://140.109.17.71/api/3/action/package_show?id=earthquake";
            LatLng = "";
            Time = startDate_s + "&" + endDate_s;
            Intensity = "";


            request = (HttpWebRequest)WebRequest.Create(Domain);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json";

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {

                        data = reader.ReadToEnd();

                        tempEqList = eqConverter.EarthquakeTrans(data, Earthquake.DataSource.CKAN, Domain, LatLng, Time, Intensity);
                        EARTHQUAKE_LIST.AddRange(tempEqList);

                        reader.Close();
                        reader.Dispose();

                    }
                }
            }


            // Sort by date
            EARTHQUAKE_LIST.Sort((x, y) => { return (x.timeString.CompareTo(y.timeString)); });

            // Cut the data which are not in the date range
            int eqListCount = EARTHQUAKE_LIST.Count();
            for (int i = 0; i < eqListCount; i++)
            {
                DateTime temp = new DateTime(int.Parse(EARTHQUAKE_LIST[i].timeString.Substring(0, 4)),
                                int.Parse(EARTHQUAKE_LIST[i].timeString.Substring(4, 2)), int.Parse(EARTHQUAKE_LIST[i].timeString.Substring(6, 2)));

                if (temp > endDate_dt || temp < startDate_dt)
                {
                    EARTHQUAKE_LIST.RemoveAt(i);
                    eqListCount--;
                    i--;
                }
            }

            if (data == null)
            {
                return NotFound();
            }

            return Ok(EARTHQUAKE_LIST);
        }

        
    }

    
}
