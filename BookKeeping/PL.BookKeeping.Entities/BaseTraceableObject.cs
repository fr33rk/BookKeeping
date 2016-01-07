using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PL.BookKeeping.Entities
{
    /// <summary>Almost all objects that are stored in the database must include a key, the date and time
    /// the record has been created and a reference to the creator. To avoid these elements have to be copied
    /// to all objects they can inherit from this class.
    /// </summary>
    public abstract class BaseTraceableObject
    {
        /// <summary>Initializes a new instance of the <see cref="BaseTraceableObject"/> class.
        /// </summary>
        public BaseTraceableObject()
        {
        }

        /// <summary>The unique identifier for this object. Should never be manually set as it is
        /// manager by the Entity Framework.
        /// </summary>
        [Key]
        public int Key { get; set; }

        /// <summary>Gets or sets the creation date and time. Should never be manually set.
        /// </summary>
        [Required]
        public DateTime CreationDT { get; set; }

        [ForeignKey("Creator")]
        public int? CreatorKey { get; set; }

        /// <summary>Gets or sets the creator. Should never be manually set.
        /// </summary>
        public virtual User Creator { get; set; }
    }
}