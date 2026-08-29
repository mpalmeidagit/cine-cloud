using BuildingBlocks.Core.DomainObjects;
using FluentAssertions;
using Xunit;

namespace BuildingBlocks.Core.Tests.DomainObjects;

public class EntityTests
{
    [Fact]
    public void Constructor_ShouldGenerateNonEmptyId()
    {
        var entity = new Entity();

        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAt()
    {
        var before = DateTime.Now;

        var entity = new Entity();

        entity.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.Now);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenSameId()
    {
        var id = Guid.NewGuid();
        var entityA = new Entity { Id = id };
        var entityB = new Entity { Id = id };

        entityA.Equals(entityB).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenDifferentId()
    {
        var entityA = new Entity();
        var entityB = new Entity();

        entityA.Equals(entityB).Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenBothNull()
    {
        Entity? entityA = null;
        Entity? entityB = null;

        (entityA == entityB).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnFalse_WhenOnlyOneIsNull()
    {
        var entityA = new Entity();
        Entity? entityB = null;

        (entityA == entityB).Should().BeFalse();
    }

    [Fact]
    public void InequalityOperator_ShouldReturnTrue_WhenDifferentId()
    {
        var entityA = new Entity();
        var entityB = new Entity();

        (entityA != entityB).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ShouldBeConsistentWithEquals()
    {
        var id = Guid.NewGuid();
        var entityA = new Entity { Id = id };
        var entityB = new Entity { Id = id };

        entityA.GetHashCode().Should().Be(entityB.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldContainTypeNameAndId()
    {
        var entity = new Entity();

        entity.ToString().Should().Be($"Entity [Id={entity.Id}]");
    }
}
