using System.Collections.Generic;
using System.Net;
using System;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ServiceChatApp_APIAI_.Dialogs
{
    internal class Logger
    {
         public static string GetTablename()
         {
            List<string> labelDetails = new List<string>();
            try
            {
                string username = Constants.ServiceNowUserName;
                string password = Constants.ServiceNowPassword;
                string URL = "https://dev56432.service-now.com/api/now/table/sys_db_object?sysparm_query=sys_id=4c3fb9e6b96013006517ce7df7ee4671";
                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                HttpWebRequest RetrieveRequest = WebRequest.Create(URL) as HttpWebRequest;
                RetrieveRequest.Headers.Add("Authorization", auth);
                RetrieveRequest.Method = "GET";
                using (HttpWebResponse SnowResponse = RetrieveRequest.GetResponse() as HttpWebResponse)
                {
                    var result = new StreamReader(SnowResponse.GetResponseStream()).ReadToEnd();

                    JObject jResponse = JObject.Parse(result.ToString());
                    JToken obObject = jResponse["result"];
                    
                    JEnumerable<JToken> labelName = (JEnumerable<JToken>)obObject.Values("name");
                    //return labelName.ToString();
                    foreach (var item in labelName)
                    {
                        if (item != null)
                        {
                            return(((JValue)item).Value.ToString());
                            //return GetTableDetails(labelDetails);
                        }
                    }
                }

                return string.Empty;
            }
            catch(Exception e)
            {
                return e.Message;
            }
         }

        internal static string CreateServiceNow(string detail_description, string short_description, string contactOption, string category_choice)
        {
            try
            {
                string username = Constants.ServiceNowUserName;
                string password = Constants.ServiceNowPassword;
                //string URL = ConfigurationManager.AppSettings["ServiceNowURL"];
                string URL = Constants.ServiceNowUrl + GetTablename();

                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                request.Headers.Add("Authorization", auth);
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string Json = JsonConvert.SerializeObject(new
                    {
                        description = detail_description,
                        short_description = short_description,
                        contact_type = contactOption,
                        category = category_choice,
                        subcategory = Constants.ServiceNowSubCategory,
                        assignment_group = Constants.ServiceNowAssignmentGroup,
                        impact = Constants.ServiceNowIncidentImpact,
                        priority = Constants.ServiceNowIncidentPriority,
                        caller_id = Constants.ServiceNowCallerId,
                        cmdb_id = Constants.ServiceNowCatalogueName,
                        comments = Constants.ServiceNowComments + " for the issue: " + short_description
                    });

                    streamWriter.Write(Json);

                }

                /**
                 * HttpWebResponse captures the details send by the REST Table API
                 **/
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var res = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    JObject joResponse = JObject.Parse(res.ToString());
                    JObject ojObject = (JObject)joResponse["result"];
                    string incidentNumber = ((JValue)ojObject.SelectToken("number")).Value.ToString();
                    return incidentNumber;
                }
            }
            catch (Exception message)
            {
                Console.WriteLine(message.Message);
                return message.Message;
            }
        }
    }
}