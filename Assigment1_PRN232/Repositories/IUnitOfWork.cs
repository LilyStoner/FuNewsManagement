using Assigment1_PRN232_BE.Models;

namespace Assigment1_PRN232_BE.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<SystemAccount> AccountRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<NewsArticle> NewsArticleRepository { get; }
        IRepository<Tag> TagRepository { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}