using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a person in the system with a unique identifier and a name.
    /// Each instance of this class corresponds to a record in the "person" table.
    /// </summary>
    [Table("persons")]
    public class PersonsEntity
    {
        #region Properties

        /// <summary>
        /// An unique id of each person.
        /// </summary>
        [Column("id"), Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// A string that specifies the name of the person.
        /// </summary>
        [Column("name"), Required, StringLength(100)]
        public required string Name { get; set; }

        /// <summary>
        /// A collection of tasks associated with the person.
        /// </summary>
        public virtual ICollection<TasksEntity>? Tasks { get; set; }

        #endregion
    }
}
