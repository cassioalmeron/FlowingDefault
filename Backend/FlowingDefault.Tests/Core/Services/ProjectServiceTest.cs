using FlowingDefault.Core;
using FlowingDefault.Core.Dtos;
using FlowingDefault.Core.Models;
using FlowingDefault.Core.Services;
using FlowingDefault.Tests.Mocks;

namespace FlowingDefault.Tests.Core.Services
{
    [TestClass]
    public class ProjectServiceTest
    {
        private readonly TestDbContext _context;
        private readonly ProjectService _service;
        private readonly User _testUser;
        private readonly ProjectDto _testProjectDto;

        public ProjectServiceTest()
        {
            _context = new TestDbContext();
            _context.InitializeTestDatabase();
            _service = new ProjectService(_context);
            
            _testUser = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = "123456"
            };

            _testProjectDto = new ProjectDto
            {
                Name = "Test Project",
            };
        }

        private async Task<Project> CreateTestProjectAsync()
        {
            var project = new Project
            {
                Name = "Test Project",
                UserId = _testUser.Id
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        [TestMethod]
        public async Task ProjectService_SaveNewProject()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            var project = new ProjectDto
            {
                Name = "My First Project",
            };

            // Act
            await _service.Save(project, _testUser.Id);

            // Assert
            var savedProject = _context.Projects.Single();
            Assert.AreEqual("My First Project", savedProject.Name);
            Assert.AreEqual(_testUser.Id, savedProject.UserId);
        }

        [TestMethod]
        public async Task ProjectService_SaveNewProject_2x()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            var project1 = new ProjectDto
            {
                Name = "First Project",
            };

            var project2 = new ProjectDto
            {
                Name = "Second Project",
            };

            // Act
            await _service.Save(project1, _testUser.Id);
            await _service.Save(project2, _testUser.Id);

            // Assert
            Assert.AreEqual(2, _context.Projects.Count());
        }

        [TestMethod]
        public async Task ProjectService_SaveNewProject_RepeatedNameForSameUser()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            var project1 = new ProjectDto
            {
                Name = "My Project",
            };

            await _service.Save(project1, _testUser.Id);

            var project2 = new ProjectDto
            {
                Name = "My Project",
            };

            // Act & Assert
            try
            {
                await _service.Save(project2, _testUser.Id);
                Assert.Fail();
            }
            catch (FlowingDefaultException e)
            {
                Assert.AreEqual($"Project '{project2.Name}' already exists for this user.", e.Message);
            }
        }

        [TestMethod]
        public async Task ProjectService_SaveNewProject_SameNameDifferentUsers()
        {
            // Arrange
            var user1 = new User
            {
                Name = "User 1",
                Username = "user1",
                Password = "123456"
            };

            var user2 = new User
            {
                Name = "User 2",
                Username = "user2",
                Password = "123456"
            };

            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();

            var project1 = new ProjectDto
            {
                Name = "Same Name Project",
            };

            var project2 = new ProjectDto
            {
                Name = "Same Name Project",
            };

            // Act
            await _service.Save(project1, user1.Id);
            await _service.Save(project2, user2.Id);

            // Assert
            Assert.AreEqual(2, _context.Projects.Count());
        }

        [TestMethod]
        public async Task ProjectService_SaveNewProject_UserDoesNotExist()
        {
            // Arrange
            var project = new ProjectDto
            {
                Name = "My Project",
            };

            // Act & Assert
            try
            {
                await _service.Save(project, 999);
                Assert.Fail();
            }
            catch (FlowingDefaultException e)
            {
                Assert.AreEqual("User with ID 999 not found.", e.Message);
            }
        }

        [TestMethod]
        public async Task ProjectService_Delete()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            var projectDto = new ProjectDto
            {
                Name = "Test Project",
            };

            await _service.Save(projectDto, _testUser.Id);

            // Act
            var result = await _service.Delete(projectDto.Id, _testUser.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.Projects.Count());
        }

        [TestMethod]
        public async Task ProjectService_Delete_ProjectDoesNotExist()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.Delete(999, _testUser.Id);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ProjectService_GetAll()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();
            await CreateTestProjectAsync();

            // Act
            var projects = await _service.GetAll(_testUser.Id);

            // Assert
            Assert.AreEqual(1, projects.Count());
            var projectResult = projects.First();
            Assert.AreEqual("Test Project", projectResult.Name);
            Assert.IsNotNull(projectResult.User);
            Assert.AreEqual("Cassio Almeron", projectResult.User.Name);
        }

        [TestMethod]
        public async Task ProjectService_GetById()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();
            var project = await CreateTestProjectAsync();

            // Act
            var projectResult = await _service.GetById(project.Id, _testUser.Id);

            // Assert
            Assert.IsNotNull(projectResult);
            Assert.AreEqual("Test Project", projectResult.Name);
            Assert.IsNotNull(projectResult.User);
            Assert.AreEqual("Cassio Almeron", projectResult.User.Name);
        }

        [TestMethod]
        public async Task ProjectService_GetById_ProjectDoesNotExist()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            // Act
            var project = await _service.GetById(999, _testUser.Id);

            // Assert
            Assert.IsNull(project);
        }

        [TestMethod]
        public async Task ProjectService_GetByUserId()
        {
            // Arrange
            var user1 = new User
            {
                Name = "User 1",
                Username = "user1",
                Password = "123456"
            };

            var user2 = new User
            {
                Name = "User 2",
                Username = "user2",
                Password = "123456"
            };

            var project1 = new Project
            {
                Name = "User 1 Project",
                User = user1
            };

            var project2 = new Project
            {
                Name = "User 2 Project",
                User = user2
            };

            _context.Users.AddRange(user1, user2);
            _context.Projects.AddRange(project1, project2);
            await _context.SaveChangesAsync();

            // Act
            var userProjects = await _service.GetByUserId(user1.Id);

            // Assert
            Assert.AreEqual(1, userProjects.Count());
            var project = userProjects.First();
            Assert.AreEqual("User 1 Project", project.Name);
            Assert.AreEqual(user1.Id, project.UserId);
        }

        [TestMethod]
        public async Task ProjectService_ProjectNameExistsForUser_WhenProjectExists()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();
            await CreateTestProjectAsync();

            // Act
            var result = await _service.ProjectNameExistsForUser("Test Project", _testUser.Id);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ProjectService_ProjectNameExistsForUser_WhenProjectDoesNotExist()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();
            await CreateTestProjectAsync();

            // Act
            var result = await _service.ProjectNameExistsForUser("Non-existent Project", _testUser.Id);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ProjectService_ProjectExists_WhenProjectExists()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();
            var project = await CreateTestProjectAsync();

            // Act
            var result = await _service.ProjectExists(project.Id, _testUser.Id);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ProjectService_ProjectExists_WhenProjectDoesNotExist()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ProjectExists(999, _testUser.Id);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ProjectService_UpdateProject()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();
            var project = await CreateTestProjectAsync();

            // Update the existing tracked entity
            _testProjectDto.Id = project.Id;
            _testProjectDto.Name = "Updated Project Name";

            // Act
            await _service.Save(_testProjectDto, _testUser.Id);

            // Assert
            var updatedProject = _context.Projects.Single();
            Assert.AreEqual("Updated Project Name", updatedProject.Name);
            Assert.AreEqual(project.Id, updatedProject.Id);
        }

        [TestMethod]
        public async Task ProjectService_UpdateProject_RepeatedNameForSameUser()
        {
            // Arrange
            var user = new User
            {
                Name = "Test User",
                Username = "testuser",
                Password = "123456"
            };

            var project1 = new Project
            {
                Name = "First Project",
                User = user
            };

            var project2 = new Project
            {
                Name = "Second Project",
                User = user
            };

            var projectDto2 = new ProjectDto
            {
                Name = "First Project",
            };

            _context.Users.Add(user);
            _context.Projects.AddRange(project1, project2);
            await _context.SaveChangesAsync();

            // Update the existing tracked entity to have the same name as project1
            projectDto2.Name = "First Project";

            // Act & Assert
            try
            {
                await _service.Save(projectDto2, user.Id);
                Assert.Fail();
            }
            catch (FlowingDefaultException e)
            {
                Assert.AreEqual($"Project '{project1.Name}' already exists for this user.", e.Message);
            }
        }

        //[TestMethod]
        //public async Task ProjectService_UpdateProject_WithDetachedEntity()
        //{
        //    // Arrange
        //    _context.Users.Add(_testUser);
        //    _context.Projects.Add(_testProject);
        //    await _context.SaveChangesAsync();

        //    // Create a new detached entity with the same ID
        //    var detachedProject = new Project
        //    {
        //        Id = _testProject.Id,
        //        Name = "Updated Project Name",
        //        UserId = _testUser.Id
        //    };

        //    // Act
        //    await _service.Save(detachedProject);

        //    // Assert
        //    var project = _context.Projects.Single();
        //    Assert.AreEqual("Updated Project Name", project.Name);
        //    Assert.AreEqual(_testProject.Id, project.Id);
        //}

        [TestMethod]
        public async Task ProjectService_UpdateProject_ProjectDoesNotExist()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            var nonExistentProject = new ProjectDto
            {
                Id = 999,
                Name = "Non-existent Project",
            };

            // Act & Assert
            try
            {
                await _service.Save(nonExistentProject, _testUser.Id);
                Assert.Fail();
            }
            catch (FlowingDefaultException e)
            {
                Assert.AreEqual("Project with ID 999 not found.", e.Message);
            }
        }
    }
}