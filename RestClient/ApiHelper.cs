using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestApiClient
{
  public class ApiHelper
  {
    const string BaseUrl = "http://dts.location-world.com/api/";

    readonly string _accountSid;
    readonly string _secretKey;
    readonly string _secretToken;

    public ApiHelper(string accountSid, string secretKey)
    {
      _accountSid = accountSid;
      _secretKey = secretKey;
    }

    public ApiHelper(string secretToken)
    {
      _secretToken = secretToken;
    }

    public T Execute<T>(RestRequest request) where T : new()
    {
      var client = new RestClient(BaseUrl);
      //client.BaseUrl = new Uri(BaseUrl);

      //client.Authenticator = new HttpBasicAuthenticator(_accountSid, _secretKey);
      //request.AddParameter("AccountSid", _accountSid, ParameterType.UrlSegment); // used on every request


      var response = client.Execute<T>(request);

      if (response.ErrorException != null)
      {
        const string message = "Error retrieving response.  Check inner details for more info.";
        var twilioException = new ApplicationException(message, response.ErrorException);
        throw twilioException;
      }
      return response.Data;
    }

    public IRestResponse Execute(RestRequest request)
    {
      var client = new RestClient(BaseUrl);

      var response = client.Execute(request);

      if (response.ErrorException != null)
      {
        const string message = "Error retrieving response.  Check inner details for more info.";
        var twilioException = new ApplicationException(message, response.ErrorException);
        throw twilioException;
      }
      return response;
    }

    public List<dailyHistory> GetdailyHistory(string imei, int year, int month, int day, string timeZone, string culture)
    {
      var request = new RestRequest("fleet/dailyhistory?token={token}", Method.GET);

      request.AddParameter("token", _secretToken, ParameterType.UrlSegment);

      request.AddQueryParameter("imei", imei);
      request.AddQueryParameter("year", year.ToString());
      request.AddQueryParameter("month", month.ToString());
      request.AddQueryParameter("day", day.ToString());
      request.AddQueryParameter("timezoneoffset", timeZone);
      request.AddQueryParameter("culture", culture);

      request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

      var response = Execute(request);

      var data = JsonConvert.DeserializeObject<dailyHistoryJson>(response.Content);

      return data.rows;
    }


    public deviceInfo[] GetdeviceInfo()
    {
      var request = new RestRequest("fleet/devicesinfo?token={token}", Method.GET);

      request.AddParameter("token", _secretToken, ParameterType.UrlSegment);

      request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

      var response = Execute(request);

      var data = JsonConvert.DeserializeObject<deviceInfoJson>(response.Content);

      return data.rows;
    }

  }
}
