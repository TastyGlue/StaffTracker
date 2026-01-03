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
        // Not implemented because it is not used directly
        throw new NotImplementedException();
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var entry = await _context.Entries.FindAsync(id);
            if (entry == null)
                return false;

            _context.Entries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while deleting entry ID {EntryId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to delete entry ID {EntryId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
    }

    public async Task<List<EntryBase>> GetAll()
    {
        try
        {
            return await _context.Entries
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while retrieving all entries: {ErrorMessage}", errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to retrieve all entries: {ErrorMessage}", errorMessage);
            throw;
        }
    }

    public Task<EntryBase?> GetById(int id)
    {
        // Not implemented because it is not used directly
        throw new NotImplementedException();
    }

    public Task<EntryBase> Update(EntryBase entry)
    {
        // Not implemented because it is not used directly
        throw new NotImplementedException();
    }
}
