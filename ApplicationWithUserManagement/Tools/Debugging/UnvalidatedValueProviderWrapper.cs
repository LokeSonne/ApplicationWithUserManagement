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
    public sealed class UnvalidatedValueProviderWrapper :
        System.Web.Mvc.IValueProvider, System.Web.Mvc.IUnvalidatedValueProvider
    {
        private readonly System.Web.Mvc.IValueProvider _backingProvider;
    
        public UnvalidatedValueProviderWrapper(System.Web.Mvc.IValueProvider backingProvider)
        {
            _backingProvider = backingProvider;
        }
    
        public System.Web.Mvc.ValueProviderResult GetValue(string key, bool skipValidation)
        {
            // 'skipValidation' isn't understood by the backing provider and can be ignored
            return GetValue(key);
        }
    
        public bool ContainsPrefix(string prefix)
        {
            return _backingProvider.ContainsPrefix(prefix);
        }
    
        public System.Web.Mvc.ValueProviderResult GetValue(string key)
        {
            return _backingProvider.GetValue(key);
        }
    }
}
