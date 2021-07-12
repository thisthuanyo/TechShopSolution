﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TechShopSolution.ViewModels.Common;
using TechShopSolution.ViewModels.Website;

namespace TechShopSolution.Application.Website.Slide
{
    public interface ISlideService
    {
        Task<ApiResult<bool>> ChangeStatus(int id);
        Task<ApiResult<bool>> Create(SlideCreateRequest request);
        Task<ApiResult<bool>> Delete(int id);
        Task<PagedResult<SlideViewModel>> GetAllPaging(PagingRequestBase request);
        Task<ApiResult<SlideViewModel>> GetById(int id);
        Task<ApiResult<bool>> Update(SlideUpdateRequest request);
        Task<List<SlideViewModel>> GetPublicSlide();
        Task<ApiResult<bool>> DisplayOrder(int slide_id, int display_position);

    }
}
