namespace Appalachia.Data.Core.Serializers
{
    public interface IDataSerializer<TIn, TOut>
    {
        public TOut Serialize(TIn value);
        public TIn Deserialize(TOut input);
    }
}
