using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;
using Newtonsoft.Json;

namespace WebApplication2.Upload
{
    static class FileUploader
    {

        public static void Upload(Stream stream, string fileName,  string datasetName, string fileShowName )
        {
            var files = new[]
            {
                new FileToUpload
                {
                    Name = "upload",
                    Filename = fileName,
                    ContentType = "text/plain",
                    Stream = stream
                }
            };

            //
            // Upload to the earthquake dataset
            //
            // package_id   : dataset name
            //                  Ex : Eq_201604011328
            // url          : You need this parameter, or ckan will block your upload. Just leave it be empty.
            // name         : The name of the file.
            //
            var values = new NameValueCollection
                {
                    { "package_id", datasetName },
                    { "url", "" },
                    { "name", fileShowName },
                };

            // resource_create is the data upload API of CKAN
            //byte[] result = upF.UploadFiles("http://140.109.17.71/api/action/resource_create", files, values);

            string result = UploadFiles(files, values);

            if (result == "Success")
                ;
            // When result == "NotFound",
            // We need to create a dataset for it, and then upload again.
            else if (result == "NotFound")
            {
                CreateDataSet(datasetName);

                // The stream position of the file is at the End Of the File
                // Need to go back to the beginning
                // And then upload again.
                stream.Position = 0;
                UploadFiles(files, values);
            }
            else
                ;
        }

        // I steal these code from somewhere in the great universe.
        static string UploadFiles(IEnumerable<FileToUpload> files, NameValueCollection values)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConstCkan.CkanAPIPrefix + "resource_create");
            request.Method = "POST";
            request.Headers.Add("Authorization", ConstCkan.CkanAuthorization);
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;

            using (var requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in values.Keys)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                foreach (var file in files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    file.Stream.CopyTo(requestStream);
                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }


            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                string exStr = ((HttpWebResponse)ex.Response).StatusCode.ToString();

                // Data set doesn't exist.
                if (exStr == "NotFound")
                {
                    return "NotFound";
                }
            }
            return "Success";
        }

        static void CreateDataSet(string dataSetName)
        {
            var values = new NameValueCollection
                    {
                        { "name", dataSetName },
                        { "owner_org", "earthquake" },
                    };

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConstCkan.CkanAPIPrefix + "package_create");
            request.Method = "POST";
            request.Headers.Add("Authorization", ConstCkan.CkanAuthorization);
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;

            using (var requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in values.Keys)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                string exStr = ((HttpWebResponse)ex.Response).StatusCode.ToString();

                response.Dispose();

            }
        }
    }
}