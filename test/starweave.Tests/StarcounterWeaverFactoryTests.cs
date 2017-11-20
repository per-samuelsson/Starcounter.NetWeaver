
using Starcounter.Weaver;
using Starcounter.Weaver.Rewriting;
using Starcounter2.Internal;
using Xunit;

namespace starweave.Weaver.Tests {

    public class StarcounterWeaverFactoryTests {
        
        [Fact]
        public void BadInputRenderMeaningfulErrors() {
            var f = new StarcounterWeaverFactory(new DatabaseTypeStateNames(), new DefaultCRUDMethodProvider()) as IWeaverFactory;
            Assert.NotNull(f);
        }
    }
}