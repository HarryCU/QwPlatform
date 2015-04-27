using System;

namespace QwMicroKernel.Reflection.Emit
{
    using RefConstructorBuilder = System.Reflection.Emit.ConstructorBuilder;

    /// <summary>
    /// A wrapper around the <see cref="ConstructorBuilder"/> class.
    /// </summary>
    public class ConstructorBuilder : MethodBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorBuilder"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="typeBuilder">Associated <see cref="TypeBuilder"/>.</param>
        /// <param name="constructorBuilder">A <see cref="ConstructorBuilder"/></param>
        public ConstructorBuilder(TypeBuilder typeBuilder, RefConstructorBuilder constructorBuilder)
            : base(typeBuilder)
        {
            if (constructorBuilder == null) throw new ArgumentNullException("constructorBuilder");

            _constructorBuilder = constructorBuilder;
        }

        private readonly RefConstructorBuilder _constructorBuilder;
        /// <summary>
        /// Gets ConstructorBuilder.
        /// </summary>
        public RefConstructorBuilder Builder
        {
            get { return _constructorBuilder; }
        }

        /// <summary>
        /// Converts the supplied <see cref="ConstructorBuilder"/> to a <see cref="MethodBuilder"/>.
        /// </summary>
        /// <param name="constructorBuilder">The <see cref="ConstructorBuilder"/>.</param>
        /// <returns>A <see cref="ConstructorBuilder"/>.</returns>
        public static implicit operator RefConstructorBuilder(ConstructorBuilder constructorBuilder)
        {
            if (constructorBuilder == null) throw new ArgumentNullException("constructorBuilder");

            return constructorBuilder.Builder;
        }

        private EmitHelper _emitter;
        /// <summary>
        /// Gets <see cref="EmitHelper"/>.
        /// </summary>
        public override EmitHelper Emitter
        {
            get
            {
                if (_emitter == null)
                    _emitter = new EmitHelper(this, _constructorBuilder.GetILGenerator());

                return _emitter;
            }
        }
    }
}
