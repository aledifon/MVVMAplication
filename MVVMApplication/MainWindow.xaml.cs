using MVVMApplication.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MVVMApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TabItem? _previoustab = null;

        public MainWindow()
        {
            InitializeComponent();            
            DataContext = new MainWindowViewModel();
        }

        #region Event Handlers
        // Event handler to detect switching between the Tabs of the TabControl.
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is TabControl tabControl && DataContext is MainWindowViewModel vm)
            {
                if(tabControl.SelectedItem is TabItem selectedTab && selectedTab != _previoustab)
                {
                    CleanRefsOfSelectedData(vm);
                    _previoustab = selectedTab;
                }                
            }
        }

        // Event handler to detect lost of focus of the Tab Control.
        // Only will be clean when TabControl has really lost its focus
        private void TabControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TabControl tabControl && DataContext is MainWindowViewModel vm)
            {
                if (!tabControl.IsKeyboardFocusWithin)
                    CleanRefsOfSelectedData(vm);
            }            
        }
        #endregion

        private void CleanRefsOfSelectedData(MainWindowViewModel? vm) 
        {
            if (vm is null) return;

            vm.SelectedClient = null;
            vm.SelectedOrder = null;
            vm.SelectedArticle = null;
        }
    }
}