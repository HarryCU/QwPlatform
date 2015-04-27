using System;

namespace QwMicroKernel.Reflection.Emit
{
    /// <summary>
    /// Base class for wrappers around methods and constructors.
    /// </summary>
    public abstract class MethodBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBuilder"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="typeBuilder">Associated <see cref="TypeBuilder"/>.</param>
        protected MethodBuilderBase(TypeBuilder typeBuilder)
        {
            if (typeBuilder == null) throw new ArgumentNullException("typeBuilder");

            _type = typeBuilder;
        }

        private readonly TypeBuilder _type;
        /// <summary>
        /// Gets associated <see cref="TypeBuilder"/>.
        /// </summary>
        public TypeBuilder Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets <see cref="EmitHelper"/>.
        /// </summary>
        public abstract EmitHelper Emitter { get; }
    }
}
