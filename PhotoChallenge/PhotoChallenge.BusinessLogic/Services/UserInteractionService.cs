using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.DTO.UserInteraction;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;
using System.Security.Claims;

namespace PhotoChallenge.BusinessLogic.Services
{
    public class UserInteractionService : IUserInteractionService
    {
        private readonly IUserInteractionRepository _userInteractionRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IFileService _fileService;
        private IUserService _userService;
        private readonly IMapper _mapper;

        public UserInteractionService(
            IUserInteractionRepository userInteractionRepository,
            IChallengeRepository challengeRepository,
            IFileService fileService,
            IUserService userService,
            IMapper mapper)
        {
            _userInteractionRepository = userInteractionRepository;
            _challengeRepository = challengeRepository;
            _fileService = fileService;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<List<UserInteractionDto>> GetUserInteractions()
        {
            var interactions = await _userInteractionRepository.GetUserInteractions();

            return interactions.Select(_ => _mapper.Map<UserInteractionDto>(_)).ToList();
        }

        public async Task<List<UserInteractionDto>> GetMyInteractions()
        {
            var userId = _userService.GetCurrentUserId();
            var interactions = await _userInteractionRepository.GetUserInteractions(userId);

            return interactions.Select(_ => _mapper.Map<UserInteractionDto>(_)).ToList();
        }

        public async Task<UserInteractionDto> AddUserInteraction(CreateUserInteractionDto data)
        {
            try
            {
                var newInteraction = _mapper.Map<UserInteraction>(data);
                newInteraction.Challenge = await _challengeRepository.FindChallenge(data.ChallengeId);

                if (newInteraction.Challenge.Status != ChallengeStatus.Live.ToString() || !(DateTime.Now.Date >= newInteraction.Challenge.DateStart.Date && DateTime.Now.Date <= newInteraction.Challenge.DateEnd.Date))
                    throw new AppException(ErrorCode.EntityValidationError, "Challenge is not actual");

                var userId = _userService.GetCurrentUserId();

                var existingInteraction = await _userInteractionRepository.FindUserInteraction(newInteraction.Challenge.Id, userId);
                if (existingInteraction is not null && existingInteraction.Status != UserInteractionStatus.Approved.ToString())
                {
                    existingInteraction.Status = UserInteractionStatus.Pending.ToString();
                    existingInteraction.Photo = data.FileName;
                    existingInteraction.Created = DateTime.Now;
                    existingInteraction.Updated = DateTime.Now;

                    var updatedInteraction = await _userInteractionRepository.UpdateUserInteraction(existingInteraction);

                    return _mapper.Map<UserInteractionDto>(updatedInteraction);
                }

                newInteraction.User = await _userService.GetUser(userId);
                newInteraction.Status = UserInteractionStatus.Pending.ToString();
                newInteraction.Created = DateTime.Now;
                newInteraction.Updated = DateTime.Now;

                var createdInteraction = await _userInteractionRepository.AddUserInteraction(newInteraction);

                return _mapper.Map<UserInteractionDto>(createdInteraction);
            }
            catch (Exception)
            {
                _fileService.DeleteFile(data.FileName);

                throw;
            }
        }

        public async Task<UserInteractionDto> UpdateUserInteraction(UpdateUserInteractionDto data)
        {
            if (data.Status == UserInteractionStatus.Pending)
                throw new AppException(ErrorCode.EntityValidationError, "Wrong status");

            var interactionToUpdate = await _userInteractionRepository.FindUserInteraction(data.Id);

            if (interactionToUpdate.Status != UserInteractionStatus.Pending.ToString() && interactionToUpdate.Challenge.DateEnd.Date < DateTime.Now.Date)
                throw new AppException(ErrorCode.EntityValidationError, "Interaction status cannot be changed to not 'pending' status if challend is expired");

            interactionToUpdate.Status = data.Status.ToString();

            var updatedInteraction = await _userInteractionRepository.UpdateUserInteraction(interactionToUpdate);

            return _mapper.Map<UserInteractionDto>(updatedInteraction);
        }

        public async Task DeleteUserInteraction(int id)
        {
            var userId = _userService.GetCurrentUserId();
            var existingInteraction = await _userInteractionRepository.FindUserInteraction(id);

            if (existingInteraction.User.Id != userId)
                throw new AppException(ErrorCode.EntityCannotBeDeleted, "Only owner can delete his own interaction");

            await _userInteractionRepository.DeleteUserInteraction(id);
        }
    }
}
