using Mango.Web.Models;
using Mango.Web.Models.Dtos;

namespace Mango.Web.Services.Interfaces;

public interface IBaseService : IDisposable
{
    ResponseDto ResponseModel { get; set; }
    Task<T> SendAsync<T>(ApiRequest apiRequest);
}