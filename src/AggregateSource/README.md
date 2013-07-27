AggregateSource Core
====================

# IAggregateChangeTracker
An interface that provides the contract for getting the tracked changes of the aggregate. Typically, commands will induce one or more events. It's these events - applied on the aggregate root entity - that will end up as a tracked change.

# IAggregateInitializer
An interface that provides the contract for performing aggregate initialization. Typically this involves *replaying* an enumeration of events onto the aggregate root entity which reconstructs the aggregate's state, including the root, any value objects and/or entities, however deep you may choose them to be.

# IAggregateRootEntity
An interface that aggregates ```IAggregateInitializer``` and ```IAggregateChangeTracker``` interfaces. If you have the balls, you can provide your own implementation. Especially if don't like the need to explicitly register apply methods/callbacks, and would like e.g. a more convention based way of calling apply methods.

# AggregateRootEntity
A base class for an aggregate root entity that does the usual initialization and change tracking that people have come accustomed to when doing eventsourcing in a domain model. It's a bit opnionated, in that it

* doesn't force any interfaces down your throat like IEvent, because a base class is enough coupling as it is,
* leaves "identifier" management up to you, because let's face it, everyone has a different opinion on how this should work,
* leaves "versioning" up to you, because that's meta data that has little to do with the aggregate's behavior,
* leaves "entity" management up to you and I promise, I'll never do this for you (beyond the Entity class below, that is).

It's meant to be used in your "domain model" code. The base class has before/after apply hooks for those that want to support such things as [domain events](http://www.udidahan.com/2009/06/14/domain-events-salvation/) and [deferred validation](http://c2.com/ppr/checks.html).

### Entity
A base class for entities (not the root obviously) within an aggregate that allows them to hook into the replay and recording functionality of the aggregate root. There's no magic here, it just alleviates you from writing boring, repetitive code.

### Aggregate

A separate concept for an aggregate that is mainly used as a wrapper around the aggregate root entity. It carries around version information that your preferred eventstore might fancy between reads and writes to perform optimistic concurrency updates. Don't use it in your "domain model" code. It's meant for infrastructure code.

### IAsyncRepository/IRepository
It has a Get that throws when an aggregate was not found, a GetOptional to attempt to read an aggregate when you're not sure it's there (yet), and Add, well to add an aggregate to the change tracking (i.e. the Unit of Work). This is a point of integration with an event store and even dedicated read models that act as secondary indexes. Implementations should also takes a unit of work that acts both as an identity map and an *indirect* change tracker. AggregateSource repositories are collection oriented and therefor don't save. Your code does (http://codebetter.com/iancooper/2011/04/12/repository-saveupdate-is-a-smell/). The asynchronous variant recognizes the fact that these operations are inherently I/O bound (at least for the most part).

### Concurrent-/UnitOfWork
Oddly enough it does not commit/save/persist. Its role is reduced to tracking multiple aggregates and to hand you back those that have changed. What you do with those changed aggregates, well, that's your business. Usually there's another point of integration with the event store, on the write side, that is interested in persisting the events in these changed aggregates. Use this to get them. The concurrent variant is used in conjunction with the IAsyncRepository implementations.

### ISnapshotable
An interface that provides the contract for taking and restoring snapshots. It's an optional interface you can implement on your aggregate to make it play ball with a *Snapshotable* repository. Snapshots are an optimization that should be used wisely.