﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TechShopSolution.ViewModels.Sales
{
    public class OrderDetailModel
    {
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public decimal unit_price { get; set; }
        public decimal? promotion_price { get; set; }
    }
}
