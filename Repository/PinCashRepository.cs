using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorePinCash.Data;
using CorePinCash.Model;
using Microsoft.EntityFrameworkCore;

namespace CorePinCash.Repository;

public class PinCashRepository : IPinCashRepository
{
	private readonly ApplicationDbContext _context;

	public PinCashRepository(ApplicationDbContext _context)
	{
		this._context = _context;
	}

	public async Task<int> CountNotObtained()
	{
		return await _context.PinCash.Where((PinCash x) => !x.Obtained).CountAsync();
	}

	public async Task<List<PinCash>> Get()
	{
		return await _context.PinCash.ToListAsync();
	}

	public async Task<PinCash> Get(int Id)
	{
		return await _context.PinCash.FindAsync(Id);
	}

	public async Task<List<PinCash>> GetNotObtained()
	{
		return await _context.PinCash.Where((PinCash x) => !x.Obtained).ToListAsync();
	}

	public async Task Update(PinCash PinCash)
	{
		PinCash curPinCash = await Get(PinCash.Id);
		if (curPinCash != null)
		{
			curPinCash.CashAmount = PinCash.CashAmount;
			curPinCash.Code = PinCash.Code;
			curPinCash.DateGenerated = PinCash.DateGenerated;
			curPinCash.DateObtained = PinCash.DateObtained;
			curPinCash.Obtained = PinCash.Obtained;
			curPinCash.ObtainedBy = PinCash.ObtainedBy;
		}
		await _context.SaveChangesAsync();
	}

	public async Task Add(PinCash PinCash)
	{
		await _context.PinCash.AddAsync(PinCash);
		await _context.SaveChangesAsync();
	}

	public async Task<List<string>> GetAllPincodes()
	{
		return await _context.PinCash.Select((PinCash x) => x.Code).ToListAsync();
	}

	public async Task<List<string>> GetAvailablePincodes()
	{
		return await (from x in _context.PinCash
			where x.Obtained == false
			select x.Code).ToListAsync();
	}

	public async Task<PinCash> GetPinCashByPincode(string pincode)
	{
		return await _context.PinCash.Where((PinCash x) => x.Code.Equals(pincode)).FirstOrDefaultAsync();
	}
}
