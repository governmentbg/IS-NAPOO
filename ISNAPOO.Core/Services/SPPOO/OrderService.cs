using Data.Models;
using Data.Models.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.SPPOO
{
    public class OrderService : BaseService, IOrderService
    {
        private readonly IRepository repository;

        public OrderService(IRepository repository) : base(repository)
        {
            this.repository = repository;
        }

        public async Task<OrderVM> GetOrderByIdAsync(int id)
        {
            SPPOOOrder order = await this.repository.GetByIdAsync<SPPOOOrder>(id);
            this.repository.Detach<SPPOOOrder>(order);

            return order.To<OrderVM>();
        }

        public async Task<List<OrderVM>> GetOrdersByIdsAsync(List<int> ids)
        {
            IQueryable<SPPOOOrder> orders = this.repository.All<SPPOOOrder>(this.FilterOrderByIds(ids));

            return await orders.To<OrderVM>().ToListAsync();
        }

        public async Task<int> CreateOrder(OrderVM model)
        {
            var newOrder = model.To<SPPOOOrder>();

            newOrder.UploadedFileName = "#";


            await this.repository.AddAsync<SPPOOOrder>(newOrder);
            var result = await this.repository.SaveChangesAsync();

            if (result == 1)
            {
                model.IdOrder = newOrder.IdOrder;
                return newOrder.IdOrder;
            }

            return GlobalConstants.INVALID_ID;
        }

        public async Task<IEnumerable<OrderVM>> GetAllOrdersAsync()
        {
            var data = this.repository.All<SPPOOOrder>();

            var result = data.To<OrderVM>().ToList();
            result.Sort((x, y) => DateTime.Compare((DateTime)y.OrderDate, (DateTime)x.OrderDate));

            return result.ToList();
        }

        public async Task<IEnumerable<OrderVM>> GetAllOrdersAsync(OrderVM filterOrderVM)
        {
            var data = this.repository.All<SPPOOOrder>(FilterOrderValue(filterOrderVM));

            return await data.To<OrderVM>().ToListAsync();
        }

        public async Task<bool> CheckForDublicateOrder(DateTime? orderDate, string orderNumber, int idOrder)
        {
            var date = DateTime.MinValue;
            if (orderDate.HasValue)
            {
                date = orderDate.Value;
            }

            var data = this.repository.All<SPPOOOrder>(o => o.OrderDate == date && o.OrderNumber == orderNumber && o.IdOrder != idOrder);

            if (data != null && data.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected Expression<Func<SPPOOOrder, bool>> FilterOrderValue(OrderVM model)
        {
            var predicate = PredicateBuilder.True<SPPOOOrder>();

            if (!string.IsNullOrEmpty(model.OrderNumber))
            {
                predicate = predicate.And(p => p.OrderNumber.Contains(model.OrderNumber));
            }


            if (model.OrderDate.HasValue)
            {
                predicate = predicate.And(p => p.OrderDate == model.OrderDate);
            }

            if (model.IsExactOrderDate)
            {
                if (model.OrderDateFrom.HasValue)
                {
                    predicate = predicate.And(p => p.OrderDate == model.OrderDateFrom.Value);
                }
            }
            else
            {
                if (model.OrderDateFrom.HasValue)
                {
                    predicate = predicate.And(p => p.OrderDate >= model.OrderDateFrom.Value);
                }
                if (model.OrderDateTo.HasValue)
                {
                    predicate = predicate.And(p => p.OrderDate <= model.OrderDateTo.Value);
                }
            }

            return predicate;
        }

        protected Expression<Func<SPPOOOrder, bool>> FilterOrderByIds(List<int> ids)
        {
            var predicate = PredicateBuilder.True<SPPOOOrder>();

            predicate = predicate.And(n => ids.Contains(n.IdOrder));

            return predicate;
        }

        public async Task<int> UpdateOrderAsync(OrderVM model)
        {
            if (model.IdOrder == 0)
            {
                var id = await this.CreateOrder(model);
                return id;
            }
            else
            {
                var updatedEnity = await this.GetByIdAsync<SPPOOOrder>(model.IdOrder);
                this.repository.Detach<SPPOOOrder>(updatedEnity);

                updatedEnity = model.To<SPPOOOrder>();


                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();

                return updatedEnity.IdOrder;
            }
        }

        //Changed: implemented a foreach loop to work with a list of orders and delete them consecutively
        public async Task<int> DeleteOrderAsync(OrderVM order)
        {
            await this.repository.HardDeleteAsync<SPPOOOrder>(order.IdOrder);


            return await this.repository.SaveChangesAsync();
        }


    }
}