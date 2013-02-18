AggregateSource
===============

This library/code provides lightweight infrastructure for doing eventsourcing using aggregates. It's not a framework, and it never will be. Period.

The preferred way of using it, is copying it into your project and getting rid of all the cruft you don't need.

It's well suited for those scenarios where multiple aggregates need to collaborate and is lenient to saving multiple aggregates in one go should your underlying store allow you to do so or your problem domain require you to do so. Of course, nothing is holding you back to throw when multiple aggregates have been changed. I just think this shouldn't interfere with the programming model you use. Granted, for affecting only one aggregate, there are simpler solutions and to be honest, what I bring you here is in no way unique:

* https://github.com/gregoryyoung/m-r
* https://github.com/joliver/CommonDomain
* https://github.com/Lokad/lokad-iddd-sample
* https://github.com/thinkbeforecoding/m-r
* https://github.com/elliotritchie/NES
* https://github.com/jhicks/EventSourcing
* https://github.com/tyronegroves/SimpleCQRS

## Core

### AggregateRootEntity
A base class for an aggregate root entity that does the usual initialization and change tracking that people have come accustomed to when doing eventsourcing in a domain model. It's a bit opnionated, in that it

* doesn't force any interfaces down your throat like IEvent, because a base class is enough coupling as it is,
* leaves "identifier" management up to you, because everyone has a different opinion on how this should work,
* leaves "versioning" up to you, because that's meta data that has little to do with the aggregate's behavior,
* leaves "entity" management up to you and I promise, I'll never do this for you.

It's meant to be used in your "domain model" code.

### Entity
A base class for entities (not the root obviously) within an aggregate that allows them to hook into the replay and recording functionality of the aggregate root. There's no magic here, it just alleviates you from writing boring, repetitive code.

### Aggregate

A separate concept for an aggregate that is mainly used as a wrapper around the aggregate root entity. It carries around version information that your preferred eventstore might fancy between reads and writes to perform optimistic concurrency updates. Don't use it in your "domain model" code. It's meant for infrastructure code.

### Repository
It has a Get that throws when an aggregate was not found, a TryGet to attempt to read an aggregate when you're not sure it's there (yet), and Add, well to add an aggregate to the change tracking (i.e. the Unit of Work). This is a point of integration with an event store and even dedicated read models that act as secondary indexes. Sometimes it makes sense to use this implementation which takes an aggregate root entity factory and a Func&lt;Guid, Tuple&lt;Int32, IEnumerable&lt;object&gt;&gt;&gt; as an eventstream reader (returns the version the aggregate is at and the events associated with it). It also takes a unit of work that acts both as an identity map and a change tracker. Other times a fresh new implementation of IRepository&lt;&gt; that integrates with your favorite event store makes more sense. Remember, repositories don't save, your code does (http://codebetter.com/iancooper/2011/04/12/repository-saveupdate-is-a-smell/).

### UnitOfWork
Oddly enough it does not commit/save/persist/yournameforithere. Its role is reduced to tracking multiple aggregates and to hand you back those that have changed. What you do with those changed aggregates, well, that's your business. Usually there's another point of integration with the event store, on the write side, that is interested in persisting the events in these changed aggregates. Use this to get them.

## Testing

Allows you to write command handler, application service, domain service level or even aggregate specific tests in the following format:

```csharp
Scenario.
  Given(RoleId,
    new AddedRole(RoleId, RoleName),
    new AddedPermissionToRole(RoleId, PermissionId),
    new RolePermissionDenied(RoleId, PermissionId)).
  When(new DenyRolePermission(RoleId, PermissionId)).
  AssertNothingHappened();

// or

Scenario.
  Given(RoleId,
    new AddedRole(RoleId, RoleName)).
  When(new DenyRolePermission(RoleId, UnknownPermissionId)).
  AssertThrows(new Exception("Yo bro, the permission is not known to me."));

// or

Scenario.
  Given(RoleId,
    new AddedRole(RoleId, RoleName)).
  When(new AddPermissionToRole(RoleId, PermissionId)).
  Then(RoleId,
    new AddedPermissionToRole(RoleId, PermissionId)).
  Assert();
```

Mind that the Assert* methods are **not** in the box. You have to write that kind of unit testing framework integration yourself. Looking for an example? Swing by NAuthorize (https://github.com/yreynhout/NAuthorize)
The principles discussed here are easily applicable to other frameworks and libraries. Feel free to steal ..
