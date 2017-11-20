
Each test project is a weaver target (i.e. user defined assembly) with some specific trait:

1. ClassLibraryNetFramework - standard C# library, targeting NET Framework. Contains a NuGet
reference to Ninject and a project reference to Starcounter2. Defines a single database class.

2. ClassLibraryNetStandard - same as ClassLibraryNetFramework but target .NET Standard 1.6.

3. ConsoleAppNetFramework - C# console application, targeting NET Framework. Has a single reference
to a NuGet package (Ninject). No reference to Starcounter.

4. ConsoleAppNetCore - same as ConsoleAppNetFramework, but target .NET Core 2.0.

5. ConsoleAppNetFrameworkIndirectReference - C# console application, targeting NET Framework. Has a
single reference to fellow project ClassLibraryNetFramework. Does NOT define any database class,
but should weave and include schema, because consuming database classes in ClassLibraryNetFramework.

6. ConsoleAppNetCoreIndirectReference - same as ConsoleAppNetFrameworkIndirectReference, but target
.NET Core 2.0.

7. ConsoleAppNetFrameworkSingleDbProperty - .NET Framework console application with a single database
class and a single property. Reference Starcounter2.

8. ConsoleAppNetCoreSingleDbProperty - same as ConsoleAppNetFrameworkSingleDbProperty for .NET Core 2.0.