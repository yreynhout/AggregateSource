AggregateSource
===============

This library/code provides lightweight infrastructure for doing eventsourcing using aggregates. It's not a framework, and it never will be. Period.

The preferred way of using it, is copying it into your project and getting rid of all the cruft you don't need. That said, there are NuGet packages available for those of you that are pressed for time and don't mind following the prescribed recipe.

It's well suited for those scenarios where multiple aggregates need to collaborate and is lenient to saving multiple aggregates in one go should your underlying store allow you to do so or your problem domain require you to do so. Of course, nothing is holding you back from throwing when multiple aggregates have been changed. I just think this shouldn't interfere with the programming model you use. Granted, for affecting only one aggregate, there are simpler solutions and to be honest, what I bring you here is in no way unique:

* https://github.com/gregoryyoung/m-r
* https://github.com/joliver/CommonDomain
* https://github.com/Lokad/lokad-iddd-sample
* https://github.com/thinkbeforecoding/m-r
* https://github.com/elliotritchie/NES
* https://github.com/jhicks/EventSourcing
* https://github.com/tyronegroves/SimpleCQRS

## Core

Contains the core types that you will want interact with when building your domain model. A more thorough explanation can be found [here](src/AggregateSource/README.md)

## Testing

Helps you write test specifications, using a simple, codified statechart and a fluent syntax.  A more thorough explanation can be found [here](src/AggregateSource.Testing/README.md)

## License

Licensed using a BSD 3-Clause License. See [License.txt](LICENSE.txt) for more details

## Build

```RunRightOffThe.bat``` provides a sanity check. It's a combination of ```RunMeFirst.bat``` and ```RunBuild.bat```. Before working with the solution it's probably best to run the ```RunMeFirst.bat```, well, first. It restores NuGet packages and downloads and unzips a version of GetEventStore. ```RunBuild.bat``` and ```RunTest.bat``` should speak for themselves.

### Continuous integration

The [build][1] is generously hosted and run on the [CodeBetter TeamCity][2] infrastructure, courtesy of [JetBrains](http://www.jetbrains.com/).

|  | Status of last build |
| :------ | :------: |
| **master** | [![master][3]][4] |
 
 [1]: http://teamcity.codebetter.com/project.html?projectId=project328&guest=1
 [2]: http://codebetter.com/codebetter-ci/
 [3]: http://teamcity.codebetter.com/app/rest/builds/buildType:(id:bt977)/statusIcon
 [4]: http://teamcity.codebetter.com/viewType.html?buildTypeId=bt977&guest=1

![YouTrack and TeamCity](http://www.jetbrains.com/img/banners/Codebetter300x250.png)

## Contributors

* Yves Reynhout ([@yreynhout](https://github.com/yreynhout)): Maintainer
* Martijn Van den Broek ([@martijnvdbrk](https://github.com/martijnvdbrk)): ```Optional<T>``` as a struct
* James Nugent ([@jen20](https://github.com/jen20)): ```ConstructorScenarioFor<TAggregateRoot>```, GetEventStore integration
