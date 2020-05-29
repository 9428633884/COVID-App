using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Web.Script.Services;
using System.Web.Services;
using COVIDApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace COVIDApp
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class WebService : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public string GetTableData()
        {
            var echo = int.Parse (HttpContext.Current.Request.Params["sEcho"]);
            var roleId = HttpContext.Current.Request.Params["roleId"].ToString(CultureInfo.CurrentCulture);

            var records = GetRecordsFromDatabaseWithFilter().ToList();
            if (!records.Any())
            {
                return string.Empty;
            }

            var orderedResults = records.OrderByDescending(o => o.TotalCases);
            var hasMoreRecords = false;

            var sb = new StringBuilder();
            sb.Append(@"{" + "\"sEcho\": " + echo + ",");
            sb.Append("\"recordsTotal\": " + records.Count + ",");
            sb.Append("\"recordsFiltered\": " + records.Count + ",");
            sb.Append("\"iTotalRecords\": " + records.Count + ",");
            sb.Append("\"iTotalDisplayRecords\": " + records.Count + ",");
            sb.Append("\"aaData\": [");
            foreach (var result in orderedResults)
            {
                if (hasMoreRecords)
                {
                    sb.Append(",");
                }

                sb.Append("[");
                sb.Append("\"" + result.Country + "\",");
                sb.Append("\"" + result.TotalCases + "\",");
                sb.Append("\"" + result.NewCases + "\",");
                sb.Append("\"" + result.TotalDeaths + "\",");
                sb.Append("\"" + result.NewDeaths + "\",");
                sb.Append("\"" + result.TotalRecovered + "\",");
                sb.Append("\"" + result.ActiveCases + "\",");
                sb.Append("\"<img class='image-details' src='content/details_open.png' runat='server' height='16' width='16' alt='View Details'/>\"");
                sb.Append("]");
                hasMoreRecords = true;
            }
            sb.Append("]}");
            return sb.ToString();
        }

        private static IEnumerable<CustomUser> GetRecordsFromDatabaseWithFilter()
        {
            List<CustomUser> clpUsers = new List<CustomUser>() ;
           
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                WebClient clWeb = new WebClient();

                string clJson = clWeb.DownloadString("https://pomber.github.io/covid19/timeseries.json");

                if( clJson.Length > 0 )
                {
                    JObject clObj = (JObject) JsonConvert.DeserializeObject(clJson);

                    CustomUser clTotals = new CustomUser();
                    clTotals.Country = "World";
                    clpUsers.Add(clTotals);

                    foreach (var clItr in clObj)
                    {
                        CustomUser clUser = new CustomUser();
                        clUser.Country = clItr.Key;

                        JToken clLast = clItr.Value.Last();

                        JToken clPrevLast = clItr.Value.Last().Previous ;

                        {
                            string strCase = clLast["confirmed"].ToString();
                            clUser.TotalCases = int.Parse(strCase);

                            string strDeath = clLast["deaths"].ToString();
                            clUser.TotalDeaths = int.Parse(strDeath);

                            string strRecovered = clLast["recovered"].ToString();
                            clUser.TotalRecovered = int.Parse(strRecovered);

                            clUser.ActiveCases = clUser.TotalCases - clUser.TotalDeaths - clUser.TotalRecovered;

                            string strPrevCase = clPrevLast["confirmed"].ToString();
                            int iPrevCases = int.Parse(strPrevCase);
                            clUser.NewCases = clUser.TotalCases - iPrevCases;

                            string strPrevDeath = clPrevLast["deaths"].ToString();
                            int iPrevDeaths = int.Parse(strPrevDeath);
                            clUser.NewDeaths = clUser.TotalDeaths - iPrevDeaths;
                        }

                        clpUsers.Add(clUser);

                        clTotals.TotalCases += clUser.TotalCases;
                        clTotals.TotalDeaths += clUser.TotalDeaths;
                        clTotals.TotalRecovered += clUser.TotalRecovered;
                        clTotals.ActiveCases += clUser.ActiveCases;
                        clTotals.NewCases += clUser.NewCases;
                        clTotals.NewDeaths += clUser.NewDeaths;
                    }
                }
            }
            catch(Exception)
            {
                
            }

            return clpUsers;
        }
    }
}
