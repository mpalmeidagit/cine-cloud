using BuildingBlocks.Core.DomainObjects;
using CineCloud.Domain.Entities;
using CineCloud.Domain.Entities.Enums;
using FluentAssertions;
using Xunit;

namespace CineCloud.Domain.Tests.Entities;

public class DvdTests
{
    private static Dvd NewDvd(int copies = 5) =>
        new("Jaws", 0, DateTime.Now.AddYears(-40), copies, Guid.NewGuid());

    [Fact]
    public void Constructor_ShouldCreateAvailableDvd_WhenAllFieldsAreValid()
    {
        var directorId = Guid.NewGuid();

        var dvd = new Dvd("Jaws", 0, DateTime.Now.AddYears(-40), 5, directorId);

        dvd.Title.Should().Be("Jaws");
        dvd.Genre.Should().Be(EGenre.Action);
        dvd.Copies.Should().Be(5);
        dvd.DirectorId.Should().Be(directorId);
        dvd.Available.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenGenreIsInvalid()
    {
        var act = () => new Dvd("Jaws", 99, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("Invalid genre option!");
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenPublishedDateIsInTheFuture()
    {
        var act = () => new Dvd("Jaws", 0, DateTime.Now.AddDays(1), 5, Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("Invalid published date");
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenDirectorIdIsEmpty()
    {
        var act = () => new Dvd("Jaws", 0, DateTime.Now.AddYears(-40), 5, Guid.Empty);

        act.Should().Throw<DomainException>().WithMessage("Invalid director's Id");
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    public void Constructor_ShouldThrowDomainException_WhenTitleIsShorterThanMinLength(string title)
    {
        var act = () => new Dvd(title, 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenTitleIsLongerThanMaxLength()
    {
        var act = () => new Dvd(new string('a', Dvd.MAX_TITLE_LENGTH + 1), 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_ShouldThrowDomainException_WhenCopiesIsNegative()
    {
        var act = () => new Dvd("Jaws", 0, DateTime.Now.AddYears(-40), -1, Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("Number of copies must be greater than zero.");
    }

    [Fact]
    public void RentCopy_ShouldDecrementCopies_WhenCopiesAreAvailable()
    {
        var dvd = NewDvd(copies: 5);

        dvd.RentCopy();

        dvd.Copies.Should().Be(4);
    }

    [Fact]
    public void RentCopy_ShouldThrowDomainException_WhenNoCopiesAreLeft()
    {
        var dvd = NewDvd(copies: 0);

        var act = () => dvd.RentCopy();

        act.Should().Throw<DomainException>().WithMessage($"DVD {dvd.Title} is not available to rent");
    }

    [Fact]
    public void ReturnCopy_ShouldIncrementCopies()
    {
        var dvd = NewDvd(copies: 3);

        dvd.ReturnCopy();

        dvd.Copies.Should().Be(4);
    }

    [Fact]
    public void ReturnCopy_ShouldThrowDomainException_WhenDvdIsNotAvailable()
    {
        var dvd = NewDvd();
        dvd.DeleteDvd();

        var act = () => dvd.ReturnCopy();

        act.Should().Throw<DomainException>().WithMessage($"DVD {dvd.Title} is not available");
    }

    [Fact]
    public void UpdateTitle_ShouldChangeTitle_WhenValid()
    {
        var dvd = NewDvd();

        dvd.UpdateTitle("Jaws 2");

        dvd.Title.Should().Be("Jaws 2");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void UpdateTitle_ShouldThrowDomainException_WhenTitleIsEmpty(string title)
    {
        var dvd = NewDvd();

        var act = () => dvd.UpdateTitle(title);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdateTitle_ShouldThrowDomainException_WhenDvdIsNotAvailable()
    {
        var dvd = NewDvd();
        dvd.DeleteDvd();

        var act = () => dvd.UpdateTitle("Jaws 2");

        act.Should().Throw<DomainException>().WithMessage($"DVD {dvd.Title} is not available");
    }

    [Theory]
    [InlineData(0, EGenre.Action)]
    [InlineData(6, EGenre.Drama)]
    [InlineData(18, EGenre.Family)]
    public void UpdateGenre_ShouldMapIntToEnum_WhenValid(int genre, EGenre expected)
    {
        var dvd = NewDvd();

        dvd.UpdateGenre(genre);

        dvd.Genre.Should().Be(expected);
    }

    [Fact]
    public void UpdateGenre_ShouldThrowDomainException_WhenGenreIsInvalid()
    {
        var dvd = NewDvd();

        var act = () => dvd.UpdateGenre(19);

        act.Should().Throw<DomainException>().WithMessage("Invalid genre option!");
    }

    [Fact]
    public void UpdatePublishedDate_ShouldChangeDate_WhenValid()
    {
        var dvd = NewDvd();
        var newDate = DateTime.Now.AddYears(-10);

        dvd.UpdatePublishedDate(newDate);

        dvd.Published.Should().Be(newDate);
    }

    [Fact]
    public void UpdatePublishedDate_ShouldThrowDomainException_WhenDateIsInTheFuture()
    {
        var dvd = NewDvd();

        var act = () => dvd.UpdatePublishedDate(DateTime.Now.AddDays(1));

        act.Should().Throw<DomainException>().WithMessage("Invalid published date");
    }

    [Fact]
    public void UpdateDirector_ShouldChangeDirectorId_WhenValid()
    {
        var dvd = NewDvd();
        var newDirectorId = Guid.NewGuid();

        dvd.UpdateDirector(newDirectorId);

        dvd.DirectorId.Should().Be(newDirectorId);
    }

    [Fact]
    public void UpdateDirector_ShouldThrowDomainException_WhenDirectorIdIsEmpty()
    {
        var dvd = NewDvd();

        var act = () => dvd.UpdateDirector(Guid.Empty);

        act.Should().Throw<DomainException>().WithMessage("Invalid director's Id");
    }

    [Fact]
    public void UpdateCopies_ShouldChangeCopies_WhenValid()
    {
        var dvd = NewDvd();

        dvd.UpdateCopies(10);

        dvd.Copies.Should().Be(10);
    }

    [Fact]
    public void UpdateCopies_ShouldThrowDomainException_WhenCopiesIsNegative()
    {
        var dvd = NewDvd();

        var act = () => dvd.UpdateCopies(-1);

        act.Should().Throw<DomainException>().WithMessage("Number of copies must be greater than zero.");
    }

    [Fact]
    public void DeleteDvd_ShouldMarkAsUnavailableAndZeroCopies()
    {
        var dvd = NewDvd(copies: 5);

        dvd.DeleteDvd();

        dvd.Available.Should().BeFalse();
        dvd.Copies.Should().Be(0);
        dvd.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void DeleteDvd_ShouldThrowDomainException_WhenAlreadyDeleted()
    {
        var dvd = NewDvd();
        dvd.DeleteDvd();

        var act = () => dvd.DeleteDvd();

        act.Should().Throw<DomainException>().WithMessage("DVD is already deleted.");
    }
}
