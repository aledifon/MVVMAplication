using MVVMApplication.Model;
using MVVMApplication.MVVM;
using System.Collections.ObjectModel;

namespace MVVMApplication.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Item> Items { get; set; }

        public RelayCommand AddCommand => new RelayCommand(execute => AddItem());
        public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteItem(), canExecute => SelectedItem != null);
        public RelayCommand SaveCommand => new RelayCommand(execute => Save(), canExecute => canSave());

        public MainWindowViewModel()
        {
            Items = new ObservableCollection<Item>();
            //Items.Add(new Item
            //{
            //    Name = "Product1",
            //    SerialNumber = "001",
            //    Quantity = 10,
            //});
            //Items.Add(new Item
            //{
            //    Name = "Product2",
            //    SerialNumber = "002",
            //    Quantity = 20,
            //});
            //Items.Add(new Item
            //{
            //    Name = "Product3",
            //    SerialNumber = "003",
            //    Quantity = 6,
            //});
        }

        private Item _selectedItem;
        public Item SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();        // As we're using the Compiler Attribute CallerMemberName
                                            // the 'SelectedItem' string will be passed automatically
                                            // as parameter
            }
        }

        private void AddItem()
        {
            Items.Add(new Item
            {
                Name = "NEW ITEM",
                SerialNumber = "XXXX",
                Quantity = 0,
            });
        }
        private void DeleteItem()
        {
            Items.Remove(SelectedItem);
        }

        private void Save()
        {
            // Save to file/db
        }

        private bool canSave()
        {
            return true;
        }
    }
}
