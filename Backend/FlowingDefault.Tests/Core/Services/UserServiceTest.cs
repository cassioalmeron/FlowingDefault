using FlowingDefault.Core;
using FlowingDefault.Core.Dtos;
using FlowingDefault.Core.Models;
using FlowingDefault.Core.Services;
using FlowingDefault.Tests.Mocks;

namespace FlowingDefault.Tests.Core.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private readonly TestDbContext _context;
        private readonly UserService _service;

        public UserServiceTest()
        {
            _context = new TestDbContext();
            _context.InitializeTestDatabase();
            _service = new UserService(_context);
        }

        [TestMethod]
        public async Task UserService_SaveNewUser()
        {
            var userDto = new UserDto
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
            };

            await _service.Save(userDto);

            var user = _context.Users.FirstOrDefault(u => u.Username == "cassioalmeron");

            Assert.IsNotNull(user);
            Assert.AreEqual("Cassio Almeron", user.Name);
            Assert.AreEqual("cassioalmeron", user.Username);
            Assert.IsNotNull(user.Password);
        }

        [TestMethod]
        public async Task UserService_SaveNewUser_2x()
        {
            var user = new UserDto
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
            };

            await _service.Save(user);

            user = new UserDto
            {
                Name = "Iclen Granzotto",
                Username = "iclengranzotto",
            };

            await _service.Save(user);

            // Should have admin user + 2 new users = 3 total
            Assert.AreEqual(3, _context.Users.Count());
        }

        [TestMethod]
        public async Task UserService_SaveNewUser_RepeatedUsername()
        {
            var user = new UserDto
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
            };

            await _service.Save(user);

            user = new UserDto
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
            };

            try
            {
                await _service.Save(user);
                Assert.Fail();
            }
            catch (FlowingDefaultException e)
            {
                Assert.AreEqual($"Username '{user.Username}' already exists.", e.Message);
            }
        }

        [TestMethod]
        public async Task UserService_Delete()
        {
            var user = new UserDto
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
            };

            await _service.Save(user);

            var result = await _service.Delete(user.Id);

            Assert.IsTrue(result);
            Assert.AreEqual(1, _context.Users.Count());
            Assert.AreEqual("admin", _context.Users.Single().Username);
        }

        [TestMethod]
        public async Task UserService_Delete_Admin()
        {
            try
            {
                await _service.Delete(1);
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("The admin user can't be deleted.", e.Message);
            }
        }

        [TestMethod]
        public async Task UserService_UsernameExists_WhenUserExists()
        {
            var user = new UserDto
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
            };

            await _service.Save(user);

            var result = await _service.UsernameExists("cassioalmeron");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UserService_UsernameExists_WhenUserDoesNotExist()
        {
            var user = new UserDto
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
            };

            await _service.Save(user);

            var result = await _service.UsernameExists("nonexistentuser");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UserService_UsernameExists_WhenDatabaseIsEmpty()
        {
            var result = await _service.UsernameExists("anyusername");

            Assert.IsFalse(result);
        }
    }
}