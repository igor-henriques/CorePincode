using CorePinCash.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorePincode.Repository
{
    public interface IPinItemRepository
    {
        Task Add(PinItem PinItem);
        Task<int> CountNotObtained();
        Task<List<PinItem>> Get();
        Task<PinItem> Get(int Id);
        Task<List<string>> GetAllPincodes();
        Task<List<string>> GetAvailablePincodes();
        Task<List<PinItem>> GetNotObtained();
        Task<PinItem> GetPinItemByPincode(string pincode);
        Task Update(PinItem PinItem);
    }
}