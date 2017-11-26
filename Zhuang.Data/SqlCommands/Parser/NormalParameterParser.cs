﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Zhuang.Data.Common;
using Zhuang.Data.EnvironmentVariable;
using Zhuang.Data.Utility;

namespace Zhuang.Data.SqlCommands.Parser
{
    public class NormalParameterParser : ISqlCommandParser
    {


        public void Parse(SqlCommand sqlCommand)
        {
            string parameterNamePrefix = SqlUtil.PARAMETER_NAME_PREFIX_AT;

            if (sqlCommand.DbAccessor.DbProviderName == DbProviderName.Oracle)
            {
                parameterNamePrefix = SqlUtil.PARAMETER_NAME_PREFIX_COLON;
            }

            IList<Replacement> lsReplacement = new List<Replacement>();

            foreach (Match match in RegexPattern.CustomParameterPattern.Matches(sqlCommand.Text))
            {
                string namedParam = match.Groups["NormalParam"].Value.Trim();
                if (string.IsNullOrEmpty(namedParam)) continue;

                lsReplacement.Add(new Replacement()
                {
                    OldText = match.Value,
                    NewText = parameterNamePrefix + namedParam
                });
            }

            foreach (var replacement in lsReplacement)
            {
                sqlCommand.Text = sqlCommand.Text.Replace(replacement.OldText, replacement.NewText);
            }
        }
    }
}
