
namespace Starcounter.Weaver.Rewriting {
    /// <summary>
    /// These names, or at least some of them, probably need to be
    /// accessible also to hosts. Hence, we define them separately in a
    /// neutral fashion.
    /// </summary>
    public static class DatabaseTypeStateNames {
        public const string DbId = "<star>_dbId";
        public const string DbRef = "<star>_dbRef";
        public const string CreateHandle = "<star>_createHandle";
        public const string DeleteHandle = "<star>_deleteHandle";

        public static string GetPropertyHandleName(string propertyName) {
            return $"<star>_{propertyName}_propertyhandle";
        }
    }
}