using AA.ConsumeAPI.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AA.ConsumeAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMesseage = await client.GetAsync("http://localhost:5000/api/products");
            if(responseMesseage.StatusCode == HttpStatusCode.OK)
            {
                var jsonData = await responseMesseage.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ProductResponseModel>>(jsonData);
                return View(result);
            }
            else
            {
                return View(null);
            }
            
        }

        public IActionResult Create()
        {
            return View();  
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductResponseModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData,Encoding.UTF8,"application/json");
            var responseMessage = await client.PostAsync("http://localhost:5000/api/products",content);
            
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["errorMesseage"] = $"Bir hata oluştu. Hata kodu: {(int)responseMessage.StatusCode}";
                return View(model);
            }

        }
        
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMesseage = await client.GetAsync($"http://localhost:5000/api/products/{id}");
            if (responseMesseage.IsSuccessStatusCode)
            {
                var jsonData = await responseMesseage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProductResponseModel>(jsonData);
                return View(data);
            }
            else
            {
                return View(null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductResponseModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            var contet = new StringContent(jsonData,Encoding.UTF8,"application/json");
            var responseMesseage = await client.PutAsync("http://localhost:5000/api/products",contet);
            if (responseMesseage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");   
            }
            else
            {
                return View(model);
            }
        }
        public async Task<IActionResult> Remove(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"http://localhost:5000/api/products{id}");
            return RedirectToAction("Index");
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var client = _httpClientFactory.CreateClient();
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            
            var bytes = stream.ToArray();
            
            ByteArrayContent content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType); 
            MultipartFormDataContent formdata = new MultipartFormDataContent();
            formdata.Add(content,"formFile",file.FileName);

            await client.PostAsync("http://localhost:5000/api/products/upload", formdata);
            return RedirectToAction("Index");
        }
    }
}
