using Domain.Models;
using Domain.Enums;

namespace Core.Tests.TestSupport.Builders;

/// <summary>
/// Builder pattern helper for creating test domain objects
/// </summary>
public class CourseBuilder
{
    private int _id = 1;
    private string _name = "Test Course";
    private string _description = "Test Description";
    private CourseStatus _status = CourseStatus.Concept;
    private List<LearningOutcome> _learningOutcomes = new();

    public CourseBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public CourseBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CourseBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public CourseBuilder WithStatus(CourseStatus status)
    {
        _status = status;
        return this;
    }

    public CourseBuilder WithLearningOutcomes(List<LearningOutcome> outcomes)
    {
        _learningOutcomes = outcomes;
        return this;
    }

    public Course Build()
    {
        return new Course
        {
            Id = _id,
            Name = _name,
            Description = _description,
            Status = _status,
            LearningOutcomes = _learningOutcomes
        };
    }
}

public class LearningOutcomeBuilder
{
    private int _id = 1;
    private int _courseId = 1;
    private string _name = "Learning Outcome 1";
    private List<Lesson> _lessons = new();

    public LearningOutcomeBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public LearningOutcomeBuilder WithCourseId(int courseId)
    {
        _courseId = courseId;
        return this;
    }

    public LearningOutcomeBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public LearningOutcomeBuilder WithLessons(List<Lesson> lessons)
    {
        _lessons = lessons;
        return this;
    }

    public LearningOutcome Build()
    {
        return new LearningOutcome
        {
            Id = _id,
            CourseId = _courseId,
            Name = _name,
            Lessons = _lessons
        };
    }
}

public class TeacherBuilder
{
    private int _id = 1;
    private string _firstName = "John";
    private string _middleName = "Michael";
    private string _lastName = "Doe";
    private string _email = "john.doe@example.com";
    private string _teacherCode = "T001";

    public TeacherBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public TeacherBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public TeacherBuilder WithMiddleName(string middleName)
    {
        _middleName = middleName;
        return this;
    }

    public TeacherBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public TeacherBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public TeacherBuilder WithTeacherCode(string teacherCode)
    {
        _teacherCode = teacherCode;
        return this;
    }

    public Teacher Build()
    {
        return new Teacher
        {
            Id = _id,
            FirstName = _firstName,
            MiddleName = _middleName,
            LastName = _lastName,
            Email = _email,
            TeacherCode = _teacherCode
        };
    }
}

public class MaterialBuilder
{
    private int _id = 1;
    private string _name = "Test Material";
    private string _content = "Test Content";

    public MaterialBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public MaterialBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public MaterialBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public Material Build()
    {
        return new Material
        {
            Id = _id,
            Name = _name,
            Content = _content
        };
    }
}

public class PlanningBuilder
{
    private int _id = 1;
    private int _courseId = 1;
    private List<Lesson> _lessons = new();

    public PlanningBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public PlanningBuilder WithCourseId(int courseId)
    {
        _courseId = courseId;
        return this;
    }

    public PlanningBuilder WithLessons(List<Lesson> lessons)
    {
        _lessons = lessons;
        return this;
    }

    public Planning Build()
    {
        return new Planning
        {
            Id = _id,
            CourseId = _courseId,
            Lessons = _lessons
        };
    }
}
