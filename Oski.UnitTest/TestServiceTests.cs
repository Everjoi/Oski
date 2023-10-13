using Moq;
using Oski.Application.Extensions;
using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Entities;
using Oski.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Oski.UnitTest
{
    public class TestServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Test>> _mockTestRepo;
        private readonly Mock<IGenericRepository<UserTestAttempt>> _mockAttemptRepo;
        private readonly Mock<IGenericRepository<Answer>> _mockAnswerRepo;
        private readonly Mock<IGenericRepository<UserAnswer>> _mockUserAnswerRepo;

        private readonly TestService _service;

        public TestServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTestRepo = new Mock<IGenericRepository<Test>>();
            _mockAttemptRepo = new Mock<IGenericRepository<UserTestAttempt>>();
            _mockAnswerRepo = new Mock<IGenericRepository<Answer>>();
            _mockUserAnswerRepo = new Mock<IGenericRepository<UserAnswer>>();

            _mockUnitOfWork.Setup(u => u.Repository<Test>()).Returns(_mockTestRepo.Object);
            _mockUnitOfWork.Setup(u => u.Repository<UserTestAttempt>()).Returns(_mockAttemptRepo.Object);
            _mockUnitOfWork.Setup(u => u.Repository<Answer>()).Returns(_mockAnswerRepo.Object);
            _mockUnitOfWork.Setup(u => u.Repository<UserAnswer>()).Returns(_mockUserAnswerRepo.Object);

            _service = new TestService(_mockUnitOfWork.Object);
        }


        [Fact]
        public void StartTest_WithNonExistingTest_ShouldThrowException()
        {
            _mockTestRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult<Test>(null));

            Assert.Throws<CompletedTestExcetion>(() => _service.StartTest(Guid.NewGuid(),Guid.NewGuid()));
        }



        [Fact]
        public async Task AnswerQuestion_WithValidAttempt_ShouldReturnTrue()
        {
            var userId = Guid.Parse("dcaf232a-d385-4c5f-a65e-b56a83d6e30a");
            var attemptId = Guid.Parse("116da284-e427-4261-a4e5-78fdd9dcbc37");

            _mockAttemptRepo.Setup(r => r.Entities).Returns(new List<UserTestAttempt>
            {
                new UserTestAttempt { Id = attemptId, UserId = userId, IsCompleted = false }
            }.AsQueryable());

            var result = _service.AnswerQuestion(userId,attemptId,Guid.NewGuid(),Guid.NewGuid());

            Assert.True(result);
        }

        [Fact]
        public async Task FinishTest_WithInvalidAttempt_ShouldReturnMinusOne()
        {
            _mockAttemptRepo.Setup(r => r.Entities).Returns(new List<UserTestAttempt>().AsQueryable());

            var result = _service.FinishTest(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid());

            Assert.Equal(-1,result);
        }

        [Fact]
        public void RegisterUserAnswer_WithNonExistingAttempt_ShouldThrowException()
        {
            _mockAttemptRepo.Setup(r => r.Entities).Returns(new List<UserTestAttempt>().AsQueryable());

            Assert.Throws<InvalidOperationException>(() => _service.RegisterUserAnswer(Guid.NewGuid(),Guid.NewGuid()));
        }

        [Fact]
        public void CalculateScore_WithNoUserAnswers_ShouldThrowException()
        {
            _mockUserAnswerRepo.Setup(r => r.Entities).Returns(new List<UserAnswer>().AsQueryable());

            Assert.Throws<InvalidOperationException>(() => _service.CalculateScore(Guid.NewGuid(),100));
        }

        [Fact]
        public void GetAllTest_WithNoTests_ShouldThrowNotFoundException()
        {
            _mockTestRepo.Setup(r => r.Entities).Returns(new List<Test>().AsQueryable());

            Assert.Throws<NotFoundException>(() => _service.GetAllTest());
        }

        [Fact]
        public void GetTestById_WithNonExistingTestId_ShouldThrowNotFoundException()
        {
            _mockTestRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Test)null);

            Assert.Throws<NotFoundException>(() => _service.GetTestById(Guid.NewGuid()));
        }






        [Fact]
        public void StartTest_WithExistingAttempt_ShouldThrowException()
        {
            Guid testUserId = Guid.NewGuid();
            Guid testTestId = Guid.NewGuid();

            _mockAttemptRepo.Setup(r => r.Entities).Returns(new List<UserTestAttempt>
            {
                new UserTestAttempt { UserId = testUserId, TestId = testTestId, IsCompleted = false }
            }.AsQueryable());

            _mockTestRepo.Setup(r => r.GetByIdAsync(testTestId)).ReturnsAsync(new Test());

            Assert.Throws<CompletedTestExcetion>(() => _service.StartTest(testUserId,testTestId));
        }




        [Fact]
        public async Task AnswerQuestion_WithInvalidAttempt_ShouldReturnFalse()
        {
            Guid testAttemptId = Guid.NewGuid();

            _mockAttemptRepo.Setup(r => r.Entities).Returns(new List<UserTestAttempt>().AsQueryable());

            var result = _service.AnswerQuestion(Guid.NewGuid(),testAttemptId,Guid.NewGuid(),Guid.NewGuid());

            Assert.False(result);
        }


    }





}
