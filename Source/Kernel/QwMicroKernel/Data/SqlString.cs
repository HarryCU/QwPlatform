using System;
using System.Collections.Generic;
using System.Data;
using QwMicroKernel.Text;

namespace QwMicroKernel.Data
{
    public sealed class SqlString : Disposer
    {
        private readonly StringBuilder _builder;
        private readonly IList<IDbDataParameter> _parameters;
        private readonly IList<string> _columns;

        public SqlString()
        {
            _builder = new StringBuilder(1024);
            _columns = new List<string>();
            _parameters = new List<IDbDataParameter>();
        }

        public SqlString Add(string block)
        {
            if (string.IsNullOrEmpty(block)) throw new ArgumentNullException("block");
            _builder.Append(block);
            return this;
        }

        public SqlString AddColumn(string colName)
        {
            if (string.IsNullOrWhiteSpace(colName)) throw new ArgumentNullException("colName");
            _columns.Add(colName);
            Add(colName);
            return this;
        }

        public bool HasColumn(string colName)
        {
            if (string.IsNullOrWhiteSpace(colName)) throw new ArgumentNullException("colName");
            return _columns.Contains(colName);
        }

        public SqlString AddParameter(IDbDataParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");
            _parameters.Add(parameter);
            return this;
        }

        public IList<IDbDataParameter> GetParameters()
        {
            return _parameters;
        }

        public override string ToString()
        {
            var sqlString = _builder.ToString();
            return sqlString;
        }

        protected override void Release()
        {
            _builder.Clear();
            _columns.Clear();
            _parameters.Clear();
        }
    }
}
