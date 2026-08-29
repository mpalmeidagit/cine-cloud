using BuildingBlocks.Core.DomainObjects;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CineCloud.Domain.Tests.Entities;

public class DirectorTests
{
    [Fact]
    public void Constructor_ShouldCreateDirector_WhenNameAndSurnameAreValid()
    {
        var director = new Director("Steven", "Spielberg");

        director.Name.Should().Be("Steven");
        director.Surname.Should().Be("Spielberg");
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenNameIsInvalid()
    {
        var act = () => new Director("st", "Spielberg");

        act.Should().Throw<DomainException>().WithMessage("Invalid name for director");
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenSurnameIsInvalid()
    {
        var act = () => new Director("Steven", "sp");

        act.Should().Throw<DomainException>().WithMessage("Invalid surname for director");
    }

    [Fact]
    public void FullName_ShouldCombineNameAndSurname()
    {
        var director = new Director("Steven", "Spielberg");

        director.FullName().Should().Be("Steven Spielberg");
    }

    [Fact]
    public void UpdateName_ShouldChangeNameAndUpdatedAt_WhenValid()
    {
        var director = new Director("Steven", "Spielberg");
        var before = DateTime.Now;

        director.UpdateName("George");

        director.Name.Should().Be("George");
        director.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void UpdateName_ShouldThrowAndKeepPreviousName_WhenInvalid()
    {
        var director = new Director("Steven", "Spielberg");

        var act = () => director.UpdateName("st");

        act.Should().Throw<DomainException>();
        director.Name.Should().Be("Steven");
    }

    [Fact]
    public void UpdateSurname_ShouldChangeSurnameAndUpdatedAt_WhenValid()
    {
        var director = new Director("Steven", "Spielberg");
        var before = DateTime.Now;

        director.UpdateSurname("Lucas");

        director.Surname.Should().Be("Lucas");
        director.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void UpdateSurname_ShouldThrowAndKeepPreviousSurname_WhenInvalid()
    {
        var director = new Director("Steven", "Spielberg");

        var act = () => director.UpdateSurname("sp");

        act.Should().Throw<DomainException>();
        director.Surname.Should().Be("Spielberg");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("steven")]
    [InlineData("STEVEN")]
    [InlineData("Steven1")]
    [InlineData("Steven Spielberg Steven Spielberg")]
    public void ValidateName_ShouldReturnFalse_ForInvalidValues(string? value)
    {
        var director = new Director("Steven", "Spielberg");

        director.ValidateName(value!).Should().BeFalse();
    }

    [Theory]
    [InlineData("Steven")]
    [InlineData("Léa")]
    [InlineData("Jane")]
    public void ValidateName_ShouldReturnTrue_ForValidValues(string value)
    {
        var director = new Director("Steven", "Spielberg");

        director.ValidateName(value).Should().BeTrue();
    }
}
