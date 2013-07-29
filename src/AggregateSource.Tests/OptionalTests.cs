using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace AggregateSource
{
    namespace OptionalTests
    {
        //We need test for reference and value type optionals separately.

        [TestFixture]
        public class WithAnyInstance
        {
            private Optional<AggregateRootEntityStub> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = Optional<AggregateRootEntityStub>.Empty;
            }

            [Test]
            public void IsEquatable()
            {
                Assert.That(_sut,
                            Is.InstanceOf<IEquatable<Optional<AggregateRootEntityStub>>>());
            }
        }

        [TestFixture]
        public class WithEmptyReferenceTypeInstance : WithEmptyInstanceFixture<Object>
        {
            protected override Optional<object> SutFactory()
            {
                return Optional<object>.Empty;
            }

            protected override Optional<object> WithValueFactory()
            {
                return new Optional<object>(new object());
            }
        }

        [TestFixture]
        public class WithEmptyValueTypeInstance : WithEmptyInstanceFixture<Int32>
        {
            protected override Optional<int> SutFactory()
            {
                return Optional<int>.Empty;
            }

            protected override Optional<int> WithValueFactory()
            {
                return new Optional<int>(123);
            }
        }

        [TestFixture]
        public abstract class WithEmptyInstanceFixture<T>
        {
            private Optional<T> _sut;

            [SetUp]
            public void SetUp()
            {
                _sut = SutFactory();
            }

            protected abstract Optional<T> SutFactory();
            protected abstract Optional<T> WithValueFactory();

            [Test]
            public void HasValueReturnsFalse()
            {
                Assert.That(_sut.HasValue, Is.False);
            }

            [Test]
            public void ValueThrows()
            {
                // ReSharper disable UnusedVariable
                Assert.Throws<InvalidOperationException>(() => { var _ = _sut.Value; });
                // ReSharper restore UnusedVariable
            }

            [Test]
            public void TypedEnumerationIsEmpty()
            {
                Assert.That(_sut, Is.EquivalentTo(Enumerable.Empty<T>()));
            }

            [Test]
            public void ObjectEnumerationIsEmpty()
            {
                var sut = (IEnumerable) _sut;

                Assert.That(sut, Is.EquivalentTo(Enumerable.Empty<T>()));
            }

            [Test]
            public void DoesNotEqualNull()
            {
                Assert.IsFalse(_sut.Equals(null));
            }

            [Test]
            public void DoesNotObjectEqualNull()
            {
                Assert.IsFalse(_sut.Equals((object) null));
            }

            [Test]
            public void DoesNotEqualObjectOfOtherType()
            {
                Assert.IsFalse(_sut.Equals(new object()));
            }

            [Test]
            public void DoesObjectEqualItself()
            {
                Assert.IsTrue(_sut.Equals((object) _sut));
            }

            [Test]
            public void DoesEqualItself()
            {
                Assert.IsTrue(_sut.Equals(_sut));
            }

            [Test]
            public void TwoInstancesAreEqualIfTheyBothDoNotHaveAValueAndAreOfTheSameValueType()
            {
                Assert.IsTrue(_sut.Equals(Optional<T>.Empty));
            }

            [Test]
            public void TwoInstancesAreNotEqualIfTheOtherHasAValueAndIsOfTheSameValueType()
            {
                Assert.IsFalse(_sut.Equals(WithValueFactory()));
            }

            [Test]
            public void TwoInstancesAreNotEqualIfTheOtherHasAValueAndIsOfDifferentValueType()
            {
                Assert.IsFalse(_sut.Equals(new Optional<MismatchObject>(new MismatchObject())));
            }

            [Test]
            public void TwoInstancesAreNotEqualIfTheOtherHasNoValueAndIsOfDifferentValueType()
            {
                Assert.IsFalse(_sut.Equals(Optional<MismatchObject>.Empty));
            }

            [Test]
            public void TwoInstanceHaveTheSameHashCodeIfTheyBothDoNotHaveAValueAndAreOfTheSameValueType()
            {
                Assert.IsTrue(_sut.GetHashCode().Equals(Optional<T>.Empty.GetHashCode()));
            }

            [Test]
            public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasAValueAndIsOfTheSameValueType()
            {
                Assert.IsFalse(_sut.GetHashCode().Equals(WithValueFactory().GetHashCode()));
            }

            [Test]
            public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasAValueAndIsOfDifferentValueType()
            {
                Assert.IsFalse(_sut.GetHashCode().
                                    Equals(new Optional<MismatchObject>(new MismatchObject()).GetHashCode()));
            }

            [Test]
            public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasNoValueAndIsOfDifferentValueType()
            {
                Assert.IsFalse(_sut.GetHashCode().
                                    Equals(Optional<MismatchObject>.Empty.GetHashCode()));
            }
        }

        [TestFixture]
        public class WithFilledReferenceTypeInstance : WithFilledInstance<Object>
        {
            protected override object ValueFactory()
            {
                return new object();
            }

            protected override Optional<object> InstanceFactory(object value)
            {
                return new Optional<object>(value);
            }
        }

        [TestFixture]
        public class WithFilledValueTypeInstance : WithFilledInstance<Int32>
        {
            protected override int ValueFactory()
            {
                return new Randomizer(Randomizer.RandomSeed).
                    GetInts(Int32.MinValue, Int32.MaxValue, 1)[0];
            }

            protected override Optional<int> InstanceFactory(int value)
            {
                return new Optional<int>(value);
            }
        }

        [TestFixture]
        public abstract class WithFilledInstance<T>
        {
            private Optional<T> _sut;
            private T _value;

            [SetUp]
            public void SetUp()
            {
                _value = ValueFactory();
                _sut = InstanceFactory(_value);
            }

            protected abstract T ValueFactory();
            protected abstract Optional<T> InstanceFactory(T value);

            [Test]
            public void HasValueReturnsTrue()
            {
                Assert.That(_sut.HasValue, Is.True);
            }

            [Test]
            public void ValueReturnsInitializationValue()
            {
                Assert.That(_sut.Value, Is.EqualTo(_value));
            }

            [Test]
            public void TypedEnumerationReturnsInitializationValue()
            {
                Assert.That(_sut, Is.EquivalentTo(new[] {_value}));
            }

            [Test]
            public void ObjectEnumerationReturnsInitializationValue()
            {
                var sut = (IEnumerable) _sut;

                Assert.That(sut, Is.EquivalentTo(new object[] {_value}));
            }

            [Test]
            public void DoesNotEqualNull()
            {
                Assert.IsFalse(_sut.Equals(null));
            }

            [Test]
            public void DoesNotObjectEqualNull()
            {
                Assert.IsFalse(_sut.Equals((object) null));
            }

            [Test]
            public void DoesNotEqualObjectOfOtherType()
            {
                Assert.IsFalse(_sut.Equals(new object()));
            }

            [Test]
            public void DoesEqualItself()
            {
                Assert.IsTrue(_sut.Equals(_sut));
            }

            [Test]
            public void DoesObjectEqualItself()
            {
                Assert.IsTrue(_sut.Equals((object) _sut));
            }

            [Test]
            public void TwoInstancesAreEqualIfTheyBothHaveTheSameValue()
            {
                Assert.IsTrue(_sut.Equals(InstanceFactory(_value)));
            }

            [Test]
            public void TwoInstancesAreNotEqualIfTheOtherHasNoValue()
            {
                Assert.IsFalse(_sut.Equals(Optional<T>.Empty));
            }

            [Test]
            public void TwoInstancesAreNotEqualIfTheOtherHasDifferentValue()
            {
                Assert.IsFalse(_sut.Equals(InstanceFactory(ValueFactory())));
            }

            [Test]
            public void TwoInstancesAreNotEqualIfTheOtherHasNoValueAndIsOfDifferentType()
            {
                Assert.IsFalse(_sut.Equals(Optional<MismatchObject>.Empty));
            }

            [Test]
            public void TwoInstancesAreNotEqualIfTheOtherHasDifferentValueAndIsOfDifferentType()
            {
                Assert.IsFalse(_sut.Equals(new Optional<MismatchObject>(new MismatchObject())));
            }

            [Test]
            public void TwoInstancesHaveTheSameHashCodeIfTheyBothHaveTheSameValue()
            {
                Assert.IsTrue(_sut.GetHashCode().Equals(InstanceFactory(_value).GetHashCode()));
            }

            [Test]
            public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasNoValue()
            {
                Assert.IsFalse(_sut.GetHashCode().Equals(Optional<T>.Empty.GetHashCode()));
            }

            [Test]
            public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasDifferentValue()
            {
                Assert.IsFalse(_sut.GetHashCode().Equals(InstanceFactory(ValueFactory()).GetHashCode()));
            }

            [Test]
            public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasNoValueAndIsOfDifferentValueType()
            {
                Assert.IsFalse(_sut.GetHashCode().Equals(Optional<MismatchObject>.Empty.GetHashCode()));
            }

            [Test]
            public void TwoInstancesDoNotHaveTheSameHashCodeIfTheOtherHasDifferentValueAndIsOfDifferentValueType()
            {
                Assert.IsFalse(
                    _sut.GetHashCode().Equals(new Optional<MismatchObject>(new MismatchObject()).GetHashCode()));
            }
        }

        internal class MismatchObject
        {
        }
    }
}