using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using JamSoft.Xam.DragNDrop.CustomControls;
using JamSoft.Xam.DragNDrop.iOS.CustomRenderers;
using JamSoft.Xam.DragNDrop.Models;
using MobileCoreServices;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DragAndDropListView), typeof(DragAndDropListViewRenderer))]

namespace JamSoft.Xam.DragNDrop.iOS.CustomRenderers
{
    public partial class DragAndDropListViewRenderer : ListViewRenderer, IUITableViewDragDelegate, IUITableViewDropDelegate
    {
        private static UITableView _sourceTableView;
        private static ListView _sourceListView;
        private static Item _dragItem;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control.Source = new DragAndDropListViewTableSource(Control.Source, new WeakReference<ListView>(Element));
                Control.SetEditing(true, true);
                Control.DragDelegate = this;
                Control.DropDelegate = this;
                Control.DragInteractionEnabled = true;

                //DD();
            }
        }

        public UIDragItem[] GetItemsForBeginningDragSession(UITableView tableView, IUIDragSession session, NSIndexPath indexPath)
        {
            _sourceTableView = tableView;
            _sourceListView = Element;

            _dragItem = (Item)((IList) Element.ItemsSource)[indexPath.Row];

            // https://forums.xamarin.com/discussion/66953/how-to-save-c-object-into-nsdata-and-get-the-same-object-from-nsdata
            //var data = NSKeyedArchiver.ArchivedDataWithRootObject(indexPath);
            
            //var data = NSData.FromString(placeName, NSStringEncoding.UTF8);
            var data = NSData.FromString(_dragItem.Id);

            var itemProvider = new NSItemProvider();
            itemProvider.RegisterDataRepresentation(UTType.PlainText, NSItemProviderRepresentationVisibility.All,
                (completion) =>
                {
                    completion(data, null);
                    return null;
                });

            var uidragItem = new UIDragItem(itemProvider);

            return new[] {uidragItem};
        }

        //[Export("tableView:canHandleDropSession:")]
        public bool CanHandleDropSession(UITableView tableView, IUIDropSession session)
        {
            //return session.CanLoadObjects(typeof(NSString));
            return true;
        }

        //[Export("tableView:performDrop:")]
        public void PerformDrop(UITableView tableView, IUITableViewDropCoordinator coordinator)
        {
            var sourceCollection = ((IList)DragAndDropListViewRenderer._sourceListView.ItemsSource);

            ListView targetListView;
            (tableView.Source as DragAndDropListViewTableSource).FormsElementWeakReference.TryGetTarget(out targetListView);
            var targetCollection = targetListView.ItemsSource as IList;
            NSIndexPath indexPath, destinationIndexPath;
            if (coordinator.DestinationIndexPath != null)
            {
                indexPath = coordinator.DestinationIndexPath;
                destinationIndexPath = indexPath;
            }
            else
            {
                // Get last index path of table view
                var section = tableView.NumberOfSections() - 1;
                var row = tableView.NumberOfRowsInSection(section);
                destinationIndexPath = NSIndexPath.FromRowSection(row, section);
            }

            // ReSharper disable once PossibleUnintendedReferenceComparison
            // we want to check reference!
            if (_sourceTableView == tableView)
            {
                // sorting see DragAndDropListViewTableSource.MoveRow
            }
            else
            {
                if ((int) destinationIndexPath.Item > targetCollection.Count)
                {
                    targetCollection.Add(_dragItem);
                    sourceCollection.Remove(_dragItem);
                }
                else
                {
                    targetCollection.Insert((int)destinationIndexPath.Item, _dragItem);
                    sourceCollection.Remove(_dragItem);
                }
            }
        }

        private void DD()
        {
            int from = -1;
            NSIndexPath pathTo = null;
            //var draggedViewCell = null;

            var longPressGesture = new UILongPressGestureRecognizer(gesture =>
            {
                switch (gesture.State)
                {
                    case UIGestureRecognizerState.Began:
                        var selectedIndexPath = Control.IndexPathForRowAtPoint(gesture.LocationInView(Control));
                        if (selectedIndexPath != null)
                        {
                            from = (int)selectedIndexPath.Item;
                            var cell = Control.CellAt(selectedIndexPath);
                            cell.DragStateDidChange(UITableViewCellDragState.Dragging);
                        }
                        break;
                    case UIGestureRecognizerState.Changed:
                        break;
                    case UIGestureRecognizerState.Ended:
                        break;
                    default:
                        break;
                }
            });

            Control.AddGestureRecognizer(longPressGesture);
        }
    }
}