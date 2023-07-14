using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts
{
    /// <summary>
    /// Базова услуга за достъп до базата данни
    /// </summary>
    public interface IBaseService
    {
        /// <summary>
        /// Извлича данни за един обект по идентификатор
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdObjectAsync<T>(object id) where T : class;

        /// <summary>
        /// Извлича данни за един обект по идентификатор
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync<T>(int id) where T : class;
        Task<long> GetSequenceNextValue(string resource, int? idResource = 0, int? year = 0);
    }
}
