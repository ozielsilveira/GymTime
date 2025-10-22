using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTime.Api.Controllers;
using GymTime.Application.Dtos.GymMembers;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GymTime.Api.Tests.Controllers
{
    public class GymMembersControllerTests
    {
        private class FakeGymMemberService : IGymMemberService
        {
            public IEnumerable<GymMemberDto> AllReturn { get; set; } = Array.Empty<GymMemberDto>();
            public GymMemberDto? ByIdReturn { get; set; }
            public GymMemberDto? CreateReturn { get; set; }
            public bool UpdateReturn { get; set; } = true;
            public bool DeleteReturn { get; set; } = true;

            public Guid? LastGetById { get; private set; }
            public Guid? LastUpdateId { get; private set; }
            public UpdateGymMemberRequest? LastUpdateRequest { get; private set; }
            public Guid? LastDeleteId { get; private set; }
            public CreateGymMemberRequest? LastCreateRequest { get; private set; }

            public Task<IEnumerable<GymMemberDto>> GetAllAsync() => Task.FromResult(AllReturn);
            public Task<GymMemberDto?> GetByIdAsync(Guid id) { LastGetById = id; return Task.FromResult(ByIdReturn); }
            public Task<GymMemberDto> CreateAsync(CreateGymMemberRequest request) { LastCreateRequest = request; return Task.FromResult(CreateReturn!); }
            public Task<bool> UpdateAsync(Guid id, UpdateGymMemberRequest request) { LastUpdateId = id; LastUpdateRequest = request; return Task.FromResult(UpdateReturn); }
            public Task<bool> DeleteAsync(Guid id) { LastDeleteId = id; return Task.FromResult(DeleteReturn); }
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithItems()
        {
            var fake = new FakeGymMemberService();
            fake.AllReturn = new[] { new GymMemberDto { Id = Guid.NewGuid(), Name = "John", PlanType = GymTime.Domain.Enums.PlanType.Monthly } };
            var controller = new GymMembersController(fake);

            var result = await controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<GymMemberDto>>(ok.Value);
            Assert.NotEmpty(items);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var fake = new FakeGymMemberService();
            var dto = new GymMemberDto { Id = Guid.NewGuid(), Name = "Ana", PlanType = GymTime.Domain.Enums.PlanType.Annual };
            fake.ByIdReturn = dto;
            var controller = new GymMembersController(fake);

            var result = await controller.GetById(dto.Id);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
            Assert.Equal(dto.Id, fake.LastGetById);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var fake = new FakeGymMemberService();
            fake.ByIdReturn = null;
            var controller = new GymMembersController(fake);

            var result = await controller.GetById(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenModelValid()
        {
            var fake = new FakeGymMemberService();
            var created = new GymMemberDto { Id = Guid.NewGuid(), Name = "Paulo", PlanType = GymTime.Domain.Enums.PlanType.Quarterly };
            fake.CreateReturn = created;
            var controller = new GymMembersController(fake);

            var request = new CreateGymMemberRequest { Name = "Paulo", PlanType = GymTime.Domain.Enums.PlanType.Quarterly };
            var result = await controller.Create(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
            Assert.Equal(created.Id, createdResult.RouteValues!["id"]);
            Assert.Equal(request, fake.LastCreateRequest);
        }

        [Fact]
        public async Task Create_ReturnsValidationProblem_WhenModelInvalid()
        {
            var fake = new FakeGymMemberService();
            var controller = new GymMembersController(fake);
            controller.ModelState.AddModelError("Name", "Required");

            var request = new CreateGymMemberRequest();
            var result = await controller.Create(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var status = objectResult.StatusCode ?? 400;
            Assert.Equal(400, status);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdated()
        {
            var fake = new FakeGymMemberService();
            fake.UpdateReturn = true;
            var controller = new GymMembersController(fake);

            var id = Guid.NewGuid();
            var request = new UpdateGymMemberRequest { Name = "Updated", PlanType = GymTime.Domain.Enums.PlanType.Monthly };

            var result = await controller.Update(id, request);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(id, fake.LastUpdateId);
            Assert.Equal(request, fake.LastUpdateRequest);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenMissing()
        {
            var fake = new FakeGymMemberService { UpdateReturn = false };
            var controller = new GymMembersController(fake);

            var result = await controller.Update(Guid.NewGuid(), new UpdateGymMemberRequest { Name = "x", PlanType = GymTime.Domain.Enums.PlanType.Monthly });
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsValidationProblem_WhenModelInvalid()
        {
            var fake = new FakeGymMemberService();
            var controller = new GymMembersController(fake);
            controller.ModelState.AddModelError("Name", "Required");

            var result = await controller.Update(Guid.NewGuid(), new UpdateGymMemberRequest());
            var objectResult = Assert.IsType<ObjectResult>(result);
            var status = objectResult.StatusCode ?? 400;
            Assert.Equal(400, status);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            var fake = new FakeGymMemberService { DeleteReturn = true };
            var controller = new GymMembersController(fake);

            var id = Guid.NewGuid();
            var result = await controller.Delete(id);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(id, fake.LastDeleteId);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            var fake = new FakeGymMemberService { DeleteReturn = false };
            var controller = new GymMembersController(fake);

            var result = await controller.Delete(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
