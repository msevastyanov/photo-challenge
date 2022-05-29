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
    internal class AreaRepositoryTests
    {
        private DataContext _db;
        private AreaRepository _areaRepository;

        [SetUp]
        public void Setup()
        {
            _db = new InMemoryDbContextFactory().GetDataContext();
            _areaRepository = new AreaRepository(_db);
        }

        [Test]
        public async Task GetAreas_TwoAreasInDb_ReturnAreas()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1 },
                new Area { Id = 2 },
            };

            _db.Area.AddRange(areas);
            _db.SaveChanges();

            // Act
            var result = await _areaRepository.GetAreas();
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<List<Area>>(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetActualAreas_OneOfThreeIsActual_ReturnActualAreas()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Challenges = new List<Challenge> { new Challenge { Id = 1, Status = "Draft" }, } },
                new Area { Id = 2, Challenges = new List<Challenge> { new Challenge { Id = 2, Status = "Removed" } } },
                new Area { Id = 3, Challenges = new List<Challenge> { new Challenge { Id = 3, Status = "Live" } } },
                new Area { Id = 4, Challenges = new List<Challenge>() },
            };

            _db.Area.AddRange(areas);
            _db.SaveChanges();

            // Act
            var result = await _areaRepository.GetActualAreas();
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<List<Area>>(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task FindArea_AreaExists_ReturnArea()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1 },
                new Area { Id = 2 },
            };

            _db.Area.AddRange(areas);
            _db.SaveChanges();

            // Act
            var result = await _areaRepository.FindArea(1);
            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<Area>(result);
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public void FindArea_AreaDoesNotExist_Exception()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1 },
                new Area { Id = 2 },
            };

            _db.Area.AddRange(areas);
            _db.SaveChanges();

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _areaRepository.FindArea(3));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityNotFound, ex?.Code);
        }

        [Test]
        public async Task AddArea_AddAreaWithNewName_CorrectAdding()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Name = "test1" },
                new Area { Id = 2, Name = "test2" },
            };

            _db.Area.AddRange(areas);
            _db.SaveChanges();

            // Act
            var area = new Area
            {
                Name = "test3"
            };
            var result = await _areaRepository.AddArea(area);
            var areasCount = _db.Area.Count();

            _db.Database.EnsureDeleted();

            // Assert
            Assert.IsInstanceOf<Area>(result);
            Assert.AreEqual(3, result.Id);
            Assert.AreEqual(3, areasCount);
        }

        [Test]
        public void AddArea_AddAreaWithTheSameName_Exception()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Name = "test1" },
                new Area { Id = 2, Name = "test2" },
            };

            _db.Area.AddRange(areas);
            _db.SaveChanges();

            // Act
            var area = new Area
            {
                Name = "test2"
            };
            var ex = Assert.ThrowsAsync<AppException>(() => _areaRepository.AddArea(area));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityValidationError, ex?.Code);
            Assert.AreEqual("Area with the same name already exists", ex?.Message);
        }

        [Test]
        public async Task DeleteArea_CorrectDeletion()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Name = "test1" },
                new Area { Id = 2, Name = "test2" },
            };

            _db.Area.AddRange(areas);
            _db.SaveChanges();

            // Act
            await _areaRepository.DeleteArea(1);
            var areasCount = _db.Area.Count();

            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(1, areasCount);
        }

        [Test]
        public void DeleteArea_AreaDoesNotExist_Exception()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Name = "test1" },
                new Area { Id = 2, Name = "test2" },
            };

            _db.Area.AddRange(areas);
            _db.SaveChanges();

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _areaRepository.DeleteArea(3));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityNotFound, ex?.Code);
            Assert.AreEqual("Area is not found", ex?.Message);
        }

        [Test]
        public void DeleteArea_AreaCannotBeDeleted_Exception()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Name = "test1", Challenges= new List<Challenge> { new Challenge { Id = 1, Status = "Draft" } } },
                new Area { Id = 2, Name = "test2", Challenges= new List<Challenge>() },
            };

            _db.Area.AddRange(areas);
            _db.SaveChanges();

            // Act
            var ex = Assert.ThrowsAsync<AppException>(() => _areaRepository.DeleteArea(1));
            _db.Database.EnsureDeleted();

            // Assert
            Assert.AreEqual(ErrorCode.EntityCannotBeDeleted, ex?.Code);
            Assert.AreEqual("Area cannot be deleted because of active challenges", ex?.Message);
        }
    }
}
