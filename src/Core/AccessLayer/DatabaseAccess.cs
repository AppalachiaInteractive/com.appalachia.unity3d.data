using System;
using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Data.Core.Collections;
using Appalachia.Data.Core.Configuration;
using Appalachia.Data.Core.Databases;
using Appalachia.Data.Core.Documents;
using Appalachia.Utility.Reflection.Extensions;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using Unity.Profiling;

namespace Appalachia.Data.Core.AccessLayer
{
    [Serializable]
    public abstract class DatabaseAccess<TConnection> : DatabaseAccess
        where TConnection : IDisposable
    {
        protected internal DatabaseAccess(string dataStoragePath, DatabaseConfigurationSettings settings) :
            base(dataStoragePath, settings)
        {
        }

        protected DatabaseAccess()
        {
        }

        #region Fields and Autoproperties

        protected TConnection Connection { get; }

        #endregion
    }

    [Serializable]
    public abstract class DatabaseAccess : IDisposable, IDataAccess
    {
        protected DatabaseAccess()
        {
        }

        protected DatabaseAccess(string dataStorageFilePath, DatabaseConfigurationSettings settings)
        {
            DataStorageFilePath = dataStorageFilePath;
            DataStorageDirectoryPath = AppaPath.GetDirectoryName(DataStorageFilePath);
            DataStorageFileName = AppaPath.GetFileName(DataStorageFilePath);
            DataStorageFileExtension = AppaPath.GetExtension(DataStorageFilePath);
            DataStorageFileNameWithoutExtension = AppaPath.GetFileNameWithoutExtension(DataStorageFilePath);
            Settings = settings;
        }

        #region Static Fields and Autoproperties

        private static readonly ProfilerMarker _PRF_GetDatabaseAccess =
            new ProfilerMarker(_PRF_PFX + nameof(GetDatabaseAccess));

        protected static
            Dictionary<DatabaseTechnology, Func<string, DatabaseConfigurationSettings, DatabaseAccess>>
            _generators;

        #endregion

        #region Fields and Autoproperties

        public string DataStorageDirectoryPath { get; }
        public string DataStorageFileExtension { get; }
        public string DataStorageFileName { get; }

        public string DataStorageFileNameWithoutExtension { get; }
        public string DataStorageFilePath { get; }

        protected DatabaseConfigurationSettings Settings { get; private set; }

        #endregion

        [ShowInInspector] public abstract DatabaseStyle DatabaseStyle { get; }

        [ShowInInspector] public abstract DatabaseTechnology DatabaseTechnology { get; }

        [ShowInInspector] protected abstract bool RequiresCustomTypeSerialization { get; }

        public static DatabaseAccess GetDatabaseAccess(
            string dataStorageLocation,
            DatabaseConfigurationSettings settings)
        {
            using (_PRF_GetDatabaseAccess.Auto())
            {
                BuildRegistry();

                var dataStorageDirectory = AppaPath.GetDirectoryName(dataStorageLocation);

                if (!AppaDirectory.Exists(dataStorageDirectory))
                {
                    AppaDirectory.CreateDirectory(dataStorageDirectory);
                }

                var generator = _generators[settings.technology];
                var instance = generator(dataStorageLocation, settings);

                instance.Initialize();

                return instance;
            }
        }

        public void Backup()
        {
            using (_PRF_Backup.Auto())
            {
                var filePath = DataStorageFilePath;
                var extension = AppaPath.GetExtension(filePath);
                var fileNameWithoutExtension = AppaPath.GetFileNameWithoutExtension(filePath);
                var directory = AppaPath.GetDirectoryName(filePath);

                var backupDate = GetBackupDate();
                var backupFileName = ZString.Format(
                    "{0}.{1}{2}",
                    fileNameWithoutExtension,
                    backupDate,
                    extension
                );

                var backupPath = AppaPath.Combine(directory, backupFileName);

                AppaFile.Copy(filePath, backupPath);
            }
        }

        public void Initialize()
        {
            using (_PRF_Initialize.Auto())
            {
                if (RequiresCustomTypeSerialization)
                {
                    RegisterCustomTypeSerialization();
                }

                if (!AppaFile.Exists(DataStorageFilePath))
                {
                    CreateDatabaseStorage();
                }
                else
                {
                    InitializeDatabaseStorage();
                }

                PrepareConnection();
            }
        }

        protected abstract Func<string, DatabaseConfigurationSettings, DatabaseAccess> GetGenerator();

        protected abstract void OnCreateDatabaseStorage();

        protected abstract void OnInitializeDatabaseStorage();

        protected abstract void PrepareConnection();

        protected virtual void RegisterCustomTypeSerialization()
        {
        }

        protected static void BuildRegistry()
        {
            using (_PRF_BuildRegistry.Auto())
            {
                if ((_generators != null) && (_generators.Count > 0))
                {
                    return;
                }

                _generators =
                    new Dictionary<DatabaseTechnology,
                        Func<string, DatabaseConfigurationSettings, DatabaseAccess>>();

                var types = typeof(DatabaseAccess).GetAllConcreteInheritorsWithDefaultConstructors();

                foreach (var type in types)
                {
                    var instance = Activator.CreateInstance(type) as DatabaseAccess;

                    _generators.Add(instance.DatabaseTechnology, instance.GetGenerator());
                }
            }
        }

        protected string GetBackupDate()
        {
            using (_PRF_GetBackupDate.Auto())
            {
                var dateTime = DateTime.UtcNow;
                var date = dateTime.ToString("yyyyMMdd");
                var time = dateTime.ToString("hhmmssfffffff");

                var backupDate = ZString.Format("{0}T{1}", date, time);

                return backupDate;
            }
        }

        private void CreateDatabaseStorage()
        {
            using (_PRF_CreateDatabaseStorage.Auto())
            {
                OnCreateDatabaseStorage();
                Settings.Serialize(DataStorageFilePath);
            }
        }

        private void InitializeDatabaseStorage()
        {
            using (_PRF_InitializeDatabaseStorage.Auto())
            {
                Settings = DatabaseConfigurationSettings.Deserialize(DataStorageFilePath);
            }
        }

#if UNITY_EDITOR
        [Button]
        private void SelectStorage()
        {
            AssetDatabaseManager.SetSelection(DataStorageFilePath);
        }
#endif

        #region IDataAccess Members

        public abstract TC CreateCollection<TD, TC>()
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>;

        public abstract TDB CreateDatabase<TDB>()
            where TDB : AppaDatabase<TDB>, new();

        public abstract TD CreateDocument<TD, TC>()
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>;

        public abstract TDB LoadDatabase<TDB>()
            where TDB : AppaDatabase<TDB>, new();

        public abstract void SaveCollection<TD, TC>(TC collection)
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>;

        public abstract void SaveDatabase<TDB>(TDB database)
            where TDB : AppaDatabase<TDB>, new();

        public abstract void SaveDocument<TD, TC>(TD document)
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>;

        #endregion

        #region IDisposable Members

        public abstract void Dispose();

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(DatabaseAccess) + ".";

        private static readonly ProfilerMarker _PRF_CreateDatabaseStorage =
            new ProfilerMarker(_PRF_PFX + nameof(CreateDatabaseStorage));

        private static readonly ProfilerMarker _PRF_InitializeDatabaseStorage =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeDatabaseStorage));

        private static readonly ProfilerMarker _PRF_GetBackupDate =
            new ProfilerMarker(_PRF_PFX + nameof(GetBackupDate));

        private static readonly ProfilerMarker _PRF_Backup = new ProfilerMarker(_PRF_PFX + nameof(Backup));

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker _PRF_BuildRegistry =
            new ProfilerMarker(_PRF_PFX + nameof(BuildRegistry));

        #endregion
    }
}
