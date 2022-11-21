using CorePinCash.Data;
using CorePinCash.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorePincode.Repository
{
    public class PinItemRepository : IPinItemRepository
    {
        private readonly ApplicationDbContext _context;

        public PinItemRepository(ApplicationDbContext _context)
        {
            this._context = _context;
        }

        public async Task<int> CountNotObtained()
        {
            return await _context.PinItem.Where((PinItem x) => !x.Obtained).CountAsync();
        }

        public async Task<List<PinItem>> Get()
        {
            return await _context.PinItem.ToListAsync();
        }

        public async Task<PinItem> Get(int Id)
        {
            return await _context.PinItem.FindAsync(Id);
        }

        public async Task<List<PinItem>> GetNotObtained()
        {
            return await _context.PinItem.Where((PinItem x) => !x.Obtained).ToListAsync();
        }

        public async Task Update(PinItem PinItem)
        {
            PinItem curPinItem = await Get(PinItem.Id);
            if (curPinItem != null)
            {
                curPinItem.ItemId = PinItem.ItemId;
                curPinItem.ItemAmount = PinItem.ItemAmount;
                curPinItem.Code = PinItem.Code;
                curPinItem.DateGenerated = PinItem.DateGenerated;
                curPinItem.DateObtained = PinItem.DateObtained;
                curPinItem.Obtained = PinItem.Obtained;
                curPinItem.ObtainedBy = PinItem.ObtainedBy;
            }
            await _context.SaveChangesAsync();
        }

        public async Task Add(PinItem PinItem)
        {
            await _context.PinItem.AddAsync(PinItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetAllPincodes()
        {
            return await _context.PinItem.Select((PinItem x) => x.Code).ToListAsync();
        }

        public async Task<List<string>> GetAvailablePincodes()
        {
            return await (from x in _context.PinItem
                          where x.Obtained == false
                          select x.Code).ToListAsync();
        }

        public async Task<PinItem> GetPinItemByPincode(string pincode)
        {
            return await _context.PinItem.Where((PinItem x) => x.Code.Equals(pincode)).FirstOrDefaultAsync();
        }
    }
}
