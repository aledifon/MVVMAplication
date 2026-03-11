using MVVMApplication.Model;
using MVVMApplication.MVVM;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace MVVMApplication.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        // DB Manager Instance
        private DBManager _dbRepository;

        #region Properties
        public ObservableCollection<Client> Clients { get; set; }       // No need to call OnPropertyChanged for these props as                                                                        // ObservableCollection implements INotifyPropertyChanged
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
        private Client? _newClient;
        public Client? NewClient
        {
            get { return _newClient; }
            set
            {
                _newClient = value;
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
        private Order? _newOrder;
        public Order? NewOrder
        {
            get { return _newOrder; }
            set
            {
                _newOrder = value;
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
        private Article? _newArticle;
        public Article? NewArticle
        {
            get { return _newArticle; }
            set
            {
                _newArticle = value;
                OnPropertyChanged();        // As we're using the Compiler Attribute CallerMemberName
                                            // the 'SelectedItem' string will be passed automatically
                                            // as parameter
            }
        }
        #endregion 

        public MainWindowViewModel()
        {
            _dbRepository = new DBManager();

            // Create instances of every Command
            GetAllClientsCommand = new RelayCommand(GetAllClients);
            GetAllOrdersCommand = new RelayCommand(GetAllOrders, canExecute => true);
            GetAllArticlesCommand = new RelayCommand(GetAllArticles);

            InsertClientCommand = new RelayCommand(AddNewClient);
            InsertOrderCommand = new RelayCommand(AddNewOrder);
            InsertArticleCommand = new RelayCommand(AddNewArticle);

            // Create instances of every collection
            Clients = new ObservableCollection<Client>();
            Orders = new ObservableCollection<Order>();
            Articles = new ObservableCollection<Article>();

            NewClient = new Client();
            NewOrder = new Order();
            NewArticle = new Article();
        }

        #region Commands
        public ICommand GetAllClientsCommand { get; }        
        public ICommand GetAllOrdersCommand { get; }        
        public ICommand GetAllArticlesCommand { get; }

        public ICommand InsertClientCommand { get; }
        public ICommand InsertOrderCommand { get; }
        public ICommand InsertArticleCommand { get; }
        #endregion

        #region Checking Methods
        private bool FindClientByName(Client client) => _dbRepository.FindClientByName(client);
        private bool FindArticleByName(Article article) => _dbRepository.FindArticleByName(article);
        private bool FindClientByOrderId(int clientId) => _dbRepository.FindClientByOrderId(clientId);
        #endregion

        #region Data Validation Methods
        private bool NewClientsEmptyFieldsValidation(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.ClientName)) return false;
            if (string.IsNullOrWhiteSpace(client.Address)) return false;
            if (string.IsNullOrWhiteSpace(client.Location)) return false;
            if (string.IsNullOrWhiteSpace(client.Telephone)) return false;

            return true;
        }
        private bool NewArticlesEmptyFieldsValidation(Article article)
        {
            if (string.IsNullOrWhiteSpace(article.Section)) return false;
            if (string.IsNullOrWhiteSpace(article.ArticleName)) return false;
            if (article.Price <= 0) return false;
            if (article.Date == null || article.Date > DateTime.Today) return false;
            if (string.IsNullOrWhiteSpace(article.OriginCountry)) return false;

            return true;
        }
        private bool NewOrderEmptyFieldsValidation(Order order)
        {            
            if (order.CClient <= 0) return false;
            if (order.DateOrder == null || order.DateOrder > DateTime.Today) return false;
            if (string.IsNullOrWhiteSpace(order.TypePayment)) return false;

            return true;
        }
        #endregion

        #region Read Methods
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
        #endregion

        #region Create Methods
        private void AddNewClient(object? parameter)
        {
            // Data validation
            if (NewClient == null)
            {
                MessageBox.Show($"Client data is missing");
                return;
            }                
            else if (!NewClientsEmptyFieldsValidation(NewClient))
            {
                MessageBox.Show($"All Client fields are required.");
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Duplicates validation
            else if (FindClientByName(NewClient))
            {
                MessageBox.Show($"The inserted Client already exists on the DB.");
                return;
            }

            // If all previous validations are ok then insert data on the DB
            if (_dbRepository.AddNewClient(NewClient)>0)
                MessageBox.Show($"The new Client {NewClient.ClientName} was properly added to the DB.");
            else
                MessageBox.Show($"Any new client was not added to the DB. Please check with the suport!");
        }        
        private void AddNewOrder(object? parameter)
        {
            // Data validation
            if (NewOrder == null)
            {
                MessageBox.Show($"Order data is missing");
                return;
            }
            else if (!NewOrderEmptyFieldsValidation(NewOrder))
            {
                MessageBox.Show($"All Order fields are required.");
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Client Id validation
            else if (!FindClientByOrderId(NewOrder.CClient))
            {
                MessageBox.Show($"The order cannot be created because there is no client " +
                                $"with the ID you entered as CClient.\n" +
                                $"Pleas select an exising Client ID and try again.");
                return;
            }

            // If all previous validations are ok then insert data on the DB
            if (_dbRepository.AddNewOrder(NewOrder) > 0)
                MessageBox.Show($"The new Order was properly added to the DB.");
            else
                MessageBox.Show($"Any new order was not added to the DB. Please check with the support!");
        }
        private void AddNewArticle(object? parameter)
        {
            // Data validation
            if (NewArticle == null)
            {
                MessageBox.Show($"Article data is missing");
                return;
            }
            else if (!NewArticlesEmptyFieldsValidation(NewArticle))
            {
                MessageBox.Show($"All Article fields are required.");
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Duplicates validation
            else if (FindArticleByName(NewArticle))
            {
                MessageBox.Show($"The inserted Article already exists on the DB.");
                return;
            }

            // If all previous validations are ok then insert data on the DB
            if (_dbRepository.AddNewArticle(NewArticle) > 0)
                MessageBox.Show($"The new Article {NewArticle.ArticleName} was properly added to the DB.");
            else
                MessageBox.Show($"Any new article was not added to the DB. Please check with the support!");
        }
        #endregion
        #region Update Methods

        #endregion
        #region Delete Methods

        #endregion

    }
}
