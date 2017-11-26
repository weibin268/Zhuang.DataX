using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Zhuang.Data.BulkCopy;
using Zhuang.Data.Common;

namespace Zhuang.Data.DbProviders.SqlServer
{
    public class SqlServerBulkCopy : DbBulkCopy
    {
        public SqlServerBulkCopy(string connectionString) : base(connectionString)
        { }

        public override void WriteToServer(string destinationTableName, DbDataReader reader,
            int batchSize = 0, params BulkCopyColumnMapping[] columnMappings)
        {
            SqlBulkCopy bulkCopy = null;
            try
            {
                if (DbTransaction != null)
                {
                    bulkCopy = new SqlBulkCopy((SqlConnection)DbTransaction.Connection, SqlBulkCopyOptions.Default, (SqlTransaction)DbTransaction);
                }
                else
                {
                    bulkCopy = new SqlBulkCopy(_connectionString);
                }

                bulkCopy.DestinationTableName = destinationTableName;
                if (batchSize != 0)
                {
                    bulkCopy.BatchSize = batchSize;
                }
                if (BulkCopyTimeout != (int)CommandTimeoutValue.None)
                {
                    bulkCopy.BulkCopyTimeout = BulkCopyTimeout;
                }
                if (columnMappings != null)
                {
                    foreach (var item in columnMappings)
                    {
                        var mapping = new SqlBulkCopyColumnMapping();

                        if (item.DestinationColumn != null)
                            mapping.DestinationColumn = item.DestinationColumn;
                        if (item.DestinationOrdinal != -1)
                            mapping.DestinationOrdinal = item.DestinationOrdinal;
                        if (item.SourceColumn != null)
                            mapping.SourceColumn = item.SourceColumn;
                        if (item.SourceOrdinal != -1)
                            mapping.SourceOrdinal = item.SourceOrdinal;

                        bulkCopy.ColumnMappings.Add(mapping);
                    }
                }
                bulkCopy.WriteToServer(reader);
            }
            finally
            {
                if (bulkCopy != null) { bulkCopy.Close(); }
            }
        }
    }
}
