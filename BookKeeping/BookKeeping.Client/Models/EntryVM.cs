using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using PL.BookKeeping.Entities;

namespace BookKeeping.Client.Models
{
	public class EntryVM : BaseTracableObjectVM
    {
        public string Description { get; set; }

        public int? ParentEntryKey { get; set; }

        public Entry ParentEntry { get; set; }

        public ICollection<Entry> ChildEntries { get; set; }

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

        #region Mapping

        public Entry ToEntry()
        {
            return Mapper.Map<Entry>(this);
        }

        public static EntryVM FromEntry(Entry entry)
        {
            return Mapper.Map<EntryVM>(entry);
        }

        #endregion Mapping



        public bool UpdateChild(EntryVM newChild)
        {
            var childEntry = ChildEntryVms.FirstOrDefault(e => e.Key == newChild.Key);
            if (childEntry != null)
            {
                newChild.ParentEntry = childEntry.ParentEntry;
                //childEntry = newChild;

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

		internal bool HasChildNode(EntryVM child)
		{
			var foundChild = ChildEntryVms.Where(c => c.Key == child.Key);
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
	}
}
