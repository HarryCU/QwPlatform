using System;
using System.Configuration.Assemblies;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Threading;

namespace QwMicroKernel.Reflection.Emit
{
    using RefAssemblyBuilder = System.Reflection.Emit.AssemblyBuilder;
    using RefModuleBuilder = ModuleBuilder;
    using RefCustomAttributeBuilder = CustomAttributeBuilder;
    using RefAssemblyBuilderAccess = AssemblyBuilderAccess;

    public class MemoryAssemblyBuilder
    {
        private readonly Action<int> _createAssemblyBuilder;
        private AssemblyName _assemblyName = new AssemblyName();
        private RefModuleBuilder _moduleBuilder;
        private RefAssemblyBuilder _assemblyBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBuilder"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="path">The path where the assembly will be saved.</param>
        public MemoryAssemblyBuilder()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBuilder"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="path">The path where the assembly will be saved.</param>
        /// <param name="version">The assembly version.</param>
        /// <param name="keyFile">The key pair file to sign the assembly.</param>
        public MemoryAssemblyBuilder(Version version, string keyFile)
        {
            _assemblyName.Name = Guid.NewGuid().ToString();

            if (version != null)
                _assemblyName.Version = version;

            if (!string.IsNullOrEmpty(keyFile))
            {
                _assemblyName.Flags |= AssemblyNameFlags.PublicKey;
                _assemblyName.KeyPair = new StrongNameKeyPair(File.OpenRead(keyFile));
                _assemblyName.HashAlgorithm = AssemblyHashAlgorithm.SHA1;
            }
#if DEBUG
            _assemblyName.Flags |= AssemblyNameFlags.EnableJITcompileTracking;
#else
            _assemblyName.Flags |= AssemblyNameFlags.EnableJITcompileOptimizer;
#endif

            _createAssemblyBuilder = delegate(int _)
            {
                _assemblyBuilder = CreateAssemblyBuilder();
                _assemblyBuilder.SetCustomAttribute(new RefCustomAttributeBuilder(typeof(AllowPartiallyTrustedCallersAttribute).GetConstructor(Type.EmptyTypes), new object[0]));
            };
        }

        /// <summary>
        /// Gets AssemblyName.
        /// </summary>
        public AssemblyName AssemblyName
        {
            get { return _assemblyName; }
            protected set { _assemblyName = value; }
        }

        /// <summary>
        /// Gets AssemblyBuilder.
        /// </summary>
        public RefAssemblyBuilder AssemblyBuilder
        {
            get
            {
                if (_assemblyBuilder == null)
                    _createAssemblyBuilder(0);
                return _assemblyBuilder;
            }
        }

        /// <summary>
        /// Gets ModuleBuilder.
        /// </summary>
        public RefModuleBuilder ModuleBuilder
        {
            get
            {
                if (_moduleBuilder == null)
                {
                    _moduleBuilder = AssemblyBuilder.DefineDynamicModule(AssemblyName.Name);
                }
                return _moduleBuilder;
            }
        }

        /// <summary>
        /// Converts the supplied <see cref="AssemblyBuilder"/> to a <see cref="AssemblyBuilder"/>.
        /// </summary>
        /// <param name="assemblyBuilder">The <see cref="AssemblyBuilder"/>.</param>
        /// <returns>An <see cref="AssemblyBuilder"/>.</returns>
        public static implicit operator RefAssemblyBuilder(MemoryAssemblyBuilder assemblyBuilder)
        {
            if (assemblyBuilder == null) throw new ArgumentNullException("assemblyBuilder");

            return assemblyBuilder.AssemblyBuilder;
        }

        /// <summary>
        /// Converts the supplied <see cref="AssemblyBuilder"/> to a <see cref="ModuleBuilder"/>.
        /// </summary>
        /// <param name="assemblyBuilder">The <see cref="AssemblyBuilder"/>.</param>
        /// <returns>A <see cref="ModuleBuilder"/>.</returns>
        public static implicit operator RefModuleBuilder(MemoryAssemblyBuilder assemblyBuilder)
        {
            if (assemblyBuilder == null) throw new ArgumentNullException("assemblyBuilder");

            return assemblyBuilder.ModuleBuilder;
        }

        protected virtual RefAssemblyBuilder CreateAssemblyBuilder()
        {
            var currentDomain = Thread.GetDomain();
            return currentDomain.DefineDynamicAssembly(_assemblyName, RefAssemblyBuilderAccess.RunAndCollect);
        }

        /// <summary>
        /// Saves this dynamic assembly to disk.
        /// </summary>
        public virtual void Save()
        {
        }

        #region DefineType Overrides

        /// <summary>
        /// Constructs a <see cref="TypeBuilder"/> for a type with the specified name.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <returns>Returns the created <see cref="TypeBuilder"/>.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string)">ModuleBuilder.DefineType Method</seealso>
        public TypeBuilder DefineType(string name)
        {
            return new TypeBuilder(this, ModuleBuilder.DefineType(name));
        }

        /// <summary>
        /// Constructs a <see cref="TypeBuilder"/> for a type with the specified name and base type.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <param name="parent">The Type that the defined type extends.</param>
        /// <returns>Returns the created <see cref="TypeBuilder"/>.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type)">ModuleBuilder.DefineType Method</seealso>
        public TypeBuilder DefineType(string name, Type parent)
        {
            return new TypeBuilder(this, ModuleBuilder.DefineType(name, TypeAttributes.Public, parent));
        }

        /// <summary>
        /// Constructs a <see cref="TypeBuilder"/> for a type with the specified name, its attributes, and base type.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <param name="attrs">The attribute to be associated with the type.</param>
        /// <param name="parent">The Type that the defined type extends.</param>
        /// <returns>Returns the created <see cref="TypeBuilder"/>.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type)">ModuleBuilder.DefineType Method</seealso>
        public TypeBuilder DefineType(string name, TypeAttributes attrs, Type parent)
        {
            return new TypeBuilder(this, ModuleBuilder.DefineType(name, attrs, parent));
        }

        /// <summary>
        /// Constructs a <see cref="TypeBuilder"/> for a type with the specified name, base type,
        /// and the interfaces that the defined type implements.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <param name="parent">The Type that the defined type extends.</param>
        /// <param name="interfaces">The list of interfaces that the type implements.</param>
        /// <returns>Returns the created <see cref="TypeBuilder"/>.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type,Type[])">ModuleBuilder.DefineType Method</seealso>
        public TypeBuilder DefineType(string name, Type parent, params Type[] interfaces)
        {
            return new TypeBuilder(
                this,
                ModuleBuilder.DefineType(name, TypeAttributes.Public, parent, interfaces));
        }

        /// <summary>
        /// Constructs a <see cref="TypeBuilder"/> for a type with the specified name, its attributes, base type,
        /// and the interfaces that the defined type implements.
        /// </summary>
        /// <param name="name">The full path of the type.</param>
        /// <param name="attrs">The attribute to be associated with the type.</param>
        /// <param name="parent">The Type that the defined type extends.</param>
        /// <param name="interfaces">The list of interfaces that the type implements.</param>
        /// <returns>Returns the created <see cref="TypeBuilder"/>.</returns>
        /// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type,Type[])">ModuleBuilder.DefineType Method</seealso>
        public TypeBuilder DefineType(string name, TypeAttributes attrs, Type parent, params Type[] interfaces)
        {
            return new TypeBuilder(
                this,
                ModuleBuilder.DefineType(name, attrs, parent, interfaces));
        }

        #endregion
    }
}
