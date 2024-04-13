﻿using Microsoft.EntityFrameworkCore;
using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public class EFOrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public EFOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.Include(x => x.OrderDetails).ToListAsync();
        }
        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(x => x.OrderDetails).SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task UpdateAsync(bool status)
        {
            //Cần xử lý
            await _context.SaveChangesAsync();
        }
    }
}
