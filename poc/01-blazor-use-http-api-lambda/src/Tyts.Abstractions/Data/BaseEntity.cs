
namespace Tyts.Abstractions.Data
{
    public abstract partial class BaseEntity : BaseEntity<int>
    {
    }

    public abstract partial class BaseEntity<TId>
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public TId Id { get; set; }
    }
}
