using Android.Content;
using JamSoft.Xam.DragNDrop.CustomControls;
using JamSoft.Xam.DragNDrop.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DragAndDropListView), typeof(DragAndDropListViewRenderer))]

namespace JamSoft.Xam.DragNDrop.Droid.CustomRenderers
{
    public partial class DragAndDropListViewRenderer : ListViewRenderer
    {
        private DragAndDropListViewAdapter _dragAndDropListViewAdapter = null;

        public DragAndDropListViewRenderer(Context ctx)
            :base(ctx)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.Adapter?.Dispose();
                Control.Adapter = null;
                Control.SetOnDragListener(null);
                Control.OnItemLongClickListener = null;
            }

            if (e.NewElement != null)
            {
                _dragAndDropListViewAdapter = new DragAndDropListViewAdapter(Control, Element);
                Control.Adapter = _dragAndDropListViewAdapter;
                Control.SetOnDragListener(_dragAndDropListViewAdapter);
                Control.OnItemLongClickListener = _dragAndDropListViewAdapter;
            }
        }
    }
}