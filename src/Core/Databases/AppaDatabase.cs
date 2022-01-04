using System;
using Appalachia.Data.Core.AccessLayer;
using Appalachia.Data.Core.Collections;
using Appalachia.Data.Core.Documents;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Profiling;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Appalachia.Data.Core.Databases
{
    [Serializable]
    public abstract class AppaDatabase<TDB> : AppaDatabaseBase, IDocumentAccess, ICollectionAccess
        where TDB : AppaDatabase<TDB>, new()
    {
        #region Fields and Autoproperties

        [NonSerialized, JsonIgnore]
        private DatabaseAccess _access;

        #endregion

        [ShowInInspector] public DatabaseAccess Access => _access;

        [ShowInInspector, JsonIgnore]
        public DatabaseStyle DatabaseStyle => Access?.DatabaseStyle ?? DatabaseStyle.None;

        [ShowInInspector, JsonIgnore]
        public DatabaseTechnology DatabaseTechnology => Access?.DatabaseTechnology ?? DatabaseTechnology.None;

        public void RegisterCollection<TD, TC>(ref TC collection)
            where TC : AppaCollection<TD, TC>
            where TD : AppaDocument<TD, TC>
        {
            using (_PRF_RegisterCollection.Auto())
            {
#if UNITY_EDITOR
                var documentType = typeof(TD);
                var collectionType = typeof(TC);

                if (collectionType.Name != (documentType.Name + "Collection"))
                {
                    Context.Log.Error("The collection name must match the document type name!");
                }
#endif

                if (collection == null)
                {
                    collection = Access.CreateCollection<TD, TC>();
                }

                _collections.Add(collection);
            }
        }

        public void Save()
        {
            using (_PRF_Save.Auto())
            {
                Access.SaveDatabase(this as TDB);
            }
        }

        /*public TDB CreateDatabase()
        {
            return _access.CreateDatabase<TDB>();
        }

        public TDB LoadDatabase()
        {
            return _access.LoadDatabase<TDB>();
        }*/

        public void SaveDatabase(TDB database)
        {
            _access.SaveDatabase(database);
        }

        protected static TDB InitializeDatabase(DatabaseAccess access)
        {
            using (_PRF_InitializeDatabase.Auto())
            {
                var instance = access.CreateDatabase<TDB>();

                instance._access = access;
                instance.InitializeSynchronous();

                return instance;
            }
        }

        #region ICollectionAccess Members

        public TC CreateCollection<TD, TC>()
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>
        {
            return _access.CreateCollection<TD, TC>();
        }

        public void SaveCollection<TD, TC>(TC collection)
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>
        {
            _access.SaveCollection<TD, TC>(collection);
        }

        #endregion

        #region IDocumentAccess Members

        public TD CreateDocument<TD, TC>()
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>
        {
            return _access.CreateDocument<TD, TC>();
        }

        public void SaveDocument<TD, TC>(TD document)
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>
        {
            _access.SaveDocument<TD, TC>(document);
        }

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(AppaDatabase<TDB>) + ".";

        private static readonly ProfilerMarker _PRF_RegisterCollection =
            new ProfilerMarker(_PRF_PFX + nameof(RegisterCollection));

        private static readonly ProfilerMarker _PRF_Save = new ProfilerMarker(_PRF_PFX + nameof(Save));

        private static readonly ProfilerMarker _PRF_InitializeDatabase =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeDatabase));

        #endregion
    }
}
