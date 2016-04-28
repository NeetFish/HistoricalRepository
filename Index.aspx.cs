using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using System.IO;
using System.Diagnostics;
using WebApplication2.Models;
using WebApplication2.Upload;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace WebApplication2
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }


        protected void Button1_Click1(object sender, EventArgs e)
        {


            if (FileUp.HasFile)
            {
                UploadFile upF = new UploadFile();

                //using (var stream1 = File.Open("C:\\Users\\NeetFish\\Documents\\hi.txt", FileMode.Open))
                using (var stream1 = FileUp.FileContent)
                {
                    var files = new[]
                    {
                        new UploadFile
                        {
                            Name = "upload",
                            //Filename = "hi.txt",
                            Filename = FileUp.FileName,
                            ContentType = "text/plain",
                            Stream = stream1
                        }
                    };

                    var values = new NameValueCollection
                    {
                        { "package_id", "earthquake" },
                        { "url", "" },
                        { "name", uploadName.Text },
                    };

                    byte[] result = upF.UploadFiles("http://140.109.17.71/api/action/resource_create", files, values);
                }
            }
        }
            
    }
}