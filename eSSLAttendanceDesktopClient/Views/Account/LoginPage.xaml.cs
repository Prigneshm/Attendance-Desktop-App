using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Service;
using eSSLAttendanceDesktopClient.Utility;
using eSSLAttendanceDesktopClient.Views.Menu;

namespace eSSLAttendanceDesktopClient.Views.Account
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    /// 
    public partial class LoginPage : Page
    {
        #region [Object]
        private readonly Infrastructure.IService.IAuthenticationService authenticationService;

        public bool isRememberMe = Properties.Settings.Default.IsRememberMe;
        #endregion

        #region [CTOR]
        public LoginPage()
        {
            InitializeComponent();
            authenticationService = new AuthenticationService();
            LoadUserSettings();
        }
        #endregion

        #region [Methods]
        private void LoadUserSettings()
        {
            if (isRememberMe)
            {
                txtEmail.Text = Properties.Settings.Default.EmailAddress;
                txtPassword.Password = Properties.Settings.Default.Password;
                isRememberMe = Properties.Settings.Default.IsRememberMe;
                imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_checkbox.png"));
            }
            else
            {
                txtEmail.Text = string.Empty;
                txtPassword.Password = string.Empty;
                isRememberMe = false;
                imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_uncheckbox.png"));
            }
        }

        private bool IsValidData()
        {
            bool isvalid = false;
            if (txtEmail.Text.IsNullOrEmpty() && txtPassword.Password.IsNullOrEmpty())
            {
                EreEmail.Visibility = Visibility.Visible;
                ErePassword.Visibility = Visibility.Visible;
            }
            else if (txtEmail.Text.IsNullOrEmpty())
            {
                EreEmail.Visibility = Visibility.Visible;
            }
            else if (txtPassword.Password.IsNullOrEmpty())
            {
                ErePassword.Visibility = Visibility.Visible;
            }
            else
            {
                isvalid = true;
            }
            return isvalid;
        }

        private async void AuthenticateUser()
        {
            try
            {
                if (IsValidData())
                {
                    var mAuthentication = new Authentication();
                    mAuthentication.EmailAddress = txtEmail.Text;
                    mAuthentication.Password = txtPassword.Password;
                    loaderView.Visibility = Visibility.Visible;
                    await Task.Delay(100);
                    var mUser = await authenticationService.Authenticate(mAuthentication);
                    if (mUser != null)
                    {
                        if (isRememberMe)
                        {
                            Properties.Settings.Default.EmailAddress = txtEmail.Text;
                            Properties.Settings.Default.Password = txtPassword.Password;
                            Properties.Settings.Default.IsRememberMe = true;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            Properties.Settings.Default.EmailAddress = string.Empty;
                            Properties.Settings.Default.Password = string.Empty;
                            Properties.Settings.Default.IsRememberMe = false;
                            Properties.Settings.Default.Save();
                        }
                        NavigationService.Navigate(new HomePage());
                    }
                    loaderView.Visibility = Visibility.Hidden;

                }
            }
            catch (Exception ex)
            {
                loaderView.Visibility = Visibility.Hidden;
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }
        #endregion

        #region [Events]
        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (imgCheckBox.Source.ToString().Contains("icon_uncheckbox.png"))
            {
                imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_checkbox.png"));
                isRememberMe = true;
            }
            else
            {
                imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_uncheckbox.png"));
                isRememberMe = false;
            }
        }

        private void stkRememberMe_MouseEnter(object sender, MouseEventArgs e)
        {
            if (imgCheckBox.Source.ToString().Contains("icon_uncheckbox.png"))
            {
                imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_checkbox.png"));
                isRememberMe = true;
            }
            else
            {
                imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_uncheckbox.png"));
                isRememberMe = false;
            }
        }

        private void bdrLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AuthenticateUser();
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AuthenticateUser();
            }
        } 
        #endregion
    }
}
