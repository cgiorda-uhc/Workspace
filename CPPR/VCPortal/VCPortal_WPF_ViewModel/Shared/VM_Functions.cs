using SharedFunctionsLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VCPortal_Models.Dtos.ETGFactSymmetry;

namespace VCPortal_WPF_ViewModel.Shared;
internal static class VM_Functions
{
    internal static async Task<List<T>> APIGetResultAsync<T>(string baseurl, string url)
    {

        WebAPIConsume.BaseURI = baseurl;
        var response = WebAPIConsume.GetCall(url);
        if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var reponseStream = await response.Result.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<List<T>>(reponseStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result;
        }
        else
        {
            return null;
        }

    }

}
