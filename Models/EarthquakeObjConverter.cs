using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace WebApplication2.Models
{
    public class EarthquakeObjConverter
    {
        public EarthquakeObjConverter()
        {

        }

        List<Earthquake> eqList = new List<Earthquake>();
        private string Domain;
        private string LatLng;
        private string Time;
        private string Intensity;

        private string startTime;
        private string endTime;

        public List<Earthquake> EarthquakeTrans(object earthquakeEvent, Earthquake.DataSource dataSource, 
                                                string Domain, string LatLng, string Time, string Intensity)
        {
            this.Domain = Domain;
            this.LatLng = LatLng;
            this.Time = Time;
            this.Intensity = Intensity;
            eqList.Clear();

            if (earthquakeEvent == null)
                return null;

            switch (dataSource)
            {
                case Earthquake.DataSource.PALERT:
                    List<Palert> palertEventList = JsonConvert.DeserializeObject<List<Palert>>((string)earthquakeEvent);
                    foreach (Palert palertEvent in palertEventList)
                    {
                        Earthquake eq = palertTrans(palertEvent);
                        eqList.Add(eq);
                    }
                    return eqList;

                case Earthquake.DataSource.CWB:
                    cwbTrans((string)earthquakeEvent);
                    return eqList;

                case Earthquake.DataSource.CKAN:
                    JObject ckanlist = JObject.Parse((string)earthquakeEvent);
                    ckanTrans(ckanlist);

                    return eqList;
                default:
                    break;
            }

            // DEBUG
            return null;
        }
        public List<Earthquake> EarthquakeTrans(object earthquakeEvent, Earthquake.DataSource dataSource,
                                                string startTime, string endTime)
        {
            eqList.Clear();
            this.startTime = startTime;
            this.endTime = endTime;

            if (earthquakeEvent == null)
                return null;

            switch (dataSource)
            {
                case Earthquake.DataSource.PALERT:
                    List<Palert> palertEventList = JsonConvert.DeserializeObject<List<Palert>>((string)earthquakeEvent);
                    foreach (Palert palertEvent in palertEventList)
                    {
                        Earthquake eq = palertTrans(palertEvent);
                        eqList.Add(eq);
                    }
                    return eqList;

                case Earthquake.DataSource.CWB:
                    cwbTrans((string)earthquakeEvent);
                    return eqList;

                case Earthquake.DataSource.CKAN:
                    JObject ckanlist = JObject.Parse((string)earthquakeEvent);
                    ckanTrans(ckanlist);

                    return eqList;
                default:
                    break;
            }

            // DEBUG
            return null;
        }

        //
        // Transform palert object to earthquake object
        //
        private Earthquake palertTrans(Palert palertEvent)
        {

            // The time string format of palert : YYYYMMDDhhmmss
            // We only need YYYYMMDDhhmm
            // The time format in P-alert is UTC, we need to convert it into GMT+8       
            string timeString_ = palertEvent.Timestring;
            DateTime timeString_dt = new DateTime(int.Parse(timeString_.Substring(0, 4)), int.Parse(timeString_.Substring(4, 2)), 
                                        int.Parse(timeString_.Substring(6, 2)), int.Parse(timeString_.Substring(8, 2)), 
                                        int.Parse(timeString_.Substring(10, 2)), int.Parse(timeString_.Substring(12, 2)));

            // UTC time + 8 hours = GMT+8 (Time zone of Taiwan)
            timeString_dt = timeString_dt.AddHours(8);

            string timeString = timeString_dt.ToString("yyyyMMddHHmm");

            string lat = palertEvent.Lat;
            string lng = palertEvent.Lng;

            // The unit of depth of palert event is KM too.
            string depth = palertEvent.Depth;
            string magnitude = palertEvent.Magnitude;
            Earthquake.DataSource dataSource = Earthquake.DataSource.PALERT;

            // The format of the link of the p-alert data : Domain + Key
            //      Domain  :   http://palert.earth.sinica.edu.tw/db/index1.clt2.php?time=
            //      Key     :   YYYYMMDDhhmmss
            //      And the Key is equal to palertEvent.Timestring

            string Domain = "http://palert.earth.sinica.edu.tw/db/index1.clt2.php?time=";
            string Key = palertEvent.Timestring;

            string dataLink = Domain + Key;

            Earthquake eq = new Earthquake(timeString, lat, lng, depth, magnitude, dataSource, dataLink);

            return eq;

        }


        //
        // Transform CWB typhoon data to earthquake object
        //
        private void cwbTrans(string htmlData)
        {
            bool datalistFound = false;
            bool firstRowSkip = false;
            string[] data = htmlData.Split('\n');

            string timeString;
            string lat;
            string lng;
            string depth;
            string magnitude;
            string dataLink;

            for (int i = 0; i < data.Length; i++)
            {

                // First, find the table datalist4
                if (data[i].Contains("datalist4"))
                {
                    datalistFound = true;
                    continue;
                }

                // Skip the first row of datalist4
                if (datalistFound && !firstRowSkip)
                {
                    if (data[i].Contains("<tr>"))
                    {
                        firstRowSkip = true;
                        continue;
                    }
                }

                // Every other rows are the data we need
                if (datalistFound && firstRowSkip)
                {
                    // The data in the row is like :
                    // ID, Time, Lat, Lng, Magnitude, Depth, Central
                    //
                    // And the HTML of the table row is like :
                    // <td>
                    // <a href="....."> THE DATA WE NEED </a>
                    // </td><td>
                    // <a href="....."> THE DATA WE NEED 2</a>
                    // ...
                    // </tr><tr>
                    if (data[i].Contains("<tr>"))
                    {
                        i++; // go to next line


                        // This line is <td>
                        // Skip.
                        i++;

                        // <a href="?ItemId=49&fileString=2016030119411338">EARTHQUAKE ID</a>
                        // We can get timestring here.
                        // We can also get data link here.
                        string[] timeStringtmp = data[i].Split(new char[] { '=', '\'' });
                        timeString = timeStringtmp[4].Substring(0, 12);
                        dataLink = "http://scweb.cwb.gov.tw/GraphicContent.aspx?ItemId=49&fileString=" + timeStringtmp[4];
                        i++;

                        // </td><td>
                        i++;


                        // Time, but we already got time string.
                        // Skip them.

                        //<a href="xxx">X月X日X時X分</a>
                        //</td><td>
                        i = i + 2;


                        // Longitude

                        // <a href="xxx">123.4</a>
                        string[] tmp = data[i].Split(new char[] { '>', '<' });
                        lng = tmp[2];
                        i++;

                        //</td><td>
                        i++;

                        // Latitude
                        // Same as above.
                        tmp = data[i].Split(new char[] { '>', '<' });
                        lat = tmp[2];
                        i = i + 2;

                        // Magnitude
                        tmp = data[i].Split(new char[] { '>', '<' });
                        magnitude = tmp[2];
                        i = i + 2;

                        // Depth
                        tmp = data[i].Split(new char[] { '>', '<' });
                        depth = tmp[2];
                        


                        Earthquake eq = new Earthquake(timeString, lat, lng, depth, magnitude, Earthquake.DataSource.CWB, dataLink);
                        eqList.Add(eq);

                        continue;
                    }
                    else if (data[i].Contains("</table>"))
                        break;
                }
            }
        }


        private void ckanTrans(JObject ckanlist)
        {
            IList<JToken> results = ckanlist["result"]["packages"].Children().ToList();
            IList<Ckan> ckanList = new List<Ckan>();  
            foreach (JToken result in results)
            {

                // The JSON file name format is equal to timestring
                string name = result["name"].ToString();
                //string[] dateParse = Time.Split('&','-');
                string[] dateParse = (startTime + '&' + endTime).Split('&', '-');


                // Find the Dataset
                if (int.Parse(name.Substring(0,8)) >= int.Parse(dateParse[0] + dateParse[1] + dateParse[2] ) && 
                    int.Parse(name.Substring(0,8)) <= int.Parse(dateParse[3] + dateParse[4] + dateParse[5] ))
                {

                    // Go to the dataset and take out the earthquake data
                    //string dataset_url = "http://140.109.17.71/api/3/action/package_show?id=" + name;
                    string dataset_url = ConstCkan.CkanAPIPrefix + "/package_show?id=" + name;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(dataset_url);
                    request.Method = WebRequestMethods.Http.Get;
                    request.ContentType = "application/json";

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var stream = response.GetResponseStream())
                            using (var reader = new StreamReader(stream))
                            {

                                string data = reader.ReadToEnd();

                                JObject datalist = JObject.Parse(data);
                                IList<JToken> datalist_JSON = datalist["result"]["resources"].Children().ToList();

                                foreach (JToken dataResult in datalist_JSON)
                                {
                                    string dataName = dataResult["name"].ToString();

                                    if (dataName == name)
                                    {
                                        HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(dataResult["url"].ToString());
                                        request2.Method = WebRequestMethods.Http.Get;
                                        request2.ContentType = "application/json";
                                        using (var response2 = (HttpWebResponse)request2.GetResponse())
                                        {
                                            if (response2.StatusCode == HttpStatusCode.OK)
                                            {
                                                using (var stream2 = response2.GetResponseStream())
                                                using (var reader2 = new StreamReader(stream2))
                                                {
                                                    string data2 = reader2.ReadToEnd();

                                                    Ckan ckan = JsonConvert.DeserializeObject<Ckan>(data2.ToString());
                                                    Earthquake eq = new Earthquake(ckan.Timestring, ckan.Lat, ckan.Lng, ckan.Depth, ckan.Magnitude, Earthquake.DataSource.CKAN, "http://" + ConstCkan.CkanIP + "/dataset/" + name);
                                                    eqList.Add(eq);
                                                }
                                            }
                                        }
                                    }
                                }
                                reader.Close();
                                reader.Dispose();
                            }
                        }
                    }
                }
            }


        }
    }
}