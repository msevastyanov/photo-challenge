using NUnit.Framework;
using PhotoChallenge.DataAccess.Context;
using PhotoChallenge.DataAccess.Repositories;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoChallenge.Test.DataAccess.Repositories
{
    internal class ChallengeRepositoryTests
    {
        private DataContext _db;
        private ChallengeRepository _challengeRepository;

        private Area area;

        [SetUp]
        public void Setup()
        {
            _db = new InMemoryDbContextFactory().GetDataContext();
            _challengeRepository = new ChallengeRepository(_db);
            area = new Area { Id = 1 };
        }

        [Test]
        public async Task GetChallenges_TwoChallengesInDb_ReturnChallenges()
        {
            // Arrange
            var challenges = new List<Challenge>
            {
                new Challenge { Id = 1, Status = "Draft", Area = area },
                new Challenge { Id = 2, Status = "Live", Area = area },
            };

            _db.Challenge.AddRange(challenges);
            _db.SaveChanges();

            // Act
            var result = await _challengeRepository.GetChallenges();
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<List<Challenge>>(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetLiveChallenges_TwoChallengesInDb_ReturnChallenges()
        {
            // Arrange
            var challenges = new List<Challenge>
            {
                new Challenge { Id = 1, Status = "Draft", Area = area },
                new Challenge { Id = 2, Status = "Live", Area = area },
                new Challenge { Id = 3, Status = "Removed", Area = area },
            };

            _db.Challenge.AddRange(challenges);
            _db.SaveChanges();

            // Act
            var result = await _challengeRepository.GetLiveChallenges();
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<List<Challenge>>(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task FindChallenge_ChallengeExists_ReturnChallenge()
        {
            // Arrange
            var challenges = new List<Challenge>
            {
                new Challenge { Id = 1, Status = "Draft", Area = area },
                new Challenge { Id = 2, Status = "Live", Area = area },
            };

            _db.Challenge.AddRange(challenges);
            _db.SaveChanges();

            // Act
            var result = await _challengeRepository.FindChallenge(1);
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<Challenge>(result);
            Assert.AreEqual(1, result.Id);
            Assert.IsNotNull(result.Area);
        }

        [Test]
        public void FindChallenge_ChallengeDoesNotExist_Exception()
        {
            // Arrange
            var challenges = new List<Challenge>
            {
                new Challenge { Id = 1, Status = "Draft", Area = area },
                new Challenge { Id = 2, Status = "Live", Area = area },
            };

            _db.Challenge.AddRange(challenges);
            _db.SaveChanges();

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _challengeRepository.FindChallenge(3));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityNotFound, ex?.Code);
        }

        [Test]
        public async Task AddChallenge_AddNewChallenge_CorrectAdding()
        {
            // Arrange
            var challenges = new List<Challenge>
            {
                new Challenge { Id = 1, Status = "Draft", Area = area },
                new Challenge { Id = 2, Status = "Live", Area = area },
            };

            _db.Challenge.AddRange(challenges);
            _db.SaveChanges();

            // Act
            var challenge = new Challenge
            {
                Status = "Draft"
            };
            var result = await _challengeRepository.AddChallenge(challenge);
            var challengesCount = _db.Challenge.Count();

            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<Challenge>(result);
            Assert.AreEqual(3, result.Id);
            Assert.AreEqual(3, challengesCount);
        }

        [Test]
        public async Task UpdateChallenge_ChallengeExists_CorrectUpdating()
        {
            // Arrange
            var challenges = new List<Challenge>
            {
                new Challenge { Id = 1, Description = "1", Status = "Draft", Area = area },
                new Challenge { Id = 2, Description = "2", Status = "Live", Area = area },
            };

            _db.Challenge.AddRange(challenges);
            _db.SaveChanges();

            // Act
            var challenge = new Challenge { Id = 1, Description = "3", Status = "Draft" };
            var result = await _challengeRepository.UpdateChallenge(challenge);
            var updatedChallenge = await _db.Challenge.FindAsync(1);

            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<Challenge>(result);
            Assert.AreEqual("3", updatedChallenge?.Description);
        }

        [Test]
        public void UpdateChallenge_ChallengeDoesNotExist_Exception()
        {
            // Arrange
            var challenges = new List<Challenge>
            {
                new Challenge { Id = 1, Description = "1", Status = "Draft", Area = area },
                new Challenge { Id = 2, Description = "2", Status = "Live", Area = area },
            };

            _db.Challenge.AddRange(challenges);
            _db.SaveChanges();

            // Act
            var challenge = new Challenge { Id = 3, Description = "3", Status = "Draft" };
            var ex = Assert.ThrowsAsync<AppException>(() => _challengeRepository.UpdateChallenge(challenge));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityNotFound, ex?.Code);
        }
    }
}
