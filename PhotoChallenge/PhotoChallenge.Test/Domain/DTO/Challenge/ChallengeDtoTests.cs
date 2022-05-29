using NUnit.Framework;
using PhotoChallenge.Domain.DTO.Challenge;
using PhotoChallenge.Domain.Enums;
using System;

namespace PhotoChallenge.Test.Domain.DTO
{
    internal class ChallengeDtoTests
    {
        [TestCase("2030-01-01", "2030-01-10", ChallengeStatus.Draft, "Draft")]
        [TestCase("2030-01-01", "2030-01-10", ChallengeStatus.Live, "Live")]
        [TestCase("2030-01-01", "2030-01-10", ChallengeStatus.Removed, "Removed")]
        [TestCase("2010-01-01", "2030-01-10", ChallengeStatus.Live, "Actual")]
        [TestCase("2010-01-01", "2010-01-10", ChallengeStatus.Live, "Expired")]
        public void CurrentStatus_SetDates_CorrectCurrentStatus(DateTime startDate, DateTime endDate, ChallengeStatus status, string expectedStatus)
        {
            // Arrange
            var challenge = new ChallengeDto
            {
                DateStart = startDate,
                DateEnd = endDate,
                Status = status
            };

            // Act
            var currentStatus = challenge.CurrentStatus;

            // Assert
            Assert.AreEqual(expectedStatus, currentStatus);
        }
    }
}
