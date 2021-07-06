﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShopSolution.Application.Catalog.Order;
using TechShopSolution.ViewModels.Sales;

namespace TechShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CheckoutRequest request)
        {
            var result = await _orderService.Create(request);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("paging")]
        public IActionResult GetAllPaging([FromQuery] GetOrderPagingRequest requet)
        {
            var customer = _orderService.GetAllPaging(requet);
            return Ok(customer);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var result = await _orderService.Detail(id);
            return Ok(result);
        }
        [HttpGet("paymentconfirm/{id}")]
        public async Task<IActionResult> PaymentConfirm(int id)
        {
            var result = await _orderService.PaymentConfirm(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("cancelorder/{id}")]
        public async Task<IActionResult> Cancelorder(int id)
        {
            var result = await _orderService.CancelOrder(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
