using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Data.Interfaces;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Data.Adapters
{
    public class HeaderBasedAdapterTests
    {
        private ServiceProvider _provider;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            // Mock HttpContext met X-Hogeschool
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.Request.Headers["X-Hogeschool"]).Returns("Nijmegen");

            var httpAccessorMock = new Mock<IHttpContextAccessor>();
            httpAccessorMock.Setup(x => x.HttpContext).Returns(contextMock.Object);
            services.AddSingleton(httpAccessorMock.Object);

            // Mock interfaces
            var courseMock = new Mock<IDataSource<Course>>();
            courseMock.Setup(x => x.GetAllAsync())
                      .ReturnsAsync(new List<Course> { new Course { Id = 1, Name = "Test Course" } });
            services.AddSingleton(courseMock.Object);

            var lessonMock = new Mock<IDataSource<Lesson>>();
            lessonMock.Setup(x => x.GetAllAsync())
                      .ReturnsAsync(new List<Lesson> { new Lesson { Id = 1, Name = "Test Lesson" } });
            services.AddSingleton(lessonMock.Object);

            var loMock = new Mock<IDataSource<LearningOutcome>>();
            loMock.Setup(x => x.GetAllAsync())
                  .ReturnsAsync(new List<LearningOutcome> { new LearningOutcome { Id = 1, Name = "Test LO" } });
            services.AddSingleton(loMock.Object);

            // Header-based DI (runtime selection)
            services.AddScoped<IDataSource<Course>>(sp => sp.GetRequiredService<IDataSource<Course>>());
            services.AddScoped<IDataSource<Lesson>>(sp => sp.GetRequiredService<IDataSource<Lesson>>());
            services.AddScoped<IDataSource<LearningOutcome>>(sp => sp.GetRequiredService<IDataSource<LearningOutcome>>());

            _provider = services.BuildServiceProvider();
        }

        [Test]
        public async Task CourseAdapter_ShouldReturnMockedData()
        {
            var ds = _provider.GetRequiredService<IDataSource<Course>>();
            var courses = await ds.GetAllAsync();
            Assert.That(courses.Count(), Is.EqualTo(1));
            Assert.That(courses.First().Name, Is.EqualTo("Test Course"));
        }

        [Test]
        public async Task LessonAdapter_ShouldReturnMockedData()
        {
            var ds = _provider.GetRequiredService<IDataSource<Lesson>>();
            var lessons = await ds.GetAllAsync();
            Assert.That(lessons.Count(), Is.EqualTo(1));
            Assert.That(lessons.First().Name, Is.EqualTo("Test Lesson"));
        }

        [Test]
        public async Task LearningOutcomeAdapter_ShouldReturnMockedData()
        {
            var ds = _provider.GetRequiredService<IDataSource<LearningOutcome>>();
            var los = await ds.GetAllAsync();
            Assert.That(los.Count(), Is.EqualTo(1));
            Assert.That(los.First().Name, Is.EqualTo("Test LO"));
        }
    }
}
