namespace Dismissal_Appointment.Services;

public class EntryBaseService : IEntryService<EntryBase>
{
    private readonly AppDbContext _context;

    public EntryBaseService(AppDbContext context)
    {
        _context = context;
    }

    public Task<EntryBase> Add(EntryBase entry)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<EntryBase>> GetAll()
    {
        return await _context.Entries
            .OrderByDescending(a => a.Id)
            .ToListAsync();
    }

    public Task<EntryBase?> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<EntryBase> Update(EntryBase entry)
    {
        throw new NotImplementedException();
    }
}
