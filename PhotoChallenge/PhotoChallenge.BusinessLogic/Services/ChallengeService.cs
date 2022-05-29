using AutoMapper;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.DTO.Challenge;
using PhotoChallenge.Domain.Enums;
using PhotoChallenge.Domain.Exceptions;
using PhotoChallenge.Domain.Models;

namespace PhotoChallenge.BusinessLogic.Services
{
    public class ChallengeService : IChallengeService
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IMapper _mapper;

        public ChallengeService(IChallengeRepository challengeRepository, IAreaRepository areaRepository, IMapper mapper)
        {
            _challengeRepository = challengeRepository;
            _areaRepository = areaRepository;
            _mapper = mapper;
        }

        public async Task<List<ChallengeDto>> GetChallenges()
        {
            var challenges = await _challengeRepository.GetChallenges();

            return challenges.Select(_ => _mapper.Map<ChallengeDto>(_)).ToList();
        }

        public async Task<List<ChallengeDto>> GetLiveChallenges()
        {
            var challenges = await _challengeRepository.GetLiveChallenges();

            return challenges.Select(_ => _mapper.Map<ChallengeDto>(_)).ToList();
        }

        public async Task<ChallengeDto> GetChallenge(int id)
        {
            var challenge = await _challengeRepository.FindChallenge(id);

            return _mapper.Map<ChallengeDto>(challenge);
        }

        public async Task<ChallengeDto> AddChallenge(ChallengeDto data)
        {
            await ValidateChallenge(data, true);

            var newChallenge = _mapper.Map<Challenge>(data);
            newChallenge.Area = await _areaRepository.FindArea(data.AreaId);

            var createdChallenge = await _challengeRepository.AddChallenge(newChallenge);

            return _mapper.Map<ChallengeDto>(createdChallenge);
        }

        public async Task<ChallengeDto> UpdateChallenge(ChallengeDto data)
        {
            await ValidateChallenge(data, false);

            var challengeToUpdate = _mapper.Map<Challenge>(data);

            var updatedChallenge = await _challengeRepository.UpdateChallenge(challengeToUpdate);

            return _mapper.Map<ChallengeDto>(updatedChallenge);
        }

        private async Task ValidateChallenge(ChallengeDto data, bool isNew)
        {
            if (isNew)
            {
                ValidateChallengeDates(data);

                return;
            }

            var existingChallenge = await _challengeRepository.FindChallenge(data.Id);
            if (!isNew && existingChallenge is null)
                throw new AppException(ErrorCode.EntityValidationError, "The challenge is not found");

            if (existingChallenge.Status == ChallengeStatus.Draft.ToString())
                ValidateChallengeDates(data);

            if (existingChallenge?.Status == ChallengeStatus.Live.ToString() && data.Status == ChallengeStatus.Removed && DateTime.Now.Date > existingChallenge.DateEnd.Date)
                throw new AppException(ErrorCode.EntityValidationError, "'Live' challenge with end date less than current date cannot be 'removed'");

            if (existingChallenge?.Status == ChallengeStatus.Live.ToString() && data.Status == ChallengeStatus.Draft && DateTime.Now.Date > existingChallenge.DateEnd.Date)
                throw new AppException(ErrorCode.EntityValidationError, "'Live' challenge with end date less than current date cannot be 'draft'");

            if (existingChallenge?.Status == ChallengeStatus.Removed.ToString() && DateTime.Now.Date > existingChallenge.DateEnd.Date)
                throw new AppException(ErrorCode.EntityValidationError, "'Removed' challenge with end date less than current date cannot be changed");

            if (existingChallenge?.Status == ChallengeStatus.Live.ToString() && data.Status != ChallengeStatus.Draft && (existingChallenge.DateStart != data.DateStart || existingChallenge.DateEnd != data.DateEnd))
                throw new AppException(ErrorCode.EntityValidationError, "Challenge start and end date cannot be changed in 'live' status");

            ValidateChallengeDates(data);

            if (data.Status != ChallengeStatus.Draft && existingChallenge.Status != ChallengeStatus.Draft.ToString())
            {
                data.Award = (ChallengeAward)existingChallenge.Award;
                data.Description = existingChallenge.Description;
            }
        }

        private static void ValidateChallengeDates(ChallengeDto data)
        {
            if (data.DateStart.Date < DateTime.Now.Date)
                throw new AppException(ErrorCode.EntityValidationError, "Challenge start date cannot be less than current date");

            if (data.DateEnd.Date < data.DateStart.Date)
                throw new AppException(ErrorCode.EntityValidationError, "Challenge end date cannot be less than start date");
        }
    }
}
