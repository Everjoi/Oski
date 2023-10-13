using Microsoft.EntityFrameworkCore;
using Moq;
using Oski.Domain.Entities;
using Oski.Persistance.Contexts;
using Oski.Persistance.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Oski.IntegrationTest
{
    public class UnitOfWorkIntegrationTests
    {
        private AppDataContext _context;
        private UnitOfWork _unitOfWork;

        public UnitOfWorkIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDataContext>()
                .UseInMemoryDatabase(databaseName: "OskiDb")  
                .Options;

            _context = new AppDataContext(options);
            _unitOfWork = new UnitOfWork(_context);
        }


        [Fact]
        public void Repository_ShouldReturnRepositoryForEntityType()
        {
            var userRepo = _unitOfWork.Repository<User>();

            Assert.NotNull(userRepo);
        }


        [Fact]
        public void Dispose_ShouldDisposeDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDataContext>()
                .Options;

            var mockDbContext = new Mock<AppDataContext>();

            var uow = new UnitOfWork(mockDbContext.Object);

            uow.Dispose();

            mockDbContext.Verify(db => db.Dispose(),Times.Once());
        }


        [Fact]
        public async Task Rollback_ShouldDiscardChanges()
        {
            var uow = new UnitOfWork(_context);

            var user = new User { Id = Guid.NewGuid(),FullName = "hwretgw",Email = "wergw@example.com",Password = "hwtergwr" };
            _context.Users.Add(user);

            await uow.Rollback();

            Assert.False(_context.Users.Any(u => u.Id == user.Id));
        }


        [Fact]
        public async Task Save_ShouldAuditAndSaveChanges()
        {
            var unitOfWork = new UnitOfWork(_context);

            
            var user = new User { FullName = "Test User",Email = "test@email.com",Password = "password" };
            _context.Users.Add(user);

             
            var userId = Guid.NewGuid();
            await unitOfWork.Save(userId,CancellationToken.None);

            user = _context.Users.FirstOrDefault(u => u.Email == "test@email.com");
            Assert.NotNull(user);
            Assert.Equal(userId,user.CreatedBy);
            Assert.NotNull(user.CreatedDate);
        }



        [Fact]
        public void Repository_ShouldReturnRepositoryForType()
        {
            var userRepo = _unitOfWork.Repository<User>();

            Assert.NotNull(userRepo);
            Assert.IsType<GenericRepository<User>>(userRepo);
        }

        [Fact]
        public void Repository_ShouldReturnSameInstanceForSameType()
        {
            var firstUserRepo = _unitOfWork.Repository<User>();
            var secondUserRepo = _unitOfWork.Repository<User>();

            Assert.Same(firstUserRepo,secondUserRepo);
        }

        [Fact]
        public void Repository_ShouldReturnDifferentInstancesForDifferentTypes()
        {
            var userRepo = _unitOfWork.Repository<User>();
            var testRepo = _unitOfWork.Repository<Test>();

            Assert.NotSame(userRepo,testRepo);
        }



    }
}
