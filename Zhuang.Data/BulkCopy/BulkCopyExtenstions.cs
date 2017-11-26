using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Zhuang.Data.BulkCopy;

namespace Zhuang.Data
{
    public static class BulkCopyExtenstions
    {
        public static void BulkWriteToServer(this DbAccessor db,string tableName, DbDataReader reader, int batchSize = 0, params BulkCopyColumnMapping[] columnMappings)
        {
            DbBulkCopy dbBulkCopy = DbBulkCopyFactory.GetDbBulkCopy(db, db.ConnectionString);
            dbBulkCopy.BulkCopyTimeout = db.CommandTimeout;
            dbBulkCopy.DbTransaction = db.DbTransaction;
            dbBulkCopy.WriteToServer(tableName, reader, batchSize, columnMappings);
        }

    }
}
