Tailor [![Build status](https://ci.appveyor.com/api/projects/status/e3pxi66m30owoekw?svg=true)](https://ci.appveyor.com/project/andrewabest/tailor)
============
A set of opinionated Query abstractions and accompanying Convention Tests to make sure your Dapper queries measure up!

Tailor 1.x and up targets .NET Standard 2.0

<img src="https://raw.github.com/andrewabest/Tailor/master/suit.png" width="15%">

## To install from NuGet

    Install-Package Tailor

And then in your test project...

    Install-Package Tailor.Test

## What is it?

A set of strongly typed query abstractions to encapsulate [Dapper](https://github.com/StackExchange/Dapper) queries and their parameters, that also allow us to enforce the following Conventions via [Conventional](https://github.com/andrewabest/Conventional)

* Queries *must* execute
* Query parameters *must* match SQL parameters
* Queries *must not* perform select-star
* Query parameters *must* have either a public default constructor, or one protected default constructor with public non-default constructors

## Why?

It isn't compile-time safety, but test-time safety is the next best thing. Ensures queries are resiliant to refactoring and schema changes.

## Examples

Check out the [Samples](https://github.com/andrewabest/Tailor/tree/master/Tailor.Tests/Sample) to get started

## License

Licensed under the terms of the [MS-PL](https://opensource.org/licenses/MS-PL) license
