using System;
using System.Reflection;
using System.Collections.Generic;
using Tyts.Abstractions.Domain.Model;

namespace Tyts.Abstractions.Data
{
    [Serializable]
    public abstract class AEntity<TId> : AObject, IEntity<TId>
    {
        [EntityKey]
        public virtual TId Id { get; set; }

        /// <summary>
        /// Transient objects are not associated with an item already in storage.  For instance,
        /// a Customer is transient if its Id is 0.  It's virtual to allow NHibernate-backed 
        /// objects to be lazily loaded.
        /// 
        /// To transition an object from transient to persistent state, there are two
        ///choices. You can Save() it using the persistence manager, OR CREATE A REFERENCE TO IT from
        ///an already-persistent instance and take advantage of transitive persistence
        /// </summary>
        public virtual bool IsNew //IsTransient
        {
            get { return Equals(Id, default(TId)); }
        }

        /// <summary>
        /// The property getter for SignatureProperties should ONLY compare the properties which make up 
        /// the "domain signature" of the object.
        /// 
        /// If you choose NOT to override this method (which will be the most common scenario), 
        /// then you should decorate the appropriate property(s) with [DomainSignature] and they 
        /// will be compared automatically.  This is the preferred method of managing the domain
        /// signature of entity objects.
        /// </summary>
        protected override IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties()
        {
            foreach(var p in GetType().GetProperties())
            {
                if (Attribute.IsDefined(p, typeof(EntityKeyAttribute), true))
                {
                    yield return p;
                }
            }
        }
       
    }
}