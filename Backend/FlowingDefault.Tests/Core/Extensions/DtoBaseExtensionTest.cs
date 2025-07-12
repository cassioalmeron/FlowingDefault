//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FlowingDefault.Tests.Core.Extensions
//{
//    [TestClass]
//    public class DtoBaseExtensionTest
//    {
//        [TestMethod]
//        public void DtoBaseExtension_ToModel()
//        {
//            var dto = new MovementSaveDto
//            {
//                Id = 1,
//                Date = DateTime.Now.Date,
//                Type = MovementTypeEnum.Expense,
//                EntityId = 11,
//                CategoryId = 12,
//                Paid = true,
//                Value = 100
//            };

//            var movement = new Movement();
//            dto.CopyTo(movement);

//            Assert.AreEqual(1, movement.Id);
//            Assert.AreEqual(DateTime.Now.Date, movement.Date);
//            Assert.AreEqual(MovementTypeEnum.Expense, movement.Type);
//            Assert.IsNotNull(movement.Entity);
//            Assert.AreEqual(11, movement.Entity.Id);
//            Assert.IsNotNull(movement.Category);
//            Assert.AreEqual(12, movement.Category.Id);
//            Assert.AreEqual(true, movement.Paid);
//            Assert.AreEqual(100, movement.Value);
//        }

//        [TestMethod]
//        public void DtoBaseExtension_ToModel_EmptyEntityId()
//        {
//            var dto = new MovementSaveDto
//            {
//                Id = 1,
//                Date = DateTime.Now.Date,
//                Type = MovementTypeEnum.Expense,
//                EntityId = 0,
//                CategoryId = 12,
//                Paid = true,
//                Value = 100
//            };

//            var movement = new Movement();
//            dto.CopyTo(movement);

//            Assert.IsNull(movement.Entity);
//        }
//        [TestMethod]
//        public void DtoBaseExtension_ToModel_NullEntityId()
//        {
//            var dto = new MovementSaveDto
//            {
//                Id = 1,
//                Date = DateTime.Now.Date,
//                Type = MovementTypeEnum.Expense,
//                CategoryId = 12,
//                Paid = true,
//                Value = 100
//            };

//            var movement = new Movement();
//            dto.CopyTo(movement);

//            Assert.IsNull(movement.Entity);
//        }
//    }
//}
