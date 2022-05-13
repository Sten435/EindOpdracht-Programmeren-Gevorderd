using System;
using Domein;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window {
		private static DomeinController domeinController;

		private string _emailTextBoxPlaceholder = "Email";
		public string EmailTextBoxPlaceholder { get => _emailTextBoxPlaceholder; set { _emailTextBoxPlaceholder = value; } }

		public LoginWindow(DomeinController _domeinController) {
			domeinController = _domeinController;
			InitializeComponent();
		}

		private void OpenRegistreerWindow(object sender, RoutedEventArgs e) {
			RegistreerWindow registreerWindow = new RegistreerWindow(domeinController);
			registreerWindow.Show();
			this.Close();
		}

		private void LoginButton(object sender, RoutedEventArgs e) {
			string email = emailTextBox.Text.ToLower().Trim();
			domeinController.Login(email);

			if (domeinController.LoggedIn) {
				DashbordWindow dashbordWindow = new DashbordWindow(domeinController);
				dashbordWindow.Title = "Dashbord";
				dashbordWindow.Show();
				this.Close();
			}
		}

		private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e) {
			SetTextBoxPlaceholder(emailTextBox, EmailTextBoxPlaceholder);
		}

		private void SetTextBoxPlaceholder(TextBox textBox, string value) {
			if (textBox.Text.Trim() == string.Empty)
				textBox.Text = value;
			else if (textBox.Text.Trim().ToLower() == value.ToLower())
				textBox.Text = string.Empty;
		}
	}
}
