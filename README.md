AggregateSource
===============

This library provides lightweight infrastructure for doing eventsourcing using aggregates. It's not a framework, and it never will be. Period.

The preferred way of using it, is copying it into your project and getting rid of all the cruft you don't need.

It's well suited for those scenarios where multiple aggregates need to collaborate and is lenient to saving multiple aggregates in one go should your underlying store allow you to do so or your problem domain require you to do so. Of course, nothing is holding you back to throw when multiple aggregates have been changed. I just think this shouldn't interfere with the programming model you use. Granted, for affecting only one aggregate, there are simpler solutions and to be honest, what I bring you here is in no way unique:

* https://github.com/gregoryyoung/m-r
* https://github.com/joliver/CommonDomain
* https://github.com/elliotritchie/NES
* https://github.com/jhicks/EventSourcing
* https://github.com/tyronegroves/SimpleCQRS

### AggregateRootEntity
A base class for an aggregate root entity that does the usual initialization and change tracking that people have come accustomed to when doing eventsourcing in a domain model. It's a bit opnionated, in that it

* doesn't force any interfaces down your throat like IEvent, because a base class is enough coupling as it is,
* leaves "identifier" management up to you, because everyone has a different opinion on how this should work,
* leaves "versioning" up to you, because that's meta data that has little to do with the aggregate's behavior,
* leaves "entity" management up to you and I promise, I'll never do this for you.

It's meant to be used in your "domain model" code.

### Aggregate ( * )

A separate concept for an aggregate that is mainly used as a wrapper around the aggregate root entity. It carries around meta data (if you create a custom type that is) that your preferred eventstore might fancy between reads and writes. Don't use it in your "domain model" code. It's meant for infrastructure code.

### Repository ( * )
It has a Get that throws when an aggregate was not found, a TryGet to attempt to read an aggregate when you're not sure it's there (yet), and Add, well to add an aggregate to the change tracking (i.e. the Unit of Work). This is a point of integration with an event store. Do derive from this abstract Repository, and fill in the holes (i.e. implement the template methods). This way you can choose your own aggregate root construction strategy, implement a custom Aggregate (deriving from Aggregate) and track any meta data you want to carry over from the read to write handling.

### UnitOfWork
Oddly enough it does not commit/save/persist/yournameforithere. Its role is reduced to tracking multiple aggregates and to hand you back those that have changed. What you do with those changed aggregates, well, that's your business. Usually there's another point of integration with the event store, on the write side, that is interested in persisting the events in these changed aggregates.

### Ambient ( * ) ( ** )
You can scope a UnitOfWork using a UnitOfWorkScope. It just makes a block of code aware of the UnitOfWork in a pluggable way, i.e. you get to pick between CallContext, HttpContext, ThreadStatic or - if you choose to implement IAmbientUnitOfWorkStore - a custom way of sharing state. It has its own special AmbientRepository that integrates with this "ambient" unit of work.

(*) subject to change
(**) dragons ahead
