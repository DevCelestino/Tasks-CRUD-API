using Asp.Versioning;

namespace Api.Versioning
{
    /// <summary>
    /// Attribute that marks a class or method as part of API version 1.
    /// Allows multiple instances to be applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class V1Attribute : ApiVersionAttribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="V1Attribute"/> class.
        /// This attribute is used to indicate that a controller or action method is 
        /// associated with version 1 of the API.
        /// </summary>
        public V1Attribute() : base(1) { }

        #endregion
    }
}
