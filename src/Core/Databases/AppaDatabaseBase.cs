using System;
using System.Collections.Generic;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Data.Core.Contracts;
using Appalachia.Utility.Async;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Profiling;

namespace Appalachia.Data.Core.Databases
{
    [Serializable]
    public abstract class AppaDatabaseBase<T> : DataObject<T>, IDisposable, IAppaDatabase
        where T : AppaDatabaseBase<T>
    {
        #region Fields and Autoproperties

        [NonSerialized, JsonProperty]
        protected List<IAppaCollection> _collections;

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            using (_PRF_Dispose.Auto())
            {
                if (disposing)
                {
                }
            }
        }

        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            using (_PRF_Initialize.Auto())
            {
                initializer.Do(
                    this,
                    nameof(_collections),
                    Initializer.TagState.NonSerialized,
                    () =>
                    {
                        using (_PRF_Initialize.Auto())
                        {
                            _collections = new List<IAppaCollection>();
                        }
                    }
                );
            }
        }

        #region IAppaDatabase Members

        [ShowInInspector, JsonIgnore]
        public abstract DatabaseType Type { get; }

        [ShowInInspector, BoxGroup(nameof(Collections))]
        public IReadOnlyList<IAppaCollection> Collections => _collections;

        #endregion

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

        private static readonly ProfilerMarker _PRF_Dispose = new ProfilerMarker(_PRF_PFX + nameof(Dispose));

        #endregion
    }
}
