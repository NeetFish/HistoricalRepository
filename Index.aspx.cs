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


        protected void UploadBtn_Click(object sender, EventArgs e)
        {

            // Check if there is no date
            if (uploaddatepicker.Value == "")
                return;

            // Check if there is no time
            if (uploadtimepicker.Value == "")
                return;

            // Check if there is no file name
            if (uploadName.Text == "")
                return;

            // Check if there is no file
            if (!FileUp.HasFile)
                return;

            //  uploaddatepicker Example : 2016-04-01
            string[] date_ = uploaddatepicker.Value.Split('-');
            string date = date_[0] + date_[1] + date_[2];

            // uploadtimepicker Example : 14 : 06
            string time = uploadtimepicker.Value.ElementAt(0).ToString() + uploadtimepicker.Value.ElementAt(1).ToString() + 
                        uploadtimepicker.Value.ElementAt(3).ToString() + uploadtimepicker.Value.ElementAt(4).ToString();


            string datasetName = date + time;
            if (FileUp.HasFile)
            {
                FileUploader.Upload(FileUp.FileContent, FileUp.FileName, datasetName, uploadName.Text);
            }
        }


    }
}