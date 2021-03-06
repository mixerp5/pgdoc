﻿/********************************************************************************
Copyright (C) Binod Nepal, Mix Open Foundation (http://mixof.org).

This file is part of MixERP.

MixERP is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

MixERP is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with MixERP.  If not, see <http://www.gnu.org/licenses/>.
***********************************************************************************/
using System.Collections.ObjectModel;
using System.Data;
using MixERP.Net.Common;
using MixERP.Net.Utilities.PgDoc.DBFactory;
using MixERP.Net.Utilities.PgDoc.Helpers;
using MixERP.Net.Utilities.PgDoc.Models;
using Npgsql;

namespace MixERP.Net.Utilities.PgDoc.Processors
{
    internal static class SchemaProcessor
    {
        internal static Collection<PGSchema> GetSchemas()
        {
            Collection<PGSchema> schemas = new Collection<PGSchema>();

            string sql = FileHelper.ReadSqlResource("schemas.sql");

            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                using (DataTable table = DbOperation.GetDataTable(command))
                {
                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            PGSchema schema = new PGSchema
                            {
                                Name = Conversion.TryCastString(row["namespace"]),
                                Owner = Conversion.TryCastString(row["owner"]),
                                Description = Conversion.TryCastString(row["description"])
                            };


                            schemas.Add(schema);
                        }
                    }
                }
            }

            return schemas;
        }

        internal static PGSchema GetSchema(string schemaName)
        {
            PGSchema schema = new PGSchema();

            string sql = FileHelper.ReadSqlResource("schema.sql");

            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@SchemaName", schemaName);
                using (DataTable table = DbOperation.GetDataTable(command))
                {
                    if (table.Rows.Count.Equals(1))
                    {
                        schema.Name = schemaName;
                        schema.Owner = Conversion.TryCastString(table.Rows[0]["owner"]);
                        schema.Description = Conversion.TryCastString(table.Rows[0]["description"]);
                    }
                }
            }

            schema.Tables = TableProcessor.GetTables(schemaName);
            schema.Views = ViewProcessor.GetViews(schemaName);
            schema.Functions = FunctionProcessor.GetFunctions(schemaName);
            schema.TriggerFunctions = TriggerFunctionProcessor.GetTriggerFunctions(schemaName);
            schema.MaterializedViews = MaterializedViewProcessor.GetMaterializedViews(schemaName);
            schema.Sequences = SequenceProcessor.GetSequences(schemaName);
            schema.Types = TypeProcessor.GetTypes(schemaName);

            return schema;
        }
    }
}