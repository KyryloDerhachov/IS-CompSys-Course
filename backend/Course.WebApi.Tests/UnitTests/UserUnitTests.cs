using System;
using Course.WebApi.Domain.Entities;
using Xunit;

namespace Course.WebApi.Tests.UnitTests;

public class UserUnitTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectlyAndTrimFields()
    {
        var login = "  john_doe  ";
        var email = "  John@Store.com  ";
        var firstName = "  John  ";
        var lastName = "  Doe  ";

        var user = new User(login, email, firstName, lastName);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("john_doe", user.Login);
        Assert.Equal("john@store.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.True(user.IsActive);
        Assert.Equal(0, user.FailedLoginAttempts);
        Assert.Null(user.LockoutUntil);
        Assert.Empty(user.Roles);
    }

    [Fact]
    public void Update_ShouldModifyFieldsAndNormalizeEmail()
    {
        var user = new User("login", "old@store.com", "OldName", "OldLastName");

        user.Update("  new@store.com  ", "  NewName  ", "  NewLastName  ");

        Assert.Equal("new@store.com", user.Email);
        Assert.Equal("NewName", user.FirstName);
        Assert.Equal("NewLastName", user.LastName);
    }

    [Fact]
    public void RecordFailedLogin_IncrementLessThanFive_ShouldIncreaseCountButNotLockout()
    {
        var user = new User("login", "user@store.com", "First", "Last");

        user.RecordFailedLogin();
        user.RecordFailedLogin();

        Assert.Equal(2, user.FailedLoginAttempts);
        Assert.Null(user.LockoutUntil);
        Assert.False(user.IsLockedOut());
    }

    [Fact]
    public void RecordFailedLogin_ReachedFiveAttempts_ShouldSetLockout()
    {
        var user = new User("login", "user@store.com", "First", "Last");

        for (int i = 0; i < 5; i++)
        {
            user.RecordFailedLogin();
        }

        Assert.Equal(5, user.FailedLoginAttempts);
        Assert.NotNull(user.LockoutUntil);
        Assert.True(user.IsLockedOut());
    }

    [Fact]
    public void ResetFailedAttempts_ShouldClearCountAndLockout()
    {
        var user = new User("login", "user@store.com", "First", "Last");
        for (int i = 0; i < 5; i++)
        {
            user.RecordFailedLogin();
        }

        user.ResetFailedAttempts();

        Assert.Equal(0, user.FailedLoginAttempts);
        Assert.Null(user.LockoutUntil);
        Assert.False(user.IsLockedOut());
    }

    [Fact]
    public void IsLockedOut_PastLockoutTime_ShouldReturnFalse()
    {
        var user = new User("login", "user@store.com", "First", "Last");
        for (int i = 0; i < 5; i++)
        {
            user.RecordFailedLogin();
        }

        typeof(User).GetProperty(nameof(User.LockoutUntil))!
            .SetValue(user, DateTime.UtcNow.AddMinutes(-1));

        Assert.False(user.IsLockedOut());
    }

    [Fact]
    public void ActivateAndDeactivate_ShouldChangeIsActiveState()
    {
        var user = new User("login", "user@store.com", "First", "Last");

        user.Deactivate();
        Assert.False(user.IsActive);

        user.Activate();
        Assert.True(user.IsActive);
    }
}