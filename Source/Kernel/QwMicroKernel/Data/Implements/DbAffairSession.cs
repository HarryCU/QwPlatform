using System;
using System.Data;
using QwMicroKernel.Core;

namespace QwMicroKernel.Data.Implements
{
    public abstract class DbAffairSession : DbSession, IDbAffairSession
    {
        private readonly EventManager _eventMgr = EventManager.Create();
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        #region Event Keys
        private readonly object EVENT_ERROR = new object();
        private readonly object EVENT_COMPLUTE = new object();
        #endregion

        public event EventHandler<DbAffairEventArgs> Error
        {
            add { _eventMgr.Add(EVENT_ERROR, value); }
            remove { _eventMgr.Remove(EVENT_ERROR, value); }
        }

        public event EventHandler Complute
        {
            add { _eventMgr.Add(EVENT_COMPLUTE, value); }
            remove { _eventMgr.Remove(EVENT_COMPLUTE, value); }
        }

        protected DbAffairSession(string connectionString)
            : base(connectionString)
        {
            _connection = null;
            _transaction = null;
        }

        #region Event Triggers
        protected virtual void OnError(DbAffairEventArgs args)
        {
            var handler = _eventMgr.GetEventHandler<DbAffairEventArgs>(EVENT_ERROR);
            if (handler != null)
                handler(this, args);
        }
        protected virtual void OnComplute()
        {
            var handler = _eventMgr.GetEventHandler<EventArgs>(EVENT_COMPLUTE);
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion

        public void Begin()
        {
            Begin(IsolationLevel.ReadCommitted);
        }

        public void Begin(IsolationLevel level)
        {
            if (_connection == null)
            {
                _connection = CreateConnection(ConnectionString);
                _transaction = _connection.BeginTransaction(level);
            }
        }

        protected override T CommandActuator<T>(string sqlString, IDbDataParameter[] parameters, Func<IDbCommand, T> commandAction)
        {
            if (string.IsNullOrWhiteSpace(sqlString)) throw new ArgumentNullException("sql");

            using (IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sqlString;
                command.CommandType = CommandType.Text;
                command.Connection = _connection;
                command.Transaction = _transaction;
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
                _connection.Open();
                return commandAction(command);
            }
        }

        public void Commit()
        {
            if (_connection != null)
            {
                try
                {
                    _transaction.Commit();
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    OnError(new DbAffairEventArgs(ex));
                }
                finally
                {
                    _connection.Close();
                }
                _connection = null;
                _transaction = null;
                OnComplute();
            }
        }
    }
}
