using Bookify.Domain.UnitTests.Infrastructure;
using Bookify.Domain.Users;
using Bookify.Domain.Users.Events;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Domain.UnitTests.Users;
public class UserTests:BaseTest
{
    [Fact]
    public void Create_Should_PropertySetValues()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName,UserData.Email);

        // Assert
        user.FirstName.Should().Be(UserData.FirstName);
        user.LastName.Should().Be(UserData.LastName);
        user.Email.Should().Be(UserData.Email);
    }

    [Fact]
    public void Create_Should_CreateDomainEvent_Published()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);

        // Assert
        var domainEvent = AssertDomainEventWasPublished<UserCreatedDomainEvent>(user);
        domainEvent.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void Create_UserRoleShould_MustIncludeRegistered()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email);

        // Assert
        user.Roles.Should().Contain(Role.Registered);
    }
}
