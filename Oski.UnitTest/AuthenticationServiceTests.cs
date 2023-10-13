using Castle.Core.Configuration;
using Moq;
using Oski.Application.Extensions;
using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Entities;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace Oski.UnitTest
{
    public class AuthenticationServiceTests
    {
        private Mock<IGenericRepository<User>> _mockUserRepo;
        private Mock<Microsoft.Extensions.Configuration.IConfiguration> _mockConfiguration;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private AuthenticationService _service;

        public AuthenticationServiceTests()
        {
            _mockUserRepo = new Mock<IGenericRepository<User>>();
            _mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(_mockUserRepo.Object);

            _service = new AuthenticationService(_mockConfiguration.Object,_mockUnitOfWork.Object);
        }

        [Fact]
        public void Authenticate_WithValidCredentials_ShouldReturnToken()
        {
            _mockUserRepo.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(new List<User>
            {
                new User { Email = "test@email.com", Password = BCrypt.Net.BCrypt.HashPassword("password") }
            }.ToList()));

            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("YourJwtKeyHere12345678");

            var result = _service.Authenticate("test@email.com","password");

            Assert.NotNull(result);
        }

        [Fact]
        public void Authenticate_WithInvalidEmail_ShouldReturnNull()
        {
            _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
            var result = _service.Authenticate("invalid@email.com","password");
            Assert.Null(result);
        }


        [Fact]
        public void Authenticate_WithInvalidPassword_ShouldReturnNull()
        {
            var mockUser = new User
            {
                Email = "test@email.com",
                Password = BCrypt.Net.BCrypt.HashPassword("hashed_actual_password")
            };

            _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { mockUser });

            var result = _service.Authenticate("test@email.com","invalid_password");

            Assert.Null(result);
        }


        [Fact]
        public async Task Register_WithExistingEmail_ShouldReturnEmptyGuid()
        {
           
            var existingUser = new User
            {
                Email = "existing@email.com",
                Password = "hashed_existing_password"  
            };

            _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { existingUser });

            var result = await _service.Register("Test","existing@email.com","password");

            Assert.Equal(Guid.Empty,result);
        }



        [Fact]
        public async Task Register_WithNewEmail_ShouldReturnUserId()
        {
            // Налаштовуємо мок, щоб повертати пустий список користувачів (оскільки ми очікуємо, що електронна адреса нова)
            _mockUserRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

            var result = await _service.Register("Test","newnew@email.com","password");

            Assert.NotEqual(Guid.Empty,result); 
        }


    }

}
