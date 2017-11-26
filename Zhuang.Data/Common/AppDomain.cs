//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyModel;
//using System.Reflection;

//namespace Zhuang.Data.Common
//{
//    public class AppDomain
//    {
//        public static AppDomain CurrentDomain { get; private set; }

//        static AppDomain()
//        {
//            CurrentDomain = new AppDomain();
//        }

//        public Assembly[] GetAssemblies()
//        {
//            var assemblies = new List<Assembly>();
//            foreach (var name in DependencyContext.Default.GetDefaultAssemblyNames())
//            {
//                var assembly = Assembly.Load(name);
//                assemblies.Add(assembly);
//            }
//            return assemblies.ToArray();
//        }
        
//    }
//}
