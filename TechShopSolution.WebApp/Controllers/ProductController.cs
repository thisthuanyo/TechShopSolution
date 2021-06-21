﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShopSolution.ApiIntegration;
using TechShopSolution.ViewModels.Catalog.Category;
using TechShopSolution.ViewModels.Catalog.Product;
using TechShopSolution.WebApp.Models;

namespace TechShopSolution.WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ICategoryApiClient _categorytApiClient;
        private readonly IBrandApiClient _brandApiClient;

        public ProductController(IProductApiClient productApiClient, ICategoryApiClient categorytApiClient, IBrandApiClient brandApiClient)
        {
            _productApiClient = productApiClient;
            _categorytApiClient = categorytApiClient;
            _brandApiClient = brandApiClient;
        }
        [Route("san-pham/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            var product = await _productApiClient.GetBySlug(slug);
            string[] CateId = product.ResultObject.CateID.Split(",");
            var Category = await _categorytApiClient.GetById(int.Parse(CateId[0]));
            var Brand = await _brandApiClient.GetById(product.ResultObject.brand_id);
            
            return View(new ProductDetailViewModel()
            {
                Product = product.ResultObject,
                Category = Category.ResultObject,
                Brand = Brand.ResultObject,
                ProductsRelated = await _productApiClient.GetProductsRelated(product.ResultObject.id, 4),
                ImageList = await _productApiClient.GetImageByProductID(product.ResultObject.id),
            });
        }
        [Route("danh-muc/{slug}")]
        public async Task<IActionResult> Category(string slug, int page = 1)
        {
            var Category = await _categorytApiClient.GetBySlug(slug);
            List<int?> CateID = new List<int?>();
            CateID.Add(Category.ResultObject.id);
            var products = await _productApiClient.GetProductPagingsWithMainImage(new GetProductPagingRequest()
            {
                CategoryID = CateID,
                PageIndex = page,
                PageSize = 9,
            });
            ViewBag.PageResult = products;
            return View(new ProductCategoryViewModel() { 
                Category = Category.ResultObject,
                Products = products
            });
        }
        public async Task<IActionResult> SearchProducts(string tukhoa, string danhmuc, string thuonghieu, int idsort = 1, decimal? giathapnhat = null, decimal? giacaonhat = null, bool tinhtrang = true, int pageIndex = 1)
        {
            var Category = await _categorytApiClient.GetBySlug(danhmuc);
            if (danhmuc != null)
            {
                var products = await _productApiClient.GetPublicProducts(new GetPublicProductPagingRequest()
                {
                    CategorySlug = danhmuc,
                    Keyword = tukhoa,
                    PageIndex = pageIndex,
                    PageSize = 9,
                    BrandSlug = thuonghieu,
                    Highestprice = giacaonhat,
                    Lowestprice = giathapnhat,
                    idSortType = idsort
                });
                ViewBag.PageResult = products;
                return View(new ProductCategoryViewModel()
                {
                    Category = Category.ResultObject,
                    Products = products
                });
            }
            else
            {
                var products = await _productApiClient.GetPublicProducts(new GetPublicProductPagingRequest()
                {
                    CategorySlug = danhmuc,
                    Keyword = tukhoa,
                    PageIndex = pageIndex,
                    PageSize = 9,
                    BrandSlug = thuonghieu,
                    Highestprice = giacaonhat,
                    Lowestprice = giathapnhat,
                    idSortType = idsort
                });
                ViewBag.PageResult = products;
                return View(new ProductCategoryViewModel()
                {
                    Category = null,
                    Products = products
                });
            }
        }
        public async Task<List<CategoryViewModel>> OrderCateToTree(List<CategoryViewModel> lst, int parent_id = 0, int level = 0)
        {
            if (lst != null)
            {
                List<CategoryViewModel> result = new List<CategoryViewModel>();
                foreach (CategoryViewModel cate in lst)
                {
                    if (cate.parent_id == parent_id)
                    {
                        CategoryViewModel tree = new CategoryViewModel();
                        tree = cate;
                        tree.level = level;
                        tree.cate_name = String.Concat(Enumerable.Repeat("|————", level)) + tree.cate_name;

                        result.Add(tree);
                        List<CategoryViewModel> child = await OrderCateToTree(lst, cate.id, level + 1);
                        result.AddRange(child);
                    }
                }
                return result;
            }
            return null;
        }

        //public async Task<IActionResult> SortProducts(string idType, string slug)
        //{
        //    var Category = await _categorytApiClient.GetBySlug(slug);
        //    List<int?> CateID = new List<int?>();
        //    CateID.Add(Category.ResultObject.id);
        //    var products = await _productApiClient.GetProductPagingsWithMainImage(new GetProductPagingRequest()
        //    {
        //        CategoryID = CateID,
        //        PageIndex = page,
        //        PageSize = 9,
        //    });
        //    ViewBag.PageResult = products;
        //    return View(new ProductCategoryViewModel()
        //    {
        //        Category = Category.ResultObject,
        //        Products = products
        //    });
        //}
    }
}
