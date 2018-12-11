using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using System;

namespace PL.BookKeeping.Business
{
	public class AuthorizationService : IAuthorizationService
	{
		private readonly IUnitOfWorkFactory mUOWFactory;

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
					using (var unitOfWork = mUOWFactory.Create())
					{
						var repository = unitOfWork.GetRepository<User>();

						mCurrentuser = repository.SingleOrDefault(u => u.Key == 1);
					}
				}

				return mCurrentuser;
			}

			set => throw new NotImplementedException();
		}
	}
}