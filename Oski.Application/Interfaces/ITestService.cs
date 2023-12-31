﻿
using Oski.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oski.Application.Interfaces
{
    public interface ITestService
    {
        Guid StartTest(Guid userId,Guid testId);
        bool AnswerQuestion(Guid userId,Guid attemptId,Guid questionId,Guid answerId);
        int FinishTest(Guid userId,Guid attemptId, Guid testId);
        IEnumerable<Test> GetAllTest();
        Test GetTestById(Guid testId);
    }
}
