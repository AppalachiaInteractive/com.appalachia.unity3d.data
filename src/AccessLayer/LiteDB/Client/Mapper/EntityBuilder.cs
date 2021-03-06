using System;
using System.Linq;
using System.Linq.Expressions;
using Appalachia.Utility.Strings;

namespace LiteDB
{
    /// <summary>
    ///     Helper class to modify your entity mapping to document. Can be used instead attribute decorates
    /// </summary>
    public class EntityBuilder<T>
    {
        internal EntityBuilder(BsonMapper mapper, ITypeNameBinder typeNameBinder)
        {
            _mapper = mapper;
            _typeNameBinder = typeNameBinder;
            _entity = mapper.GetEntityMapper(typeof(T));
        }

        #region Fields and Autoproperties

        private BsonMapper _mapper;
        private EntityMapper _entity;
        private ITypeNameBinder _typeNameBinder;

        #endregion

        /// <summary>
        ///     Define which property is your document id (primary key). Define if this property supports auto-id
        /// </summary>
        public EntityBuilder<T> Ctor(Func<BsonDocument, T> createInstance)
        {
            _entity.CreateInstance = v => createInstance(v);

            return this;
        }

        /// <summary>
        ///     Define a subdocument (or a list of) as a reference
        /// </summary>
        public EntityBuilder<T> DbRef<K>(Expression<Func<T, K>> member, string collection = null)
        {
            return GetMember(
                member,
                p =>
                {
                    BsonMapper.RegisterDbRef(
                        _mapper,
                        p,
                        _typeNameBinder,
                        collection ?? _mapper.ResolveCollectionName(typeof(K))
                    );
                }
            );
        }

        /// <summary>
        ///     Define a custom name for a property when mapping to document
        /// </summary>
        public EntityBuilder<T> Field<K>(Expression<Func<T, K>> member, string field)
        {
            if (field.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(field));
            }

            return GetMember(member, p => { p.FieldName = field; });
        }

        /// <summary>
        ///     Define which property is your document id (primary key). Define if this property supports auto-id
        /// </summary>
        public EntityBuilder<T> Id<K>(Expression<Func<T, K>> member, bool autoId = true)
        {
            return GetMember(
                member,
                p =>
                {
                    // if contains another _id, remove-it
                    var oldId = _entity.Members.FirstOrDefault(x => x.FieldName == "_id");

                    if (oldId != null)
                    {
                        oldId.FieldName = _mapper.ResolveFieldName(oldId.MemberName);
                        oldId.AutoId = false;
                    }

                    p.FieldName = "_id";
                    p.AutoId = autoId;
                }
            );
        }

        /// <summary>
        ///     Define which property will not be mapped to document
        /// </summary>
        public EntityBuilder<T> Ignore<K>(Expression<Func<T, K>> member)
        {
            return GetMember(member, p => { _entity.Members.Remove(p); });
        }

        /// <summary>
        ///     Get a property based on a expression. Eg.: 'x => x.UserId' return string "UserId"
        /// </summary>
        private EntityBuilder<T> GetMember<TK, K>(Expression<Func<TK, K>> member, Action<MemberMapper> action)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            var memb = _entity.GetMember(member);

            if (memb == null)
            {
                throw new ArgumentNullException(
                    ZString.Format(
                        "Member '{0}' not found in type '{1}' (use IncludeFields in BsonMapper)",
                        member.GetPath(),
                        _entity.ForType.Name
                    )
                );
            }

            action(memb);

            return this;
        }
    }
}
