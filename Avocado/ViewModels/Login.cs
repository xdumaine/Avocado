using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MetroMVVM;
using MetroMVVM.Commands;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net;
using Avocado.DataModel;
using MetroMVVM.Interfaces;

namespace Avocado.ViewModel
{
    class Login : ObservableObject
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public AuthClient AuthClient { get; set; }
        

        public Login()
        {
            Email = "xander.dumaine@gmail.com";
            Password = "T12lion20";
        }

        public void AttemptLogin()
        {
            AuthClient = new AuthClient();
            AuthClient.Email = Email;
            AuthClient.Password = Password;
            if (AuthClient.AttemptLogin())
            {
                var couple = AuthClient.GetUsers();
                if (couple.CurrentUser != null)
                {
                    NavigationService n = new NavigationService();
                    n.NavigateTo(typeof(Avocado.Views.Activities), AuthClient);
                }
            }
        }

        public ICommand AttemptLoginCommand
        {
            get
            {
                return new RelayCommand(() => { AttemptLogin(); });
            }
        }
    }
}
