using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Mvc.Properties;

namespace System.Web.Mvc
{
    internal delegate bool TryGetValueDelegate(object dictionary, string key, out object value);
}
