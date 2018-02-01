
Each test project is a weaver target (i.e. user defined assembly) with some specific trait:

* ClassLibraryNetStandard - C# library, targeting .NET Standard 1.6. Contains a NuGet
reference to Ninject and a project reference to Starcounter.ReferenceRuntime. Defines a single
database class.

* ConsoleAppNetCore - C# console application, targeting .NET Core 2.0. Has a single reference
to a NuGet package (Ninject). No reference to any runtime.

* ConsoleAppNetCoreIndirectReference - C# console application, targeting .NET Core 2.0. Has a
single reference to fellow project ClassLibraryNetStandard. Does NOT define any database class,
but should weave and include schema, because consuming database classes in ClassLibraryStandard.

* ConsoleAppNetCoreSingleDbProperty - .NET Core 2.0 console application with a single database
class and a single property. Reference Starcounter.ReferenceRuntime.