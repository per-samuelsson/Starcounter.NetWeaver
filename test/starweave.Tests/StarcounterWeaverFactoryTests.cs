
using Starcounter.Weaver;
using Starcounter2.Internal;
using Xunit;

namespace starweave.Weaver.Tests {

    public class StarcounterWeaverFactoryTests {
        
        [Fact]
        public void BadInputRenderMeaningfulErrors() {
            var f = new StarcounterWeaverFactory(new DatabaseTypeStateNames(), new DefaultDbCrudMethodProvider()) as IWeaverFactory;
            Assert.NotNull(f);
        }
    }
}