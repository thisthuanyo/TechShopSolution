﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShopSolution.ApiIntegration;
using TechShopSolution.ViewModels.Catalog.Coupon;

namespace TechShopSolution.AdminApp.Controllers
{
    [Authorize]
    public class CouponController : Controller
    {
        private readonly ICouponApiClient _couponApiClient;
        public CouponController(ICouponApiClient couponApiClient)
        {
            _couponApiClient = couponApiClient;
        }
        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetCouponPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };
            var data = await _couponApiClient.GetCouponPagings(request);
            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _couponApiClient.ChangeStatus(id);
            if (result == null)
            {
                ModelState.AddModelError("Cập nhật thất bại", result.Message);
            }
            if (result.IsSuccess)
            {
                TempData["result"] = "Thay đổi trạng thái thành công";
                return RedirectToAction("Index");
            }
            return View("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var result = await _couponApiClient.GetById(id);
            if (!result.IsSuccess || result.ResultObject == null)
            {
                ModelState.AddModelError("", result.Message);
                return View("Index");
            }
            var updateRequest = new CouponUpdateRequest()
            {
                id = id,
                value = result.ResultObject.value,
                type = result.ResultObject.type,
                code = result.ResultObject.code,
                end_at = result.ResultObject.end_at,
                isActive = result.ResultObject.isActive,
                name = result.ResultObject.name,
                quantity = result.ResultObject.quantity,
                start_at = result.ResultObject.start_at,
            };
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(updateRequest);
        }
        [HttpPost]
        public async Task<IActionResult> Update(CouponUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _couponApiClient.UpdateCoupon(request);
            if (result.IsSuccess)
            {
                TempData["result"] = "Cập nhật mã giảm giá thành công";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message);
            return View(request);
        }
    }
}
