using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Entities;

namespace PL.BookKeeping.Infrastructure.Services
{
    /// <summary>Contains the current logged on user and handles changing this user.
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>The user who is currently working with the system.
        /// </summary>
        User CurrentUser { get; set; }
    }
}
