using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace SharedFunctionsLibrary;

public class WebAPIConsume
{
    public static string BaseURI { get; set; }
    
    public static Task<HttpResponseMessage> GetCall(string url)
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            //string apiUrl = BaseURI + url;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseURI);
                client.Timeout = TimeSpan.FromSeconds(900);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(url);
                response.Wait();
                return response;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }



    public static Task<HttpResponseMessage> PostCall<T>(string url, T model) where T : class
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string apiUrl = BaseURI + url;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(900);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsJsonAsync(apiUrl, model);
                response.Wait();
                return response;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }



    public static Task<HttpResponseMessage> PutCall<T>(string url, T model) where T : class
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string apiUrl = BaseURI + url;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(900);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PutAsJsonAsync(apiUrl, model);
                response.Wait();
                return response;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static Task<HttpResponseMessage> DeleteCall(string url)
    {
        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string apiUrl = BaseURI + url;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.Timeout = TimeSpan.FromSeconds(900);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.DeleteAsync(apiUrl);
                response.Wait();
                return response;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
