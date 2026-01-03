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
        try
        {
            return await _context.Dismissals
                .OrderByDescending(d => d.Id)
                .ToListAsync();
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while retrieving dismissals: {ErrorMessage}", errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to retrieve dismissals: {ErrorMessage}", errorMessage);
            throw;
        }
    }

    public async Task<Dismissal?> GetById(int id)
    {
        try
        {
            return await _context.Dismissals.FindAsync(id);
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while retrieving dismissal by ID {DismissalId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to retrieve dismissal by ID {DismissalId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
    }

    public async Task<Dismissal> Add(Dismissal entry)
    {
        try
        {
            _context.Dismissals.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while adding dismissal for {FirstName} {Surname}: {ErrorMessage}", entry.FirstName, entry.Surname, errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to add dismissal for {FirstName} {Surname}: {ErrorMessage}", entry.FirstName, entry.Surname, errorMessage);
            throw;
        }
    }

    public async Task<Dismissal> Update(Dismissal entry)
    {
        try
        {
            _context.Dismissals.Update(entry);
            await _context.SaveChangesAsync();
            return entry;
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while updating dismissal ID {DismissalId}: {ErrorMessage}", entry.Id, errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to update dismissal ID {DismissalId}: {ErrorMessage}", entry.Id, errorMessage);
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var dismissal = await _context.Dismissals.FindAsync(id);
            if (dismissal == null)
                return false;

            _context.Dismissals.Remove(dismissal);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while deleting dismissal ID {DismissalId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to delete dismissal ID {DismissalId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
    }
}
