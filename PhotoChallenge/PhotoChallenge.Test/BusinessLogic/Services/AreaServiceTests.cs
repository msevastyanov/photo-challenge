using AutoMapper;
using Moq;
using NUnit.Framework;
using PhotoChallenge.BusinessLogic.Services;
using PhotoChallenge.BusinessLogic.Services.Interfaces;
using PhotoChallenge.DataAccess.Repositories.Interfaces;
using PhotoChallenge.Domain.DTO.Area;
using PhotoChallenge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoChallenge.Test.BusinessLogic.Services
{
    internal class AreaServiceTests
    {
        private Mock<IAreaRepository> _areaRepository;
        private Mock<IMapper> _mapper;

        private IAreaService _areaService;

        [SetUp]
        public void Setup()
        {
            _areaRepository = new Mock<IAreaRepository>();
            _mapper = new Mock<IMapper>();

            _areaService = new AreaService(_areaRepository.Object, _mapper.Object);
        }

        [Test]
        public async Task GetAreas_ReturnsAreas()
        {
            // Arrange
            var areas = new List<Area> { new Area { Id = 1 } };
            _areaRepository.Setup(_ => _.GetAreas())
                .Returns(Task.FromResult(areas));
            _mapper.Setup(_ => _.Map<AreaDto>(It.IsAny<Area>()))
                .Returns(new AreaDto());

            // Act
            var result = await _areaService.GetAreas();

            // Assert
            Assert.IsInstanceOf<List<AreaDto>>(result);
            Assert.AreEqual(areas.Count, result.Count);
        }

        [Test]
        public async Task GetActualAreas_ReturnsAreas()
        {
            // Arrange
            var areas = new List<Area> { new Area { Id = 1 } };
            _areaRepository.Setup(_ => _.GetActualAreas())
                .Returns(Task.FromResult(areas));
            _mapper.Setup(_ => _.Map<AreaDto>(It.IsAny<Area>()))
                .Returns(new AreaDto());

            // Act
            var result = await _areaService.GetActualAreas();

            // Assert
            Assert.IsInstanceOf<List<AreaDto>>(result);
            Assert.AreEqual(areas.Count, result.Count);
        }

        [Test]
        public async Task AddArea_CorrectAdding()
        {
            // Arrange
            _areaRepository.Setup(_ => _.AddArea(It.IsAny<Area>()))
                .Returns(Task.FromResult(new Area()));
            _mapper.Setup(_ => _.Map<Area>(It.IsAny<AreaDto>()))
                .Returns(new Area());
            _mapper.Setup(_ => _.Map<AreaDto>(It.IsAny<Area>()))
                .Returns(new AreaDto());

            // Act
            var areaToAdd = new AreaDto { Id = 1 };
            var result = await _areaService.AddArea(areaToAdd);

            // Assert
            _areaRepository.Verify(_ => _.AddArea(It.IsAny<Area>()), Times.Once());
            Assert.IsInstanceOf<AreaDto>(result);
        }

        [Test]
        public async Task DeleteArea_CorrectDeletion()
        {
            // Act
            await _areaService.DeleteArea(1);

            // Assert
            _areaRepository.Verify(_ => _.DeleteArea(It.IsAny<int>()), Times.Once());
        }
    }
}
