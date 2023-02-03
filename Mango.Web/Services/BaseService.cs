using System.Text;
using System.Text.Json.Serialization;
using Mango.Web.Models;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.Interfaces;
using Newtonsoft.Json;

namespace Mango.Web.Services;

public class BaseService : IBaseService
{
    public ResponseDto ResponseModel { get; set; }
    protected readonly IHttpClientFactory _httpClient;

    public BaseService(IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
        ResponseModel = new ResponseDto();
    }
    
    public async Task<T> SendAsync<T>(ApiRequest apiRequest)
    {
        try
        {
            var client = _httpClient.CreateClient("MangoApi");
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(apiRequest.Url);
            client.DefaultRequestHeaders.Clear();
            if (apiRequest.Data is not null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage apiResponse = null;
            switch (apiRequest.ApiType)
            {
                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                
                case SD.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;

                case SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            apiResponse = await client.SendAsync(message);

            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);
            return apiResponseDto;
        }
        catch (Exception ex)
        {
            var errorDto = new ResponseDto()
            {
                DisplayMessage = "Error",
                ErrorMessages = new List<string>() { Convert.ToString(ex.Message) },
                IsSuccess = false
            };
            var res = JsonConvert.SerializeObject(errorDto);
            var apiResponseDto = JsonConvert.DeserializeObject<T>(res);
            return apiResponseDto;
        }
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(true);
    }
}