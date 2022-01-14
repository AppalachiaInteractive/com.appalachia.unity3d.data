using Appalachia.Core.Objects.Root;

namespace Appalachia.Data.Core
{
    public interface IDataObject
    {
    }

    public class DataObject<T> : AppalachiaObject<T>, IDataObject
        where T : DataObject<T>
    {
    }
}
