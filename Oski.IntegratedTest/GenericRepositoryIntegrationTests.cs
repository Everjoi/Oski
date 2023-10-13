using Microsoft.EntityFrameworkCore;
using Oski.Domain.Entities;
using Oski.Domain.Exceptions;
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
    public class GenericRepositoryIntegrationTests 
    {
        private AppDataContext _context;
        private GenericRepository<User> _userRepository;

        public GenericRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;
            _context = new AppDataContext(options);
            _userRepository = new GenericRepository<User>(_context);
        }


         
        [Fact]
        public async Task AddAsync_ShouldAddUserToDatabase()
        {
            var user = new User { Id = Guid.NewGuid(),FullName = "asdg",Email = "hwer@example.com",Password = "wertwerg" };

            await _userRepository.AddAsync(user);
            await _context.SaveChangesAsync();

            var retrievedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.FullName,retrievedUser.FullName);
        }


        [Fact]
        public async Task UpdateAsync_ShouldUpdateUserCorrectly()
        {
            var originalUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = "TestUser",
                Email = "test@example.com",
                Password = "qwerty"
            };
            _context.Users.Add(originalUser);
            _context.SaveChanges();

            originalUser.FullName = "TestUser2";
            originalUser.Email = "test2@example.com";

            await _userRepository.UpdateAsync(originalUser);
            await _context.SaveChangesAsync(); 


            var updatedUser = _context.Users.Find(originalUser.Id);


            Assert.NotNull(updatedUser);
            Assert.Equal("TestUser2",updatedUser.FullName);
            Assert.Equal("test2@example.com",updatedUser.Email);
        }





        [Fact]
        public async Task DeleteAsync_ShouldDeleteUserFromDatabase()
        {
            var userToDelete = new User
            {
                Id = Guid.NewGuid(),
                FullName = "sdffgsdf",
                Email = "dfsgdsfg@example.com",
                Password = "rtherther"
            };
            _context.Users.Add(userToDelete);
            _context.SaveChanges();

            await _userRepository.DeleteAsync(userToDelete);
            await _context.SaveChangesAsync();  

            var fetchedUser = _context.Users.Find(userToDelete.Id);

             
            Assert.Null(fetchedUser);  
        }



        [Fact]
        public void Entities_ShouldReturnAllUsers()
        {
            var user1 = new User { Id = Guid.NewGuid(),FullName = "John Doe",Email = "john@example.com",Password = "secure123" };
            var user2 = new User { Id = Guid.NewGuid(),FullName = "Jane Smith",Email = "jane@example.com",Password = "secure456" };

            _context.Users.AddRange(user1,user2);
            _context.SaveChanges();

            var usersFromRepo = _userRepository.Entities.ToList();

            Assert.Equal(2,usersFromRepo.Count);
            Assert.Contains(usersFromRepo,u => u.Id == user1.Id);
            Assert.Contains(usersFromRepo,u => u.Id == user2.Id);
        }



        [Fact]
        public async Task GetAllAsync_ShouldRetrieveAllUsersFromDatabase()
        {
            var user1 = new User { Id = Guid.NewGuid(),FullName = "asdfasd",Email = "asdf@example.com",Password = "asdgasdg" };
            var user2 = new User { Id = Guid.NewGuid(),FullName = "asdfasd",Email = "adfas@example.com",Password = "asdgasg" };
            _context.Users.AddRange(user1,user2);
            _context.SaveChanges();

            var users = await _userRepository.GetAllAsync();

            Assert.Equal(2,users.Count); 
        }

        [Fact]
        public async Task GetByIdAsync_ShouldRetrieveUserByIdFromDatabase()
        {
            var user = new User { Id = Guid.NewGuid(),FullName = "asdgfas",Email = "asdf@example.com",Password = "gasdfgsd" };
            _context.Users.Add(user);
            _context.SaveChanges();

            var retrievedUser = await _userRepository.GetByIdAsync(user.Id);


            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id,retrievedUser.Id);
            Assert.Equal(user.FullName,retrievedUser.FullName);
            Assert.Equal(user.Email,retrievedUser.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldThrowNotFoundException()
        {
            var invalidId = Guid.NewGuid();

            await Assert.ThrowsAsync<NotFoundException>(() => _userRepository.GetByIdAsync(invalidId));
        }

       
    }
    
}
