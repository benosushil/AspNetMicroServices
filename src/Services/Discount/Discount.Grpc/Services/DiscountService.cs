using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (coupon == null)
            {
                _logger.LogError($"Get Discount with Product {coupon.ProductName} is not found");
                throw new RpcException(new Status(StatusCode.NotFound, $"Get Discount with product name {request.ProductName} is not found"));
            }

            _logger.LogInformation($"Get Discount with Product {coupon.ProductName} is {coupon.Amount}");
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var success = await _repository.CreateDiscount(_mapper.Map<Coupon>(request.Coupon));
            if(!success)
            {
                _logger.LogError($"Create Discount with Product {request.Coupon.ProductName} is failed");
                throw new RpcException(new Status(StatusCode.NotFound, $"Create Discount with Product {request.Coupon.ProductName} is failed"));
            }

            _logger.LogInformation($"Create Discount with Product {request.Coupon.ProductName} is success");
            return request.Coupon;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var success = await _repository.UpdateDiscount(_mapper.Map<Coupon>(request.Coupon));
            if (!success)
            {
                _logger.LogError($"Update Discount with Product {request.Coupon.ProductName} is failed");
                throw new RpcException(new Status(StatusCode.NotFound, $"Update Discount with Product {request.Coupon.ProductName} is failed"));
            }

            _logger.LogInformation($"Update Discount with Product {request.Coupon.ProductName} is success");
            return request.Coupon;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var success = await _repository.DeleteDiscount(request.ProductName);
            if (!success)
            {
                _logger.LogError($"Delete Discount with Product {request.ProductName} is failed");
                throw new RpcException(new Status(StatusCode.NotFound, $"Update Discount with Product {request.ProductName} is failed"));
            }

            _logger.LogInformation($"Update Discount with Product {request.ProductName} is success");
            return new DeleteDiscountResponse { Success = success };
        }
    }
}
