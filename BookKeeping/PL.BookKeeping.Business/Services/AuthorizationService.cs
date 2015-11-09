using PL.BookKeeping.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;

namespace PL.BookKeeping.Business
{
    public class AuthorizationService : IAuthorizationService
    {
        private IUnitOfWorkFactory mUOWFactory;

        public AuthorizationService(IUnitOfWorkFactory uowFactory)
        {
            mUOWFactory = uowFactory;
        }

        private User mCurrentuser;

        public User CurrentUser
        {
            get
            {
                if (mCurrentuser == null)
                {
                    using (var unitOfWork = this.mUOWFactory.Create())
                    {
                        var repository = unitOfWork.GetRepository<User>();

                        mCurrentuser = repository.SingleOrDefault(u => u.Key == 1);
                    }
                }

                return mCurrentuser;
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
