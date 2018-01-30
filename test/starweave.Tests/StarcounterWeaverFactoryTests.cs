
using Starcounter.Weaver;
using Starcounter.Weaver.Runtime;
using Xunit;

namespace starweave.Weaver.Tests {

    public class StarcounterWeaverFactoryTests {
        
        [Fact]
        public void BadInputRenderMeaningfulErrors() {
            var f = new StarcounterWeaverFactory("mock.dll", new DatabaseTypeStateNames()) as IWeaverFactory;
            Assert.NotNull(f);
        }
    }
}