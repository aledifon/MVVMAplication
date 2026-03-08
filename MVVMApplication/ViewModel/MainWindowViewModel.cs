using MVVMApplication.Model;
using MVVMApplication.MVVM;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MVVMApplication.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        // DB Manager Instance
        private DBManager _dbRepository;

        // Properties used as binding with the View (UI)
        public ObservableCollection<Client> Clients { get; set; }       // No need to call OnPropertyChanged for these props as
                                                                        // ObservableCollection implements INotifyPropertyChanged
        public ObservableCollection<Order> Orders { get; set; }       
        public ObservableCollection<Article> Articles { get; set; }     
        private Client? _selectedClient;
        public Client? SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                OnPropertyChanged();        // As we're using the Compiler Attribute CallerMemberName
                                            // the 'SelectedItem' string will be passed automatically
                                            // as parameter
            }
        }
        private Order? _selectedOrder;
        public Order? SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();        // As we're using the Compiler Attribute CallerMemberName
                                            // the 'SelectedItem' string will be passed automatically
                                            // as parameter
            }
        }
        private Article? _selectedArticle;
        public Article? SelectedArticle
        {
            get { return _selectedArticle; }
            set
            {
                _selectedArticle = value;
                OnPropertyChanged();        // As we're using the Compiler Attribute CallerMemberName
                                            // the 'SelectedItem' string will be passed automatically
                                            // as parameter
            }
        }

        // Commands       
        public ICommand GetAllClientsCommand { get; }        
        public ICommand GetAllOrdersCommand { get; }        
        public ICommand GetAllArticlesCommand { get; }                          

        public MainWindowViewModel()
        {            
            _dbRepository = new DBManager();    

            // Create instances of every Command
            GetAllClientsCommand = new RelayCommand(GetAllClients);
            GetAllOrdersCommand = new RelayCommand(GetAllOrders, canExecute => true);
            GetAllArticlesCommand = new RelayCommand(GetAllArticles);

            // Create instances of every collection
            Clients = new ObservableCollection<Client>();
            Orders = new ObservableCollection<Order>();
            Articles = new ObservableCollection<Article>();
        }        

        private void GetAllClients(object? parameter)
        {
            var tempClients = _dbRepository.GetAllClients();

            Clients.Clear();
            foreach(var c in tempClients)
                Clients.Add(c);
        }
        private void GetAllOrders(object? parameter)
        {           
            var tempOrders = _dbRepository.GetAllOrders();

            Orders.Clear();
            foreach (var c in tempOrders)
                Orders.Add(c);
        }
        private void GetAllArticles(object? parameter)
        {
            var tempArticles = _dbRepository.GetAllArticles();

            Articles.Clear();
            foreach (var c in tempArticles)
                Articles.Add(c);
        }        
    }
}
