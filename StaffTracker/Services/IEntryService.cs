namespace StaffTracker.Services;

public interface IEntryService<T> where T : class
{
    Task<List<T>> GetAll();
    Task<T?> GetById(int id);
    Task<T> Add(T entry);
    Task<T> Update(T entry);
    Task<bool> Delete(int id);
}
