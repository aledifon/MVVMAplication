using MVVMApplication.Model;
using MVVMApplication.MVVM;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
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
                OnPropertyChanged();
                ((AsyncRelayCommand)UpdateClientCommand).   // Force to reevaluate canExecute
                    RaiseCanExecuteChanged();
                ((AsyncRelayCommand)DeleteClientCommand).   
                    RaiseCanExecuteChanged();
            }
        }
        private Client? _newClient;
        public Client? NewClient
        {
            get { return _newClient; }
            set
            {
                _newClient = value;
                OnPropertyChanged();                        
            }
        }

        private Order? _selectedOrder;
        public Order? SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();        
                ((AsyncRelayCommand)UpdateOrderCommand).   // Force to reevaluate canExecute
                    RaiseCanExecuteChanged();
                ((AsyncRelayCommand)DeleteOrderCommand).
                    RaiseCanExecuteChanged();
            }
        }
        private Order? _newOrder;
        public Order? NewOrder
        {
            get { return _newOrder; }
            set
            {
                _newOrder = value;
                OnPropertyChanged();                        
            }
        }

        private Article? _selectedArticle;
        public Article? SelectedArticle
        {
            get { return _selectedArticle; }
            set
            {
                _selectedArticle = value;
                OnPropertyChanged();
                ((AsyncRelayCommand)UpdateArticleCommand).   // Force to reevaluate canExecute
                    RaiseCanExecuteChanged();
                ((AsyncRelayCommand)DeleteArticleCommand).
                    RaiseCanExecuteChanged();
            }
        }
        private Article? _newArticle;
        public Article? NewArticle
        {
            get { return _newArticle; }
            set
            {
                _newArticle = value;
                OnPropertyChanged();                
            }
        }
        #endregion 

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

        #region Delete Commands
        public ICommand DeleteClientCommand { get; }
        public ICommand DeleteOrderCommand { get; }
        public ICommand DeleteArticleCommand { get; }
        #endregion

        public MainWindowViewModel()
        {
            _dbRepository = new DBManager();

            // Create instances of every Command
            GetAllClientsCommand = new AsyncRelayCommand(GetAllClientsAsync);
            GetAllOrdersCommand = new AsyncRelayCommand(GetAllOrdersAsync);
            GetAllArticlesCommand = new AsyncRelayCommand(GetAllArticlesAsync);

            InsertClientCommand = new AsyncRelayCommand(AddClientAsync, canAddClient);
            InsertOrderCommand = new AsyncRelayCommand(AddOrderAsync, canAddOrder);
            InsertArticleCommand = new AsyncRelayCommand(AddArticleAsync, canAddArticle);

            UpdateClientCommand = new AsyncRelayCommand(UpdateClientAsync, canUpdateClient);
            UpdateOrderCommand = new AsyncRelayCommand(UpdateOrderAsync, canUpdateOrder);
            UpdateArticleCommand = new AsyncRelayCommand(UpdateArticleAsync, canUpdateArticle);

            DeleteClientCommand = new AsyncRelayCommand(DeleteClientAsync, canDeleteClient);
            DeleteOrderCommand = new AsyncRelayCommand(DeleteOrderAsync, canDeleteOrder);
            DeleteArticleCommand = new AsyncRelayCommand(DeleteArticleAsync, canDeleteArticle);

            // Create instances of every collection
            Clients = new ObservableCollection<Client>();
            Orders = new ObservableCollection<Order>();
            Articles = new ObservableCollection<Article>();

            NewClient = new Client();
            NewOrder = new Order();
            NewArticle = new Article();
        }

        

        #region Checking Methods
        private async Task<int> FindClientByNameAsync(Client client) => await _dbRepository.FindClientByNameAsync(client);
        private async Task<int> FindArticleByNameAsync(Article article) => await _dbRepository.FindArticleByNameAsync(article);
        private async Task<int> FindClientByOrderIdAsync(int clientId) => await _dbRepository.FindClientByOrderIdAsync(clientId);
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
        private async Task GetAllClientsAsync(object? parameter)
        {
            var tempClients = await _dbRepository.GetAllClientsAsync();

            Clients.Clear();                                    // Doing this instead of replacing the ref. with a new one
                                                                // Otherwise this will break the bindings with the UI and therefore
                                                                // the autom. updates!!
                                        
            foreach(var c in tempClients)
                Clients.Add(c);
        }
        private async Task GetAllOrdersAsync(object? parameter)
        {           
            var tempOrders =  await _dbRepository.GetAllOrdersAsync();

            Orders.Clear();
            foreach (var c in tempOrders)
                Orders.Add(c);
        }
        private async Task GetAllArticlesAsync(object? parameter)
        {
            var tempArticles = await _dbRepository.GetAllArticlesAsync();

            Articles.Clear();
            foreach (var c in tempArticles)
                Articles.Add(c);
        }
        #endregion

        #region Create Methods
        private async Task AddClientAsync(object? parameter)
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
            else if (await FindClientByNameAsync(NewClient) > 0)
            {
                MessageBox.Show($"The inserted Client already exists on the DB.\n" +
                                $"Please, try with a different Client Name");
                return;
            }

            // If all previous validations are ok then insert data on the DB
            if (await _dbRepository.AddClientAsync(NewClient) > 0)
            {
                MessageBox.Show($"The new Client {NewClient.ClientName} was properly added to the DB.");
                await GetAllClientsAsync(null);
            }
            else
                MessageBox.Show($"No new client was added to the DB. Please check with the suport!");
        }        
        private async Task AddOrderAsync(object? parameter)
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
            else if (await FindClientByOrderIdAsync(NewOrder.CClient) != 1)
            {
                MessageBox.Show($"The order cannot be created because there is no client " +
                                $"with the ID you entered as CClient.\n" +
                                $"Pleas select an exising Client ID and try again.");
                return;
            }

            // If all previous validations are ok then insert data on the DB
            if (await _dbRepository.AddOrderAsync(NewOrder) > 0)
            {
                MessageBox.Show($"The new Order was properly added to the DB.");
                await GetAllOrdersAsync(null);
            }
            else
                MessageBox.Show($"No new order was added to the DB. Please check with the support!");
        }
        private async Task AddArticleAsync(object? parameter)
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
            else if (await FindArticleByNameAsync(NewArticle) > 0)
            {
                MessageBox.Show($"The inserted Article already exists on the DB.\n" +
                                $"Please, try with a different Article Name");
                return;
            }

            // If all previous validations are ok then insert data on the DB
            if (await _dbRepository.AddArticleAsync(NewArticle) > 0)
            {
                MessageBox.Show($"The new Article {NewArticle.ArticleName} was properly added to the DB.");
                await GetAllArticlesAsync(null);
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
        private async Task UpdateClientAsync(object? parameter)
        {
            // User Confirmation
            if (MessageBox.Show(
                "Are you sure you want to update the selected Client data?",
                "Update Client Data Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
            {
                await GetAllClientsAsync(null);
                return;
            }                

            // Data validation
            if (SelectedClient == null)
            {
                MessageBox.Show($"Client data is missing");
                await GetAllClientsAsync(null);
                return;
            }
            else if (!AreClientFieldsFilled(SelectedClient))
            {
                MessageBox.Show($"All Client fields are required.");
                await GetAllClientsAsync(null);
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Duplicates validation
            else if (await FindClientByNameAsync(SelectedClient) > 1)
            {
                MessageBox.Show($"The inserted Client already exists on the DB.\n" +
                                $"Please, try with a different Client Name");
                await GetAllClientsAsync(null);
                return;
            }
            
            // DB consult
            if (!(await _dbRepository.UpdateClientAsync(SelectedClient)))
            {
                MessageBox.Show($"The client data cannot be updated because there is no client " +
                                $"with the ID you entered .\n" +
                                $"Please select an exising Client and try again.");
                await GetAllClientsAsync(null);
                return;
            }
            else
                MessageBox.Show($"The Selected client was properly updtated from the DB.");
        }
        private async Task UpdateOrderAsync(object? parameter)
        {
            // User Confirmation
            if (MessageBox.Show(
                "Are you sure you want to update the selected Order data?",
                "Update Order Data Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
            {
                await GetAllOrdersAsync(null);
                return;
            }

            // Data validation
            if (SelectedOrder == null)
            {
                MessageBox.Show($"Order data is missing");
                await GetAllOrdersAsync(null);
                return;
            }
            else if (!AreOrderFieldsFilled(SelectedOrder))
            {
                MessageBox.Show($"All Order fields are required.");
                await GetAllOrdersAsync(null);
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Client Id validation
            else if (await FindClientByOrderIdAsync(SelectedOrder.CClient) != 1)
            {
                MessageBox.Show($"The order cannot be updated because there is no client " +
                                $"with the ID you entered as CClient.\n" +
                                $"Please select an exising Client ID and try again.");
                await GetAllOrdersAsync(null);
                return;
            }

            // DB consult
            if (!(await _dbRepository.UpdateOrderAsync(SelectedOrder)))
            {
                MessageBox.Show($"The order data cannot be updated because there is no order " +
                                $"with the ID you entered .\n" +
                                $"Please select an exising Order and try again.");
                await GetAllOrdersAsync(null);
                return;
            }
            else
                MessageBox.Show($"The Selected Order was properly updtated from the DB.");
        }
        private async Task UpdateArticleAsync(object? parameter)
        {
            // User Confirmation
            if (MessageBox.Show(
                "Are you sure you want to update the selected Article data?",
                "Update Article Data Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
            {
                await GetAllArticlesAsync(null);
                return;
            }

            // Data validation
            if (SelectedArticle == null)
            {
                MessageBox.Show($"Article data is missing");
                await GetAllArticlesAsync(null);
                return;
            }
            else if (!AreArticleFieldsFilled(SelectedArticle))
            {
                MessageBox.Show($"All Article fields are required.");
                await GetAllArticlesAsync(null);
                return;
            }

            // Format Data Validation (Optional)
            // Max Length Validation (Optional)

            // Duplicates validation
            else if (await FindArticleByNameAsync(SelectedArticle) > 1)
            {
                MessageBox.Show($"The inserted Article already exists on the DB.\n" +
                                $"Please, try with a different Article Name");
                await GetAllArticlesAsync(null);
                return;
            }

            // DB consult
            if (!(await _dbRepository.UpdateArticleAsync(SelectedArticle)))
            {
                MessageBox.Show($"The article data cannot be updated because there is no article " +
                                $"with the ID you entered .\n" +
                                $"Please, select an exising Article and try again.");
                await GetAllArticlesAsync(null);
                return;
            }
            else
                MessageBox.Show($"The Selected Article was properly updtated from the DB.");
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
        private async Task DeleteClientAsync(object? parameter)
        {
            // User Confirmation
            if (MessageBox.Show(
                "Are you sure you want to delete the selected Client data?",
                "Deletion client data confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
            {                
                return;
            }

            // Data validation
            if (SelectedClient == null)
            {
                MessageBox.Show($"Client data is missing");             
                return;
            }

            // Associated Orders validation            
            if (await _dbRepository.ClientHasOrdersAsync(SelectedClient.Id))
            {
                if (MessageBox.Show(
                "The selected Client has some orders associated to him/her. " +
                "To delete the customer, you'll first need to delete all their " +
                "associated orders. Are you sure you want to continue?",
                "Deletion asssociated orders data confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
                {
                    await GetAllClientsAsync(null);
                    return;
                }                
            }

            // DB consult (Once The Selected Client has no associated orders anymore)
            if (await _dbRepository.DeleteClientAsync(SelectedClient.Id) == 0)
            {
                MessageBox.Show($"The client data cannot be deleted because there is no client " +
                                $"with the ID you entered .\n" +
                                $"Please select an exising Client and try again.");
                await GetAllClientsAsync(null);
                await GetAllOrdersAsync(null);
            }
            else
            {
                MessageBox.Show($"The Selected client was properly deleted from the DB.");
                await GetAllClientsAsync(null);
                await GetAllOrdersAsync(null);
            }
        }
        private async Task DeleteOrderAsync(object? parameter)
        {
            // User Confirmation
            if (MessageBox.Show(
                "Are you sure you want to delete the selected Order data?",
                "Deletion Order data confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
            {
                await GetAllOrdersAsync(null);
                return;
            }

            // Data validation
            if (SelectedOrder == null)
            {
                MessageBox.Show($"Order data is missing");
                await GetAllOrdersAsync(null);
                return;
            }

            // DB consult
            if (await _dbRepository.DeleteOrderAsync(SelectedOrder.Id) == 0)
            {
                MessageBox.Show($"The Order data cannot be deleted because there is no Order " +
                                $"with the ID you entered .\n" +
                                $"Please select an exising Order and try again.");
                await GetAllOrdersAsync(null);                
            }
            else
            {
                MessageBox.Show($"The Selected Order was properly deleted from the DB.");
                await GetAllOrdersAsync(null);
            }                
        }
        private async Task DeleteArticleAsync(object? parameter)
        {
            // User Confirmation
            if (MessageBox.Show(
                "Are you sure you want to delete the selected Article data?",
                "Deletion Article data confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.No)
            {
                await GetAllArticlesAsync(null);
                return;
            }

            // Data validation
            if (SelectedArticle == null)
            {
                MessageBox.Show($"Article data is missing");
                await GetAllArticlesAsync(null);
                return;
            }

            // DB consult
            if (await _dbRepository.DeleteArticleAsync(SelectedArticle.Id) == 0)
            {
                MessageBox.Show($"The Article data cannot be deleted because there is no Article " +
                                $"with the ID you entered .\n" +
                                $"Please select an exising Article and try again.");
                await GetAllArticlesAsync(null);               
            }
            else
            {
                MessageBox.Show($"The Selected Article was properly deleted from the DB.");
                await GetAllArticlesAsync(null);
            }                
        }
        private bool canDeleteClient(object? obj)
        {
            return (SelectedClient != null && AreClientFieldsFilled(SelectedClient));
        }
        private bool canDeleteOrder(object? obj)
        {
            return (SelectedOrder != null && AreOrderFieldsFilled(SelectedOrder));
        }
        private bool canDeleteArticle(object? obj)
        {
            return (SelectedArticle != null && AreArticleFieldsFilled(SelectedArticle));
        }
        #endregion
    }
}
