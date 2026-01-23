using Microsoft.EntityFrameworkCore.Storage;
using Assigment1_PRN232_BE.Models;

namespace Assigment1_PRN232_BE.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FunewsManagementContext _context;
        private IDbContextTransaction? _transaction;
        
        private IRepository<SystemAccount>? _accountRepository;
        private IRepository<Category>? _categoryRepository;
        private IRepository<NewsArticle>? _newsArticleRepository;
        private IRepository<Tag>? _tagRepository;

        public UnitOfWork(FunewsManagementContext context)
        {
            _context = context;
        }

        public IRepository<SystemAccount> AccountRepository
        {
            get { return _accountRepository ??= new Repository<SystemAccount>(_context); }
        }

        public IRepository<Category> CategoryRepository
        {
            get { return _categoryRepository ??= new Repository<Category>(_context); }
        }

        public IRepository<NewsArticle> NewsArticleRepository
        {
            get { return _newsArticleRepository ??= new Repository<NewsArticle>(_context); }
        }

        public IRepository<Tag> TagRepository
        {
            get { return _tagRepository ??= new Repository<Tag>(_context); }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}