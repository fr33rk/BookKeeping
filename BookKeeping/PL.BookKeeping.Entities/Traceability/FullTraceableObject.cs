using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PL.BookKeeping.Entities.Traceability
{
    public abstract class FullTraceableObject : BaseTraceableObject
    {
        /// <summary>The ObjectState tells what state of the entity is stored in this object.
        /// </summary>
        public enum ObjectState
        {
            /// <summary>Default value.
            /// </summary>
            Unspecified = 0,

            /// <summary>This object is the current version of the entity that is still in use.
            /// </summary>
            Active,

            /// <summary>This object is an older version of the entity.
            /// </summary>
            Historic,

            /// <summary>This object is the current version of the entity but is not longer used (deleted).
            /// </summary>
            Deleted,

            /// <summary>This object is a 'special' object which cannot be modified by the user. (E.g. Factory default)
            /// </summary>
            System
        }

        /// <summary>Information about the action that has been executed which lead to the storage of this object.
        /// </summary>
        public enum ObjectAction
        {
            /// <summary>Default value.
            /// </summary>
            Unspecified = 0,

            /// <summary>The entity has been created.
            /// </summary>
            Created,

            /// <summary>The entity was edited.
            /// </summary>
            Edited,

            /// <summary>The entity was deleted.
            /// </summary>
            Deleted,

            // Examples of other actions that might be added in the future: Approved, RevokedApproval
        }

        /// <summary>Id of the entity. Will never change during the 'life' of this entity.
        /// Never manually set this property.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>The ObjectState tells what state of the entity is stored in this object.
        /// Never manually set this property.
        /// </summary>
        public ObjectState State { get; set; }

        /// <summary>Information about the action that has been executed which lead to the storage of this object.
        /// Never manually set this property.
        /// </summary>
        public ObjectAction Action { get; set; }
    }
}