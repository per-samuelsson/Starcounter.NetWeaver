using Starcounter.Weaver;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedTestUtilities {

    public static class SharedTesting {
        static WeaverDiagnostics quietDiagnostics;

        public static WeaverDiagnostics QuietDiagnostics {
            // Use this. We don't now if we want to keep WeaverDiagnostics.Quiet.
            get {
                if (quietDiagnostics == null) {
                    quietDiagnostics = WeaverDiagnostics.Quiet;
                }
                return quietDiagnostics;
            }
        }
    }
}
