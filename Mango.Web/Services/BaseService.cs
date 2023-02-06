using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using Mango.Web.Models;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Mango.Web.Services;

public class BaseService : IBaseService
{
    public ResponseDto ResponseModel { get; set; }
    protected readonly IHttpClientFactory _httpClient;
    private readonly IHttpContextAccessor _accessor;

    public BaseService(IHttpClientFactory httpClient, IHttpContextAccessor accessor)
    {
        _accessor = accessor;
        _httpClient = httpClient;
        ResponseModel = new ResponseDto();
    }
    
    public async Task<T> SendAsync<T>(ApiRequest apiRequest)
    {
        try
        {
            var client = _httpClient.CreateClient("MangoApi");
            
            var accessToken = await _accessor.HttpContext.GetTokenAsync("access_token");
            
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(apiRequest.Url);
            client.DefaultRequestHeaders.Clear();
            if (apiRequest.Data is not null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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