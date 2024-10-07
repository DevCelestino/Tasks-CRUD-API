namespace Api.Versioning
{
    /// <summary>
    /// Contains constants for API versioning.
    /// </summary>
    public static class VersioningConstant
    {
        #region Properties

        /// <summary>
        /// The placeholder format for API versioning in routes.
        /// </summary>
        public const string Placeholder = "v{version:apiVersion}";

        #endregion
    }
}
