using Microsoft.IdentityModel.Tokens;
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

            if(existingAttempt != null || _unitOfWork.Repository<Test>().GetByIdAsync(testId).Result == null)
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

            var userAnswer = new Answer
            {
                Id = answerId, 
                QuestionId = questionId,
            };

            _unitOfWork.Repository<Answer>().AddAsync(userAnswer);  
            _unitOfWork.Save(userId,default);  

            return true;
        }

        public int FinishTest(Guid userId,Guid attemptId, Guid testId)
        {
            var attempt = _unitOfWork.Repository<UserTestAttempt>().Entities
                .FirstOrDefault(x => x.UserId == userId && x.Id == attemptId && x.IsCompleted == false);

            if(attempt == null)
                return -1;

            attempt.IsCompleted = true;

            _unitOfWork.Repository<UserTestAttempt>().UpdateAsync(attempt);
            _unitOfWork.Save(userId,default);


            var test = _unitOfWork.Repository<Test>().Entities
                .FirstOrDefault(x => x.Id == testId);


            return CalculateScore(attemptId,test.MaxScore);
        }

        

        public void RegisterUserAnswer(Guid attemptId,Guid answerId)
        {
            var attempt = _unitOfWork.Repository<UserTestAttempt>().Entities
                .FirstOrDefault(x => x.Id == attemptId);

            if(attempt == null)
                throw new InvalidOperationException("Attempt not found.");

            var selectedAnswer = _unitOfWork.Repository<Answer>().Entities
                .FirstOrDefault(a => a.Id == answerId);

            if(selectedAnswer == null)
                throw new InvalidOperationException("Answer not found.");


            var userAnswer = new UserAnswer
            {
                AttemptId = attemptId,
                AnswerId = answerId,
                IsCorrect = selectedAnswer.IsCorrect
            };

            _unitOfWork.Repository<UserAnswer>().AddAsync(userAnswer);
            _unitOfWork.Save(default,default);
        }


        public int CalculateScore(Guid attemptId, int maxScore)
        {

            var userAnswers = _unitOfWork.Repository<UserAnswer>().Entities
                .Where(x => x.AttemptId == attemptId).ToList();

            if(!userAnswers.Any())
                throw new InvalidOperationException("No answers found for this attempt.");


            int correctAnswersCount = userAnswers.Count(ua => ua.IsCorrect);


            var attempt = _unitOfWork.Repository<UserTestAttempt>().Entities
                .FirstOrDefault(x => x.Id == attemptId);

            if(attempt == null)
                throw new InvalidOperationException("Attempt not found.");



            double percentageCorrect = (double)correctAnswersCount / userAnswers.Count;
            int score = (int)(percentageCorrect * maxScore);

             
            attempt.Score = score;
            _unitOfWork.Repository<UserTestAttempt>().UpdateAsync(attempt);
            _unitOfWork.Save(default,default);

            return score;  
        }



        public IEnumerable<Test> GetAllTest()
        {
            var tests = _unitOfWork.Repository<Test>().Entities.AsEnumerable();

            if(tests.IsNullOrEmpty())
                throw new NotFoundException();

            return tests;
        }



        public  Test GetTestById(Guid testid)
        {
            var test =  _unitOfWork.Repository<Test>().GetByIdAsync(testid);

            if(test.Result==null)
                throw new NotFoundException();

            return test.Result;
        }


    }
}
