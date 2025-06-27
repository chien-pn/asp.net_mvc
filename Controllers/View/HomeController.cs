using Microsoft.AspNetCore.Mvc;
using net_chat.Model;
using net_chat.Model.Response;
using Newtonsoft.Json;

namespace net_chat.Controllers.View
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string GET_ALL_POST = "api/Posts/GetAllPost";

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7052/"); // chỉnh sửa cho đúng
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("AccessToken"); // Nếu bạn có lưu JWT trong session/cookie

            var request = new HttpRequestMessage(HttpMethod.Get, GET_ALL_POST);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return this.View(new List<Post>());

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<List<Post>>>(content);

            return View(result?.Data ?? new List<Post>());
        }
    }
}
