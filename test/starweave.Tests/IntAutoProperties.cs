
namespace starweave.Weaver.Tests {

    class IntAutoProperties {

        public int Int1 { get; set; }

        public int Int2 { get; }

        public int Int3 { get; } = 42;

        public int Int4 { get; set; } = 42;

        public int Int5 { get; private set; }

        public int Int6 { get; private set; } = 42;
    }
}
