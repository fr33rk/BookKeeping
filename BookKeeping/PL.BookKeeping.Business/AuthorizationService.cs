using PL.BookKeeping.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Entities;

namespace PL.BookKeeping.Business
{
    public class AuthorizationService : IAuthorizationService
    {
        public User CurrentUser
        {
            get
            {
                return null;
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
