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
using WpfApplication1.InputWindows;
using CredManager.Data;

namespace WpfApplication1.UserControls
{
    /// <summary>
    /// Interaction logic for ListViewCtrl.xaml
    /// </summary>
    public partial class ListViewCtrl : UserControl
    {
        private CredManager.Data.CredManager cMgr;
        public event EventHandler<StatusEventArg> OnUpdateStatusBarMsg;
        CollectionView view;
       

        public ListViewCtrl(CredManager.Data.CredManager cMgr)
        {
            InitializeComponent();
            this.cMgr = cMgr;
            
            this.DataViewRes.ItemsSource = this.cMgr.CredItems.OrderBy(i => i.Name);

            this.view = (CollectionView)CollectionViewSource.GetDefaultView(DataViewRes.ItemsSource);
            this.view.Filter = FilterOn;

        }

        private void SearchBx_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(DataViewRes.ItemsSource).Refresh();
        }

        private bool FilterOn(object item)
        {
            if (String.IsNullOrEmpty(SearchBx.Text))
                return true;
            else
                return (item as CredItem).Name.IndexOf(SearchBx.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void UpdateDatasourceFromChild(object sender, StatusEventArg e)
        {
            this.DataViewRes.ItemsSource = this.cMgr.CredItems;
            
            this.view = (CollectionView)CollectionViewSource.GetDefaultView(DataViewRes.ItemsSource);
            this.view.Filter = FilterOn;
            this.view.Refresh();
            
            //TODO: Force a rebind of the menu item command parameters here!

            OnUpdateStatusBarMsg(this, e);
        }

        #region Context Menu Events

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string param = item.CommandParameter.ToString();
            CredItem cItem = cMgr.CredItems.Where(i => i.Name == param).FirstOrDefault();
            Clipboard.SetText(cItem.Password);
            OnUpdateStatusBarMsg(this, new StatusEventArg() { Message = "Password copied to the Clipboard" });
        }

        private void CopyUserNameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string param = item.CommandParameter.ToString();
            CredItem cItem = cMgr.CredItems.Where(i => i.Name == param).FirstOrDefault();
            Clipboard.SetText(cItem.Username);
            OnUpdateStatusBarMsg(this, new StatusEventArg() { Message = "Username copied to the Clipboard" });
        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NewEntryBox newWindow = new NewEntryBox(this.cMgr);
            newWindow.OnDatasourceUpdate += new EventHandler<StatusEventArg>(this.UpdateDatasourceFromChild);
            newWindow.Show();
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are sure you want to delete this record??", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                MenuItem item = sender as MenuItem;
                string param = item.CommandParameter.ToString();
                cMgr.Remove(param);
                CollectionViewSource.GetDefaultView(DataViewRes.ItemsSource).Refresh();
            }

        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            string param = item.CommandParameter.ToString();
            CredItem cItem = cMgr.CredItems.Where(i => i.Name == param).FirstOrDefault();

            NewEntryBox eWindow = new NewEntryBox(this.cMgr, cItem);
            eWindow.OnDatasourceUpdate += new EventHandler<StatusEventArg>(this.UpdateDatasourceFromChild);
            eWindow.Show();
        }
        #endregion

        
    }
}
