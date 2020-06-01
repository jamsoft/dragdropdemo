using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Android.Animation;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using JamSoft.Xam.DragNDrop.CustomControls;
using JamSoft.Xam.DragNDrop.Models;
using JamSoft.Xam.DragNDrop.ViewModels;
using Object = Java.Lang.Object;

namespace JamSoft.Xam.DragNDrop.Droid.CustomRenderers
{
    public class DragAndDropListViewAdapter : BaseAdapter, IWrapperListAdapter, View.IOnDragListener, AdapterView.IOnItemLongClickListener
    {
        private ListView _listView;
        private DataSetObserver _observer;
        private Xamarin.Forms.ListView _element;
        public readonly ObservableListCollection<Item> Collection;

        private static int _lastPosition = 0;

        private List<View> _translatedItems = new List<View>();

        private ScrollView _hostScrollView;

        public DragAndDropListViewAdapter(ListView listView, Xamarin.Forms.ListView element)
        {
            _listView = listView;
            WrappedAdapter = ((IWrapperListAdapter)_listView.Adapter).WrappedAdapter;
            _element = element;
            Collection = element.ItemsSource as ObservableListCollection<Item>;

            // var c = ((DragAndDropListView)_element).Host;
            //_hostScrollView = (ScrollView)_element.Parent.Parent;
            _hostScrollView = (ScrollView)listView.Parent.Parent;
        }

        public override Object GetItem(int position)
        {
            return WrappedAdapter.GetItem(position);
        }

        public override long GetItemId(int position)
        {
            return WrappedAdapter.GetItemId(position);
        }

        public override int GetItemViewType(int position)
        {
            return WrappedAdapter.GetItemViewType(position);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = WrappedAdapter.GetView(position, convertView, parent);
            view.SetOnDragListener(this);
            return view;
        }

        public override bool IsEnabled(int position)
        {
            return WrappedAdapter.IsEnabled(position);
        }

        public override void RegisterDataSetObserver(DataSetObserver observer)
        {
            _observer = observer;
            base.RegisterDataSetObserver(observer);
            WrappedAdapter.RegisterDataSetObserver(observer);
        }

        public override void UnregisterDataSetObserver(DataSetObserver observer)
        {
            base.UnregisterDataSetObserver(observer);
            WrappedAdapter.UnregisterDataSetObserver(observer);
        }

        public IListAdapter WrappedAdapter { get; private set; }

        public override int Count => WrappedAdapter.Count;

        //public override bool HasStableIds => WrappedAdapter.HasStableIds;

        //public override bool IsEmpty => WrappedAdapter.IsEmpty;

        //public override int ViewTypeCount => WrappedAdapter.ViewTypeCount;

        //public override bool AreAllItemsEnabled() => WrappedAdapter.AreAllItemsEnabled();

        public bool OnDrag(View v, DragEvent e)
        {
            var dragItem = (DragItem)e.LocalState;

            switch (e.Action)
            {
                case DragAction.Started:
                    break;
                case DragAction.Entered:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Entered from {v.GetType()}");

                    if (!(v is ListView))
                    {
                        //var dragItem = (DragItem)e.LocalState;

                        var targetPosition = InsertOntoView(v, dragItem);

                        dragItem.Index = targetPosition;

                        // Keep a list of items that has translation so we can reset
                        // them once the drag'n'drop is finished.
                        _translatedItems.Add(v);
                        _listView.Invalidate();
                    }
                    break;
                case DragAction.Location:

                    var currentPosition = (int)e.GetX();

                    var point = GetTouchPositionFromDragEvent(v, e);

                    System.Diagnostics.Debug.WriteLine($"DragAction.Location from {v.GetType()} => {currentPosition}");
                    System.Diagnostics.Debug.WriteLine($"DragAction.GLOBAL   from {v.GetType()} => {point.X}");

                    int y = Java.Lang.Math.Round(e.GetY());
                    int translatedY = y - 50;
                    int threshold = 50;
                    // make a scrolling up due the y has passed the threshold
                    if (translatedY < threshold)
                    {
                        // make a scroll up by 30 px
                        _hostScrollView.ScrollBy(0, -30);
                    }
                    // make a autoscrolling down due y has passed the 500 px border
                    if (translatedY + threshold > 500)
                    {
                        // make a scroll down by 30 px
                        _hostScrollView.ScrollBy(0, 30);
                    }

                    //if (point.X > App.AppScreenWidth - 50)
                    //{
                    //    //_hostScrollView.ScrollToAsync(_hostScrollView.ScrollX + 30, 0, true);
                    //    _hostScrollView.StartScroll(50);
                    //}

                    //if (point.X < 50)
                    //{
                    //    //_hostScrollView.ScrollToAsync(_hostScrollView.ScrollX - 30, 0, true);
                    //    _hostScrollView.StartScroll(-50);
                    //}

                    //if (currentPosition > _element.Width - 40)
                    //{
                    //    _hostScrollView.ScrollToAsync(_hostScrollView.ScrollX + 30, 0, true);
                    //}
                    //else if (currentPosition < 40)
                    //{
                    //    _hostScrollView.ScrollToAsync(_hostScrollView.ScrollX - 30, 0, true);
                    //}

                    //if (v.Right < (_hostScrollView.ScrollX + _hostScrollView.Width - 100))
                    //{
                    //    _hostScrollView.ScrollToAsync(_hostScrollView.ScrollX + 30, 0, true);
                    //}

                    //if (v.Left > (_hostScrollView.ScrollX + _hostScrollView.Width - 100))
                    //{
                    //    _hostScrollView.ScrollToAsync(_hostScrollView.ScrollX - 30, 0, true);
                    //}

                    break;
                case DragAction.Exited:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Entered from {v.GetType()}");

                    _lastPosition = _listView.GetPositionForView(v);

                    if (!(v is ListView))
                    {
                        var positionEntered = GetListPositionForView(v);
                        //var item1 = _listAdapter.GetItem(positionEntered);

                        System.Diagnostics.Debug.WriteLine($"DragAction.Exited index {positionEntered}");
                    }
                    break;
                case DragAction.Drop:

                    System.Diagnostics.Debug.WriteLine($"DragAction.Drop from {v.GetType()}");

                    if (v is ListView)
                    {
                        bool isDropped = true;
                        int positionSource = -1;
                        int insertLocation = -1;

                        var dropItem = (DragItem)e.LocalState;
                        Android.Views.View view = dropItem.View;
                        object passedItem = dropItem.Item;

                        Android.Widget.ListView oldParent = (Android.Widget.ListView)view.Parent;

                        //DragListAdapter oldParentAdapter = (DragListAdapter)oldParent.;
                        BaseAdapter sourceAdapter = (oldParent.Adapter is IWrapperListAdapter)
                            ? ((IWrapperListAdapter)oldParent.Adapter).WrappedAdapter as BaseAdapter
                            : ((BaseAdapter)oldParent.Adapter);
                        DragAndDropListViewAdapter sourceDragListAdaptor = sourceAdapter as DragAndDropListViewAdapter;

                        //positionSource = (int)view.Tag;

                        Console.WriteLine(v.Parent.Parent);
                        Android.Widget.ListView newParent = _listView;

                        //Person customList = (Person)sourceAdapter.Collection[positionSource];
                        ObservableListCollection<Item> sourceList = sourceDragListAdaptor.Collection;
                        //customListSource.RemoveAt(positionSource);



                        //Android.Widget.ListView target = this.Element.
                        //DragListAdapter adapterTarget = (DragListAdapter)_listView.Adapter;
                        BaseAdapter destinationAdapter = (oldParent.Adapter is IWrapperListAdapter)
                            ? ((IWrapperListAdapter)newParent.Adapter).WrappedAdapter as BaseAdapter
                            : ((BaseAdapter)oldParent.Adapter);
                        DragAndDropListViewAdapter destinationDragListAdaptor = destinationAdapter as DragAndDropListViewAdapter;

                        ObservableListCollection<Item> targetList = destinationDragListAdaptor.Collection;

                        int removeLocation = oldParent.GetPositionForView(view);
                        insertLocation = newParent.GetPositionForView(dropItem.View);

                        var item = dropItem.Item as Item;

                        //if (sourceList.Contains(dropItem.Item as Item) && sourceList != targetList)
                        if (sourceList != targetList)
                        {
                            if (insertLocation >= 0)
                            {
                                targetList.Insert(insertLocation, item);
                                targetList.ItemAdded(item);
                            }
                            else
                            {
                                targetList.Add(dropItem.Item as Item);
                                targetList.ItemAdded(item);
                            }

                            sourceList.Remove(item);


                            sourceAdapter.NotifyDataSetChanged();
                            destinationAdapter.NotifyDataSetChanged();
                            v.Visibility = ViewStates.Visible; //(View.VISIBLE);
                        }
                        else
                        {
                            if (_element.ItemsSource is IOrderable orderable)
                            {
                                if (((IList)_element.ItemsSource).Contains(dropItem.Item))
                                {
                                    orderable.ChangeOrdinal(dropItem.OriginalIndex, dropItem.Index);
                                }
                            }
                        }

                        //foreach (var viewTrans in _translatedItems)
                        //{
                        //    viewTrans.TranslationY = 0;
                        //}

                        //_translatedItems.Clear();
                    }

                    break;
                case DragAction.Ended:
                    if (!(v is ListView))
                    {
                        return false;
                    }

                    //System.Diagnostics.Debug.WriteLine($"DragAction.Ended from {v.GetType()}");
                    
                    var mobileItem = (DragItem)e.LocalState;

                    mobileItem.View.Visibility = ViewStates.Visible;

                    foreach (var view in _translatedItems)
                    {
                        view.TranslationY = 0;
                    }

                    _translatedItems.Clear();

                    //if (_element.ItemsSource is IOrderable orderable)
                    //{
                    //    if (((IList)_element.ItemsSource).Contains(mobileItem.Item))
                    //    {
                    //        orderable.ChangeOrdinal(mobileItem.OriginalIndex, mobileItem.Index);
                    //    }
                    //    else
                    //    {
                    //        orderable.ItemAdded(mobileItem.Item);
                    //    }
                    //}

                    break;
            }

            return true;
        }

        public bool OnItemLongClick(AdapterView parent, View view, int position, long id)
        {
            var selectedItem = ((IList)_element.ItemsSource)[(int)id];

            DragItem dragItem = new DragItem(NormalizeListPosition(position), view, selectedItem);

            var data = ClipData.NewPlainText(string.Empty, string.Empty);

            View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(view);

            view.Visibility = ViewStates.Invisible;

            view.StartDragAndDrop(data, shadowBuilder, dragItem, 0);
            return true;
        }

        private int InsertOntoView(View view, DragItem item)
        {
            var positionEntered = GetListPositionForView(view);
            var correctedPosition = positionEntered;

            // If the view already has a translation, we need to adjust the position
            // If the view has a positive translation, that means that the current position
            // is actually one index down then where it started.
            // If the view has a negative translation, that means it actually moved
            // up previous now we will need to move it down.
            if (view.TranslationY > 0)
            {
                correctedPosition += 1;
            }
            else if (view.TranslationY < 0)
            {
                correctedPosition -= 1;
            }

            // If the current index of the dragging item is bigger than the target
            // That means the dragging item is moving up, and the target view should
            // move down, and vice-versa
            var translationCoef = item.Index > correctedPosition ? 1 : -1;

            // We translate the item as much as the height of the drag item (up or down)
            var translationTarget = view.TranslationY + (translationCoef * item.View.Height);

            ObjectAnimator anim = ObjectAnimator.OfFloat(view, "TranslationY", view.TranslationY, translationTarget);
            anim.SetDuration(100);
            anim.Start();

            return correctedPosition;
        }

        private int GetListPositionForView(View view)
        {
            return NormalizeListPosition(_listView.GetPositionForView(view));
        }

        private int NormalizeListPosition(int position)
        {
            return position - _listView.HeaderViewsCount;
        }

        public static Point GetTouchPositionFromDragEvent(View item, DragEvent e) 
        {
            Rect rItem = new Rect();
            item.GetGlobalVisibleRect(rItem);
            return new Point(rItem.Left + (int)Math.Round(e.GetX()), rItem.Top + (int)Math.Round(e.GetY()));
        }

        private void StartScroll(int scrollDistance)
        {
            if (Timer != null && Timer.Enabled)
            {
                return;
            }

            _scrollDistance = scrollDistance;
            Timer = new System.Timers.Timer();
            Timer.Elapsed += new ElapsedEventHandler(Scroll);
            Timer.Interval = 50;
            Timer.Enabled = true;
        }

        public void StopScroll()
        {
            Timer.Enabled = false;
        }

        public Timer Timer { get; set; }

        private double _scrollDistance;

        private void Scroll(object source, ElapsedEventArgs e)
        {
            double scrollTo = _hostScrollView.ScrollX + _scrollDistance;

            using (var h = new Handler(Looper.MainLooper))
            {
                h.PostDelayed(() =>
                {
                    //_hostScrollView.ScrollToAsync(scrollTo, scrollTo, true);
                }, 12);
            }
        }
    }
}