using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Appalachia.Data.Core.Contracts
{
    public interface IAppaDatabase : IDisposable
    {
        DatabaseType Type { get; }

        [ShowInInspector, BoxGroup(nameof(Collections))]
        public IReadOnlyList<IAppaCollection> Collections { get; }
    }
}
