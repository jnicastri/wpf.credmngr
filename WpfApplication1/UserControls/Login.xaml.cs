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
using CredManager.Data;

namespace WpfApplication1.UserControls
{
    public partial class Login : UserControl
    {
        private string _currentCred = "";
        private CredManager.Data.CredManager cMgr;

        public event EventHandler OnLogInSuccess;
        public event EventHandler<StatusEventArg> StatusUpdate;

        public Login(CredManager.Data.CredManager cMgr)
        {
            InitializeComponent();
            this.cMgr = cMgr;
        }

        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginKeyIndicator.Text += "\u2022";

            Button trigger = sender as Button;

            string entry = trigger.CommandParameter.ToString();

            if (entry == "C")
            {
                StatusUpdate(this, new StatusEventArg() { Message = "Action: Code Cleared." });
                LoginKeyIndicator.Text = String.Empty;
                _currentCred = String.Empty;
            }
            else if (entry == "X")
                Environment.Exit(0);
            else
            {
                bool onFinalKey = _currentCred.Length == 3;

                _currentCred += entry.ToString();

                if (onFinalKey && cMgr.AuthenticateCredAccess(_currentCred))
                {
                    StatusUpdate(this, new StatusEventArg() { Message = "Action: Login Successful." });
                    OnLogInSuccess(this, EventArgs.Empty);
                }
                else if (onFinalKey && !cMgr.AuthenticateCredAccess(_currentCred))
                {
                    _currentCred = String.Empty;
                    LoginKeyIndicator.Text = String.Empty;
                    StatusUpdate(this, new StatusEventArg() { Message = "Incorrect Code Entered." });
                }
            }
        }
    }
}
