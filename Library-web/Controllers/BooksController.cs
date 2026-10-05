using Library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Library_web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        public BooksController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        
        public async Task<IActionResult> Index([FromQuery] string filterOn = null, string filterQuery = null, string sortBy = null, bool isAscending = true)
        {
            List<BookDTO> response = new List<BookDTO>();
            try
            {
                // lấy dữ liệu books from API
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.GetAsync("https://localhost:7255/api/Books/get-all-books?filterOn=" + filterOn + "&filterQuery=" + filterQuery + "&sortBy=" + sortBy + "&isAscending=" + isAscending);
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<BookDTO>>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(response);   // trả về danh sách rỗng, hiển thị lỗi bằng ViewBag.Error
            }
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> addBook()
        {

            var client = httpClientFactory.CreateClient();
            List<authorDTO> responseAu = new List<authorDTO>();
            var httpResponseAu = await client.GetAsync("https://localhost:7255/api/Authors/get-all-author");
            httpResponseAu.EnsureSuccessStatusCode();
            responseAu.AddRange(await httpResponseAu.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>());
            ViewBag.listAuthor = responseAu;

            List<publisherDTO> responsePu = new List<publisherDTO>();
            var httpResponsePu = await client.GetAsync("https://localhost:7255/api/Publishers/get-all-publisher");
            httpResponsePu.EnsureSuccessStatusCode();
            responsePu.AddRange(await httpResponsePu.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>());
            ViewBag.listPublisher = responsePu;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> addBook(addBookDTO addBookDTO)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:7255/api/Books/add-book"),
                    Content = new StringContent( JsonSerializer.Serialize(addBookDTO), Encoding.UTF8, MediaTypeNames.Application.Json)
                };

                //Console.WriteLine(JsonSerializer.Serialize(addBookDTO));

                var httpResponseMess = await client.SendAsync(httpRequestMess);
                httpResponseMess.EnsureSuccessStatusCode();

                var response = await httpResponseMess.Content
                .ReadFromJsonAsync<addBookDTO>();

                if (response != null)
                {
                    return RedirectToAction("Index", "Books");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }

        public async Task<IActionResult> listBookById(int id)
        {
            BookDTO response = new BookDTO();

            try
            {
                // lấy dữ liệu books from API
                var client = httpClientFactory.CreateClient();

                var httpResponseMess = await client.GetAsync( "https://localhost:7255/api/Books/get-book-by-id/" + id);

                httpResponseMess.EnsureSuccessStatusCode();

                var stringResponseBody =
                await httpResponseMess.Content.ReadAsStringAsync();

                response = await httpResponseMess.Content.ReadFromJsonAsync<BookDTO>();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> delBook(int id)
        {
            try
            {
                //lay du lieu books from API
                var client = httpClientFactory.CreateClient();
                var httpResponseMess = await client.DeleteAsync("https://localhost:7255/api/Books/detele-book-by-id" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View("Index","Books");
        }
    }
}
