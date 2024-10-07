namespace Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for representing a person.
    /// </summary>
    public class PersonsDTO
    {
        #region Properties

        /// <summary>
        /// An unique id of each person.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the person. This field is required.
        /// </summary>
        public required string Name { get; set; }

        #endregion
    }
}
