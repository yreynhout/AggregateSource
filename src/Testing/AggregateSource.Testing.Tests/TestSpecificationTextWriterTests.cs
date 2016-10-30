using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace AggregateSource.Testing
{
    [TestFixture]
    public class TestSpecificationTextWriterTests : TestSpecificationDataPointFixture
    {
        [Test]
        public void TextWriterCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TestSpecificationTextWriter(null));
        }

        [TestCaseSource("ExceptionCentricTestSpecificationCases")]
        public void WriteExceptionCentricTestSpecificationResultsInExpectedOutput(
            ExceptionCentricTestSpecification specification, string result)
        {
            using (var writer = new StringWriter())
            {
                var sut = new TestSpecificationTextWriter(writer);

                sut.Write(specification);

                Assert.That(writer.ToString(), Is.EqualTo(result));
            }
        }

        public static IEnumerable<TestCaseData> ExceptionCentricTestSpecificationCases
        {
            get
            {
                yield return new TestCaseData(
                    new ExceptionCentricTestSpecification(OneEvent, Message, Exception),
                    "Given\r\n  System.Object\r\nWhen\r\n  System.Object\r\nThrows\r\n  [Exception] Message\r\n"
                    );

                yield return new TestCaseData(
                    new ExceptionCentricTestSpecification(TwoEventsOfTheSameSource, Message, Exception),
                    "Given\r\n  System.Object,\r\n  System.Object\r\nWhen\r\n  System.Object\r\nThrows\r\n  [Exception] Message\r\n"
                    );

                yield return new TestCaseData(
                    new ExceptionCentricTestSpecification(NoEvents, Message, Exception),
                    "When\r\n  System.Object\r\nThrows\r\n  [Exception] Message\r\n"
                    );
            }
        }

        [TestCaseSource("EventCentricTestSpecificationCases")]
        public void WriteEventCentricTestSpecificationResultsInExpectedOutput(
            EventCentricTestSpecification specification, string result)
        {
            using (var writer = new StringWriter())
            {
                var sut = new TestSpecificationTextWriter(writer);

                sut.Write(specification);

                Assert.That(writer.ToString(), Is.EqualTo(result));
            }
        }

        public static IEnumerable<TestCaseData> EventCentricTestSpecificationCases
        {
            get
            {
                yield return new TestCaseData(
                    new EventCentricTestSpecification(NoEvents, Message, NoEvents),
                    "When\r\n  System.Object\r\nThen\r\n  nothing happened\r\n"
                    );

                yield return new TestCaseData(
                    new EventCentricTestSpecification(OneEvent, Message, NoEvents),
                    "Given\r\n  System.Object\r\nWhen\r\n  System.Object\r\nThen\r\n  nothing happened\r\n"
                    );

                yield return new TestCaseData(
                    new EventCentricTestSpecification(TwoEventsOfTheSameSource, Message, NoEvents),
                    "Given\r\n  System.Object,\r\n  System.Object\r\nWhen\r\n  System.Object\r\nThen\r\n  nothing happened\r\n"
                    );

                yield return new TestCaseData(
                    new EventCentricTestSpecification(NoEvents, Message, OneEvent),
                    "When\r\n  System.Object\r\nThen\r\n  System.Object\r\n"
                    );

                yield return new TestCaseData(
                    new EventCentricTestSpecification(NoEvents, Message, TwoEventsOfTheSameSource),
                    "When\r\n  System.Object\r\nThen\r\n  System.Object,\r\n  System.Object\r\n"
                    );

                yield return new TestCaseData(
                    new EventCentricTestSpecification(OneEvent, Message, OneEvent),
                    "Given\r\n  System.Object\r\nWhen\r\n  System.Object\r\nThen\r\n  System.Object\r\n"
                    );

                yield return new TestCaseData(
                    new EventCentricTestSpecification(TwoEventsOfTheSameSource, Message, TwoEventsOfTheSameSource),
                    "Given\r\n  System.Object,\r\n  System.Object\r\nWhen\r\n  System.Object\r\nThen\r\n  System.Object,\r\n  System.Object\r\n"
                    );
            }
        }
    }
}