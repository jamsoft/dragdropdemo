using System;
using Foundation;
using JamSoft.Xam.DragNDrop.ViewModels;
using UIKit;
using Xamarin.Forms;

namespace JamSoft.Xam.DragNDrop.iOS.CustomRenderers
{
    public class DragAndDropListViewTableSource : UITableViewSource
    {
        private UITableViewSource _originalSource;

        public WeakReference<ListView> FormsElementWeakReference;

        public DragAndDropListViewTableSource(UITableViewSource source, WeakReference<ListView> element)
        {
            _originalSource = source;
            FormsElementWeakReference = element;
        }

        public UITableViewSource OriginalSource
        {
            get
            {
                return _originalSource;
            }
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            // We do not want the "-" icon near each row (or the "+" icon)
            return UITableViewCellEditingStyle.None;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            // We still want the row to be editable
            return true;
        }

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            // We do want each row to be movable
            return true;
        }

        public override bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
        {
            // We do not want the "weird" indent for the rows when they are in editable mode.
            return false;
        }
        
        // fires even when dragDelegate and dropDelegates are enabled IF the item is just moving within the same ListView
        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            ListView lv;
            if (FormsElementWeakReference.TryGetTarget(out lv))
            {
                if (lv.ItemsSource is IOrderable orderableList)
                {
                    var deleteAt = sourceIndexPath.Row;
                    var insertAt = destinationIndexPath.Row;

                    orderableList.ChangeOrdinal(deleteAt, insertAt);
                }
            }
        }

        //public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
        //{
        //    switch (editingStyle)
        //    {
        //        case UITableViewCellEditingStyle.Delete:
        //            break;
        //        case UITableViewCellEditingStyle.None:
        //            break;
        //    }
        //}

        //public override NSIndexPath CustomizeMoveTarget(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath)
        //{
        //    var numRows = tableView.NumberOfRowsInSection(0);
        //    if (proposedIndexPath.Row >= numRows)
        //        return NSIndexPath.FromRowSection(numRows, 0);
        //    else
        //        return proposedIndexPath;
        //}

        //public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        //{
        //    _originalSource.DraggingEnded(scrollView, willDecelerate);
        //}

        //public override void DraggingStarted(UIScrollView scrollView)
        //{
        //    _originalSource.DraggingStarted(scrollView);
        //}

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return OriginalSource.GetCell(tableView, indexPath);
        }

        //public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        //{
        //    return OriginalSource.GetHeightForHeader(tableView, section);
        //}

        //public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        //{
        //    return OriginalSource.GetHeightForRow(tableView, indexPath);
        //}

        //public override UIView GetViewForHeader(UITableView tableView, nint section)
        //{
        //    return OriginalSource.GetViewForHeader(tableView, section);
        //}

        //public override void HeaderViewDisplayingEnded(UITableView tableView, UIView headerView, nint section)
        //{
        //    OriginalSource.HeaderViewDisplayingEnded(tableView, headerView, section);
        //}

        //public override nint NumberOfSections(UITableView tableView)
        //{
        //    return OriginalSource.NumberOfSections(tableView);
        //}

        //public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        //{
        //    OriginalSource.RowDeselected(tableView, indexPath);
        //}

        //public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        //{
        //    OriginalSource.RowSelected(tableView, indexPath);
        //}

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return OriginalSource.RowsInSection(tableview, section);
        }

        //public override void Scrolled(UIScrollView scrollView)
        //{
        //    OriginalSource.Scrolled(scrollView);
        //}

        //public override string[] SectionIndexTitles(UITableView tableView)
        //{
        //    return OriginalSource.SectionIndexTitles(tableView);
        //}
    }
}