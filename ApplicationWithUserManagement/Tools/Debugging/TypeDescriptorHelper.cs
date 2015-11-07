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
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    internal static class TypeDescriptorHelper
    {
        public static ICustomTypeDescriptor Get(Type type)
        {
            return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
        }
    }
}
