using Tyts.Abstractions.Domain.Model;

namespace Tyts.Abstractions.Data
{
    /// <summary>
    /// Persistable Object
    /// </summary>
    public interface IEntity : IObject
    {
    }

    /// <summary>
    /// Persistable Object
    /// </summary>
    public interface IEntity<TId> : IEntity
    {
        [EntityKey]
        TId Id { get; }  
    }

    /// <summary>
    /// It is like an IEntity but with all props 'marked' as DomainSignature
    /// </summary>
    public interface IUnique : IObject
    {
    }
}