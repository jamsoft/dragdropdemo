using System;

using JamSoft.Xam.DragNDrop.Models;

namespace JamSoft.Xam.DragNDrop.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {
            Title = item?.Text;
            Item = item;
        }
    }
}
