
using Starcounter.Weaver;
using starweave.Weaver;
using Xunit;

namespace starweave.Tests {

    public class StarcounterWeaverFactoryTests {
        
        [Fact]
        public void BadInputRenderMeaningfulErrors() {
            var f = new StarcounterWeaverFactory() as IWeaverFactory;
            Assert.NotNull(f);

            // TODO:
        }
    }
}