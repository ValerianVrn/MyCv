namespace MyCv.Common.Domain.UnitTests
{
    [TestClass]
    public class EnumerationTest
    {
        private class TestEnumeration : Enumeration
        {
            public static TestEnumeration TestEnumeration1 = new(1, "Value 1");

            public static TestEnumeration TestEnumeration2 = new(2, "Value 2");

            public TestEnumeration(int id, string name) : base(id, name)
            {

            }
        }

        [TestMethod]
        public void ToString_ReturnsName()
        {
            // Arrange.
            var value = TestEnumeration.TestEnumeration1;

            // Act.
            var result = value.ToString();

            // Assert.
            Assert.AreEqual("Value 1", result);
        }

        [TestMethod]
        public void GetAll_ReturnsAllEnumerations()
        {
            // Act.
            var allValues = Enumeration.GetAll<TestEnumeration>().ToList();

            // Assert.
            Assert.AreEqual(2, allValues.Count);
            Assert.IsTrue(allValues.Contains(TestEnumeration.TestEnumeration1));
            Assert.IsTrue(allValues.Contains(TestEnumeration.TestEnumeration2));
        }

        [TestMethod]
        public void Equals_SameIdAndType_ReturnsTrue()
        {
            // Arrange.
            var value1 = TestEnumeration.TestEnumeration1;
            var value2 = TestEnumeration.TestEnumeration1;

            // Act.
            var result = value1.Equals(value2);

            // Assert.
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Arrange.
            var value1 = TestEnumeration.TestEnumeration1;
            var value2 = "string";

            // Act.
            var result = value1.Equals(value2);

            // Assert.
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_DifferentId_ReturnsFalse()
        {
            // Arrange.
            var value1 = TestEnumeration.TestEnumeration1;
            var value2 = TestEnumeration.TestEnumeration2;

            // Act.
            var result = value1.Equals(value2);

            // Assert.
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetHashCode_SameId_ReturnsEqualHashCode()
        {
            // Arrange.
            var value1 = TestEnumeration.TestEnumeration1;
            var value2 = TestEnumeration.TestEnumeration1;

            // Act.
            var hashCode1 = value1.GetHashCode();
            var hashCode2 = value2.GetHashCode();

            // Assert.
            Assert.AreEqual(hashCode1, hashCode2);
        }

        [TestMethod]
        public void GetHashCode_DifferentId_ReturnsDifferentHashCode()
        {
            // Arrange.
            var value1 = TestEnumeration.TestEnumeration1;
            var value2 = TestEnumeration.TestEnumeration2;

            // Act.
            var hashCode1 = value1.GetHashCode();
            var hashCode2 = value2.GetHashCode();

            // Assert.
            Assert.AreNotEqual(hashCode1, hashCode2);
        }

        [TestMethod]
        public void FromValue_2_ReturnsCorrectEnumeration()
        {
            // Act.
            var enumeration = Enumeration.FromValue<TestEnumeration>(2);

            // Assert.
            Assert.AreEqual(TestEnumeration.TestEnumeration2, enumeration);
        }

        [TestMethod]
        public void FromValue_InvalidValue_ThrowsInvalidOperationException()
        {
            // Act.
            var act = () => Enumeration.FromValue<TestEnumeration>(999);

            // Assert.
            Assert.ThrowsException<InvalidOperationException>(act);
        }

        [TestMethod]
        public void FromDisplayName_ReturnsCorrectEnumeration()
        {
            // Arrange & Act.
            var enumeration = Enumeration.FromDisplayName<TestEnumeration>("Value 2");

            // Assert.
            Assert.AreEqual(TestEnumeration.TestEnumeration2, enumeration);
        }

        [TestMethod]
        public void FromDisplayName_InvalidName_ThrowsInvalidOperationException()
        {
            // Act.
            var act = () => Enumeration.FromDisplayName<TestEnumeration>("InvalidName");

            // Assert.
            Assert.ThrowsException<InvalidOperationException>(act);
        }

        [TestMethod]
        public void CompareTo_LessThan_ReturnsNegativeValue()
        {
            // Arrange.
            var enumeration1 = TestEnumeration.TestEnumeration1;
            var enumeration2 = TestEnumeration.TestEnumeration2;

            // Act.
            var result = enumeration1.CompareTo(enumeration2);

            // Assert.
            Assert.IsTrue(result < 0);
        }

        [TestMethod]
        public void CompareTo_GreaterThan_ReturnsPositiveValue()
        {
            // Arrange.
            var enumeration1 = TestEnumeration.TestEnumeration1;
            var enumeration2 = TestEnumeration.TestEnumeration2;

            // Act.
            var result = enumeration2.CompareTo(enumeration1);

            // Assert.
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void CompareTo_Equal_ReturnsZero()
        {
            // Arrange.
            var enumeration1 = TestEnumeration.TestEnumeration1;
            var enumeration2 = TestEnumeration.TestEnumeration1;

            // Act.
            var result = enumeration1.CompareTo(enumeration2);

            // Assert.
            Assert.AreEqual(0, result);
        }
    }
}
