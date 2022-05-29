using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PhotoChallenge.BusinessLogic.Services;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.DTO.UserInteraction;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoChallenge.Test.BusinessLogic.Services
{
    internal class UserInteractionServiceTests
    {

        private Mock<IUserInteractionRepository> _userInteractionRepository;
        private Mock<IChallengeRepository> _challengeRepository;
        private Mock<IFileService> _fileService;
        private Mock<IUserService> _userService;
        private Mock<IMapper> _mapper;

        private IUserInteractionService _userInteractionService;

        [SetUp]
        public void Setup()
        {
            _userInteractionRepository = new Mock<IUserInteractionRepository>();
            _challengeRepository = new Mock<IChallengeRepository>();
            _fileService = new Mock<IFileService>();
            _userService = new Mock<IUserService>();
            _mapper = new Mock<IMapper>();

            _userInteractionService = new UserInteractionService(
                _userInteractionRepository.Object,
                _challengeRepository.Object,
                _fileService.Object,
                _userService.Object,
                _mapper.Object);
        }

        [Test]
        public async Task GetUserInteractions_ReturnsInteractions()
        {
            // Arrange
            var interactions = new List<UserInteraction> { new UserInteraction() { Id = 1 } };
            _userInteractionRepository.Setup(_ => _.GetUserInteractions())
                .Returns(Task.FromResult(interactions));
            _mapper.Setup(_ => _.Map<UserInteractionDto>(It.IsAny<UserInteraction>()))
                .Returns(new UserInteractionDto());

            // Act
            var result = await _userInteractionService.GetUserInteractions();

            // Assert
            Assert.IsInstanceOf<List<UserInteractionDto>>(result);
            Assert.AreEqual(interactions.Count, result.Count);
        }

        [Test]
        public async Task GetMyInteractions_ReturnsInteractions()
        {
            // Arrange
            var interactions = new List<UserInteraction> { new UserInteraction() { Id = 1 } };
            var userId = Guid.NewGuid().ToString();
            string passedUserId = string.Empty;
            _userService.Setup(_ => _.GetCurrentUserId())
                .Returns(userId);
            _userInteractionRepository.Setup(_ => _.GetUserInteractions(It.IsAny<string>()))
                .Callback<string>(_ => passedUserId = _)
                .Returns(Task.FromResult(interactions));
            _mapper.Setup(_ => _.Map<UserInteractionDto>(It.IsAny<UserInteraction>()))
                .Returns(new UserInteractionDto());

            // Act
            var result = await _userInteractionService.GetMyInteractions();

            // Assert
            Assert.IsInstanceOf<List<UserInteractionDto>>(result);
            Assert.AreEqual(interactions.Count, result.Count);
            Assert.AreEqual(userId, passedUserId);
        }

        [Test]
        public void AddUserInteraction_ChallengeIsNotLive_Exceptions()
        {
            // Arrange
            var createInteraction = new CreateUserInteractionDto { ChallengeId = 1, FileName = "test" };
            var userInteraction = new UserInteraction { Photo = "test" };
            var challenge = new Challenge { Id = 1, Status = ChallengeStatus.Draft.ToString(), Area = new Area { Id = 1 } };
            var passedChallengeId = 0;

            _mapper.Setup(_ => _.Map<UserInteraction>(It.IsAny<CreateUserInteractionDto>()))
                .Returns(userInteraction);
            _challengeRepository.Setup(_ => _.FindChallenge(It.IsAny<int>()))
                .Callback<int>(_ => passedChallengeId = _)
                .Returns(Task.FromResult(challenge));

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _userInteractionService.AddUserInteraction(createInteraction));

            // Assert
            _fileService.Verify(_ => _.DeleteFile(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(createInteraction.ChallengeId, passedChallengeId);
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("Challenge is not actual", ex?.Message);
        }

        [Test]
        public void AddUserInteraction_ChallengeIsExpired_Exceptions()
        {
            // Arrange
            var createInteraction = new CreateUserInteractionDto { ChallengeId = 1, FileName = "test" };
            var userInteraction = new UserInteraction { Photo = "test" };
            var challenge = new Challenge { Id = 1, Status = ChallengeStatus.Live.ToString(), DateEnd = new DateTime(2010, 1, 1), Area = new Area { Id = 1 } };

            _mapper.Setup(_ => _.Map<UserInteraction>(It.IsAny<CreateUserInteractionDto>()))
                .Returns(userInteraction);
            _challengeRepository.Setup(_ => _.FindChallenge(createInteraction.ChallengeId))
                .Returns(Task.FromResult(challenge));

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _userInteractionService.AddUserInteraction(createInteraction));

            // Assert
            _fileService.Verify(_ => _.DeleteFile(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("Challenge is not actual", ex?.Message);
        }

        [Test]
        public async Task AddUserInteraction_InteractionDoesNotExist_CorrectAdding()
        {
            // Arrange
            var createInteraction = new CreateUserInteractionDto { ChallengeId = 1, FileName = "test" };
            var userInteraction = new UserInteraction { Photo = "test" };
            var challenge = new Challenge
            {
                Id = 1,
                Status = ChallengeStatus.Live.ToString(),
                DateStart = new DateTime(2010, 1, 1),
                DateEnd = new DateTime(2030, 1, 1),
                Area = new Area { Id = 1 }
            };
            var userId = Guid.NewGuid().ToString();
            var user = new ApplicationUser { Id = userId };
            var passedChallengeId = 0;
            var passedUserId = string.Empty;
            var passedUserIdToUserService = string.Empty;
            var passedNewUserInteraction = new UserInteraction();

            _userService.Setup(_ => _.GetCurrentUserId())
                .Returns(userId);
            _userService.Setup(_ => _.GetUser(It.IsAny<string>()))
                .Callback<string>(_ => passedUserIdToUserService = _)
                .Returns(Task.FromResult(user));
            _challengeRepository.Setup(_ => _.FindChallenge(createInteraction.ChallengeId))
                .Returns(Task.FromResult(challenge));
            _userInteractionRepository.Setup(_ => _.FindUserInteraction(It.IsAny<int>(), It.IsAny<string>()))
                .Callback<int, string>((c, u) => { passedChallengeId = c; passedUserId = u; })
                .Returns(Task.FromResult((UserInteraction)null));
            _userInteractionRepository.Setup(_ => _.AddUserInteraction(It.IsAny<UserInteraction>()))
                .Callback<UserInteraction>(_ => passedNewUserInteraction = _)
                .Returns(Task.FromResult(new UserInteraction()));
            _mapper.Setup(_ => _.Map<UserInteraction>(It.IsAny<CreateUserInteractionDto>()))
                .Returns(userInteraction);
            _mapper.Setup(_ => _.Map<UserInteractionDto>(It.IsAny<UserInteraction>()))
                .Returns(new UserInteractionDto());

            // Act
            var result = await _userInteractionService.AddUserInteraction(createInteraction);

            // Assert
            _userInteractionRepository.Verify(_ => _.AddUserInteraction(It.IsAny<UserInteraction>()), Times.Once());
            _userInteractionRepository.Verify(_ => _.UpdateUserInteraction(It.IsAny<UserInteraction>()), Times.Never());
            Assert.IsInstanceOf<UserInteractionDto>(result);
            Assert.AreEqual(userId, passedUserId);
            Assert.AreEqual(userId, passedUserIdToUserService);
            Assert.AreEqual(user.Id, passedNewUserInteraction.User.Id);
            Assert.AreEqual(UserInteractionStatus.Pending.ToString(), passedNewUserInteraction.Status);
        }

        [Test]
        public async Task AddUserInteraction_ExistingInteractionIsApproved_CorrectAdding()
        {
            // Arrange
            var createInteraction = new CreateUserInteractionDto { ChallengeId = 1, FileName = "test" };
            var userInteraction = new UserInteraction { Photo = "test" };
            var challenge = new Challenge
            {
                Id = 1,
                Status = ChallengeStatus.Live.ToString(),
                DateStart = new DateTime(2010, 1, 1),
                DateEnd = new DateTime(2030, 1, 1),
                Area = new Area { Id = 1 }
            };
            var userId = Guid.NewGuid().ToString();
            var user = new ApplicationUser { Id = userId };
            var passedChallengeId = 0;
            var passedUserId = string.Empty;
            var passedUserIdToUserService = string.Empty;
            var passedNewUserInteraction = new UserInteraction();
            var existingInteraction = new UserInteraction { Status = UserInteractionStatus.Approved.ToString() };

            _userService.Setup(_ => _.GetCurrentUserId())
                .Returns(userId);
            _userService.Setup(_ => _.GetUser(It.IsAny<string>()))
                .Callback<string>(_ => passedUserIdToUserService = _)
                .Returns(Task.FromResult(user));
            _challengeRepository.Setup(_ => _.FindChallenge(createInteraction.ChallengeId))
                .Returns(Task.FromResult(challenge));
            _userInteractionRepository.Setup(_ => _.FindUserInteraction(It.IsAny<int>(), It.IsAny<string>()))
                .Callback<int, string>((c, u) => { passedChallengeId = c; passedUserId = u; })
                .Returns(Task.FromResult(existingInteraction));
            _userInteractionRepository.Setup(_ => _.AddUserInteraction(It.IsAny<UserInteraction>()))
                .Callback<UserInteraction>(_ => passedNewUserInteraction = _)
                .Returns(Task.FromResult(new UserInteraction()));
            _mapper.Setup(_ => _.Map<UserInteraction>(It.IsAny<CreateUserInteractionDto>()))
                .Returns(userInteraction);
            _mapper.Setup(_ => _.Map<UserInteractionDto>(It.IsAny<UserInteraction>()))
                .Returns(new UserInteractionDto());

            // Act
            var result = await _userInteractionService.AddUserInteraction(createInteraction);

            // Assert
            _userInteractionRepository.Verify(_ => _.AddUserInteraction(It.IsAny<UserInteraction>()), Times.Once());
            _userInteractionRepository.Verify(_ => _.UpdateUserInteraction(It.IsAny<UserInteraction>()), Times.Never());
            Assert.IsInstanceOf<UserInteractionDto>(result);
            Assert.AreEqual(userId, passedUserId);
            Assert.AreEqual(userId, passedUserIdToUserService);
            Assert.AreEqual(user.Id, passedNewUserInteraction.User.Id);
            Assert.AreEqual(UserInteractionStatus.Pending.ToString(), passedNewUserInteraction.Status);
        }

        [Test]
        public async Task AddUserInteraction_InteractionExists_CorrectUpdating()
        {
            // Arrange
            var createInteraction = new CreateUserInteractionDto { ChallengeId = 1, FileName = "test" };
            var userInteraction = new UserInteraction { Photo = "test" };
            var challenge = new Challenge
            {
                Id = 1,
                Status = ChallengeStatus.Live.ToString(),
                DateStart = new DateTime(2010, 1, 1),
                DateEnd = new DateTime(2030, 1, 1),
                Area = new Area { Id = 1 }
            };
            var userId = Guid.NewGuid().ToString();
            var user = new ApplicationUser { Id = userId };
            var passedChallengeId = 0;
            var passedUserId = string.Empty;
            var passedExistingInteraction = new UserInteraction();
            var existingInteraction = new UserInteraction { Status = UserInteractionStatus.Pending.ToString() };

            _userService.Setup(_ => _.GetCurrentUserId())
                .Returns(userId);
            _challengeRepository.Setup(_ => _.FindChallenge(createInteraction.ChallengeId))
                .Returns(Task.FromResult(challenge));
            _userInteractionRepository.Setup(_ => _.FindUserInteraction(It.IsAny<int>(), It.IsAny<string>()))
                .Callback<int, string>((c, u) => { passedChallengeId = c; passedUserId = u; })
                .Returns(Task.FromResult(existingInteraction));
            _userInteractionRepository.Setup(_ => _.UpdateUserInteraction(It.IsAny<UserInteraction>()))
                .Callback<UserInteraction>(_ => passedExistingInteraction = _)
                .Returns(Task.FromResult(new UserInteraction()));
            _mapper.Setup(_ => _.Map<UserInteraction>(It.IsAny<CreateUserInteractionDto>()))
                .Returns(userInteraction);
            _mapper.Setup(_ => _.Map<UserInteractionDto>(It.IsAny<UserInteraction>()))
                .Returns(new UserInteractionDto());

            // Act
            var result = await _userInteractionService.AddUserInteraction(createInteraction);

            // Assert
            _userInteractionRepository.Verify(_ => _.AddUserInteraction(It.IsAny<UserInteraction>()), Times.Never());
            _userInteractionRepository.Verify(_ => _.UpdateUserInteraction(It.IsAny<UserInteraction>()), Times.Once());
            Assert.IsInstanceOf<UserInteractionDto>(result);
            Assert.AreEqual(userId, passedUserId);
            Assert.AreEqual(createInteraction.FileName, existingInteraction.Photo);
            Assert.AreEqual(UserInteractionStatus.Pending.ToString(), existingInteraction.Status);
        }

        [Test]
        public async Task UpdateUserInteraction_InteractionIsPending_CorrectUpdating()
        {
            // Arrange
            var updateInteraction = new UpdateUserInteractionDto { Id = 1, Status = UserInteractionStatus.Approved };
            var interactionToUpdate = new UserInteraction { Id = 1, Status = UserInteractionStatus.Pending.ToString() };
            var passedInteractionId = 0;
            var passedInteractionToUpdate = new UserInteraction();

            _userInteractionRepository.Setup(_ => _.FindUserInteraction(It.IsAny<int>()))
                .Callback<int>(_ => passedInteractionId = _)
                .Returns(Task.FromResult(interactionToUpdate));
            _userInteractionRepository.Setup(_ => _.UpdateUserInteraction(It.IsAny<UserInteraction>()))
                .Callback<UserInteraction>(_ => passedInteractionToUpdate = _)
                .Returns(Task.FromResult(new UserInteraction()));
            _mapper.Setup(_ => _.Map<UserInteractionDto>(It.IsAny<UserInteraction>()))
                .Returns(new UserInteractionDto());

            // Act
            var result = await _userInteractionService.UpdateUserInteraction(updateInteraction);

            // Assert
            _userInteractionRepository.Verify(_ => _.UpdateUserInteraction(It.IsAny<UserInteraction>()), Times.Once());
            Assert.IsInstanceOf<UserInteractionDto>(result);
            Assert.AreEqual(updateInteraction.Id, passedInteractionId);
            Assert.AreEqual(updateInteraction.Status.ToString(), passedInteractionToUpdate.Status);
        }

        [Test]
        public async Task UpdateUserInteraction_InteractionIsNotPendingAndChallengeNotExpired_CorrectUpdating()
        {
            // Arrange
            var updateInteraction = new UpdateUserInteractionDto { Id = 1, Status = UserInteractionStatus.Approved };
            var interactionToUpdate = new UserInteraction
            {
                Id = 1,
                Status = UserInteractionStatus.Approved.ToString(),
                Challenge = new Challenge
                {
                    Id = 1,
                    Status = ChallengeStatus.Live.ToString(),
                    DateStart = new DateTime(2010, 1, 1),
                    DateEnd = new DateTime(2030, 1, 1),
                    Area = new Area { Id = 1 }
                }
            };
            var passedInteractionId = 0;
            var passedInteractionToUpdate = new UserInteraction();

            _userInteractionRepository.Setup(_ => _.FindUserInteraction(It.IsAny<int>()))
                .Callback<int>(_ => passedInteractionId = _)
                .Returns(Task.FromResult(interactionToUpdate));
            _userInteractionRepository.Setup(_ => _.UpdateUserInteraction(It.IsAny<UserInteraction>()))
                .Callback<UserInteraction>(_ => passedInteractionToUpdate = _)
                .Returns(Task.FromResult(new UserInteraction()));
            _mapper.Setup(_ => _.Map<UserInteractionDto>(It.IsAny<UserInteraction>()))
                .Returns(new UserInteractionDto());

            // Act
            var result = await _userInteractionService.UpdateUserInteraction(updateInteraction);

            // Assert
            _userInteractionRepository.Verify(_ => _.UpdateUserInteraction(It.IsAny<UserInteraction>()), Times.Once());
            Assert.IsInstanceOf<UserInteractionDto>(result);
            Assert.AreEqual(updateInteraction.Id, passedInteractionId);
            Assert.AreEqual(updateInteraction.Status.ToString(), passedInteractionToUpdate.Status);
        }

        [Test]
        public void UpdateUserInteraction_NewStatusIsPending_Exception()
        {
            // Arrange
            var updateInteraction = new UpdateUserInteractionDto { Id = 1, Status = UserInteractionStatus.Pending };

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _userInteractionService.UpdateUserInteraction(updateInteraction));

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("Wrong status", ex?.Message);
        }

        [Test]
        public void UpdateUserInteraction_InteractionIsPendingAndChallengeIsExpired_Exception()
        {
            // Arrange
            var updateInteraction = new UpdateUserInteractionDto { Id = 1, Status = UserInteractionStatus.Approved };
            var interactionToUpdate = new UserInteraction
            {
                Id = 1,
                Status = UserInteractionStatus.Approved.ToString(),
                Challenge = new Challenge
                {
                    Id = 1,
                    Status = ChallengeStatus.Live.ToString(),
                    DateStart = new DateTime(2010, 1, 1),
                    DateEnd = new DateTime(2010, 1, 10),
                    Area = new Area { Id = 1 }
                }
            };

            _userInteractionRepository.Setup(_ => _.FindUserInteraction(It.IsAny<int>()))
                .Returns(Task.FromResult(interactionToUpdate));
            _mapper.Setup(_ => _.Map<UserInteractionDto>(It.IsAny<UserInteraction>()))
                .Returns(new UserInteractionDto());

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _userInteractionService.UpdateUserInteraction(updateInteraction));

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("Interaction status cannot be changed to not 'pending' status if challend is expired", ex?.Message);
        }

        [Test]
        public async Task DeleteDeleteUserInteraction_UserIsOwner_CorrectDeletion()
        {
            // Arrange 
            var userId = "testId";
            _userService.Setup(_ => _.GetCurrentUserId())
                .Returns(userId);
            _userInteractionRepository.Setup(_ => _.FindUserInteraction(It.IsAny<int>()))
                .Returns(Task.FromResult(new UserInteraction
                {
                    User = new ApplicationUser
                    {
                        Id = userId
                    }
                }));

            // Act
            await _userInteractionService.DeleteUserInteraction(1);

            // Assert
            _userInteractionRepository.Verify(_ => _.DeleteUserInteraction(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void DeleteDeleteUserInteraction_UserIsNotOwner_CorrectDeletion()
        {
            // Arrange 
            _userService.Setup(_ => _.GetCurrentUserId())
                .Returns("testId");
            _userInteractionRepository.Setup(_ => _.FindUserInteraction(It.IsAny<int>()))
                .Returns(Task.FromResult(new UserInteraction
                {
                    User = new ApplicationUser
                    {
                        Id = "anotherTestId"
                    }
                }));

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _userInteractionService.DeleteUserInteraction(1));

            // Assert
            _userInteractionRepository.Verify(_ => _.DeleteUserInteraction(It.IsAny<int>()), Times.Never());
            Assert.AreEqual(ErrorCode.EntityCannotBeDeleted, ex?.Code);
            Assert.AreEqual("Only owner can delete his own interaction", ex?.Message);
        }
    }
}
