namespace Starcounter.Weaver.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xunit;
    using Mono.Cecil;

    public class Tests
    {
        [Fact]
        public void CurrentAssemblyCouldBeRead()
        {
          var thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
          var module = ModuleDefinition.ReadModule(thisAssemblyPath);
        }
    }
}