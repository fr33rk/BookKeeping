using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using PL.BookKeeping.Entities;

namespace BookKeeping.Client.Models
{
	public class EntryVM : BaseTracableObjectVMOfT<Entry, EntryVM>
	{
		#region Entry

		public string Description { get; set; }

		public int? ParentEntryKey { get; set; }

		public Entry ParentEntry { get; set; }

		public ICollection<Entry> ChildEntries { get; set; }

		#endregion Entry

		#region Property ChildEntryVms

		private ObservableCollection<EntryVM> mChildEntryVms;

		public ObservableCollection<EntryVM> ChildEntryVms
		{
			get
			{
				if (mChildEntryVms == null)
				{
					mChildEntryVms = new ObservableCollection<EntryVM>();
					if (ChildEntries != null)
					{
						foreach (var childEntry in ChildEntries.OrderBy(e => e.Description))
						{
							mChildEntryVms.Add(Mapper.Map<EntryVM>(childEntry));
						}
					}
				}

				return mChildEntryVms;
			}
		}

		#endregion Property ChildEntryVms

		#region Property RouteString

		public string mRouteString;

		public string RouteString
		{
			get
			{
				if (mRouteString == null)
				{
					if (ParentEntry != null)
					{
						mRouteString = Mapper.Map<EntryVM>(ParentEntry).RouteString;
						mRouteString += " > ";
					}

					mRouteString += Description;
				}

				return mRouteString;
			}
		}

		#endregion Property RouteString

		#region Child mutations

		public bool UpdateChild(EntryVM newChild)
		{
			var childEntry = ChildEntryVms.FirstOrDefault(e => e.Key == newChild.Key);
			if (childEntry != null)
			{
				newChild.ParentEntry = childEntry.ParentEntry;

				var index = ChildEntryVms.IndexOf(childEntry);
				ChildEntryVms.Remove(childEntry);
				ChildEntryVms.Insert(index, newChild);

				return true;
			}
			else
			{
				foreach (var child in ChildEntryVms)
				{
					if (child.UpdateChild(newChild))
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool DeleteChild(EntryVM obsoleteChild)
		{
			var childEntry = ChildEntryVms.FirstOrDefault(e => e.Key == obsoleteChild.Key);
			if (childEntry != null)
			{
				ChildEntryVms.Remove(childEntry);
				return true;
			}
			else
			{
				foreach (var child in ChildEntryVms)
				{
					if (child.DeleteChild(obsoleteChild))
					{
						return true;
					}
				}
			}

			return false;
		}

		internal bool HasChildNode(EntryVM child)
		{
			var foundChild = ChildEntryVms.Where(c => c.Key == child.Key).FirstOrDefault();
			if (foundChild != null)
			{
				return true;
			}
			else
			{
				foreach (var myChild in ChildEntryVms)
				{
					if (myChild.HasChildNode(child))
					{
						return true;
					}
				}
			}

			return false;
		}

		#endregion Child mutations
	}
}