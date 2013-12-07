using System;
using System.Linq;
using AggregateSource;
using NUnit.Framework;

namespace SampleSource
{
    namespace UsingObjectInheritance
    {
        using Messaging;

        [TestFixture]
        public class SampleUsage
        {
            [Test]
            public void Renting_a_video_tape_emits_a_recent_enough_video_title()
            {
                var videoTitleId = new VideoTitleId(Guid.NewGuid());
                var title = VideoTitle.Register(videoTitleId, "Shaving Ryan's Privates");
                var videoTapeId = new VideoTapeId(Guid.NewGuid());
                var tape = title.ScanNewTape(videoTapeId, new BarCode("A432"));
                tape.ClearChanges();

                title.CorrectTitle("Saving Private Ryan");
                var videoStoreMemberId = new VideoStoreMemberId(Guid.NewGuid());
                tape.Rent(videoStoreMemberId, new RentalPeriod(DateTime.Today, DateTime.Today.AddDays(3)));

                Assert.That(tape.GetChanges(),
                            Is.EquivalentTo(new object[]
                            {
                                new RentedVideoTape(
                                                videoTapeId,
                                                videoTitleId,
                                                "Saving Private Ryan",
                                                videoStoreMemberId,
                                                DateTime.Today,
                                                DateTime.Today.AddDays(3))
                            }));
            }
        }

        public class VideoTitle : AggregateRootEntity, IVideoTitleProfile
        {
            public static VideoTitle Register(VideoTitleId videoTitleId, string title)
            {
                return new VideoTitle(new VideoTitleRegistered(videoTitleId, title));
            }

            VideoTitle(VideoTitleRegistered @event) : this()
            {
                ApplyChange(@event);
            }

            VideoTitle()
            {
                Register<VideoTitleRegistered>(When);
                Register<CorrectedVideoTitle>(When);
            }

            public VideoTape ScanNewTape(VideoTapeId tapeId, BarCode barCode)
            {
                return new VideoTape(
                    this,
                    new ScannedNewVideoTape(Id, tapeId, barCode));
            }

            void When(VideoTitleRegistered @event)
            {
                Id = new VideoTitleId(@event.Id);
                Title = @event.Title;
            }

            void When(CorrectedVideoTitle @event)
            {
                Title = @event.Title;
            }

            public VideoTitleId Id { get; private set; }

            public string Title { get; private set; }

            public void CorrectTitle(string correction)
            {
                ApplyChange(new CorrectedVideoTitle(Id, correction));
            }
        }

        public interface IVideoTitleProfile
        {
            VideoTitleId Id { get; }

            string Title { get; }
        }

        public class VideoTape : AggregateRootEntity
        {
            readonly IVideoTitleProfile _title;

            VideoTape(IVideoTitleProfile title)
            {
                if (title == null) throw new ArgumentNullException("title");
                _title = title;
                Register<ScannedNewVideoTape>(When);
            }

            internal VideoTape(IVideoTitleProfile title, ScannedNewVideoTape @event)
                : this(title)
            {
                ApplyChange(@event);
            }

            public void Rent(VideoStoreMemberId memberId, RentalPeriod period)
            {
                ApplyChange(
                    new RentedVideoTape(
                        Id, _title.Id, _title.Title,
                        memberId, period.FromDate, period.ToDate));
            }

            void When(ScannedNewVideoTape @event)
            {
                Id = new VideoTapeId(@event.VideoTapeId);
            }

            public VideoTapeId Id { get; private set; }
        }

        public class RentalPeriod
        {
            readonly DateTime _fromDate;
            readonly DateTime _toDate;

            public RentalPeriod(DateTime from, DateTime to)
            {
                if (from.Date < DateTime.Today)
                    throw new ArgumentOutOfRangeException("from");
                if (from.Date > to.Date)
                    throw new ArgumentException("The from date should be less than or equal to the to date.");
                _fromDate = from.Date;
                _toDate = to.Date;
            }

            public DateTime FromDate
            {
                get { return _fromDate; }
            }

            public DateTime ToDate
            {
                get { return _toDate; }
            }
        }

        public class BarCode
        {
            readonly string _value;

            public BarCode(string value)
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (value.Length != 4)
                    throw new ArgumentOutOfRangeException("value", "The value of a barcode should be 4 characters long.");
                if (value.Any(@char => !@char.IsHexSymbol()))
                    throw new ArgumentOutOfRangeException("value",
                                                          "The value of a barcode should consist of hexadecimal characters only.");
                _value = value;
            }

            protected bool Equals(BarCode other)
            {
                return string.Equals(_value, other._value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((BarCode) obj);
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }

            public static implicit operator string(BarCode code)
            {
                return code._value;
            }
        }

        public static class CharExtensions
        {
            static readonly char[] HexChars = new[]
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e',
                'f'
            };

            public static bool IsHexSymbol(this char @char)
            {
                return HexChars.Contains(@char);
            }
        }

        public struct VideoTapeId : IEquatable<VideoTapeId>
        {
            readonly Guid _value;

            public VideoTapeId(Guid value)
            {
                _value = value;
            }

            public bool Equals(VideoTapeId other)
            {
                return _value.Equals(other._value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is VideoTapeId && Equals((VideoTapeId) obj);
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }

            public static implicit operator Guid(VideoTapeId id)
            {
                return id._value;
            }
        }

        public struct VideoTitleId : IEquatable<VideoTitleId>
        {
            readonly Guid _value;

            public VideoTitleId(Guid value)
            {
                _value = value;
            }

            public bool Equals(VideoTitleId other)
            {
                return _value.Equals(other._value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is VideoTitleId && Equals((VideoTitleId) obj);
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }

            public static implicit operator Guid(VideoTitleId id)
            {
                return id._value;
            }
        }

        public struct VideoStoreMemberId : IEquatable<VideoStoreMemberId>
        {
            readonly Guid _value;

            public VideoStoreMemberId(Guid value)
            {
                _value = value;
            }

            public bool Equals(VideoStoreMemberId other)
            {
                return _value.Equals(other._value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is VideoStoreMemberId && Equals((VideoStoreMemberId) obj);
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }

            public static implicit operator Guid(VideoStoreMemberId id)
            {
                return id._value;
            }
        }

        namespace Messaging
        {
            public class VideoTitleRegistered
            {
                public readonly Guid Id;
                public readonly string Title;

                public VideoTitleRegistered(Guid id, string title)
                {
                    Id = id;
                    Title = title;
                }
            }

            public class ScannedNewVideoTape
            {
                public readonly Guid VideoTitleId;
                public readonly Guid VideoTapeId;
                public readonly string BarCode;

                public ScannedNewVideoTape(Guid videoTitleId, Guid videoTapeId, string barCode)
                {
                    VideoTitleId = videoTitleId;
                    VideoTapeId = videoTapeId;
                    BarCode = barCode;
                }
            }

            public class RentedVideoTape
            {
                public readonly Guid VideoTapeId;
                public readonly Guid VideoTitleId;
                public readonly string Title;
                public readonly Guid VideoStoreMemberId;
                public readonly DateTime FromDate;
                public readonly DateTime ToDate;

                public RentedVideoTape(Guid videoTapeId, Guid videoTitleId, string title, Guid videoStoreMemberId,
                                       DateTime fromDate, DateTime toDate)
                {
                    VideoTapeId = videoTapeId;
                    VideoTitleId = videoTitleId;
                    Title = title;
                    VideoStoreMemberId = videoStoreMemberId;
                    FromDate = fromDate;
                    ToDate = toDate;
                }

                protected bool Equals(RentedVideoTape other)
                {
                    return VideoTapeId.Equals(other.VideoTapeId) && string.Equals(Title, other.Title) &&
                           VideoTitleId.Equals(other.VideoTitleId) &&
                           VideoStoreMemberId.Equals(other.VideoStoreMemberId) && FromDate.Equals(other.FromDate) &&
                           ToDate.Equals(other.ToDate);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != GetType()) return false;
                    return Equals((RentedVideoTape) obj);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        var hashCode = VideoTapeId.GetHashCode();
                        hashCode = (hashCode*397) ^ (Title != null ? Title.GetHashCode() : 0);
                        hashCode = (hashCode*397) ^ VideoTitleId.GetHashCode();
                        hashCode = (hashCode*397) ^ VideoStoreMemberId.GetHashCode();
                        hashCode = (hashCode*397) ^ FromDate.GetHashCode();
                        hashCode = (hashCode*397) ^ ToDate.GetHashCode();
                        return hashCode;
                    }
                }
            }

            public class CorrectedVideoTitle
            {
                public readonly Guid Id;
                public readonly string Title;

                public CorrectedVideoTitle(Guid id, string title)
                {
                    Id = id;
                    Title = title;
                }
            }
        }
    }
}