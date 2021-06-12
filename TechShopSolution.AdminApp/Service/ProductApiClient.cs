﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TechShopSolution.ViewModels.Catalog.Product;
using TechShopSolution.ViewModels.Common;

namespace TechShopSolution.AdminApp.Service
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
    
        public ProductApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHostingEnvironment environment) 
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;

        }

        public Task<ApiResult<bool>> ChangeStatus(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResult<bool>> CreateProduct(ProductCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            if(request.Image!=null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.Image.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.Image.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "Image", request.Image.FileName);
            }

            var provider = new PhysicalFileProvider(_environment.WebRootPath);
            var contents = provider.GetDirectoryContents(Path.Combine("assets", "ProductImage"));
            var objFiles = contents.OrderBy(m => m.LastModified);
            string[] imageList = request.Name_more_images.Split(",");
            foreach (var item in contents.ToList())
            {
                bool flag = false;
                foreach (string name in imageList)
                {
                    if (name.Equals(item.Name))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(item.PhysicalPath);
                        ByteArrayContent byteArr = new ByteArrayContent(bytes);
                        requestContent.Add(byteArr, "More_images", item.Name);
                        flag = true;
                        File.Delete(item.PhysicalPath);
                    }
                }
                if (!flag)
                    File.Delete(item.PhysicalPath);
            }
              
            string month = DateTime.Now.Month.ToString();
            if (month.Length == 1) month = "0" + month;
            string date = DateTime.Now.Day.ToString();
            if (date.Length == 1) date = "0" + date;
            string hour = DateTime.Now.Hour.ToString();
            if (hour.Length == 1) hour = "0" + hour;
            string minute = DateTime.Now.Minute.ToString();
            if (minute.Length == 1) minute = "0" + minute;
            string second = DateTime.Now.Second.ToString();
            if (second.Length == 1) second = "0" + second;


            request.Code = DateTime.Now.Year.ToString().Substring(2, 2) + month + date + hour + minute + second;

            requestContent.Add(new StringContent(request.Best_seller.ToString()), "Best_seller");
            requestContent.Add(new StringContent("1"), "Brand_id");
            requestContent.Add(new StringContent("1"), "CateID");
            requestContent.Add(new StringContent(request.Code.ToString()), "Code");
            requestContent.Add(new StringContent(request.Descriptions.ToString()), "Descriptions");
            requestContent.Add(new StringContent(request.Featured.ToString()), "Featured");
            requestContent.Add(new StringContent(request.Instock.ToString()), "Instock");
            requestContent.Add(new StringContent(request.IsActive.ToString()), "IsActive");
            requestContent.Add(new StringContent(request.Meta_descriptions.ToString()), "Meta_descriptions");
            requestContent.Add(new StringContent(request.Meta_keywords.ToString()), "Meta_keywords");
            requestContent.Add(new StringContent(request.Meta_tittle.ToString()), "Meta_tittle");
            requestContent.Add(new StringContent(request.Name.ToString()), "Name");
            requestContent.Add(new StringContent(request.Promotion_price.ToString()), "Promotion_price");
            requestContent.Add(new StringContent(request.Short_desc.ToString()), "Short_desc");
            requestContent.Add(new StringContent(request.Slug.ToString()), "Slug");
            requestContent.Add(new StringContent(request.Specifications.ToString()), "Specifications");
            requestContent.Add(new StringContent(request.Unit_price.ToString()), "Unit_price");
            requestContent.Add(new StringContent(request.Warranty.ToString()), "Warranty");
            var respone = await client.PostAsync($"/api/product", requestContent);
            var result = await respone.Content.ReadAsStringAsync();
            if (respone.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
            else return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        public Task<ApiResult<bool>> Delete(int cusID)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteImage(string fileName)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var respone = await client.DeleteAsync($"/api/Product/DeleteImage/{fileName}");
        }

        public Task<ApiResult<ProductViewModel>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<ProductViewModel>> GetProductPagings(GetProductPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var respone = await client.GetAsync($"/api/Product/paging?Keyword={request.Keyword}&CategoryID={request.CategoryID}&BrandID={request.BrandID}&pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}");
            var body = await respone.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<PagedResult<ProductViewModel>>(body);
            return product;
        }

        public Task<ApiResult<bool>> UpdateProduct(ProductUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
