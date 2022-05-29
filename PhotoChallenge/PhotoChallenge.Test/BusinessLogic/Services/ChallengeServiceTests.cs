using AutoMapper;
using Moq;
using NUnit.Framework;
using PhotoChallenge.BusinessLogic.Services;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.DTO.Challenge;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoChallenge.Test.BusinessLogic.Services
{
    internal class ChallengeServiceTests
    {
        private Mock<IChallengeRepository> _challengeRepository;
        private Mock<IAreaRepository> _areaRepository;
        private Mock<IMapper> _mapper;

        private IChallengeService _challengeService;

        [SetUp]
        public void Setup()
        {
            _challengeRepository = new Mock<IChallengeRepository>();
            _areaRepository = new Mock<IAreaRepository>();
            _mapper = new Mock<IMapper>();

            _challengeService = new ChallengeService(_challengeRepository.Object, _areaRepository.Object, _mapper.Object);
        }

        [Test]
        public async Task GetChallenges_ReturnsChallenges()
        {
            // Arrange
            var challenges = new List<Challenge> { new Challenge { Status = "Draft" } };
            _challengeRepository.Setup(_ => _.GetChallenges())
                .Returns(Task.FromResult(challenges));
            _mapper.Setup(_ => _.Map<ChallengeDto>(It.IsAny<Challenge>()))
                .Returns(new ChallengeDto());

            // Act
            var result = await _challengeService.GetChallenges();

            // Assert
            Assert.IsInstanceOf<List<ChallengeDto>>(result);
            Assert.AreEqual(challenges.Count, result.Count);
        }

        [Test]
        public async Task GetLiveChallenges_ReturnsChallenges()
        {
            // Arrange
            var challenges = new List<Challenge> { new Challenge { Status = "Live" } };
            _challengeRepository.Setup(_ => _.GetLiveChallenges())
                .Returns(Task.FromResult(challenges));
            _mapper.Setup(_ => _.Map<ChallengeDto>(It.IsAny<Challenge>()))
                .Returns(new ChallengeDto());

            // Act
            var result = await _challengeService.GetLiveChallenges();

            // Assert
            Assert.IsInstanceOf<List<ChallengeDto>>(result);
            Assert.AreEqual(challenges.Count, result.Count);
        }

        [Test]
        public async Task GetChallenge_ReturnsChallenge()
        {
            // Arrange
            var challenge = new Challenge { Status = "Draft" };
            _challengeRepository.Setup(_ => _.FindChallenge(1))
                .Returns(Task.FromResult(challenge));
            _mapper.Setup(_ => _.Map<ChallengeDto>(It.IsAny<Challenge>()))
                .Returns(new ChallengeDto());

            // Act
            var result = await _challengeService.GetChallenge(1);

            // Assert
            Assert.IsInstanceOf<ChallengeDto>(result);
        }

        [Test]
        public async Task AddChallenge_CorrectAdding()
        {
            // Arrange
            _challengeRepository.Setup(_ => _.AddChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(new Challenge()));
            _areaRepository.Setup(_ => _.FindArea(It.IsAny<int>()))
                .Returns(Task.FromResult(new Area()));
            _mapper.Setup(_ => _.Map<Challenge>(It.IsAny<ChallengeDto>()))
                .Returns(new Challenge());
            _mapper.Setup(_ => _.Map<ChallengeDto>(It.IsAny<Challenge>()))
                .Returns(new ChallengeDto());

            // Act
            var challengeToAdd = new ChallengeDto { DateStart = new DateTime(2030, 1, 1), DateEnd = new DateTime(2030, 1, 10), Status = ChallengeStatus.Draft };
            var result = await _challengeService.AddChallenge(challengeToAdd);

            // Assert
            _challengeRepository.Verify(_ => _.AddChallenge(It.IsAny<Challenge>()), Times.Once());
            Assert.IsInstanceOf<ChallengeDto>(result);
        }

        [Test]
        public async Task UpdateChallenge_CorrectUpdating()
        {
            // Arrange
            var updatedChallenge = new Challenge();
            _challengeRepository.Setup(_ => _.UpdateChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(updatedChallenge));
            _challengeRepository.Setup(_ => _.FindChallenge(It.IsAny<int>()))
                .Returns(Task.FromResult(new Challenge { DateStart = new DateTime(2030, 1, 1), DateEnd = new DateTime(2030, 1, 10), Status = "Draft" }));
            _mapper.Setup(_ => _.Map<Challenge>(It.IsAny<ChallengeDto>()))
                .Returns(new Challenge());
            _mapper.Setup(_ => _.Map<ChallengeDto>(It.IsAny<Challenge>()))
                .Returns(new ChallengeDto());

            // Act
            var challengeToUpdate = new ChallengeDto { DateStart = new DateTime(2030, 1, 1), DateEnd = new DateTime(2030, 1, 10), Status = ChallengeStatus.Draft };
            var result = await _challengeService.UpdateChallenge(challengeToUpdate);

            // Assert
            _challengeRepository.Verify(_ => _.UpdateChallenge(It.IsAny<Challenge>()), Times.Once());
            Assert.IsInstanceOf<ChallengeDto>(result);
        }

        [Test]
        public void ValidateChallengeDates_WrongStartDate_Exception()
        {
            // Arrange
            var createdChallenge = new Challenge();
            _challengeRepository.Setup(_ => _.AddChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(createdChallenge));

            // Act
            var challenge = new ChallengeDto { DateStart = new DateTime(2010, 1, 1), DateEnd = new DateTime(2030, 1, 10), Status = ChallengeStatus.Draft };
            var ex = Assert.ThrowsAsync<AppException>(() => _challengeService.AddChallenge(challenge));

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("Challenge start date cannot be less than current date", ex?.Message);
        }

        [Test]
        public void ValidateChallengeDates_WrongEndDate_Exception()
        {
            // Arrange
            var createdChallenge = new Challenge();
            _challengeRepository.Setup(_ => _.AddChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(createdChallenge));

            // Act
            var challenge = new ChallengeDto { DateStart = new DateTime(2030, 1, 10), DateEnd = new DateTime(2030, 1, 1), Status = ChallengeStatus.Draft };
            var ex = Assert.ThrowsAsync<AppException>(() => _challengeService.AddChallenge(challenge));

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("Challenge end date cannot be less than start date", ex?.Message);
        }

        [Test]
        public void ValidateChallenge_IsNewChallenge_NoExistingChallengeChecking()
        {
            // Arrange
            var createdChallenge = new Challenge();
            _challengeRepository.Setup(_ => _.AddChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(createdChallenge));
            _mapper.Setup(_ => _.Map<Challenge>(It.IsAny<ChallengeDto>()))
                .Returns(new Challenge());

            // Act
            var challenge = new ChallengeDto { DateStart = new DateTime(2030, 1, 1), DateEnd = new DateTime(2030, 1, 10), Status = ChallengeStatus.Draft };
            Assert.DoesNotThrowAsync(() => _challengeService.AddChallenge(challenge));

            // Assert
            _challengeRepository.Verify(_ => _.FindChallenge(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void ValidateChallenge_IsNotNewChallengeAndChallengeIsNotFound_Exception()
        {
            // Arrange
            var updatedChallenge = new Challenge();
            _challengeRepository.Setup(_ => _.UpdateChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(updatedChallenge));
            _challengeRepository.Setup(_ => _.FindChallenge(It.IsAny<int>()))
                .Returns(Task.FromResult<Challenge>(null));

            // Act
            var challenge = new ChallengeDto { Id = 1, DateStart = new DateTime(2030, 1, 1), DateEnd = new DateTime(2030, 1, 10), Status = ChallengeStatus.Draft };
            var ex = Assert.ThrowsAsync<AppException>(() => _challengeService.UpdateChallenge(challenge));

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("The challenge is not found", ex?.Message);
        }

        [Test]
        public void ValidateChallenge_IsNotNewChallengeAndStatusIsDraft_ValidateDates()
        {
            // Arrange
            var updatedChallenge = new Challenge();
            _challengeRepository.Setup(_ => _.UpdateChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(updatedChallenge));
            _challengeRepository.Setup(_ => _.FindChallenge(It.IsAny<int>()))
                .Returns(Task.FromResult(new Challenge { Id = 1, Status = "Draft" }));

            // Act
            var challenge = new ChallengeDto { Id = 1, DateStart = new DateTime(2030, 1, 10), DateEnd = new DateTime(2030, 1, 1), Status = ChallengeStatus.Draft };
            var ex = Assert.ThrowsAsync<AppException>(() => _challengeService.UpdateChallenge(challenge));

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("Challenge end date cannot be less than start date", ex?.Message);
        }

        [TestCase("Live", ChallengeStatus.Removed, "'Live' challenge with end date less than current date cannot be 'removed'")]
        [TestCase("Live", ChallengeStatus.Draft, "'Live' challenge with end date less than current date cannot be 'draft'")]
        public void ValidateChallenge_ChangeLiveStatusWithWrongEndDate_Exception(string oldStatus, ChallengeStatus newStatus, string exceptionMessage)
        {
            // Arrange
            var updatedChallenge = new Challenge();
            _challengeRepository.Setup(_ => _.UpdateChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(updatedChallenge));
            _challengeRepository.Setup(_ => _.FindChallenge(It.IsAny<int>()))
                .Returns(Task.FromResult(new Challenge { Id = 1, Status = oldStatus }));

            // Act
            var challenge = new ChallengeDto { Id = 1, DateStart = new DateTime(2010, 1, 1), DateEnd = new DateTime(2010, 1, 10), Status = newStatus };
            var ex = Assert.ThrowsAsync<AppException>(() => _challengeService.UpdateChallenge(challenge));

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual(exceptionMessage, ex?.Message);
        }

        [Test]
        public void ValidateChallenge_ChangeRemoveStatusWithWrongEndDate_Exception()
        {
            // Arrange
            var updatedChallenge = new Challenge();
            _challengeRepository.Setup(_ => _.UpdateChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(updatedChallenge));
            _challengeRepository.Setup(_ => _.FindChallenge(It.IsAny<int>()))
                .Returns(Task.FromResult(new Challenge { Id = 1, Status = "Removed" }));

            // Act
            var challenge = new ChallengeDto { Id = 1, DateStart = new DateTime(2010, 1, 1), DateEnd = new DateTime(2010, 1, 10), Status = ChallengeStatus.Draft };
            var ex = Assert.ThrowsAsync<AppException>(() => _challengeService.UpdateChallenge(challenge));

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("'Removed' challenge with end date less than current date cannot be changed", ex?.Message);
        }

        [Test]
        public void ValidateChallenge_FromRemovedToDraft_ValidateDates()
        {
            // Arrange
            var updatedChallenge = new Challenge();
            _challengeRepository.Setup(_ => _.UpdateChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(updatedChallenge));
            _challengeRepository.Setup(_ => _.FindChallenge(It.IsAny<int>()))
                .Returns(Task.FromResult(new Challenge { Id = 1, Status = "Removed", DateStart = new DateTime(2030, 1, 1), DateEnd = new DateTime(2030, 1, 10) }));

            // Act
            var challenge = new ChallengeDto { Id = 1, DateStart = new DateTime(2030, 1, 10), DateEnd = new DateTime(2030, 1, 1), Status = ChallengeStatus.Draft };
            var ex = Assert.ThrowsAsync<AppException>(() => _challengeService.UpdateChallenge(challenge));

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("Challenge end date cannot be less than start date", ex?.Message);
        }

        [TestCase("Removed", ChallengeStatus.Removed, false)]
        [TestCase("Removed", ChallengeStatus.Live, false)]
        [TestCase("Removed", ChallengeStatus.Draft, true)]
        public async Task ValidateChallenge_ChallengeIsNotDraft_DescriptionAndAwardNotChanged(string oldStatus, ChallengeStatus newStatus, bool isChanged)
        {
            // Arrange
            var updatedChallenge = new Challenge();
            var challengeDtoToMap = new ChallengeDto();
            var oldChallebge = new Challenge
            {
                Id = 1,
                Status = oldStatus,
                Description = "desc",
                Award = 50,
                DateStart = new DateTime(2030, 1, 1),
                DateEnd = new DateTime(2030, 1, 10)
            };
            _challengeRepository.Setup(_ => _.UpdateChallenge(It.IsAny<Challenge>()))
                .Returns(Task.FromResult(updatedChallenge));
            _challengeRepository.Setup(_ => _.FindChallenge(It.IsAny<int>()))
                .Returns(Task.FromResult(oldChallebge));
            _mapper.Setup(_ => _.Map<ChallengeDto>(It.IsAny<Challenge>()))
                .Returns(new ChallengeDto());
            _mapper.Setup(_ => _.Map<Challenge>(It.IsAny<object>()))
                .Callback<object>(_ => challengeDtoToMap = (ChallengeDto) _);

            // Act
            var challenge = new ChallengeDto
            {
                Id = 1,
                Status = newStatus,
                Description = "newdesc",
                Award = ChallengeAward.Silver,
                DateStart = new DateTime(2030, 1, 1),
                DateEnd = new DateTime(2030, 1, 10)
            };
            await _challengeService.UpdateChallenge(challenge);

            // Assert
            Assert.IsTrue(challengeDtoToMap.Description != oldChallebge.Description || (int)challengeDtoToMap.Award != oldChallebge.Award == isChanged);
        }
    }
}
