
namespace Starcounter.Weaver {

    /// <summary>
    /// These names, or at least some of them, probably need to be
    /// accessible also to hosts. Hence, we define them separately in a
    /// neutral fashion.
    /// </summary>
    public class DatabaseTypeStateNames {

        public virtual string DbId => "<star>_dbId";

        public virtual string DbRef => "<star>_dbRef";

        public virtual string CreateHandle => "<star>_createHandle";

        public virtual string DeleteHandle => "<star>_deleteHandle";

        public virtual string GetPropertyHandleName(string propertyName) {
            return $"<star>_{propertyName}_propertyhandle";
        }
    }
}