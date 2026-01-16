using Data.Context;
using Data.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace Data.Tests.Repositories;

[TestFixture]
public class RubricRepositoryTests
{
    private Mock<DataContext> dataContextMock;
    private Mock<DbSet<Rubric>> rubricDbSetMock;
    private RubricRepository rubricRepository;

    [SetUp]
    public void Setup()
    {
        rubricDbSetMock = new Mock<DbSet<Rubric>>();
        dataContextMock = new Mock<DataContext>(new DbContextOptions<DataContext>());

        dataContextMock
            .Setup(c => c.Set<Rubric>())
            .Returns(rubricDbSetMock.Object);

        rubricRepository = new RubricRepository(dataContextMock.Object);
    }

    #region SaveAggregate Tests

    [Test]
    public async Task SaveAggregate_CallsSaveChanges()
    {
        // Arrange
        dataContextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await rubricRepository.SaveAggregate();

        // Assert
        dataContextMock.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    #endregion
}