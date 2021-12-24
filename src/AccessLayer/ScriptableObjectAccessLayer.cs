#if UNITY_EDITOR
using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Core.Aspects;
using Appalachia.Data.Core;
using Appalachia.Data.Core.AccessLayer;
using Appalachia.Data.Core.Configuration;
using Appalachia.Utility.Reflection.Extensions;
using Appalachia.Utility.Strings;
using Unity.Profiling;

namespace Appalachia.Data.AccessLayer
{
    public class ScriptableObjectAccessLayer : DatabaseAccess<DummyDisposable>, IDataAccess
    
    {
        public ScriptableObjectAccessLayer()
        {
        }

        protected internal ScriptableObjectAccessLayer(
            string dataStoragePath,
            DatabaseConfigurationSettings settings) : base(dataStoragePath, settings)
        {
        }

        public override DatabaseStyle DatabaseStyle => DatabaseStyle.Document;
        public override DatabaseTechnology DatabaseTechnology => DatabaseTechnology.ScriptableObject;
        protected override bool RequiresCustomTypeSerialization => false;

        public override TC CreateCollection<TD, TC>()
        {
            using (_PRF_CreateCollection.Auto())
            {
                var fileName = ZString.Format("{0}.asset", typeof(TC).GetSimpleReadableName());

                var subfolder = AppaPath.Combine(DataStorageDirectoryPath, "collections");

                var instance = AppalachiaObjectFactory.LoadExistingOrCreateNewAsset<TC>(fileName, subfolder);

                return instance;
            }
        }

        public override TDB CreateDatabase<TDB>()
        {
            using (_PRF_CreateDatabase.Auto())
            {
                var fileName = ZString.Format("{0}.asset", DataStorageFileNameWithoutExtension);

                var instance = AppalachiaObjectFactory.LoadExistingOrCreateNewAsset<TDB>(
                    fileName,
                    DataStorageDirectoryPath
                );

                return instance;
            }
        }

        public override TD CreateDocument<TD, TC>()
        {
            using (_PRF_CreateDocument.Auto())
            {
                var subfolder = AppaPath.Combine(DataStorageDirectoryPath, "documents");

                var existing = AppaDirectory.GetFiles(subfolder).Length;

                var fileName = ZString.Format("{0}-{1}.asset", typeof(TD).GetSimpleReadableName(), existing);

                var instance = AppalachiaObjectFactory.LoadExistingOrCreateNewAsset<TD>(fileName, subfolder);

                return instance;
            }
        }

        public override void Dispose()
        {
        }

        public override TDB LoadDatabase<TDB>()
        {
            using (_PRF_LoadDatabase.Auto())
            {
                return AssetDatabaseManager.LoadAssetAtPath<TDB>(DataStorageFilePath);
            }
        }

        public override void SaveCollection<TD, TC>(TC collection)
        {
            using (_PRF_SaveCollection.Auto())
            {
                foreach (var document in collection.BoxedDocuments)
                {
                    document.MarkAsModified();
                }

                collection.MarkAsModified();

                AssetDatabaseManager.SaveAssets();
            }
        }

        public override void SaveDatabase<TDB>(TDB database)
        {
            using (_PRF_Save.Auto())
            {
                foreach (var collection in database.Collections)
                {
                    foreach (var document in collection.BoxedDocuments)
                    {
                        document.MarkAsModified();
                    }

                    collection.MarkAsModified();
                }

                database.MarkAsModified();

                AssetDatabaseManager.SaveAssets();
            }
        }

        public override void SaveDocument<TD, TC>(TD document)
        {
            using (_PRF_SaveDocument.Auto())
            {
                document.MarkAsModified();

                AssetDatabaseManager.SaveAssets();
                AssetDatabaseManager.Refresh();
            }
        }

        protected override Func<string, DatabaseConfigurationSettings, DatabaseAccess> GetGenerator()
        {
            return (dataStoragePath, settings) => new ScriptableObjectAccessLayer(dataStoragePath, settings);
        }

        protected override void OnCreateDatabaseStorage()
        {
        }

        protected override void OnInitializeDatabaseStorage()
        {
        }

        protected override void PrepareConnection()
        {
        }

        #region Profiling

        private const string _PRF_PFX = nameof(ScriptableObjectAccessLayer) + ".";

        private static readonly ProfilerMarker _PRF_LoadDatabase =
            new ProfilerMarker(_PRF_PFX + nameof(LoadDatabase));

        private static readonly ProfilerMarker _PRF_SaveCollection =
            new ProfilerMarker(_PRF_PFX + nameof(SaveCollection));

        private static readonly ProfilerMarker _PRF_SaveDocument =
            new ProfilerMarker(_PRF_PFX + nameof(SaveDocument));

        private static readonly ProfilerMarker _PRF_CreateCollection =
            new ProfilerMarker(_PRF_PFX + nameof(CreateCollection));

        private static readonly ProfilerMarker _PRF_CreateDatabase =
            new ProfilerMarker(_PRF_PFX + nameof(CreateDatabase));

        private static readonly ProfilerMarker _PRF_CreateDocument =
            new ProfilerMarker(_PRF_PFX + nameof(CreateDocument));

        private static readonly ProfilerMarker
            _PRF_Save = new ProfilerMarker(_PRF_PFX + nameof(SaveDatabase));

        #endregion
    }
}

#endif