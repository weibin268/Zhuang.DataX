using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Zhuang.Data.Entity.Mapping;
using Zhuang.Data.Entity.Sql;
using Zhuang.Data.Test.Models;

namespace Zhuang.Data.Test
{
    public class EntityTest
    {

        [Fact]
        public void TestDefaultSqlBuilder()
        {
            DefaultSqlBuilder builder = new DefaultSqlBuilder(new TableMapping(typeof(SysProduct)));

            Console.WriteLine(builder.BuildSelect());
        }

        [Fact]
        public void TestEntityExtenstions()
        {
            var proId = 112;

            DbAccessor dba = DbAccessor.Get();

            var pro = dba.Select<SysProduct>(proId);

            dba.Delete<SysProduct>(proId);

            pro = dba.Select<SysProduct>(new { ProductId= proId });


            var pors = dba.SelectList<SysProduct>(new { ProductName = "zwb" });


        }

    }
}
