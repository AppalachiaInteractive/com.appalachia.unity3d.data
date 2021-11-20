using Appalachia.Data.Core.Collections;
using Appalachia.Data.Core.Documents;

namespace Appalachia.Data.Core.AccessLayer
{
    public interface IDocumentAccess
    {
        TD CreateDocument<TD, TC>()
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>;

        void SaveDocument<TD, TC>(TD document)
            where TD : AppaDocument<TD, TC>
            where TC : AppaCollection<TD, TC>;
    }
}