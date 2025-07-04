using FlowingDefault.Core;
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
        private readonly Project _testProject;

        public ProjectServiceTest()
        {
            _context = new TestDbContext();
            _service = new ProjectService(_context);
            
            _testUser = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = "123456"
            };

            _testProject = new Project
            {
                Name = "Test Project",
                UserId = 1,
                User = _testUser
            };
        }

        [TestMethod]
        public async Task ProjectService_SaveNewProject()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            var project = new Project
            {
                Name = "My First Project",
                UserId = _testUser.Id
            };

            // Act
            await _service.Save(project);

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

            var project1 = new Project
            {
                Name = "First Project",
                UserId = _testUser.Id
            };

            var project2 = new Project
            {
                Name = "Second Project",
                UserId = _testUser.Id
            };

            // Act
            await _service.Save(project1);
            await _service.Save(project2);

            // Assert
            Assert.AreEqual(2, _context.Projects.Count());
        }

        [TestMethod]
        public async Task ProjectService_SaveNewProject_RepeatedNameForSameUser()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            var project1 = new Project
            {
                Name = "My Project",
                UserId = _testUser.Id
            };

            await _service.Save(project1);

            var project2 = new Project
            {
                Name = "My Project",
                UserId = _testUser.Id
            };

            // Act & Assert
            try
            {
                await _service.Save(project2);
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

            var project1 = new Project
            {
                Name = "Same Name Project",
                UserId = user1.Id
            };

            var project2 = new Project
            {
                Name = "Same Name Project",
                UserId = user2.Id
            };

            // Act
            await _service.Save(project1);
            await _service.Save(project2);

            // Assert
            Assert.AreEqual(2, _context.Projects.Count());
        }

        [TestMethod]
        public async Task ProjectService_SaveNewProject_UserDoesNotExist()
        {
            // Arrange
            var project = new Project
            {
                Name = "My Project",
                UserId = 999 // Non-existent user ID
            };

            // Act & Assert
            try
            {
                await _service.Save(project);
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
            _context.Projects.Add(_testProject);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.Delete(_testProject.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.Projects.Count());
        }

        [TestMethod]
        public async Task ProjectService_Delete_ProjectDoesNotExist()
        {
            // Act
            var result = await _service.Delete(999);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ProjectService_GetAll()
        {
            // Arrange
            _context.Users.Add(_testUser);
            _context.Projects.Add(_testProject);
            await _context.SaveChangesAsync();

            // Act
            var projects = await _service.GetAll();

            // Assert
            Assert.AreEqual(1, projects.Count());
            var project = projects.First();
            Assert.AreEqual("Test Project", project.Name);
            Assert.IsNotNull(project.User);
            Assert.AreEqual("Cassio Almeron", project.User.Name);
        }

        [TestMethod]
        public async Task ProjectService_GetById()
        {
            // Arrange
            _context.Users.Add(_testUser);
            _context.Projects.Add(_testProject);
            await _context.SaveChangesAsync();

            // Act
            var project = await _service.GetById(_testProject.Id);

            // Assert
            Assert.IsNotNull(project);
            Assert.AreEqual("Test Project", project.Name);
            Assert.IsNotNull(project.User);
            Assert.AreEqual("Cassio Almeron", project.User.Name);
        }

        [TestMethod]
        public async Task ProjectService_GetById_ProjectDoesNotExist()
        {
            // Act
            var project = await _service.GetById(999);

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
                UserId = 1,
                User = user1
            };

            var project2 = new Project
            {
                Name = "User 2 Project",
                UserId = 2,
                User = user2
            };

            _context.Users.AddRange(user1, user2);
            _context.Projects.AddRange(project1, project2);
            await _context.SaveChangesAsync();

            // Act
            var userProjects = await _service.GetByUserId(1);

            // Assert
            Assert.AreEqual(1, userProjects.Count());
            var project = userProjects.First();
            Assert.AreEqual("User 1 Project", project.Name);
            Assert.AreEqual(1, project.UserId);
        }

        [TestMethod]
        public async Task ProjectService_ProjectNameExistsForUser_WhenProjectExists()
        {
            // Arrange
            _context.Users.Add(_testUser);
            _context.Projects.Add(_testProject);
            await _context.SaveChangesAsync();

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
            _context.Projects.Add(_testProject);
            await _context.SaveChangesAsync();

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
            _context.Projects.Add(_testProject);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ProjectExists(_testProject.Id);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task ProjectService_ProjectExists_WhenProjectDoesNotExist()
        {
            // Act
            var result = await _service.ProjectExists(999);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ProjectService_UpdateProject()
        {
            // Arrange
            _context.Users.Add(_testUser);
            _context.Projects.Add(_testProject);
            await _context.SaveChangesAsync();

            // Update the existing tracked entity
            _testProject.Name = "Updated Project Name";

            // Act
            await _service.Save(_testProject);

            // Assert
            var project = _context.Projects.Single();
            Assert.AreEqual("Updated Project Name", project.Name);
            Assert.AreEqual(_testProject.Id, project.Id);
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
                UserId = 1,
                User = user
            };

            var project2 = new Project
            {
                Name = "Second Project",
                UserId = 1,
                User = user
            };

            _context.Users.Add(user);
            _context.Projects.AddRange(project1, project2);
            await _context.SaveChangesAsync();

            // Update the existing tracked entity to have the same name as project1
            project2.Name = "First Project";

            // Act & Assert
            try
            {
                await _service.Save(project2);
                Assert.Fail();
            }
            catch (FlowingDefaultException e)
            {
                Assert.AreEqual($"Project '{project2.Name}' already exists for this user.", e.Message);
            }
        }

        [TestMethod]
        public async Task ProjectService_UpdateProject_WithDetachedEntity()
        {
            // Arrange
            _context.Users.Add(_testUser);
            _context.Projects.Add(_testProject);
            await _context.SaveChangesAsync();

            // Create a new detached entity with the same ID
            var detachedProject = new Project
            {
                Id = _testProject.Id,
                Name = "Updated Project Name",
                UserId = _testUser.Id
            };

            // Act
            await _service.Save(detachedProject);

            // Assert
            var project = _context.Projects.Single();
            Assert.AreEqual("Updated Project Name", project.Name);
            Assert.AreEqual(_testProject.Id, project.Id);
        }

        [TestMethod]
        public async Task ProjectService_UpdateProject_ProjectDoesNotExist()
        {
            // Arrange
            _context.Users.Add(_testUser);
            await _context.SaveChangesAsync();

            var nonExistentProject = new Project
            {
                Id = 999,
                Name = "Non-existent Project",
                UserId = _testUser.Id
            };

            // Act & Assert
            try
            {
                await _service.Save(nonExistentProject);
                Assert.Fail();
            }
            catch (FlowingDefaultException e)
            {
                Assert.AreEqual("Project with ID 999 not found.", e.Message);
            }
        }
    }
}