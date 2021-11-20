using System.Collections.Generic;

namespace UltraLiteDB
{
	public class DataFields
	{
		private string Field;

		public DataFields(string field)
		{
			Field = field;
		}
		public IEnumerable<BsonValue> Execute(BsonDocument doc, bool includeNullIfEmpty = true)
		{
			var index = 0;
			BsonValue value=null;
			if(doc.TryGetValue(Field, out value))
			{
				index++;
				yield return value;
			}

			if(index == 0 && includeNullIfEmpty) yield return BsonValue.Null;
		}
	}
}