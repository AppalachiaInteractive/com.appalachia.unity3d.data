using System;
using Appalachia.CI.Integration.Attributes;
using Appalachia.Data.Core.Attributes;
using Appalachia.Utility.Extensions;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Data.Core.Documents
{
    [DoNotReorderFields, Serializable]
    public abstract class AppaDocumentBase : DataObject
    {
        protected internal AppaDocumentBase()
        {
            using (_PRF_AppaDatabaseTypeBase.Auto())
            {
                var now = DateTime.UtcNow;

                DateCreated = now;
                DateUpdated = now;
                InitialVersion = PKG.VersionInt;
                CurrentVersion = PKG.VersionInt;

                SetDefaults();
            }
        }

        #region Fields and Autoproperties

        [SerializeField] private bool _autoPopulated;
        [SerializeField] private long _dateCreated;
        [SerializeField] private long _dateUpdated;
        [SerializeField] private int _currentVersion;
        [SerializeField] private int _initialVersion;
        [NonSerialized] private bool _isDirty;

        #endregion

        public bool RequiresMigration => CurrentVersion < PKG.VersionInt;

        public bool AutoPopulated
        {
            get => _autoPopulated;
            set => _autoPopulated = value;
        }

        [DataIgnore]
        public bool IsDirty
        {
            get => _isDirty;
            set => _isDirty = value;
        }

        public DateTime DateCreated
        {
            get => DateTime.FromBinary(_dateCreated);
            set => _dateCreated = value.ToBinary();
        }

        public DateTime DateUpdated
        {
            get => DateTime.FromBinary(_dateUpdated);
            set => _dateUpdated = value.ToBinary();
        }

        public int CurrentVersion
        {
            get => _currentVersion;
            set => _currentVersion = value;
        }

        public int InitialVersion
        {
            get => _initialVersion;
            set => _initialVersion = value;
        }

        public void MarkModified()
        {
            using (_PRF_MarkModified.Auto())
            {
                this.MarkAsModified();
                IsDirty = true;
                DateUpdated = DateTime.UtcNow;
            }
        }

        protected virtual void SetDefaults()
        {
        }

        #region Profiling

        private const string _PRF_PFX = nameof(AppaDocumentBase) + ".";

        private static readonly ProfilerMarker _PRF_AppaDatabaseTypeBase =
            new ProfilerMarker(_PRF_PFX + nameof(AppaDocumentBase));

        private static readonly ProfilerMarker
            _PRF_MarkModified = new ProfilerMarker(_PRF_PFX + nameof(MarkModified));

        #endregion
    }
}
