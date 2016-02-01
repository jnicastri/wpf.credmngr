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
using System.Windows.Shapes;
using CredManager.Data;

namespace WpfApplication1.InputWindows
{
    /// <summary>
    /// Interaction logic for NewEntryBox.xaml
    /// </summary>
    public partial class NewEntryBox : Window
    {
        private bool _isNew;
        private string _key;
        private string _uname;
        private string _pwd;
        private CredManager.Data.CredManager _cMgr;
        private CredItem _updatingCItem;

        public event EventHandler<StatusEventArg> OnDatasourceUpdate;

        public string BxKey { get { return KeyNameTb.Text ?? String.Empty; } set { _key = value; } }
        public string BxUName { get { return UsernameTb.Text ?? String.Empty; } set { _uname = value; } }
        public string BxPwd { get { return PwdTb.Text ?? String.Empty; } set { _pwd = value; } }

        public NewEntryBox(CredManager.Data.CredManager cMgr)
        {
            InitializeComponent();
            this._cMgr = cMgr;
            _isNew = true;
            btnAddNewOK.Content = "Add";
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        public NewEntryBox(CredManager.Data.CredManager cMgr, CredItem item)
        {
            InitializeComponent();
            this._cMgr = cMgr;
            this._updatingCItem = item;
            _isNew = false;
            btnAddNewOK.Content = "Save";
            KeyNameTb.Text = item.Name.Trim();
            UsernameTb.Text = item.Username.Trim();
            PwdTb.Text = item.Password.Trim();
            this.Title = "Edit " + item.Name;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void btnAddNewOK_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(BxKey) || String.IsNullOrEmpty(BxUName) || String.IsNullOrEmpty(BxPwd))
                MessageBox.Show("All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                if (this._isNew)
                {
                    _cMgr.Add(new CredItem() { Name = BxKey.Trim(), Username = BxUName.Trim(), Password = BxPwd });
                    OnDatasourceUpdate(this, new StatusEventArg() { Message = "New Item Added and Stored" });
                }
                else if(!this._isNew && this._updatingCItem != null)
                {
                    this._updatingCItem.Name = BxKey.Trim();
                    this._updatingCItem.Username = BxUName.Trim();
                    this._updatingCItem.Password = BxPwd.Trim();
                    _cMgr.SaveCurrentState();
                    OnDatasourceUpdate(this, new StatusEventArg() { Message = "Record Updated: " + this._updatingCItem.Name });
                }
                
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
