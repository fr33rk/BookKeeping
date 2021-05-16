using AutoMapper;
using PL.BookKeeping.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BookKeeping.Client.Models
{
	public class EntryVm : BaseTraceableObjectVMOfT<Entry, EntryVm>
	{
		#region Entry

		public string Description { get; set; }

		public int? ParentEntryKey { get; set; }

		public Entry ParentEntry { get; set; }

		public ICollection<Entry> ChildEntries { get; set; }

		public DateTime ActiveFrom { get; set; }

		public DateTime? ActiveUntil { get; set; }

		#endregion Entry

		#region Property ChildEntryVms

		private ObservableCollection<EntryVm> mChildEntryVms;

		public ObservableCollection<EntryVm> ChildEntryVms
		{
			get
			{
				if (mChildEntryVms == null)
				{
					mChildEntryVms = new ObservableCollection<EntryVm>();
					if (ChildEntries != null)
					{
						foreach (var childEntry in ChildEntries.OrderBy(e => e.Description))
						{
							mChildEntryVms.Add(Mapper.Map<EntryVm>(childEntry));
						}
					}
				}

				return mChildEntryVms;
			}
		}

		#endregion Property ChildEntryVms

		#region Property RouteString

		private string mRouteString;

		public string RouteString
		{
			get
			{
				if (mRouteString == null)
				{
					if (ParentEntry != null)
					{
						mRouteString = Mapper.Map<EntryVm>(ParentEntry).RouteString;
						mRouteString += " > ";
					}

					mRouteString += Description;
				}

				return mRouteString;
			}
		}

		#endregion Property RouteString

		#region Child mutations

		public bool UpdateChild(EntryVm newChild)
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

		public bool DeleteChild(EntryVm obsoleteChild)
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

		internal bool HasChildNode(EntryVm child)
		{
			var foundChild = ChildEntryVms.FirstOrDefault(c => c.Key == child.Key);
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