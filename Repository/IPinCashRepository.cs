using System.Collections.Generic;
using System.Threading.Tasks;
using CorePinCash.Model;

namespace CorePinCash.Repository;

public interface IPinCashRepository
{
	Task<List<PinCash>> Get();

	Task<List<PinCash>> GetNotObtained();

	Task<PinCash> Get(int Id);

	Task<PinCash> GetPinCashByPincode(string pincode);

	Task<List<string>> GetAllPincodes();

	Task<List<string>> GetAvailablePincodes();

	Task Update(PinCash PinCash);

	Task<int> CountNotObtained();

	Task Add(PinCash PinCash);
}
