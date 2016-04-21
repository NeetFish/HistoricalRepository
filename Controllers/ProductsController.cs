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
    public class ProductsController : ApiController
    {
        Product[] products = new Product[] {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category= "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        Palert[] palert = new Palert[] { };

        public IEnumerable<Product> GetAllProducts()
        {
            return products;
        }
        public IHttpActionResult GetProduct(int id)
        {
            //var product = products.FirstOrDefault((p) => p.Id == id);

            var data = "";
            

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://palert.earth.sinica.edu.tw/db/querydb.php?lat%5B%5D=21&lat%5B%5D=26&lng%5B%5D=119&lng%5B%5D=123&time%5B%5D=2010-02-07&time%5B%5D=2016-03-07&intensity%5B%5D=1&intensity%5B%5D=7");
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
                        List<Palert> p_list = JsonConvert.DeserializeObject<List<Palert>>(data);
                        return Ok(p_list);
                    }
                }
                else
                {
                    data = null;
                }
            }
             

            if (data == null)
            {
                return NotFound();
            }


            return Ok(data);
        }
}
}
