using Application.DatingApp.Interface;
using AutoMapper;
using Infrastructure.Persistance.Interface;

namespace Infrastructure.Persistance
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        #region Constructor
        public UnitOfWork(IDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion

        #region Implementation
        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.HasChanges();
        }
        #endregion

    }
}
