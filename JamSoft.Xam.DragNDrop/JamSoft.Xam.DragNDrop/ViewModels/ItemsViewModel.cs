using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using JamSoft.Xam.DragNDrop.Models;
using JamSoft.Xam.DragNDrop.Views;

namespace JamSoft.Xam.DragNDrop.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private ObservableListCollection<Item> _items;

        public ObservableListCollection<Item> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        private ObservableListCollection<Item> _items2;

        public ObservableListCollection<Item> Items2
        {
            get => _items2;
            set
            {
                _items2 = value;
                OnPropertyChanged();
            }
        }

        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Browse";
            
            Items = new ObservableListCollection<Item>();
            Items2 = new ObservableListCollection<Item>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                Items.Add(item);
                await DataStore.AddItemAsync(item);
            });

            Items.OrderChanged += (sender, e) => {
                int position = 1;
                foreach (var item in Items)
                {
                    item.Position = position++;
                }

                Debug.WriteLine("OrderChanged Items");
            };

            Items2.OrderChanged += (sender, e) => {
                int position = 1;
                foreach (var item in Items2)
                {
                    item.Position = position++;
                }

                Debug.WriteLine("OrderChanged Items2");
            };

            LoadItemsCommand.Execute(null);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}