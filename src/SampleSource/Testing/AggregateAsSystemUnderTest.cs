using System;
using AggregateSource;
using AggregateSource.Testing;
using NUnit.Framework;

namespace SampleSource.Testing
{
    namespace AggregateAsSystemUnderTest
    {
        using Messaging;
        using Ticketing;

        [TestFixture]
        public class SampleUsage
        {
            public static readonly ConcertId ConcertId = new ConcertId(Guid.NewGuid());
            public static readonly TicketSaleId TicketSaleId = new TicketSaleId(Guid.NewGuid());

            [Test]
            public void ConcertIsPlannedWithIdAndValidCapacity()
            {
                var id = new ConcertId(Guid.NewGuid());

                new ConstructorScenarioFor<Concert>(() => Concert.Plan(id, 100)).
                    Then(ConcertEvents.Planned(id)).
                    Assert();
            }

            [Test]
            public void PlanningConcertWithInvalidCapacityThrows()
            {
                new ConstructorScenarioFor<Concert>(() => Concert.Plan(new ConcertId(Guid.NewGuid()), -4)).
                    Throws(new ArgumentException("venueCapacity")).
                    Assert();
            }

            [Test]
            public void PlannedConcertCanHaveTicketSaleStarted()
            {
                new FactoryScenarioFor<Concert>(Concert.Factory).
                    Given(
                        ConcertEvents.Planned(ConcertId)).
                    When(sut => sut.StartTicketSale(TicketSaleId, DateTimeOffset.UtcNow.Date)).
                    Then(
                        TicketSaleEvents.Started(TicketSaleId, ConcertId, DateTimeOffset.UtcNow.Date, 100)).
                    Assert();
            }

            [Test]
            public void CancelledConcertCanNotStartTicketSale()
            {
                new FactoryScenarioFor<Concert>(Concert.Factory).
                    Given(
                        ConcertEvents.Planned(ConcertId),
                        ConcertEvents.Cancelled(ConcertId, "Lead singer OD'ed.")).
                    When(sut => sut.StartTicketSale(TicketSaleId, DateTimeOffset.UtcNow.Date)).
                    Throws(
                        new InvalidOperationException("Starting a ticket sale for a cancelled concert is impossible.")).
                    Assert();
            }

            [Test]
            public void PlannedConcertCanBeCancelled()
            {
                new CommandScenarioFor<Concert>(Concert.Factory).
                    Given(
                        ConcertEvents.Planned(ConcertId)).
                    When(sut => sut.Cancel("Lead singer OD'ed.")).
                    Then(
                        ConcertEvents.Cancelled(ConcertId, "Lead singer OD'ed.")).
                    Assert();
            }

            [Test]
            public void CancelledConcertCanNotBeCancelled()
            {
                new CommandScenarioFor<Concert>(Concert.Factory).
                    Given(
                        ConcertEvents.Planned(ConcertId),
                        ConcertEvents.Cancelled(ConcertId, "Guitars all smashed.")).
                    When(sut => sut.Cancel("Lead singer OD'ed.")).
                    Throws(new InvalidOperationException("The concert has already been cancelled.")).
                    Assert();
            }

            [Test]
            public void TicketSaleGetAvailabilityReturnsExpectedSeatCount()
            {
                new QueryScenarioFor<TicketSale>(TicketSale.Factory).
                    Given(
                        TicketSaleEvents.Started(TicketSaleId, ConcertId, DateTimeOffset.UtcNow.Date, 100)).
                    When(sut => sut.GetAvailability()).
                    Then(new SeatCount(100)).
                    Assert();
            }

            [Test]
            public void TicketSaleGetAvailabilityThrowsWhenSaleHasEnded()
            {
                new QueryScenarioFor<TicketSale>(TicketSale.Factory).
                    Given(
                        TicketSaleEvents.Started(TicketSaleId, ConcertId, DateTimeOffset.UtcNow.Date, 100),
                        TicketSaleEvents.Ended(TicketSaleId, DateTimeOffset.UtcNow.Date)).
                    When(sut => sut.GetAvailability()).
                    Throws(new InvalidOperationException("The ticket sale has ended.")).
                    Assert();
            }
        }

        namespace Ticketing
        {
            using Messaging;

            public class Concert : AggregateRootEntity
            {
                ConcertId _id;
                bool _cancelled;

                public static readonly Func<Concert> Factory = () => new Concert();

                Concert()
                {
                    Register<ConcertPlannedEvent>(When);
                    Register<ConcertCancelledEvent>(When);
                }

                void When(ConcertPlannedEvent @event)
                {
                    _id = new ConcertId(@event.ConcertId);
                    _cancelled = false;
                }

                void When(ConcertCancelledEvent @event)
                {
                    _cancelled = true;
                }

                public TicketSale StartTicketSale(TicketSaleId ticketSaleId, DateTimeOffset date)
                {
                    if (_cancelled)
                        throw new InvalidOperationException(
                            "Starting a ticket sale for a cancelled concert is impossible.");
                    return new TicketSale(TicketSaleEvents.Started(ticketSaleId, _id, date, 100));
                }

                public void Cancel(string reason)
                {
                    if (_cancelled)
                        throw new InvalidOperationException("The concert has already been cancelled.");
                    ApplyChange(ConcertEvents.Cancelled(_id, reason));
                }

                public static Concert Plan(ConcertId id, int venueCapacity)
                {
                    if (venueCapacity < 1)
                        throw new ArgumentException("venueCapacity");

                    var concert = Factory();
                    concert.ApplyChange(ConcertEvents.Planned(id));
                    return concert;
                }
            }

            public class TicketSale : AggregateRootEntity
            {
                SeatCount _availableSeats;
                bool _ended;

                public static readonly Func<TicketSale> Factory = () => new TicketSale();

                TicketSale()
                {
                    Register<TicketSaleStartedEvent>(When);
                    Register<TicketSaleEndedEvent>(When);
                }

                void When(TicketSaleStartedEvent @event)
                {
                    _ended = false;
                    _availableSeats = new SeatCount(@event.SeatCount);
                }

                void When(TicketSaleEndedEvent @event)
                {
                    _ended = true;
                }

                internal TicketSale(TicketSaleStartedEvent @event)
                {
                    ApplyChange(@event);
                }

                public SeatCount GetAvailability()
                {
                    if (_ended)
                        throw new InvalidOperationException("The ticket sale has ended.");
                    return _availableSeats;
                }
            }

            public struct SeatCount : IEquatable<SeatCount>
            {
                public bool Equals(SeatCount other)
                {
                    return _value == other._value;
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    return obj is SeatCount && Equals((SeatCount)obj);
                }

                public override int GetHashCode()
                {
                    return _value;
                }

                public static bool operator ==(SeatCount left, SeatCount right)
                {
                    return left.Equals(right);
                }

                public static bool operator !=(SeatCount left, SeatCount right)
                {
                    return !left.Equals(right);
                }

                readonly int _value;

                public SeatCount(int value)
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException("value");
                    _value = value;
                }

                public static implicit operator Int32(SeatCount instance)
                {
                    return instance._value;
                }
            }

            public struct ConcertId : IEquatable<ConcertId>
            {
                readonly Guid _value;

                public ConcertId(Guid value)
                {
                    _value = value;
                }

                public bool Equals(ConcertId other)
                {
                    return _value.Equals(other._value);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    return obj is ConcertId && Equals((ConcertId)obj);
                }

                public override int GetHashCode()
                {
                    return _value.GetHashCode();
                }

                public static implicit operator Guid(ConcertId id)
                {
                    return id._value;
                }
            }

            public struct TicketSaleId : IEquatable<TicketSaleId>
            {
                readonly Guid _value;

                public TicketSaleId(Guid value)
                {
                    _value = value;
                }

                public bool Equals(TicketSaleId other)
                {
                    return _value.Equals(other._value);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    return obj is TicketSaleId && Equals((TicketSaleId)obj);
                }

                public override int GetHashCode()
                {
                    return _value.GetHashCode();
                }

                public static implicit operator Guid(TicketSaleId id)
                {
                    return id._value;
                }
            }
        }

        namespace Messaging
        {
            public static class ConcertEvents
            {
                public static ConcertPlannedEvent Planned(Guid concertId)
                {
                    return new ConcertPlannedEvent(concertId);
                }

                public static ConcertCancelledEvent Cancelled(Guid concertId, string reason)
                {
                    return new ConcertCancelledEvent(concertId, reason);
                }
            }

            public class ConcertPlannedEvent
            {
                protected bool Equals(ConcertPlannedEvent other)
                {
                    return ConcertId.Equals(other.ConcertId);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != GetType()) return false;
                    return Equals((ConcertPlannedEvent)obj);
                }

                public override int GetHashCode()
                {
                    return ConcertId.GetHashCode();
                }

                public readonly Guid ConcertId;

                public ConcertPlannedEvent(Guid concertId)
                {
                    ConcertId = concertId;
                }
            }

            public class ConcertCancelledEvent
            {
                protected bool Equals(ConcertCancelledEvent other)
                {
                    return ConcertId.Equals(other.ConcertId) && string.Equals(Reason, other.Reason);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != GetType()) return false;
                    return Equals((ConcertCancelledEvent)obj);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        return (ConcertId.GetHashCode() * 397) ^ Reason.GetHashCode();
                    }
                }

                public readonly Guid ConcertId;
                public readonly string Reason;

                public ConcertCancelledEvent(Guid concertId, string reason)
                {
                    ConcertId = concertId;
                    Reason = reason;
                }
            }

            public static class TicketSaleEvents
            {
                public static TicketSaleStartedEvent Started(Guid ticketSaleId, Guid concertId, DateTimeOffset date,
                                                             int seatCount)
                {
                    return new TicketSaleStartedEvent(ticketSaleId, concertId, date, seatCount);
                }

                public static TicketSaleEndedEvent Ended(Guid ticketSaleId, DateTimeOffset date)
                {
                    return new TicketSaleEndedEvent(ticketSaleId, date);
                }
            }

            public class TicketSaleStartedEvent
            {
                protected bool Equals(TicketSaleStartedEvent other)
                {
                    return TicketSaleId.Equals(other.TicketSaleId) && ConcertId.Equals(other.ConcertId) &&
                           Date.Equals(other.Date);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != GetType()) return false;
                    return Equals((TicketSaleStartedEvent)obj);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        var hashCode = TicketSaleId.GetHashCode();
                        hashCode = (hashCode * 397) ^ ConcertId.GetHashCode();
                        hashCode = (hashCode * 397) ^ Date.GetHashCode();
                        return hashCode;
                    }
                }

                public readonly Guid TicketSaleId;
                public readonly Guid ConcertId;
                public readonly DateTimeOffset Date;
                public readonly Int32 SeatCount;

                public TicketSaleStartedEvent(Guid ticketSaleId, Guid concertId, DateTimeOffset date, int seatCount)
                {
                    TicketSaleId = ticketSaleId;
                    ConcertId = concertId;
                    Date = date;
                    SeatCount = seatCount;
                }
            }

            public class TicketSaleEndedEvent
            {
                protected bool Equals(TicketSaleEndedEvent other)
                {
                    return TicketSaleId.Equals(other.TicketSaleId) && Date.Equals(other.Date);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != GetType()) return false;
                    return Equals((TicketSaleEndedEvent)obj);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        return (TicketSaleId.GetHashCode() * 397) ^ Date.GetHashCode();
                    }
                }

                public readonly Guid TicketSaleId;
                public readonly DateTimeOffset Date;

                public TicketSaleEndedEvent(Guid ticketSaleId, DateTimeOffset date)
                {
                    TicketSaleId = ticketSaleId;
                    Date = date;
                }
            }
        }
    }
}