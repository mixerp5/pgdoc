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
using System.Collections.Generic;
using System.Text;
using MixERP.Net.Utilities.PgDoc.Helpers;
using MixERP.Net.Utilities.PgDoc.Models;

namespace MixERP.Net.Utilities.PgDoc.Parsers
{
    internal static class TableParser
    {
        internal static string Parse(string content, List<string> matches, ICollection<PgTable> tables)
        {
            foreach (string match in matches)
            {
                string comment = HtmlHelper.RemoveComment(match);

                if (!comment.StartsWith("Tables"))
                {
                    continue;
                }
                
                comment = comment.ReplaceFirst("Tables", "");

                StringBuilder items = new StringBuilder();

                foreach (PgTable table in tables)
                {
                    items.Append(comment.Replace("[Name]", table.Name)
                        .Replace("[RowNumber]", table.RowNumber.ToString())
                        .Replace("[TableSchema]", table.SchemaName)
                        .Replace("[Owner]", table.Owner)
                        .Replace("[Tablespace]", table.Tablespace)
                        .Replace("[Description]", table.Description));
                }

                content = content.Replace(match, items.ToString());
            }

            return content;
        }
    }
}