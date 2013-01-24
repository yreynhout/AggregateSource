AggregateSource
===============

This library provides lightweight infrastructure for doing eventsourcing using aggregates. It's not a framework, and it never will be. Period.

It gives you a base class for an aggregate root entity (abstract AggregateRootEntity class) that does the usual initialization and change tracking that people have come accustomed to when doing eventsourcing in a domain model. It's a bit opnionated, in that it doesn't force any interfaces down your throat like IEvent, it leaves "identifier" management up to you, and it leaves "versioning" up to you. Do let your domain objects derive from it. It's meant to be used in your domain code.

It has a separate concept for an aggregate (Aggregate class) that is mainly used as a wrapper around the aggregate root entity. Don't use it in your domain code. It's meant for infrastructure purposes. It carries around meta data (if you create a custom type that is) that your preferred eventstore might fancy between reads and writes.

There's a Repository<T> that has a Get that throws when an aggregate was not found, a TryGet to attempt to read an aggregate when you're not sure it's there (yet), and Add, well to add an aggregate to the change tracking (i.e. the Unit of Work). This is a point of integration with an event store. Do derive from this abstract Repository, and fill in the holes (i.e. implement the template methods). This alllows you to:

* choose your own aggregate root construction strategy.
* implement a custom Aggregate (deriving from Aggregate) and track any meta data you want to carry over from the read to write handling.

Oddly enough the UnitOfWork does not commit/save/persist/yournameforithere. Its role is reduced to tracking multiple aggregates and to hand you back those that have been changed. What you do with those changed aggregates, well, that's your business. You can scope one using a UnitOfWorkScope. It just makes a block of code aware of the UnitOfWork in a ThreadStatic way. Repository integrates with this "ambient" unit of work when you do not provide an explicit one via the constructor. Both added and read aggregates are attached to the unit of work for you.

It's well suited for those scenarios where multiple aggregates need to collaborate and is lenient for saving multiple aggregates in one go should your underlying store allow you to do so. Of course, nothing is holding you back to throw when multiple aggregates have been changed. I just think this shouldn't interfere with the programming model you use. Granted, for affecting only one aggregate, there are simpler solutions. 
