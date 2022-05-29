using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PhotoChallenge.DataAccess.Context;
using PhotoChallenge.DataAccess.Repositories;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoChallenge.Test.DataAccess.Repositories
{
    internal class UserInteractionRepositoryTests
    {
        private DataContext _db;
        private UserInteractionRepository _userInteractionRepository;

        [SetUp]
        public void Setup()
        {
            _db = new InMemoryDbContextFactory().GetDataContext();
            _userInteractionRepository = new UserInteractionRepository(_db);
        }

        [Test]
        public async Task GetUserInteractions_InteractionInDb_ReturnInteractions()
        {
            // Arrange
            var interactions = GetTestUserInteractions(Guid.NewGuid().ToString());

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var result = await _userInteractionRepository.GetUserInteractions();
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<List<UserInteraction>>(result);
            Assert.IsTrue(result.First().Challenge is not null);
            Assert.IsTrue(result.First().Challenge.Area is not null);
            Assert.IsTrue(result.First().User is not null);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetUserInteractions_InteractionInDb_ReturnInteractionsByUser()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var interactions = GetTestUserInteractions(userId);
            interactions.Last().User = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString()
            };

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var result = await _userInteractionRepository.GetUserInteractions(userId);
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<List<UserInteraction>>(result);
            Assert.IsTrue(result.First().Challenge is not null);
            Assert.IsTrue(result.First().Challenge.Area is not null);
            Assert.IsTrue(result.First().User is not null);
            Assert.AreEqual(userId, result.First().User.Id);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task FindUserInteraction_InteractionExists_ReturnInteraction()
        {
            // Arrange
            var interactions = GetTestUserInteractions(Guid.NewGuid().ToString());

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var result = await _userInteractionRepository.FindUserInteraction(2);
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<UserInteraction>(result);
            Assert.IsTrue(result.Challenge is not null);
            Assert.IsTrue(result.Challenge?.Area is not null);
            Assert.IsTrue(result.User is not null);
            Assert.AreEqual(2, result.Id);
        }

        [Test]
        public void FindUserInteraction_InteractionDoesNotExist_Exception()
        {
            // Arrange
            var interactions = GetTestUserInteractions(Guid.NewGuid().ToString());

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _userInteractionRepository.FindUserInteraction(3));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityNotFound, ex?.Code);
        }

        [Test]
        public async Task FindUserInteraction_InteractionExists_ReturnInteractionByUserAndChallenge()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var interactions = GetTestUserInteractions(userId);

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var result = await _userInteractionRepository.FindUserInteraction(2, userId);
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<UserInteraction>(result);
            Assert.IsTrue(result.Challenge is not null);
            Assert.IsTrue(result.Challenge?.Area is not null);
            Assert.IsTrue(result.User is not null);
            Assert.AreEqual(2, result.Id);
        }

        [Test]
        public async Task AddUserInteraction_AddNewInteraction_CorrectAdding()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString()
            };
            var challenge = new Challenge
            {
                Id = 1,
                Status = ChallengeStatus.Draft.ToString(),
                Area = new Area
                {
                    Id = 1
                }
            };
            var interactions = new List<UserInteraction>
            {
                new UserInteraction {
                    Id = 1,
                    Status = UserInteractionStatus.Pending.ToString(),
                    Challenge = challenge,
                    User = user,
                },
            };

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var interaction = new UserInteraction
            {
                Status = UserInteractionStatus.Pending.ToString(),
                Challenge = new Challenge
                {
                    Id = 2,
                    Status = ChallengeStatus.Draft.ToString(),
                    Area = new Area
                    {
                        Id = 2
                    }
                },
                User = user
            };
            var result = await _userInteractionRepository.AddUserInteraction(interaction);
            var interactionsCount = _db.UserInteraction.Count();

            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<UserInteraction>(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual(2, interactionsCount);
        }

        [Test]
        public void AddUserInteraction_InteractionAlreadyExists_Exception()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString()
            };
            var challenge = new Challenge
            {
                Id = 1,
                Status = ChallengeStatus.Draft.ToString(),
                Area = new Area
                {
                    Id = 1
                }
            };
            var interactions = new List<UserInteraction>
            {
                new UserInteraction {
                    Id = 1,
                    Status = UserInteractionStatus.Pending.ToString(),
                    Challenge = challenge,
                    User = user,
                },
            };

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var interaction = new UserInteraction
            {
                Status = UserInteractionStatus.Pending.ToString(),
                Challenge = challenge,
                User = user
            };
            var ex = Assert.ThrowsAsync<AppException>(() => _userInteractionRepository.AddUserInteraction(interaction));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("You have already taken part in this challenge", ex?.Message);
        }

        [Test]
        public async Task UpdateUserInteraction_InteractionExists_CorrectUpdating()
        {
            // Arrange
            var interactions = GetTestUserInteractions(Guid.NewGuid().ToString());

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var interaction = await _db.UserInteraction.FindAsync(1);
            interaction.Status = UserInteractionStatus.Approved.ToString();
            //_db.Entry(interactions).State = EntityState.Detached;
            var result = await _userInteractionRepository.UpdateUserInteraction(interaction);
            var updatedInteraction = await _db.UserInteraction.FindAsync(1);

            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<UserInteraction>(result);
            Assert.AreEqual(UserInteractionStatus.Approved.ToString(), updatedInteraction?.Status);
        }

        [Test]
        public async Task DeleteUserInteraction_CorrectDeletion()
        {
            // Arrange
            var interactions = GetTestUserInteractions(Guid.NewGuid().ToString());

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            await _userInteractionRepository.DeleteUserInteraction(1);
            var updatedInteractionsCount = _db.UserInteraction.Count();

            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(1, updatedInteractionsCount);
        }

        [Test]
        public void DeleteUserInteraction_InteractionDoesNotExist_Exception()
        {
            // Arrange
            var interactions = GetTestUserInteractions(Guid.NewGuid().ToString());

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _userInteractionRepository.DeleteUserInteraction(3));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityNotFound, ex?.Code);
            Assert.AreEqual("User interaction is not found", ex?.Message);
        }

        [Test]
        public void DeleteUserInteraction_InteractionNotPending_Exception()
        {
            // Arrange
            var interactions = GetTestUserInteractions(Guid.NewGuid().ToString());
            interactions.First().Status = UserInteractionStatus.Approved.ToString();

            _db.UserInteraction.AddRange(interactions);
            _db.SaveChanges();

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _userInteractionRepository.DeleteUserInteraction(1));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityCannotBeDeleted, ex?.Code);
            Assert.AreEqual("User interaction cannot be deleted because of status", ex?.Message);
        }

        private static List<UserInteraction> GetTestUserInteractions(string userId)
        {
            var user = new ApplicationUser
            {
                Id = userId
            };

            return new List<UserInteraction>
            {
                new UserInteraction {
                    Id = 1,
                    Status = UserInteractionStatus.Pending.ToString(),
                    Challenge = new Challenge
                    {
                        Id = 1,
                        Status = ChallengeStatus.Draft.ToString(),
                        Area = new Area
                        {
                            Id = 1
                        }
                    },
                    User = user,
                },
                new UserInteraction {
                    Id = 2,
                    Status = UserInteractionStatus.Pending.ToString(),
                    Challenge = new Challenge
                    {
                        Id = 2,
                        Status = ChallengeStatus.Draft.ToString(),
                        Area = new Area
                        {
                            Id = 2
                        }
                    },
                    User = user,
                },
            };
        }
    }
}
