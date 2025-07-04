using FlowingDefault.Core;
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
            _service = new UserService(_context);
        }

        [TestMethod]
        public async Task UserService_SaveNewUser()
        {
            var user = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = "123456"
            };

            await _service.Save(user);

            user = _context.Users.Single();

            Assert.AreEqual("Cassio Almeron", user.Name);
            Assert.AreEqual("cassioalmeron", user.Username);
            Assert.AreEqual("123456", user.Password);
        }

        [TestMethod]
        public async Task UserService_SaveNewUser_2x()
        {
            var user = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = "123456"
            };

            await _service.Save(user);

            user = new User
            {
                Name = "Iclen Granzotto",
                Username = "iclengranzotto",
                Password = "123456"
            };

            await _service.Save(user);

            Assert.AreEqual(2, _context.Users.Count());
        }

        [TestMethod]
        public async Task UserService_SaveNewUser_RepeatedUsername()
        {
            var user = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = "123456"
            };

            await _service.Save(user);

            user = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = "123456"
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
            var user = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = "123456"
            };

            await _service.Save(user);

            var result = await _service.Delete(user.Id);

            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.Users.Count());
        }

        [TestMethod]
        public async Task UserService_UsernameExists_WhenUserExists()
        {
            var user = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = "123456"
            };

            await _service.Save(user);

            var result = await _service.UsernameExists("cassioalmeron");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UserService_UsernameExists_WhenUserDoesNotExist()
        {
            var user = new User
            {
                Name = "Cassio Almeron",
                Username = "cassioalmeron",
                Password = "123456"
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