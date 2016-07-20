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


        public IHttpActionResult GetEarthquake([FromUri]QueryEarthquake qe)
        {

            EARTHQUAKE_LIST.Clear();
            
            string startdate = qe.startdate;
            string enddate = qe.enddate;
            string min_magnitude = qe.min_magnitude;
            string max_magnitude = qe.max_magnitude;
            DateTime startDate_dt = new DateTime(int.Parse(startdate.Substring(0, 4)), int.Parse(startdate.Substring(5, 2)), int.Parse(startdate.Substring(8, 2)));
            DateTime endDate_dt = new DateTime(int.Parse(enddate.Substring(0, 4)), int.Parse(enddate.Substring(5, 2)), int.Parse(enddate.Substring(8, 2)));
            
            string data = null;
            string requestString = "";

            EarthquakeObjConverter eqConverter = new EarthquakeObjConverter();
            List<Earthquake> tempEqList = new List<Earthquake>();

            //
            //   ****************************
            //   *                          *
            //   *   P-ALERT                *
            //   *                          *
            //   ****************************
            //
            //
            //    The response data type is JSON.
            //
            //    If there is no earthquake data, P-ALERT will response "false".
            //

            requestString = RequestProducer.GetEarhquakeRequestString(Earthquake.DataSource.PALERT, startdate, enddate, min_magnitude, max_magnitude);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
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
                        //
                        //  if there is no earthquake data -> the data will be "false"
                        //
                        if (data != "false")
                        {
                            tempEqList = eqConverter.EarthquakeTrans(data, Earthquake.DataSource.PALERT, startdate, enddate);
                            EARTHQUAKE_LIST.AddRange(tempEqList);
                        }

                        reader.Close();
                        reader.Dispose();
                        
                    }
                }
            }

            //
            //   ****************************
            //   *                          *
            //   *   CWB                    *
            //   *                          *
            //   ****************************
            //
            //    The response data is HTML text.
            //    The data of the earthqukes are in each row except the first row of the table datalist4 
            //

            // The oldest date of data in CWB website is in 1995/01
            DateTime date_i;
            if (startDate_dt.Year < 1995)
                date_i = new DateTime(1995, 1, 1);
            else
                date_i = new DateTime(startDate_dt.Year, startDate_dt.Month, 1);

            for (; date_i <= endDate_dt; date_i = date_i.AddMonths(1))
            {
                requestString = RequestProducer.GetEarhquakeRequestString(Earthquake.DataSource.CWB, date_i.ToString(), enddate, min_magnitude, max_magnitude);
                request = (HttpWebRequest)WebRequest.Create(requestString);
                request.Method = WebRequestMethods.Http.Get;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {

                            data = reader.ReadToEnd();

                            tempEqList = eqConverter.EarthquakeTrans(data, Earthquake.DataSource.CWB, startdate, enddate);
                            EARTHQUAKE_LIST.AddRange(tempEqList);

                            reader.Close();
                            reader.Dispose();
                        }
                    }
                }
            }


            //
            //   ****************************
            //   *                          *
            //   *   CKAN                   *
            //   *                          *
            //   ****************************
            //
            //
            //    The response data type is JSON.
            //

            requestString = RequestProducer.GetEarhquakeRequestString(Earthquake.DataSource.CKAN, startdate, enddate, min_magnitude, max_magnitude);
            request = (HttpWebRequest)WebRequest.Create(requestString);
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

                        tempEqList = eqConverter.EarthquakeTrans(data, Earthquake.DataSource.CKAN, startdate, enddate);
                        EARTHQUAKE_LIST.AddRange(tempEqList);

                        reader.Close();
                        reader.Dispose();

                    }
                }
            }

            // Sort by date
            EARTHQUAKE_LIST.Sort((x, y) => { return (x.timeString.CompareTo(y.timeString)); });

            // Cut the data which are not in the search range
            // (date, magnitude... etc)
            int eqListCount = EARTHQUAKE_LIST.Count();
            for (int i = 0; i < eqListCount; i++)
            {
                float eqMag = float.Parse(EARTHQUAKE_LIST[i].magnitude);

                DateTime eqTime = new DateTime(int.Parse(EARTHQUAKE_LIST[i].timeString.Substring(0, 4)),
                                int.Parse(EARTHQUAKE_LIST[i].timeString.Substring(4, 2)), int.Parse(EARTHQUAKE_LIST[i].timeString.Substring(6, 2)));

                if (eqTime > endDate_dt || eqTime < startDate_dt || eqMag > float.Parse(max_magnitude) || eqMag < float.Parse(min_magnitude))
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
