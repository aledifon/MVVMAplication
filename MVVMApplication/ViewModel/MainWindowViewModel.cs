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
            GetAllOrdersCommand = new RelayCommand(GetAllOrders);
            GetAllArticlesCommand = new RelayCommand(GetAllArticles);

            InsertClientCommand = new RelayCommand(AddClient, canAddClient);
            InsertOrderCommand = new RelayCommand(AddOrder, canAddOrder);
            InsertArticleCommand = new RelayCommand(AddArticle, canAddArticle);

            UpdateClientCommand = new RelayCommand(UpdateClient, canUpdateClient);
            UpdateOrderCommand = new RelayCommand(UpdateOrder, canUpdateOrder);
            UpdateArticleCommand = new RelayCommand(UpdateArticle, canUpdateArticle);

            // Create instances of every collection
            Clients = new ObservableCollection<Client>();
            Orders = new ObservableCollection<Order>();
            Articles = new ObservableCollection<Article>();

            NewClient = new Client();
            NewOrder = new Order();
            NewArticle = new Article();
        }

        #region Get Commands
        public ICommand GetAllClientsCommand { get; }        
        public ICommand GetAllOrdersCommand { get; }        
        public ICommand GetAllArticlesCommand { get; }
        #endregion
        #region Insert Commands
        public ICommand InsertClientCommand { get; }
        public ICommand InsertOrderCommand { get; }
        public ICommand InsertArticleCommand { get; }
        #endregion

        #region Update Commands
        public ICommand UpdateClientCommand { get; }
        public ICommand UpdateOrderCommand { get; }
        public ICommand UpdateArticleCommand { get; }
        #endregion

        #region Checking Methods
        private int FindClientByName(Client client) => _dbRepository.FindClientByName(client);
        private int FindArticleByName(Article article) => _dbRepository.FindArticleByName(article);
        private int FindClientByOrderId(int clientId) => _dbRepository.FindClientByOrderId(clientId);
        #endregion

        #region Data Validation Methods
        private bool AreClientFieldsFilled(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.ClientName)) return false;
            if (string.IsNullOrWhiteSpace(client.Address)) return false;
            if (string.IsNullOrWhiteSpace(client.Location)) return false;
            if (string.IsNullOrWhiteSpace(client.Telephone)) return false;

            return true;
        }
        private bool AreArticleFieldsFilled(Article article)
        {
            if (string.IsNullOrWhiteSpace(article.Section)) return false;
            if (string.IsNullOrWhiteSpace(article.ArticleName)) return false;
            if (article.Price <= 0) return false;
            if (article.Date == null || article.Date > DateTime.Today) return false;
            if (string.IsNullOrWhiteSpace(article.OriginCountry)) return false;

            return true;
        }
        private bool AreOrderFieldsFilled(Order order)
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
        private void AddClient(object? parameter)
        {
            // Data validation
            if (NewClient == null)
            {
                MessageBox.Show($"Client data is missing");
                return;
            }                
            else if (!AreClientFieldsFilled(NewClient))
            {
                MessageBox.Show($"All Client fields are required.");
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Duplicates validation
            else if (FindClientByName(NewClient) > 0)
            {
                MessageBox.Show($"The inserted Client already exists on the DB.\n" +
                                $"Please, try with a different Client Name");
                return;
            }

            // If all previous validations are ok then insert data on the DB
            if (_dbRepository.AddClient(NewClient) > 0)
            {
                MessageBox.Show($"The new Client {NewClient.ClientName} was properly added to the DB.");
                GetAllClients(null);
            }
            else
                MessageBox.Show($"No new client was added to the DB. Please check with the suport!");
        }        
        private void AddOrder(object? parameter)
        {
            // Data validation
            if (NewOrder == null)
            {
                MessageBox.Show($"Order data is missing");
                return;
            }
            else if (!AreOrderFieldsFilled(NewOrder))
            {
                MessageBox.Show($"All Order fields are required.");
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Client Id validation
            else if (FindClientByOrderId(NewOrder.CClient) != 1)
            {
                MessageBox.Show($"The order cannot be created because there is no client " +
                                $"with the ID you entered as CClient.\n" +
                                $"Pleas select an exising Client ID and try again.");
                return;
            }

            // If all previous validations are ok then insert data on the DB
            if (_dbRepository.AddOrder(NewOrder) > 0)
            {
                MessageBox.Show($"The new Order was properly added to the DB.");
                GetAllOrders(null);
            }
            else
                MessageBox.Show($"No new order was added to the DB. Please check with the support!");
        }
        private void AddArticle(object? parameter)
        {
            // Data validation
            if (NewArticle == null)
            {
                MessageBox.Show($"Article data is missing");
                return;
            }
            else if (!AreArticleFieldsFilled(NewArticle))
            {
                MessageBox.Show($"All Article fields are required.");
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Duplicates validation
            else if (FindArticleByName(NewArticle) > 0)
            {
                MessageBox.Show($"The inserted Article already exists on the DB.\n" +
                                $"Please, try with a different Article Name");
                return;
            }

            // If all previous validations are ok then insert data on the DB
            if (_dbRepository.AddArticle(NewArticle) > 0)
            {
                MessageBox.Show($"The new Article {NewArticle.ArticleName} was properly added to the DB.");
                GetAllArticles(null);
            }
            else
                MessageBox.Show($"No new article was added to the DB. Please check with the support!");
        }

        private bool canAddClient(object? obj)
        {
            return (NewClient != null && AreClientFieldsFilled(NewClient));
        }
        private bool canAddOrder(object? obj)
        {
            return (NewOrder != null && AreOrderFieldsFilled(NewOrder));
        }
        private bool canAddArticle(object? obj)
        {
            return (NewArticle != null && AreArticleFieldsFilled(NewArticle));
        }
        #endregion
        #region Update Methods
        private void UpdateClient(object? parameter)
        {
            // User Confirmation
            if (MessageBox.Show(
                "Are you sure you want to update the selected Client data?",
                "Update Client Data Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
            {
                GetAllClients(null);
                return;
            }                

            // Data validation
            if (SelectedClient == null)
            {
                MessageBox.Show($"Client data is missing");
                GetAllClients(null);
                return;
            }
            else if (!AreClientFieldsFilled(SelectedClient))
            {
                MessageBox.Show($"All Client fields are required.");
                GetAllClients(null);
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Duplicates validation
            else if (FindClientByName(SelectedClient) > 1)
            {
                MessageBox.Show($"The inserted Client already exists on the DB.\n" +
                                $"Please, try with a different Client Name");
                GetAllClients(null);
                return;
            }
            
            // DB consult
            if (!_dbRepository.UpdateClient(SelectedClient))
            {
                MessageBox.Show($"The client data cannot be updated because there is no client " +
                                $"with the ID you entered .\n" +
                                $"Please select an exising Client and try again.");
                GetAllClients(null);
                return;
            }
            else
                MessageBox.Show($"The Selected client was properly updtated on the DB.");
        }
        private void UpdateOrder(object? parameter)
        {
            // User Confirmation
            if (MessageBox.Show(
                "Are you sure you want to update the selected Order data?",
                "Update Order Data Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
            {
                GetAllOrders(null);
                return;
            }

            // Data validation
            if (SelectedOrder == null)
            {
                MessageBox.Show($"Order data is missing");
                GetAllOrders(null);
                return;
            }
            else if (!AreOrderFieldsFilled(SelectedOrder))
            {
                MessageBox.Show($"All Order fields are required.");
                GetAllOrders(null);
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Client Id validation
            else if (FindClientByOrderId(SelectedOrder.CClient) != 1)
            {
                MessageBox.Show($"The order cannot be updated because there is no client " +
                                $"with the ID you entered as CClient.\n" +
                                $"Please select an exising Client ID and try again.");
                GetAllOrders(null);
                return;
            }

            // DB consult
            if (!_dbRepository.UpdateOrder(SelectedOrder))
            {
                MessageBox.Show($"The order data cannot be updated because there is no order " +
                                $"with the ID you entered .\n" +
                                $"Please select an exising Order and try again.");
                GetAllOrders(null);
                return;
            }
            else
                MessageBox.Show($"The Selected Order was properly updtated on the DB.");
        }
        private void UpdateArticle(object? parameter)
        {
            // User Confirmation
            if (MessageBox.Show(
                "Are you sure you want to update the selected Article data?",
                "Update Article Data Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
            {
                GetAllArticles(null);
                return;
            }

            // Data validation
            if (SelectedArticle == null)
            {
                MessageBox.Show($"Article data is missing");
                GetAllArticles(null);
                return;
            }
            else if (!AreArticleFieldsFilled(SelectedArticle))
            {
                MessageBox.Show($"All Article fields are required.");
                GetAllArticles(null);
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Duplicates validation
            else if (FindArticleByName(SelectedArticle) > 1)
            {
                MessageBox.Show($"The inserted Article already exists on the DB.\n" +
                                $"Please, try with a different Article Name");
                GetAllArticles(null);
                return;
            }

            // DB consult
            if (!_dbRepository.UpdateArticle(SelectedArticle))
            {
                MessageBox.Show($"The article data cannot be updated because there is no article " +
                                $"with the ID you entered .\n" +
                                $"Please, select an exising Article and try again.");
                GetAllArticles(null);
                return;
            }
            else
                MessageBox.Show($"The Selected Article was properly updtated on the DB.");
        }

        private bool canUpdateClient(object? obj)
        {
            return (SelectedClient != null && AreClientFieldsFilled(SelectedClient));            
        }
        private bool canUpdateOrder(object? obj)
        {
            return (SelectedOrder != null && AreOrderFieldsFilled(SelectedOrder));            
        }
        private bool canUpdateArticle(object? obj)
        {
            return (SelectedArticle != null && AreArticleFieldsFilled(SelectedArticle));            
        }
        #endregion
        #region Delete Methods

        #endregion

    }
}
