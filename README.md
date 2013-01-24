AggregateSource
===============

This library provides lightweight infrastructure for doing eventsourcing using aggregates. It's not a framework, and it never will be. Period.

It gives you a base class for an aggregate root entity that does the usual initialization and change tracking that people have come accustomed to when doing eventsourcing in a domain model. It's a bit opnionated, in that it doesn't force any interfaces down your throat like IEvent, it leaves "identifier" management up to you, and it leaves "versioning" up to you.

There's a Repository<T> that has a Get that throws when an aggregate was not found, a TryGet to attempt to read an aggregate when you're not sure it's there (yet), and Add, well to add an aggregate to the change tracking (i.e. the Unit of Work). The idea is that one wires up an aggregate reader when constructing a repository instance. The contract is simple, null when not found, an instance when found. Incidentally it plays nicely with the UnitOfWork.

Oddly enough the UnitOfWork does not commit/save/persist/yournameforithere. Its role is reduced to tracking multiple aggregates and to hand you back those that have been changed. What you do with those changed aggregates, well, that's your business. Each aggregate retains the meta data it was read with as well as a materialized instance of the aggregate root entity. Both added and read aggregates are attached to the unit of work for you.

It's well suited for those scenarios where multiple aggregates need to collaborate and is lenient for saving multiple aggregates in one go should your underlying store allow you to do so. Of course, nothing is holding you back to throw when multiple aggregates have been changed. I just think this shouldn't interfere with the programming model you use.