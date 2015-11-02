using System.ComponentModel.DataAnnotations.Schema;

namespace PL.BookKeeping.Entities
{
    /// <summary>User definition.
    /// </summary>
    public class User : BaseTraceableObject
    {
        /// <summary>Full user name.
        /// </summary>
        public string Name { get; set; }
    }
}
