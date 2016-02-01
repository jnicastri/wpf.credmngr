using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using WpfApplication1.UserControls;
using WpfApplication1.InputWindows;
using CredManager.Data;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CredManager.Data.CredManager cMgr = CredManager.Data.CredManager.Instance;
        public ListViewCtrl mainListView;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            Login logInUC = new Login(cMgr);
            logInUC.OnLogInSuccess += new EventHandler(LogInFromUc);
            logInUC.StatusUpdate += new EventHandler<StatusEventArg>(StatusBar_OnUpdate);
            this.MainContentCtrl.Content = logInUC;
            this.StatusBarLbl.Text = "Please Log In";
            this.Closing += new CancelEventHandler(MainWindow_Closing);
        }

        #region Children Events

        public void LogInFromUc(object sender, EventArgs e)
        {
            this.mainListView = new ListViewCtrl(cMgr);
            mainListView.OnUpdateStatusBarMsg += new EventHandler<StatusEventArg>(StatusBar_OnUpdate);
            this.MainContentCtrl.Content = mainListView;
            this.Title = "Cred Manager";
        }

        public void StatusBar_OnUpdate(object sender, StatusEventArg e)
        {
            if(e != null)
                StatusBarLbl.Text = e.Message;
        }

        public void OnDatasourceUpdating(object sender, StatusEventArg e)
        {
            if (this.mainListView != null)
            {
                mainListView.UpdateDatasourceFromChild(sender, e);
                this.StatusBar_OnUpdate(sender, e);
            }
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the application?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
        }

        #endregion

        #region Global Menu events

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (cMgr.IsAuthenticated)
            {
                NewEntryBox newEntry = new NewEntryBox(this.cMgr);
                newEntry.OnDatasourceUpdate += new EventHandler<StatusEventArg>(this.OnDatasourceUpdating);
                newEntry.Show();
            }
            else
                MessageBox.Show("ERROR: Please log in first", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the application?", "Confirm Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                Environment.Exit(0);
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Credential Manager\n\nVersion/Build 1.0.0\n\nAuthor: JN, 2016\n\nThis application does not guarantee to provide an impenetrable level of security. Please do not use it to store extremely important credentials (such as online banking credentials).", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion
    }
}
