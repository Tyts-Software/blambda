﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Tyts.Abstractions.Domain.Model;

namespace Tyts.Abstractions.Data
{
    /// <summary>
    /// Provides a standard base class for facilitating comparison of value objects using all the object's properties.
    /// 
    /// For a discussion of the implementation of Equals/GetHashCode, see 
    /// http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    /// and http://groups.google.com/group/sharp-architecture/browse_thread/thread/f76d1678e68e3ece?hl=en for 
    /// an in depth and conclusive resolution.
    /// </summary>
    [Serializable]
    public abstract class AUniqueObject : AObject, IUnique
    {
        /// <summary>
        /// The getter for SignatureProperties for value objects should include the properties 
        /// which make up the entirety of the object's properties; that's part of the definition 
        /// of a value object.
        /// </summary>
        /// <remarks>
        /// This ensures that the value object has no properties decorated with the 
        /// [DomainSignature] attribute.
        /// </remarks>
        protected override IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties() 
        {
            var t = GetType();
#if DEBUG
            foreach (var p in t.GetProperties())
            {
                Check.Require(Attribute.IsDefined(p, typeof(EntityKeyAttribute), true),
                        $@"Properties were found within {nameof(t)} having the
                        [DomainSignature] attribute. The domain signature of a value object includes all
                        of the properties of the object by convention; consequently, adding [DomainSignature]
                        to the properties of a value object's properties is misleading and should be removed. 
                        Alternatively, you can inherit from Entity if that fits your needs better.");

                yield return p;
            }
#else
            return t.GetProperties();
#endif
        }
    }
}