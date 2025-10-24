using GymTime.Application.Dtos.GymMembers;
using GymTime.Application.Services;
using GymTime.Domain.Entities;
using GymTime.Domain.Enums;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Api.Tests.Services;

public class GymMemberServiceTests : IDisposable
{
    private readonly GymTimeDbContext _context;
    private readonly GymMemberService _service;

 public GymMemberServiceTests()
{
  var options = new DbContextOptionsBuilder<GymTimeDbContext>()
  .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      _context = new GymTimeDbContext(options);
      _service = new GymMemberService(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGymMembers()
    {
  // Arrange
   await _context.GymMembers.AddRangeAsync(
    new GymMember { Id = Guid.NewGuid(), Name = "John", PlanType = PlanType.Monthly },
       new GymMember { Id = Guid.NewGuid(), Name = "Jane", PlanType = PlanType.Annual }
 );
 await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllAsync();

  // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
public async Task GetByIdAsync_MemberExists_ReturnsMember()
    {
        // Arrange
   var id = Guid.NewGuid();
        await _context.GymMembers.AddAsync(
   new GymMember { Id = id, Name = "John", PlanType = PlanType.Monthly }
  );
   await _context.SaveChangesAsync();

        // Act
   var result = await _service.GetByIdAsync(id);

 // Assert
    Assert.NotNull(result);
  Assert.Equal("John", result.Name);
      Assert.Equal(PlanType.Monthly, result.PlanType);
    }

  [Fact]
    public async Task GetByIdAsync_MemberNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
    Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesNewMember()
{
        // Arrange
    var request = new CreateGymMemberRequest
        {
       Name = "John Doe",
 PlanType = PlanType.Quarterly
    };

   // Act
        var result = await _service.CreateAsync(request);

      // Assert
    Assert.NotNull(result);
   Assert.Equal("John Doe", result.Name);
 Assert.Equal(PlanType.Quarterly, result.PlanType);

        var memberInDb = await _context.GymMembers.FindAsync(result.Id);
      Assert.NotNull(memberInDb);
     Assert.Equal("John Doe", memberInDb.Name);
    }

  [Fact]
    public async Task UpdateAsync_MemberExists_UpdatesAndReturnsTrue()
    {
     // Arrange
    var id = Guid.NewGuid();
        await _context.GymMembers.AddAsync(
     new GymMember { Id = id, Name = "John", PlanType = PlanType.Monthly }
        );
  await _context.SaveChangesAsync();

    var updateRequest = new UpdateGymMemberRequest
        {
 Name = "John Updated",
      PlanType = PlanType.Annual
 };

        // Act
        var result = await _service.UpdateAsync(id, updateRequest);

        // Assert
     Assert.True(result);
   var updated = await _context.GymMembers.FindAsync(id);
  Assert.Equal("John Updated", updated?.Name);
        Assert.Equal(PlanType.Annual, updated?.PlanType);
    }

    [Fact]
    public async Task UpdateAsync_MemberNotFound_ReturnsFalse()
    {
        // Arrange
    var id = Guid.NewGuid();
        var updateRequest = new UpdateGymMemberRequest
        {
        Name = "John Updated",
 PlanType = PlanType.Annual
  };

        // Act
        var result = await _service.UpdateAsync(id, updateRequest);

  // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_MemberExists_DeletesAndReturnsTrue()
    {
      // Arrange
    var id = Guid.NewGuid();
        await _context.GymMembers.AddAsync(
        new GymMember { Id = id, Name = "John", PlanType = PlanType.Monthly }
        );
        await _context.SaveChangesAsync();

        // Act
var result = await _service.DeleteAsync(id);

        // Assert
 Assert.True(result);
        var deleted = await _context.GymMembers.FindAsync(id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_MemberNotFound_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();

   // Act
        var result = await _service.DeleteAsync(id);

        // Assert
      Assert.False(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
