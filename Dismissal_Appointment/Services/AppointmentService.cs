using Microsoft.EntityFrameworkCore;

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
        return await _context.Appointments
            .OrderByDescending(a => a.Id)
            .ToListAsync();
    }

    public async Task<Appointment?> GetById(int id)
    {
        return await _context.Appointments.FindAsync(id);
    }

    public async Task<Appointment> Add(Appointment entry)
    {
        _context.Appointments.Add(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<Appointment> Update(Appointment entry)
    {
        _context.Appointments.Update(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<bool> Delete(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return false;

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }
}
