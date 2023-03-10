using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xamariners.Mvvm;

namespace SwiftCaps.Models.Models
{
    public abstract class ModelBase : ValidatableBindableBase, INotifyPropertyChanged
    {
        /// <summary>
        ///     Gets or sets object id.
        /// </summary>
        /// <value>
        ///     From object  id.
        /// </value>
        [Key]
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public DateTime Created { get; set; }


        /// <summary>
        /// Gets or sets the updated.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreObject"/> class.
        /// </summary>
        public ModelBase()
        {
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
        }
    }
}
