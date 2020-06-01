using JamSoft.Xam.DragNDrop.ViewModels;

namespace JamSoft.Xam.DragNDrop.Models
{
    public class Item : BaseViewModel
    {
        private int _position;
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }

        public int Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }
    }
}