using Microsoft.EntityFrameworkCore;

namespace Dismissal_Appointment.Services;

public class DismissalService : IEntryService<Dismissal>
{
    private readonly AppDbContext _context;

    public DismissalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Dismissal>> GetAll()
    {
        return await _context.Dismissals
            .OrderByDescending(d => d.Id)
            .ToListAsync();
    }

    public async Task<Dismissal?> GetById(int id)
    {
        return await _context.Dismissals.FindAsync(id);
    }

    public async Task<Dismissal> Add(Dismissal entry)
    {
        _context.Dismissals.Add(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<Dismissal> Update(Dismissal entry)
    {
        _context.Dismissals.Update(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<bool> Delete(int id)
    {
        var dismissal = await _context.Dismissals.FindAsync(id);
        if (dismissal == null)
            return false;

        _context.Dismissals.Remove(dismissal);
        await _context.SaveChangesAsync();
        return true;
    }
}
