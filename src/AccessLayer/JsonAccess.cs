/*
using System;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Core.Aspects;
using Appalachia.Data.Core;
using Appalachia.Data.Core.AccessLayer;
using Appalachia.Data.Core.Collections;
using Appalachia.Data.Core.Configuration;
using Appalachia.Utility.Encryption;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Profiling;

namespace Appalachia.Data.AccessLayer.Json
{
    public class JsonAccess : DatabaseAccess<JsonAccess, DummyDisposable>
    {
        public JsonAccess()
        {
        }

        public JsonAccess(string fileStoragePath, DatabaseConfigurationSettings settings) : base(
            fileStoragePath,
            settings
        )
        {
        }

        #region Fields and Autoproperties

        /// <inheritdoc />
protected override bool RequiresCustomTypeSerialization { get; set; }

        #endregion

        /// <inheritdoc />
public override DatabaseStyle DatabaseStyle => DatabaseStyle.Object;

        /// <inheritdoc />
public override DatabaseTechnology DatabaseTechnology => DatabaseTechnology.Json;

        /// <inheritdoc />
public override void LoadCollection<TD, TC>(ref AppaCollection<TD, TC> collection)
        {
            using (_PRF_LoadCollection.Auto())
            {
                var collectionName = collection.CollectionName;

                var content = AppaFile.ReadAllText(DataStoragePath);

                if (Settings.isEncrypted)
                {
                    content = AppaCipher.Decrypt(content, Settings.key);
                }

                var jObject = JObject.Parse(content);

                var jProperty = jObject[collectionName];

                var subset = jProperty?.ToString();

                if (subset == null)
                {
                    return;
                }

                JsonConvert.PopulateObject(subset, collection);
            }
        }

        /// <inheritdoc />
public override void Save<T>(T database)
        {
            using (_PRF_Save.Auto())
            {
                var content = JsonConvert.SerializeObject(database);

                Backup();

                var dataStoragePath = DataStoragePath;

                if (Settings.isEncrypted)
                {
                    content = AppaCipher.Encrypt(content, Settings.key);
                }

                AppaFile.WriteAllText(dataStoragePath, content);
            }
        }

        /// <inheritdoc />
protected override Func<string, DatabaseConfigurationSettings, DatabaseAccess> GetGenerator()
        {
            return (dataStorageLocation, settings) => new JsonAccess(dataStorageLocation, settings);
        }

        /// <inheritdoc />
protected override void OnCreateDatabaseStorage()
        {
            using (_PRF_CreateDatabaseStorage.Auto())
            {
                var content = "{}";

                if (Settings.isEncrypted)
                {
                    content = AppaCipher.Encrypt(content, Settings.key);
                }

                AppaFile.WriteAllText(DataStoragePath, content);
#if UNITY_EDITOR
                var relativePath = DataStoragePath.ToRelativePath();
                AssetDatabaseManager.ImportAsset(relativePath);
#endif
            }
        }

        /// <inheritdoc />
protected override void OnInitializeDatabaseStorage()
        {
            using (_PRF_OnInitializeDatabaseStorage.Auto())
            {
            }
        }

        /// <inheritdoc />
protected override void PrepareConnection()
        {
            using (_PRF_PrepareConnection.Auto())
            {
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(JsonAccess) + ".";

        private static readonly ProfilerMarker _PRF_OnInitializeDatabaseStorage =
            new ProfilerMarker(_PRF_PFX + nameof(OnInitializeDatabaseStorage));

        private static readonly ProfilerMarker _PRF_CreateDatabaseStorage =
            new ProfilerMarker(_PRF_PFX + nameof(OnCreateDatabaseStorage));

        private static readonly ProfilerMarker _PRF_LoadCollection =
            new ProfilerMarker(_PRF_PFX + nameof(LoadCollection));

        private static readonly ProfilerMarker _PRF_PrepareConnection =
            new ProfilerMarker(_PRF_PFX + nameof(PrepareConnection));

        private static readonly ProfilerMarker _PRF_Save = new ProfilerMarker(_PRF_PFX + nameof(Save));

        #endregion
    }
}
*/


