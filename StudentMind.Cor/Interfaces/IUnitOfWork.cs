namespace StudentMind.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        void Dispose();
        void Save();
        Task SaveAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollBack();
        ValueTask DisposeAsync();

    }
}
