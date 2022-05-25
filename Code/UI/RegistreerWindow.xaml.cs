using Domein;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI {
	/// <summary>
	/// Interaction logic for Registreer.xaml
	/// </summary>
	public partial class RegistreerWindow : Window {
		private DomeinController domeinController;
		private List<string> interesses = new();
		private DateTime geboorteDatum;

		private string voornaam = string.Empty;
		private string achternaam = string.Empty;
		private string email = string.Empty;
		private string typeKlant = string.Empty;
		private string straat = string.Empty;
		private string plaats = string.Empty;
		private string huisnummer = string.Empty;
		private int postcode = 0;

		private string errorString = string.Empty;

		public RegistreerWindow(DomeinController _domeinController) {
			domeinController = _domeinController;
			InitializeComponent();
			this.DataContext = this;
		}

		private string _voornaamTextBoxPlaceholder = "Voornaam";
		public string VoornaamTextBoxPlaceholder { get => _voornaamTextBoxPlaceholder; set { _voornaamTextBoxPlaceholder = value; } }

		private string _achternaamTextBoxPlaceholder = "Achternaam";
		public string AchternaamTextBoxPlaceholder { get => _achternaamTextBoxPlaceholder; set { _achternaamTextBoxPlaceholder = value; } }

		private string _emailTextBoxPlaceholder = "Email";
		public string EmailTextBoxPlaceholder { get => _emailTextBoxPlaceholder; set { _emailTextBoxPlaceholder = value; } }

		private string _geboorteDatumTextBoxPlaceholder = $"{DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year}";
		public string GeboorteDatumTextBoxPlaceholder { get => _geboorteDatumTextBoxPlaceholder; set { _geboorteDatumTextBoxPlaceholder = value; } }

		private string _straatTextBoxPlaceholder = "Straat";
		public string StraatTextBoxPlaceholder { get => _straatTextBoxPlaceholder; set { _straatTextBoxPlaceholder = value; } }

		private string _huisnummerTextBoxPlaceholder = "Huisnummer";
		public string HuisnummerTextBoxPlaceholder { get => _huisnummerTextBoxPlaceholder; set { _huisnummerTextBoxPlaceholder = value; } }

		private string _plaatsTextBoxPlaceholder = "Plaats";
		public string PlaatsTextBoxPlaceholder { get => _plaatsTextBoxPlaceholder; set { _plaatsTextBoxPlaceholder = value; } }

		private string _postcodeTextBoxPlaceholder = "Postcode";
		public string PostcodeTextBoxPlaceholder { get => _postcodeTextBoxPlaceholder; set { _postcodeTextBoxPlaceholder = value; } }

		private void AchternaamTextBox_GotFocus(object sender, RoutedEventArgs e) {
			SetTextBoxPlaceholder(AchternaamTextBox, AchternaamTextBoxPlaceholder);
		}

		private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e) {
			SetTextBoxPlaceholder(EmailTextBox, EmailTextBoxPlaceholder);
		}

		private void VoornaamTextBox_GotFocus(object sender, RoutedEventArgs e) {
			SetTextBoxPlaceholder(VoornaamTextBox, VoornaamTextBoxPlaceholder);
		}

		private void HuisnummerTextBox_GotFocus(object sender, RoutedEventArgs e) {
			SetTextBoxPlaceholder(HuisnummerTextBox, HuisnummerTextBoxPlaceholder);
		}

		private void StraatTextBox_GotFocus(object sender, RoutedEventArgs e) {
			SetTextBoxPlaceholder(StraatnaamTextBox, StraatTextBoxPlaceholder);
		}

		private void PlaatsTextBox_GotFocus(object sender, RoutedEventArgs e) {
			SetTextBoxPlaceholder(PlaatsTextBox, PlaatsTextBoxPlaceholder);
		}

		private void PostcodeTextBox_GotFocus(object sender, RoutedEventArgs e) {
			SetTextBoxPlaceholder(PostcodeTextBox, PostcodeTextBoxPlaceholder);
		}

		private void SetTextBoxPlaceholder(TextBox textBox, string value) {
			if (textBox.Text.Trim() == string.Empty)
				textBox.Text = value;
			else if (textBox.Text.Trim().ToLower() == value.ToLower())
				textBox.Text = string.Empty;
		}

		private void ToonVerdereVelden(object sender, RoutedEventArgs e) {

			bool isGeboorteDatumOk = DateTime.TryParse(GeboorteDatumTextBox.Text, out geboorteDatum);
			string input;

			if (!isGeboorteDatumOk) errorString = $"GeboorteDatum is niet in het juiste formaat.\n B.v. {DateTime.Now.Day}/{ DateTime.Now.Month}/{ DateTime.Now.Year}\n\n";

			input = VoornaamTextBox.Text.Trim();
			if (!string.IsNullOrEmpty(input) && input.Length > 2 && input != VoornaamTextBoxPlaceholder) {
				voornaam = input;
			} else if (input.Length <= 2) {
				errorString += "Voornaam moet langer dan 2 characters zijn.\n\n";
			} else errorString += "Voornaam mag niet leeg zijn.\n\n";

			input = AchternaamTextBox.Text.Trim();
			if (!string.IsNullOrEmpty(input) && input.Length > 2 && input != AchternaamTextBoxPlaceholder) {
				achternaam = input;
			} else if (input.Length <= 2) {
				errorString += "Achternaam moet langer dan 2 characters zijn.\n\n";
			} else errorString += "Achternaam mag niet leeg zijn.\n\n";

			input = EmailTextBox.Text.Trim();
			if (!string.IsNullOrEmpty(input) && input.Contains("@") && input != EmailTextBoxPlaceholder) {
				email = input;
			} else if (!input.Contains("@")) {
				errorString += "Email is niet geldig.\n\n";
			} else errorString += "Email mag niet leeg zijn.\n\n";

			input = AbonnementComboBox.Text.Trim();
			if (!string.IsNullOrEmpty(input) && input != "Abonnement") {
				typeKlant = input;
			} else errorString += "Gelieve een abonnement te kiezen uit de lijst.\n\n";

			input = InteresseComboBox.Text.Trim();
			if (!string.IsNullOrEmpty(input) && input != "Interesse") {
				interesses.Add(input);
			} else errorString += "Gelieve een interesse te kiezen uit de lijst.\n\n";

			if (errorString.Length != 0) {
				MessageBox.Show(errorString, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				errorString = string.Empty;
			} else {
				KlantenAside.Visibility = Visibility.Collapsed;
				AdresAside.Visibility = Visibility.Visible;
			}
		}

		private void GeboorteDatumCheck(object sender, TextCompositionEventArgs e) {
			Regex regex = new Regex("[^0-9-/]+");
			if (regex.IsMatch(e.Text))
				e.Handled = true;
		}
		private void PostCodeCheck(object sender, TextCompositionEventArgs e) {
			Regex regex = new Regex("[^0-9]+");
			if (regex.IsMatch(e.Text))
				e.Handled = true;
		}

		private void RegistreerKlant(object sender, RoutedEventArgs e) {
			string input;
			errorString = string.Empty;

			input = HuisnummerTextBox.Text.Trim();
			if (!string.IsNullOrEmpty(input) && input.Length < 6 && input != HuisnummerTextBoxPlaceholder) {
				huisnummer = input;
			} else errorString += "Huis nummer mag niet leeg zijn.\n\n";

			input = StraatnaamTextBox.Text.Trim();
			if (!string.IsNullOrEmpty(input) && input.Length > 3 && input != StraatTextBoxPlaceholder) {
				straat = input;
			} else if (input.Length <= 3) {
				errorString += "Straat naam moet langer dan 3 characters zijn.\n\n";
			} else errorString += "Straat naam mag niet leeg zijn.\n\n";

			input = PlaatsTextBox.Text.Trim();
			if (!string.IsNullOrEmpty(input) && input.Length > 3 && input != PlaatsTextBoxPlaceholder) {
				plaats = input;
			} else if (input.Length <= 3) {
				errorString += "Plaats moet langer dan 3 characters zijn.\n\n";
			} else errorString += "Plaats mag niet leeg zijn.\n\n";

			input = PostcodeTextBox.Text.Trim();
			if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int parsed) && input != PostcodeTextBoxPlaceholder) {
				postcode = parsed;
			} else if (string.IsNullOrEmpty(input)) {
				errorString += "Postcode mag niet leeg zijn.\n\n";
			} else errorString += $"{input} is geen geldige postcode.\n\n";

			if (errorString.Length != 0) {
				MessageBox.Show(errorString, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				errorString = string.Empty;
			} else {
				domeinController.RegistreerKlant(voornaam, achternaam, email, geboorteDatum, interesses, typeKlant, straat, plaats, huisnummer, postcode);

				domeinController.Login(email);

				DashbordWindow dashbord = new(domeinController);
				dashbord.Title = "Dashboard";
				dashbord.Show();
				this.Close();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			LoginWindow loginWindow = new(domeinController);
			loginWindow.Show();
			this.Close();
		}

		public void GaTerug(object sender, RoutedEventArgs e) {
			AdresAside.Visibility = Visibility.Collapsed;
			KlantenAside.Visibility = Visibility.Visible;
		}
	}
}
