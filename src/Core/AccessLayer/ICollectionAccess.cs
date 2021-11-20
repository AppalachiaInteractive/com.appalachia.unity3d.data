using Appalachia.Data.Core.Collections;
using Appalachia.Data.Core.Documents;

namespace Appalachia.Data.Core.AccessLayer
{
    public interface ICollectionAccess
    {
        TC CreateCollection<TD, TC>()
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>;

        void SaveCollection<TD, TC>(TC collection)
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>;
    }
}