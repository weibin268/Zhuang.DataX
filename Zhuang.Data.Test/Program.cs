//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Zhuang.Data.Test.Models;

//namespace Zhuang.Data.Test
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {

//            var dba = DbAccessor.Get();

//            SysProduct pro = new SysProduct();
//            pro.ProductName = "test1";
//            pro.ProductCode = "code1";

//            dba.Insert(pro);

//            var pro2 = dba.SelectList<SysProduct>(new { ProductName = "test1", ProductCode = "code1" }).FirstOrDefault();

//            Console.Read();
//        }
//    }
//}
