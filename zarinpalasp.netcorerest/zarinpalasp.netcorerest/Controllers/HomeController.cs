using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using zarinpalasp.netcorerest.Models;

namespace zarinpalasp.netcorerest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        string merchant = "cfa83c81-89b0-4993-9445-2c3fcd323455";
        string amount = "1100";
        string authority;
        string description = "خرید تستی ";
        string callbackurl = "http://localhost:2812/Home/VerifyPayment";

  

        public HomeController(ILogger<HomeController> logger)
       {
            _logger = logger;
        }

  

        public IActionResult Payment()
        {
         

            try
            {
                string[] metadata = new string[2];
                metadata[0] = "";
                metadata[1] = "";

                //be dalil in ke metadata be sorate araye ast va do meghdare mobile va email dar metadata gharar mmigirad
                //shoma mitavanid in maghadir ra az kharidar begirid va set konid dar gheir in sorat khali ersal konid

                string requesturl;
                requesturl = "https://api.zarinpal.com/pg/v4/payment/request.json?merchant_id=" +
                    merchant + "&amount=" + amount +
                    "&callback_url=" + callbackurl +
                    "&description=" + description;
                    //+"&metadata[0]=" + metadata[0] + "& metadata[1]=" + metadata[1];
                ;
                

                var client = new RestClient(requesturl);



                Method method = Method.Post;
                var request = new RestRequest("",method );

                request.AddHeader("accept", "application/json");

                request.AddHeader("content-type", "application/json");

                var  requestresponse = client.ExecuteAsync(request);

                Newtonsoft.Json.Linq.JObject jo = Newtonsoft.Json.Linq.JObject.Parse(requestresponse.Result.Content);
                string errorscode = jo["errors"].ToString();

                Newtonsoft.Json.Linq.JObject jodata = Newtonsoft.Json.Linq.JObject.Parse(requestresponse.Result.Content);
                string dataauth = jodata["data"].ToString();


                if (dataauth != "[]")
                {


                    authority = jodata["data"]["authority"].ToString();
                    string gatewayUrl = "https://www.zarinpal.com/pg/StartPay/" + authority;
                    return Redirect(gatewayUrl);

                }
                else
                {

                    //return BadRequest();
                    return BadRequest("error " + errorscode);


                }


            }

            catch (Exception ex)
            {
            //    throw new Exception(ex.Message);


            }
            return null;
        }

        public IActionResult VerifyPayment()
        {

           // string authorityverify;

            try
            {

                if (HttpContext.Request.Query["Authority"] != "") {
                    authority = HttpContext.Request.Query["Authority"];
                }

             
                string url = "https://api.zarinpal.com/pg/v4/payment/verify.json?merchant_id=" +
                    merchant + "&amount="
                    + amount + "&authority="
                    + authority;

                var client = new RestClient(url);
                Method method = Method.Post;
                var request = new RestRequest("", method);

                request.AddHeader("accept", "application/json");

                request.AddHeader("content-type", "application/json");

                var  response = client.ExecuteAsync(request);


                Newtonsoft.Json.Linq.JObject jodata = Newtonsoft.Json.Linq.JObject.Parse(response.Result.Content);
                string data = jodata["data"].ToString();

                Newtonsoft.Json.Linq.JObject jo = Newtonsoft.Json.Linq.JObject.Parse(response.Result.Content);
                string errors = jo["errors"].ToString();

                if (data != "[]")
                {
                    string refid = jodata["data"]["ref_id"].ToString();
                    ViewBag.code = refid;
                    return View();
                }
                else if (errors != "[]")
                {

                    string errorscode = jo["errors"]["code"].ToString();
                    return BadRequest($"error code {errorscode}");

                }


            }
            catch (Exception ex)
            {

               // throw new Exception(ex.Message);
            }
            return NotFound();
        }

      public async Task< IActionResult> PaymenBytHttpClient()
        {

            try
            {

                using (var client = new HttpClient())
                {
                    string requesturl = "https://api.zarinpal.com/pg/v4/payment/request.json?";



                    string amount = this.amount;
                    string callbackUrl = this.callbackurl;
                    string description = this.description;
                    string merchant = this.merchant;
                    string Reqparameters = $"merchant_id={merchant}&amount={amount}&callback_url={callbackUrl}&description={description}";

                    HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(requesturl + Reqparameters, content);
                  
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Newtonsoft.Json.Linq.JObject jo = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                    string errorscode = jo["errors"].ToString();

                    Newtonsoft.Json.Linq.JObject jodata = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                    string dataauth = jodata["data"].ToString();


                    if (dataauth != "[]")
                    {


                        authority = jodata["data"]["authority"].ToString();
                        string gatewayUrl = "https://www.zarinpal.com/pg/StartPay/" + authority;
                        return Redirect(gatewayUrl);

                    }
                    else
                    {

                        //return BadRequest();
                        return BadRequest("error " + errorscode);


                    }

                }

     
            }

            catch (Exception ex)
            {
                //    throw new Exception(ex.Message);


            }
            return null;
        }

        public async Task<IActionResult> VerifyByHttpClient()
        {
            try
            {

                if (HttpContext.Request.Query["Authority"] != "")
                {
                   this. authority = HttpContext.Request.Query["Authority"];
                }

                using (HttpClient client = new HttpClient())
                {
                string url = "https://api.zarinpal.com/pg/v4/payment/verify.json?";
                 
                string amount = this.amount;
                string authority = this.authority;
                string merchant = this.merchant;
                string Reqparameters = $"merchant_id={merchant}&amount={amount}&authority={authority}";

                HttpContent content = new StringContent(Reqparameters, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url + Reqparameters, content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    Newtonsoft.Json.Linq.JObject jodata = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                    string data = jodata["data"].ToString();

                    Newtonsoft.Json.Linq.JObject jo = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                    string errors = jo["errors"].ToString();

                    if (data != "[]")
                {
                    string refid = jodata["data"]["ref_id"].ToString();
                    ViewBag.code = refid;
                    return View();
                }
                else if (errors != "[]")
                {

                    string errorscode = jo["errors"]["code"].ToString();
                    return BadRequest($"error code {errorscode}");

                }
                }
               


            }
            catch (Exception ex)
            {

                 throw ex;
            }
            return NotFound();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }


}
