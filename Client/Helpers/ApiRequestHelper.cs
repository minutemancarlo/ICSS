using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;

public class ApiRequestHelper
{
    private readonly HttpClient _httpClient;

    public ApiRequestHelper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TResponse>();
                return ApiResponse<TResponse>.Success(data);
            }

            return ApiResponse<TResponse>.Error(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse>.Exception(ex.Message);
        }
    }

    public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest payload)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TResponse>();
                return ApiResponse<TResponse>.Success(data);
            }

            return ApiResponse<TResponse>.Error(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse>.Exception(ex.Message);
        }
    }

    public async Task<ApiResponse> PutAsync<TRequest>(string url, TRequest payload)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                return ApiResponse.Success();
            }

            return ApiResponse.Error(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return ApiResponse.Exception(ex.Message);
        }
    }

    public async Task<ApiResponse> DeleteAsync(string url)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return ApiResponse.Success();
            }

            return ApiResponse.Error(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return ApiResponse.Exception(ex.Message);
        }
    }

    public async Task<ApiResponse<TResponse>> PatchAsync<TRequest, TResponse>(string url, TRequest payload)
    {
        try
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync(url, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TResponse>();
                return ApiResponse<TResponse>.Success(data);
            }

            return ApiResponse<TResponse>.Error(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse>.Exception(ex.Message);
        }
    }

    public async Task<ApiResponse> PatchAsync<TRequest>(string url, TRequest payload)
    {
        try
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync(url, requestContent);

            if (response.IsSuccessStatusCode)
            {
                return ApiResponse.Success();
            }

            return ApiResponse.Error(response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return ApiResponse.Exception(ex.Message);
        }
    }
}

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public HttpStatusCode? StatusCode { get; set; }

    public static ApiResponse Success()
    {
        return new ApiResponse { IsSuccess = true, Message = "Request completed successfully." };
    }

    public static ApiResponse Error(HttpStatusCode statusCode, string message)
    {
        return new ApiResponse { IsSuccess = false, StatusCode = statusCode, Message = message };
    }

    public static ApiResponse Exception(string message)
    {
        return new ApiResponse { IsSuccess = false, Message = $"Exception occurred: {message}" };
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T Data { get; set; }

    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T> { IsSuccess = true, Data = data, Message = "Request completed successfully." };
    }

    public static ApiResponse<T> Error(HttpStatusCode statusCode, string message)
    {
        return new ApiResponse<T> { IsSuccess = false, StatusCode = statusCode, Message = message };
    }

    public static ApiResponse<T> Exception(string message)
    {
        return new ApiResponse<T> { IsSuccess = false, Message = $"Exception occurred: {message}" };
    }
}
