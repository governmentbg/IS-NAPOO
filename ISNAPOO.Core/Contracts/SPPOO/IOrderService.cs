using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface IOrderService : IBaseService
    {
        Task<int> CreateOrder(OrderVM model);

        Task<IEnumerable<OrderVM>> GetAllOrdersAsync(OrderVM filterOrderVM);

        Task<IEnumerable<OrderVM>> GetAllOrdersAsync();

        Task<bool> CheckForDublicateOrder(DateTime? orderDate, string orderNumber, int idOrder);

        Task<List<OrderVM>> GetOrdersByIdsAsync(List<int> ids);

        Task<OrderVM> GetOrderByIdAsync(int id);

        Task<int> UpdateOrderAsync(OrderVM model);

        Task<int> DeleteOrderAsync(OrderVM model);

    }
}
