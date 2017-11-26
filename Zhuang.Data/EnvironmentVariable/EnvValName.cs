﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Zhuang.Data.EnvironmentVariable
{
    public class EnvValName
    {
        private string _text;

        public const string SPLITTER = ":";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">格式如：datetime:yyyy-MM-dd</param>
        public EnvValName(string text)
        {
            _text = text;
        }

        public bool HasArgs()
        {
            if (_text.Contains(SPLITTER))
                return true;
            else
                return false;
        }

        public string GetName()
        {
            return _text.Split(new string[] { SPLITTER }, StringSplitOptions.None)[0];
        }

        public string GetArgs()
        {
            var arr = _text.Split(new string[] { SPLITTER }, StringSplitOptions.None);
            if (arr.Length > 1)
            {
                return arr[1];
            }
            else
            {
                return null;
            }
        }
    }
}
