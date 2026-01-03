namespace Dismissal_Appointment.Services;

public class AppointmentService : IEntryService<Appointment>
{
    private readonly AppDbContext _context;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Appointment>> GetAll()
    {
        try
        {
            return await _context.Appointments
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while retrieving appointments: {ErrorMessage}", errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to retrieve appointments: {ErrorMessage}", errorMessage);
            throw;
        }
    }

    public async Task<Appointment?> GetById(int id)
    {
        try
        {
            return await _context.Appointments.FindAsync(id);
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while retrieving appointment by ID {AppointmentId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to retrieve appointment by ID {AppointmentId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
    }

    public async Task<Appointment> Add(Appointment entry)
    {
        try
        {
            _context.Appointments.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while adding appointment for {FirstName} {Surname}: {ErrorMessage}", entry.FirstName, entry.Surname, errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to add appointment for {FirstName} {Surname}: {ErrorMessage}", entry.FirstName, entry.Surname, errorMessage);
            throw;
        }
    }

    public async Task<Appointment> Update(Appointment entry)
    {
        try
        {
            _context.Appointments.Update(entry);
            await _context.SaveChangesAsync();
            return entry;
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while updating appointment ID {AppointmentId}: {ErrorMessage}", entry.Id, errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to update appointment ID {AppointmentId}: {ErrorMessage}", entry.Id, errorMessage);
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException dbEx)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(dbEx);
            Log.Error("Database update error while deleting appointment ID {AppointmentId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
        catch (Exception ex)
        {
            string errorMessage = Utils.Utils.GetFullExceptionMessage(ex);
            Log.Error("Failed to delete appointment ID {AppointmentId}: {ErrorMessage}", id, errorMessage);
            throw;
        }
    }
}
