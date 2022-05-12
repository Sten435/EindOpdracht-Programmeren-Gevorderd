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
		public LoginWindow(DomeinController _domeinController) {
			domeinController = _domeinController;
			InitializeComponent();
		}

		private void OpenRegistreerWindow(object sender, RoutedEventArgs e) {
			RegistreerWindow registreerWindow = new RegistreerWindow();
			registreerWindow.Show();
			this.Close();
		}

		private void LoginButton(object sender, RoutedEventArgs e) {
			string email = emailTextBox.Text.ToLower().Trim();
			domeinController.Login(email);

			MessageBox.Show(domeinController.KlantOmschrijving);
		}
	}
}
