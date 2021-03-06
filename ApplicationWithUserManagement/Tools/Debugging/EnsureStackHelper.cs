using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Web.Http;
using System.Web.Mvc.Properties;

namespace System.Web.Http
{
    internal class EnsureStackHelper
    {
        private static readonly Action _ensureStackAction = InitializeEnsureStackDelegate();
    
        internal static void EnsureStack()
        {
            if (_ensureStackAction != null)
            {
                _ensureStackAction();
            }
        }
    
        // Critical: can call into RuntimeHelpers.EnsureSufficientExecutionStack which is SecurityCritical in 4.0
        // Safe: RuntimeHelpers.EnsureSufficientExecutionStack is actually a safe method and was changed to be marked as safe in 4.5
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "If an exception is thrown, we should be under 4.0 partial trust.")]
        [SuppressMessage("Microsoft.Security", "CA2136:TransparencyAnnotationsShouldNotConflictFxCopRule",
            Justification = "Since System.Web.Mvc.dll is fully transparent, [SSC] has no effect, but this method's functionality is unaffected.")]
        [SecuritySafeCritical]
        private static Action InitializeEnsureStackDelegate()
        {
            try
            {
                // The following method will do a link demand, and because RuntimeHelpers.EnsureSufficientExecutionStack is marked 
                // SecurityCritical in 4.0 and moved to SecuritySafeCritical in 4.5. The following method will only fail in 4.0 partial trust
                Action ensureStackAction = (Action)Delegate.CreateDelegate(typeof(Action), typeof(RuntimeHelpers), "EnsureSufficientExecutionStack");
    
                // Invoke the method just to be sure
                ensureStackAction();
    
                return ensureStackAction;
            }
            catch
            {
                return null;
            }
        }
    }
}
