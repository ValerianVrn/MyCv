namespace MyCv.Common.Domain.UnitTests
{
    [TestClass]
    public class ValueObjectTest
    {
        private class TestValueObject : ValueObject
        {
            private readonly int _value;

            public TestValueObject(int value)
            {
                _value = value;
            }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return _value;
            }
        }

        [TestMethod]
        public void EqualityOperator_EqualValueObjects_ReturnsTrue()
        {
            // Arrange.
            var valueObject1 = new TestValueObject(10);
            var valueObject2 = new TestValueObject(10);

            // Act.
            var result = valueObject1 == valueObject2;

            // Assert.
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EqualityOperator_NotEqualValueObjects_ReturnsFalse()
        {
            // Arrange.
            var valueObject1 = new TestValueObject(10);
            var valueObject2 = new TestValueObject(20);

            // Act.
            var result = valueObject1 == valueObject2;

            // Assert.
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EqualityOperator_Null_ReturnsFalse()
        {
            // Arrange.
            var valueObject1 = new TestValueObject(10);

            // Act.
            var result = valueObject1 == null;

            // Assert.
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void InequalityOperator_EqualValueObjects_ReturnsFalse()
        {
            // Arrange.
            var valueObject1 = new TestValueObject(10);
            var valueObject2 = new TestValueObject(10);

            // Act.
            var result = valueObject1 != valueObject2;

            // Assert.
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void InequalityOperator_NotEqualValueObjects_ReturnsTrue()
        {
            // Arrange.
            var valueObject1 = new TestValueObject(10);
            var valueObject2 = new TestValueObject(20);

            // Act.
            var result = valueObject1 != valueObject2;

            // Assert.
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DynamicData(nameof(EqualValueObjects))]
        public void Equals_EqualValueObjects_ReturnsTrue(ValueObject instanceA, ValueObject instanceB, string reason)
        {
            // Act.
            var result = EqualityComparer<ValueObject>.Default.Equals(instanceA, instanceB);

            // Assert.
            Assert.IsTrue(result, reason);
        }

        [TestMethod]
        [DynamicData(nameof(NonEqualValueObjects))]
        public void Equals_NonEqualValueObjects_ReturnsFalse(ValueObject instanceA, ValueObject instanceB, string reason)
        {
            // Act.
            var result = EqualityComparer<ValueObject>.Default.Equals(instanceA, instanceB);

            // Assert.
            Assert.IsFalse(result, reason);
        }

        [TestMethod]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            // Arrange.
            var valueObject1 = new TestValueObject(10);
            var valueObject2 = new TestValueObject(10);

            // Act.
            var hashCode1 = valueObject1.GetHashCode();
            var hashCode2 = valueObject2.GetHashCode();

            // Assert.
            Assert.AreEqual(hashCode1, hashCode2);
        }

        [TestMethod]
        public void GetHashCode_UnequalObjects_ReturnsDifferentHashCode()
        {
            // Arrange.
            var valueObject1 = new TestValueObject(10);
            var valueObject2 = new TestValueObject(20);

            // Act.
            var hashCode1 = valueObject1.GetHashCode();
            var hashCode2 = valueObject2.GetHashCode();

            // Assert.
            Assert.AreNotEqual(hashCode1, hashCode2);
        }

        [TestMethod]
        public void GetCopy_ReturnsDifferentReference()
        {
            // Arrange.
            var valueObject = new TestValueObject(10);

            // Act.
            var copy = valueObject.GetCopy();

            // Assert.
            Assert.AreNotSame(valueObject, copy);
        }

        private static readonly ValueObject APrettyValueObject = new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3"));

        public static IEnumerable<object[]> EqualValueObjects
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        null,
                        null,
                        "they should be equal because they are both null"
                    },
                    new object[]
                    {
                        APrettyValueObject,
                        APrettyValueObject,
                        "they should be equal because they are the same object"
                    },
                    new object[]
                    {
                        new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            new ComplexObject(2, "3")),
                        new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            new ComplexObject(2, "3")),
                        "they should be equal because they have equal members"
                    },
                    new object[]
                    {
                        new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            new ComplexObject(2, "3"), notAnEqualityComponent: "xpto"),
                        new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            new ComplexObject(2, "3"), notAnEqualityComponent: "xpto2"),
                        "they should be equal because all equality components are equal, even though an additional member was set"
                    },
                    new object[]
                    {
                        new ValueObjectB(1, "2", 1, 2, 3),
                        new ValueObjectB(1, "2", 1, 2, 3),
                        "they should be equal because all equality components are equal, including the 'C' list"
                    }
                };
            }
        }

        public static IEnumerable<object[]> NonEqualValueObjects
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            d: new ComplexObject(2, "3")),
                        new ValueObjectA(a: 2, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            d: new ComplexObject(2, "3")),
                        "they should not be equal because the 'A' member on ValueObjectA is different among them"
                    },
                    new object[]
                    {
                        new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            d: new ComplexObject(2, "3")),
                        new ValueObjectA(a: 1, b: null, c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            d: new ComplexObject(2, "3")),
                        "they should not be equal because the 'B' member on ValueObjectA is different among them"
                    },
                    new object[]
                    {
                        new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            d: new ComplexObject(a: 2, b: "3")),
                        new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            d: new ComplexObject(a: 3, b: "3")),
                        "they should not be equal because the 'A' member on ValueObjectA's 'D' member is different among them"
                    },
                    new object[]
                    {
                        new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"),
                            d: new ComplexObject(a: 2, b: "3")),
                        new ValueObjectB(a: 1, b: "2"),
                        "they should not be equal because they are not of the same type"
                    },
                    new object[]
                    {
                        new ValueObjectB(1, "2", 1, 2, 3),
                        new ValueObjectB(1, "2", 1, 2, 3, 4),
                        "they should be not be equal because the 'C' list contains one additional value"
                    },
                    new object[]
                    {
                        new ValueObjectB(1, "2", 1, 2, 3, 5),
                        new ValueObjectB(1, "2", 1, 2, 3),
                        "they should be not be equal because the 'C' list contains one additional value"
                    },
                    new object[]
                    {
                        new ValueObjectB(1, "2", 1, 2, 3, 5),
                        new ValueObjectB(1, "2", 1, 2, 3, 4),
                        "they should be not be equal because the 'C' lists are not equal"
                    }
                };
            }
        }

        private class ValueObjectA : ValueObject
        {
            public ValueObjectA(int a, string b, Guid c, ComplexObject d, string notAnEqualityComponent = null)
            {
                A = a;
                B = b;
                C = c;
                D = d;
                NotAnEqualityComponent = notAnEqualityComponent;
            }

            public int A { get; }
            public string B { get; }
            public Guid C { get; }
            public ComplexObject D { get; }
            public string NotAnEqualityComponent { get; }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return A;
                yield return B;
                yield return C;
                yield return D;
            }
        }

        private class ValueObjectB : ValueObject
        {
            public ValueObjectB(int a, string b, params int[] c)
            {
                A = a;
                B = b;
                C = c.ToList();
            }

            public int A { get; }
            public string B { get; }

            public List<int> C { get; }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return A;
                yield return B;

                foreach (var c in C)
                {
                    yield return c;
                }
            }
        }

        private class ComplexObject : IEquatable<ComplexObject>
        {
            public ComplexObject(int a, string b)
            {
                A = a;
                B = b;
            }

            public int A { get; set; }

            public string B { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as ComplexObject);
            }

            public bool Equals(ComplexObject other)
            {
                return other != null &&
                       A == other.A &&
                       B == other.B;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(A, B);
            }
        }
    }
}
