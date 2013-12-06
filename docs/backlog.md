# Non functional issues

- ~~Fix broken build~~
- ~~Add a license~~
- ~~Build file~~
- ~~Test build on TeamCity~~
- ~~NuSpec~~

# Functional issues

## To complete

- Extra tests that prove that an identifier can't be null (repository)
- Complete tests for test result
- Complete tests for aggregate scenario test specification infrastructure
  - Factory tests are missing
  - ThrowsFixture is missing for all
- Usage of indented text writer as the text writer for specifications and results

## Features

- ~~[DESIGN] Replace Guid by String as identifier (supersedes)~~
  - ~~[DESIGN] Support both Guid and String as identifier - using conditional compilation~~
  - ~~[DESIGN] Support both Guid and String as identifier - using yet another abstraction~~
- ~~[DESIGN] The asynchronous repository~~
- ~~[DESIGN] Snapshotting~~
- [DESIGN] StreamSource - Writing to sql
- [DESIGN] StreamSource - Reading from sql
- [DESIGN] StreamSource - Readonly resource oriented API using Nancy
- Extend SampleSource with an async sample.
- DRY up those repos, there's a SlicedEventStreamReader in them (bonus, we can test this thing separately).
- Build a realistic app on top of this API that shows off entities, aggregates, testing, bulk loading.

## Ideas

AggregateSource.Repositories

AggregateSource.CollectionRepositories.EventStore
AggregateSource.CollectionRepositories.NEventStore

AggregateSource.PersistenceRepositories.EventStore
AggregateSource.PersistenceRepositories.NEventStore
