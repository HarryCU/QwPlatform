using System;
using System.IO;
using System.Reflection.Emit;
using System.Threading;

namespace QwMicroKernel.Reflection.Emit
{
    using RefAssemblyBuilder = System.Reflection.Emit.AssemblyBuilder;
    using RefModuleBuilder = ModuleBuilder;
    using RefAssemblyBuilderAccess = AssemblyBuilderAccess;

    /// <summary>
    /// A wrapper around the <see cref="AssemblyBuilder"/> and <see cref="ModuleBuilder"/> classes.
    /// </summary>
    /// <include file="Examples.CS.xml" path='examples/emit[@name="Emit"]/*' />
    /// <include file="Examples.VB.xml" path='examples/emit[@name="Emit"]/*' />
    /// <seealso cref="System.Reflection.Emit.AssemblyBuilder">AssemblyBuilder Class</seealso>
    /// <seealso cref="System.Reflection.Emit.ModuleBuilder">ModuleBuilder Class</seealso>
    public class AssemblyBuilder : MemoryAssemblyBuilder
    {
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBuilder"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="path">The path where the assembly will be saved.</param>
        public AssemblyBuilder(string path)
            : this(path, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBuilder"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="path">The path where the assembly will be saved.</param>
        /// <param name="version">The assembly version.</param>
        /// <param name="keyFile">The key pair file to sign the assembly.</param>
        public AssemblyBuilder(string path, Version version, string keyFile)
            : base(version, keyFile)
        {
            if (path == null) throw new ArgumentNullException("path");
            int idx = path.IndexOf(',');
            if (idx > 0)
            {
                path = path.Substring(0, idx);

                if (path.Length >= 200)
                {
                    idx = path.IndexOf('`');

                    if (idx > 0)
                    {
                        int idx2 = path.LastIndexOf('.');

                        if (idx2 > 0 && idx2 > idx)
                            path = path.Substring(0, idx + 1) + path.Substring(idx2 + 1);
                    }
                }
            }

            path = path.Replace("+", ".").Replace("<", "_").Replace(">", "_");

            if (path.Length >= 260)
            {
                path = path.Substring(0, 248);

                for (int i = 0; i < int.MaxValue; i++)
                {
                    string newPath = string.Format("{0}_{1:0000}.dll", path, i);

                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }
            string assemblyName = System.IO.Path.GetFileNameWithoutExtension(path);
            _path = path;
            AssemblyName.Name = assemblyName;
        }

        /// <summary>
        /// Gets the path where the assembly will be saved.
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// Converts the supplied <see cref="AssemblyBuilder"/> to a <see cref="AssemblyBuilder"/>.
        /// </summary>
        /// <param name="assemblyBuilder">The <see cref="AssemblyBuilder"/>.</param>
        /// <returns>An <see cref="AssemblyBuilder"/>.</returns>
        public static implicit operator RefAssemblyBuilder(AssemblyBuilder assemblyBuilder)
        {
            if (assemblyBuilder == null) throw new ArgumentNullException("assemblyBuilder");

            return assemblyBuilder.AssemblyBuilder;
        }

        /// <summary>
        /// Converts the supplied <see cref="AssemblyBuilder"/> to a <see cref="ModuleBuilder"/>.
        /// </summary>
        /// <param name="assemblyBuilder">The <see cref="AssemblyBuilder"/>.</param>
        /// <returns>A <see cref="ModuleBuilder"/>.</returns>
        public static implicit operator RefModuleBuilder(AssemblyBuilder assemblyBuilder)
        {
            if (assemblyBuilder == null) throw new ArgumentNullException("assemblyBuilder");

            return assemblyBuilder.ModuleBuilder;
        }

        protected override RefAssemblyBuilder CreateAssemblyBuilder()
        {
            var currentDomain = Thread.GetDomain();
            return currentDomain.DefineDynamicAssembly(AssemblyName, RefAssemblyBuilderAccess.RunAndSave, System.IO.Path.GetDirectoryName(Path));

        }

        /// <summary>
        /// Saves this dynamic assembly to disk.
        /// </summary>
        public override void Save()
        {
            if (AssemblyBuilder != null)
            {
                var name = System.IO.Path.GetFileName(Path);
                AssemblyBuilder.Save(name);
            }
        }
    }
}
