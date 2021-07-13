using System;

namespace Tyts.Abstractions.Data
{
    /// <summary>
    /// Facilitates indicating which property(s) describe the unique signature of an 
    /// entity.  See Entity.GetTypeSpecificSignatureProperties() for when this is leveraged.
    /// </summary>
    [Serializable]
    public class EntityKeyAttribute : Attribute { }
}
