using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;


namespace UCS_Project_Manager
{
    public class PagingListCollectionView : ListCollectionView
    {
        private int _itemsPerPage;

        private int _currentPage = 1;

        public PagingListCollectionView(IList innerList, int itemsPerPage)
            : base(innerList)
        {

            this._itemsPerPage = itemsPerPage;
        }

        public override int Count
        {
            get
            {
                if (this.InternalList.Count == 0) return 0;
                if (this._currentPage < this.PageCount) // page 1..n-1
                {
                    return this._itemsPerPage;
                }
                else // page n
                {
                    var itemsLeft = this.InternalList.Count % this._itemsPerPage;
                    if (0 == itemsLeft)
                    {
                        return this._itemsPerPage; // exactly itemsPerPage left
                    }
                    else
                    {
                        // return the remaining items
                        return itemsLeft;
                    }
                }
            }
        }

        public int CurrentPage
        {
            get { return this._currentPage; }
            set
            {
                this._currentPage = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentPage"));
            }
        }

        public int ItemsPerPage { get { return this._itemsPerPage; } set {this._itemsPerPage = value; } }

        private int _pageCount;
        public int PageCount
        {
            get
            {
                _pageCount = (this.InternalList.Count + this._itemsPerPage - 1)  / this._itemsPerPage;

                return _pageCount;
            }
            set
            {
                this._pageCount = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("PageCount"));
            }
        }

        private int EndIndex
        {
            get
            {
                var end = this._currentPage * this._itemsPerPage - 1;
                return (end > this.InternalList.Count) ? this.InternalList.Count : end;
            }
        }

        private int StartIndex
        {
            get { return (this._currentPage - 1) * this._itemsPerPage; }
        }

        public override object GetItemAt(int index)
        {

            //CHRIS ADDED FOR NO RESULTS CRASH
            if (this.InternalList.Count == 0)
                return null;

            var offset = index % (this._itemsPerPage);
            return this.InternalList[this.StartIndex + offset];
        }

        public void MoveToNextPage()
        {
            if (this._currentPage < this.PageCount)
            {
                this.CurrentPage += 1;
            }
            this.Refresh();
        }

        public void MoveToPreviousPage()
        {
            if (this._currentPage > 1)
            {
                this.CurrentPage -= 1;
            }
            this.Refresh();
        }

    }

}
