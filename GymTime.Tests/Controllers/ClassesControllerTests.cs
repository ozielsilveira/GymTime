using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTime.Api.Controllers;
using GymTime.Application.Dtos.Classes;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GymTime.Api.Tests.Controllers
{
    public class ClassesControllerTests
    {
        private class FakeClassService : IClassService
        {
            public IEnumerable<ClassDto> AllReturn { get; set; } = Array.Empty<ClassDto>();
            public ClassDto? ByIdReturn { get; set; }
            public ClassDto? CreateReturn { get; set; }
            public bool UpdateReturn { get; set; } = true;
            public bool DeleteReturn { get; set; } = true;

            public Guid? LastGetById { get; private set; }
            public Guid? LastUpdateId { get; private set; }
            public UpdateClassRequest? LastUpdateRequest { get; private set; }
            public Guid? LastDeleteId { get; private set; }
            public CreateClassRequest? LastCreateRequest { get; private set; }

            public Task<IEnumerable<ClassDto>> GetAllAsync() => Task.FromResult(AllReturn);

            public Task<ClassDto?> GetByIdAsync(Guid id)
            {
                LastGetById = id;
                return Task.FromResult(ByIdReturn as ClassDto);
            }

            public Task<ClassDto> CreateAsync(CreateClassRequest request)
            {
                LastCreateRequest = request;
                return Task.FromResult(CreateReturn!);
            }

            public Task<bool> UpdateAsync(Guid id, UpdateClassRequest request)
            {
                LastUpdateId = id;
                LastUpdateRequest = request;
                return Task.FromResult(UpdateReturn);
            }

            public Task<bool> DeleteAsync(Guid id)
            {
                LastDeleteId = id;
                return Task.FromResult(DeleteReturn);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithItems()
        {
            var fake = new FakeClassService();
            fake.AllReturn = new[] { new ClassDto { Id = Guid.NewGuid(), ClassType = "Yoga", Schedule = DateTime.UtcNow, MaxCapacity =10 } };
            var controller = new ClassesController(fake);

            var result = await controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<ClassDto>>(ok.Value);
            Assert.NotEmpty(items);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var fake = new FakeClassService();
            var dto = new ClassDto { Id = Guid.NewGuid(), ClassType = "Pilates", Schedule = DateTime.UtcNow, MaxCapacity =5 };
            fake.ByIdReturn = dto;
            var controller = new ClassesController(fake);

            var result = await controller.GetById(dto.Id);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
            Assert.Equal(dto.Id, fake.LastGetById);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var fake = new FakeClassService();
            fake.ByIdReturn = null;
            var controller = new ClassesController(fake);

            var result = await controller.GetById(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenModelValid()
        {
            var fake = new FakeClassService();
            var created = new ClassDto { Id = Guid.NewGuid(), ClassType = "Box", Schedule = DateTime.UtcNow, MaxCapacity =12 };
            fake.CreateReturn = created;
            var controller = new ClassesController(fake);

            var request = new CreateClassRequest { ClassType = "Box", Schedule = DateTime.UtcNow, MaxCapacity =12 };
            var result = await controller.Create(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
            Assert.Equal(created.Id, createdResult.RouteValues!["id"]);
            Assert.Equal(request, fake.LastCreateRequest);
        }

        [Fact]
        public async Task Create_ReturnsValidationProblem_WhenModelInvalid()
        {
            var fake = new FakeClassService();
            var controller = new ClassesController(fake);
            controller.ModelState.AddModelError("ClassType", "Required");

            var request = new CreateClassRequest();
            var result = await controller.Create(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var status = objectResult.StatusCode ??400;
            Assert.Equal(400, status);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdated()
        {
            var fake = new FakeClassService();
            fake.UpdateReturn = true;
            var controller = new ClassesController(fake);

            var id = Guid.NewGuid();
            var request = new UpdateClassRequest { ClassType = "Crossfit", Schedule = DateTime.UtcNow, MaxCapacity =20 };

            var result = await controller.Update(id, request);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(id, fake.LastUpdateId);
            Assert.Equal(request, fake.LastUpdateRequest);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenMissing()
        {
            var fake = new FakeClassService { UpdateReturn = false };
            var controller = new ClassesController(fake);

            var result = await controller.Update(Guid.NewGuid(), new UpdateClassRequest { ClassType = "x", Schedule = DateTime.UtcNow, MaxCapacity =1 });
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsValidationProblem_WhenModelInvalid()
        {
            var fake = new FakeClassService();
            var controller = new ClassesController(fake);
            controller.ModelState.AddModelError("ClassType", "Required");

            var result = await controller.Update(Guid.NewGuid(), new UpdateClassRequest());
            var objectResult = Assert.IsAssignableFrom<ObjectResult>(result);
            var status = objectResult.StatusCode ??400;
            Assert.Equal(400, status);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            var fake = new FakeClassService { DeleteReturn = true };
            var controller = new ClassesController(fake);

            var id = Guid.NewGuid();
            var result = await controller.Delete(id);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(id, fake.LastDeleteId);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            var fake = new FakeClassService { DeleteReturn = false };
            var controller = new ClassesController(fake);

            var result = await controller.Delete(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
