using System.Collections.Generic;
using System.Linq;
using QwMicroKernel.Text;

namespace QwMicroKernel.Data
{
    public sealed class WhereClauseBuildResult
    {
        public string WhereClause { get; private set; }

        public IDictionary<string, object> ParameterValues { get; private set; }

        public WhereClauseBuildResult(string whereClause, IDictionary<string, object> parameterValues)
        {
            WhereClause = whereClause;
            ParameterValues = parameterValues;
        }

        public override string ToString()
        {
            using (StringBuilder sb = new StringBuilder())
            {
                sb.Append(WhereClause);
                sb.Append(System.Environment.NewLine);
                ParameterValues.ToList().ForEach(kvp =>
                {
                    sb.Append(string.Format("{0} = [{1}] (Type: {2})", kvp.Key, kvp.Value.ToString(), kvp.Value.GetType().FullName));
                    sb.Append(System.Environment.NewLine);
                });
                return sb.ToString();
            }
        }
    }
}
