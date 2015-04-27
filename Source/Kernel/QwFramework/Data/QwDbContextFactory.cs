using System;
using System.IO;
using QwFramework.Helper;
using QwMicroKernel;
using QwMicroKernel.Configuration;
using QwMicroKernel.Data;
using QwMicroKernel.Reflection;

namespace QwFramework.Data
{
    public sealed class QwDbContextFactory : IDbContextFactory
    {
        private readonly Type _contextType;
        private readonly string _connectString;

        public string ConnectionString
        {
            get { return _connectString; }
        }

        private QwDbContextFactory(Type contextType, string connectString)
        {
            Guard.AgainstNull(contextType, "contextType");

            _contextType = contextType;
            _connectString = connectString;
        }

        public IDbContext CreateContext()
        {
            return _contextType.New(_connectString) as IDbContext;
        }

        public static IDbContextFactory Build()
        {
            return Build("db.cfg");
        }

        public static IDbContextFactory Build(string configName)
        {
            Guard.AgainstNullOrEmptyString(configName, "configName");

            if (!File.Exists(configName))
                throw new FileNotFoundException("数据库配置文件不存在!", configName);

            var protperties = PropertiesParser.LoadFromFileResource(configName);

            var assemblyFileName = FileHelper.GetCurrent(protperties.GetString("db.assembly"));

            if (!File.Exists(assemblyFileName))
                throw new FileNotFoundException("数据库访问组件未能找到!", assemblyFileName);

            var connectString = protperties.GetString("db.connectString");

            Guard.AgainstNull(connectString, "connectString");

            var assembly = AssemblyResolver.Load(assemblyFileName);
            if (assembly == null)
                return null;

            var attribute = ReflectionHelper.GetAttributeOne<DbContextImplementAttribute>(assembly);

            return new QwDbContextFactory(attribute.ContextType, connectString);
        }
    }
}
