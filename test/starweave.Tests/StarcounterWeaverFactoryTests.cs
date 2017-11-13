
using Starcounter.Weaver;
using Starcounter.Weaver.Rewriting;
using starweave.Weaver;
using Xunit;

namespace starweave.Tests {

    public class StarcounterWeaverFactoryTests {
        
        [Fact]
        public void BadInputRenderMeaningfulErrors() {
            var f = new StarcounterWeaverFactory(new DatabaseTypeStateNames()) as IWeaverFactory;
            Assert.NotNull(f);

            // TODO:
        }
    }
}