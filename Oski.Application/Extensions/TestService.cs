using Oski.Application.Interfaces;
using Oski.Application.Interfaces.Repositories;
using Oski.Domain.Entities;
using Oski.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Extensions
{
    public class TestService : ITestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Test> _testRepository;

        public TestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _testRepository = _unitOfWork.Repository<Test>();
            
        }

        public Guid StartTest(Guid userId,Guid testId)
        {  

            var existingAttempt = _unitOfWork.Repository<UserTestAttempt>().Entities
                .FirstOrDefault(x => x.UserId == userId && x.TestId == testId && x.IsCompleted == false);

            if(existingAttempt != null || _unitOfWork.Repository<Test>().GetByIdAsync(testId) == null)
                throw new CompletedTestExcetion(userId);


            var newAttempt = new UserTestAttempt
            {
                UserId = userId,
                TestId = testId,
                IsCompleted = false
            };

            _unitOfWork.Repository<UserTestAttempt>().AddAsync(newAttempt);
            _unitOfWork.Save(userId,default);

            return newAttempt.Id;
        }

        public bool AnswerQuestion(Guid userId,Guid attemptId,Guid questionId,Guid answerId)
        {
            var attempt = _unitOfWork.Repository<UserTestAttempt>().Entities
                .FirstOrDefault(x => x.UserId == userId && x.Id == attemptId && x.IsCompleted == false );

            if(attempt == null)
                return false;

            return true;
        }

        public int FinishTest(Guid userId,Guid attemptId)
        {
            var attempt = _unitOfWork.Repository<UserTestAttempt>().Entities
                .FirstOrDefault(x => x.UserId == userId && x.Id == attemptId && x.IsCompleted == false);

            if(attempt == null)
                return -1;

            attempt.IsCompleted = true;

            _unitOfWork.Repository<UserTestAttempt>().UpdateAsync(attempt);
            _unitOfWork.Save(userId,default);

            return attempt.Score;
        }
    }
}
