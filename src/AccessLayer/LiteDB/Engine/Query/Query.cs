using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.Utility.Strings;

namespace LiteDB
{
    /// <summary>
    /// Represent full query options
    /// </summary>
    public partial class Query
    {
        public BsonExpression Select { get; set; } = BsonExpression.Root;

        public List<BsonExpression> Includes { get; } = new List<BsonExpression>();
        public List<BsonExpression> Where { get; } = new List<BsonExpression>();

        public BsonExpression OrderBy { get; set; } = null;
        public int Order { get; set; } = Query.Ascending;

        public BsonExpression GroupBy { get; set; } = null;
        public BsonExpression Having { get; set; } = null;

        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = int.MaxValue;
        public bool ForUpdate { get; set; } = false;

        public string Into { get; set; }
        public BsonAutoId IntoAutoId { get; set; } = BsonAutoId.ObjectId;

        public bool ExplainPlan { get; set; }

        /// <summary>
        /// [ EXPLAIN ]
        ///    SELECT {selectExpr}
        ///    [ INTO {newcollection|$function} [ : {autoId} ] ]
        ///    [ FROM {collection|$function} ]
        /// [ INCLUDE {pathExpr0} [, {pathExprN} ]
        ///   [ WHERE {filterExpr} ]
        ///   [ GROUP BY {groupByExpr} ]
        ///  [ HAVING {filterExpr} ]
        ///   [ ORDER BY {orderByExpr} [ ASC | DESC ] ]
        ///   [ LIMIT {number} ]
        ///  [ OFFSET {number} ]
        ///     [ FOR UPDATE ]
        /// </summary>
        public string ToSQL(string collection)
        {
            var sb = new StringBuilder();

            if (this.ExplainPlan)
            {
                sb.AppendLine("EXPLAIN");
            }

            sb.AppendLine(ZString.Format("SELECT {0}", Select.Source));

            if (this.Into != null)
            {
                sb.AppendLine(ZString.Format("INTO {0}:{1}", Into, IntoAutoId.ToString().ToLower()));
            }

            sb.AppendLine(ZString.Format("FROM {0}", collection));

            if (this.Includes.Count > 0)
            {
                sb.AppendLine(
                    ZString.Format("INCLUDE {0}", string.Join(", ", Includes.Select(x => x.Source)))
                );
            }

            if (this.Where.Count > 0)
            {
                sb.AppendLine(ZString.Format("WHERE {0}", string.Join(" AND ", Where.Select(x => x.Source))));
            }

            if (this.GroupBy != null)
            {
                sb.AppendLine(ZString.Format("GROUP BY {0}", GroupBy.Source));
            }

            if (this.Having != null)
            {
                sb.AppendLine(ZString.Format("HAVING {0}", Having.Source));
            }

            if (this.OrderBy != null)
            {
                sb.AppendLine(
                    ZString.Format("ORDER BY {0} {1}", OrderBy.Source, Order == Ascending ? "ASC" : "DESC")
                );
            }

            if (this.Limit != int.MaxValue)
            {
                sb.AppendLine(ZString.Format("LIMIT {0}", Limit));
            }

            if (this.Offset != 0)
            {
                sb.AppendLine(ZString.Format("OFFSET {0}", Offset));
            }

            if (this.ForUpdate)
            {
                sb.AppendLine($"FOR UPDATE");
            }

            return sb.ToString().Trim();
        }
    }
}
