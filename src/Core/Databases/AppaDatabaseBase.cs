using System;
using System.Collections.Generic;
using Appalachia.Data.Core.Collections;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Profiling;

namespace Appalachia.Data.Core.Databases
{
    [Serializable]
    public abstract class AppaDatabaseBase : DataObject, IDisposable
    {
        #region Fields and Autoproperties

        [NonSerialized, JsonProperty]
        protected List<AppaCollectionBase> _collections;

        [NonSerialized, JsonIgnore]
        private bool _initialized;

        [NonSerialized, JsonIgnore]
        private bool _initializing;

        #endregion

        [ShowInInspector, JsonIgnore]
        public abstract DatabaseType Type { get; }

        [ShowInInspector, BoxGroup(nameof(Collections))]
        public IReadOnlyList<AppaCollectionBase> Collections => _collections;

        protected abstract void OnInitialize();

        protected virtual void Dispose(bool disposing)
        {
            using (_PRF_Dispose.Auto())
            {
                if (disposing)
                {
                }
            }
        }

        protected void InitializeInternal()
        {
            using (_PRF_Initialize.Auto())
            {
                if (_initializing || _initialized)
                {
                    return;
                }

                _initializing = true;
                _collections ??= new List<AppaCollectionBase>();

                OnInitialize();

                _initializing = false;
                _initialized = true;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            using (_PRF_Dispose.Auto())
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(AppaDatabaseBase) + ".";

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(InitializeInternal));

        private static readonly ProfilerMarker _PRF_Dispose = new ProfilerMarker(_PRF_PFX + nameof(Dispose));

        #endregion
    }
}
