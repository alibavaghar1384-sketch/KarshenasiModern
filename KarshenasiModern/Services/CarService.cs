using KarshenasiModern.Database;
using KarshenasiModern.Models;
using Microsoft.EntityFrameworkCore;

namespace KarshenasiModern.Services;

public class CarService
{
    private readonly AppDbContext _db = new();

    public async Task<Car?> FindByChassisAsync(string chassis)
    {
        return await _db.Cars
            .Include(x => x.Inspections)
            .FirstOrDefaultAsync(x =>
                x.ChassisNumber == chassis);
    }

    public async Task<bool> ExistsAsync(string chassis)
    {
        return await _db.Cars.AnyAsync(x =>
            x.ChassisNumber == chassis);
    }
}