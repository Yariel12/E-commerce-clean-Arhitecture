using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Address?> GetByAsync(int id)
        {
            return await _context.Addresses
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Address>> GetByUserIdAsync(int userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Address> AddAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address?> UpdateAsync(Address address)
        {
            var existing = await _context.Addresses.FindAsync(address.Id);
            if (existing == null)
                return null;

            existing.Street = address.Street;
            existing.City = address.City;
            existing.State = address.State;
            existing.Country = address.Country;
            existing.PostalCode = address.PostalCode;
            existing.UserId = address.UserId;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task DeleteAsync(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
            }
        }
    }
}
