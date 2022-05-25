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
using System.Windows.Shapes;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;

namespace UI {
	/// <summary>
	/// Interaction logic for Reservaties.xaml
	/// </summary>
	public partial class DashbordWindow : Window {
		public Visibility IsAdmin { get; } = Visibility.Collapsed;

		private readonly DomeinController domeinController;
		private List<int> beschikbareUren;
		private readonly List<ReservatieInfo> reservatieInfo = new();

		public DashbordWindow(DomeinController _domeincontroller) {
			domeinController = _domeincontroller;
			InitializeComponent();
			this.DataContext = this;
			LaadAccountData();

			if (domeinController.isAdmin) {
				IsAdmin = Visibility.Visible;
				LaadAdminDataInDashBord();
			}

			List<string> reservaties = domeinController.GeefKlantReservaties(true);
			reservatieInfo = reservaties.Select(r => new ReservatieInfo(r)).ToList();
			UpdateToestelList();
		}

		private void LaadAdminDataInDashBord() {
			AdminListBox.Items.Clear();
			List<string> toestellenRaw = domeinController.GeefToestellenZonderReservatie();
			List<ToestelInfo> toestelInfo = toestellenRaw.Select(t => new ToestelInfo(t)).ToList();

			toestelInfo.ForEach(to => AdminListBox.Items.Add(to));
		}

		private void UpdateToestelList() {
			ToestelComboBox.ItemsSource = new List<string>();

			List<string> beschikbareToestellen = domeinController.GeefBeschikbareToestellen();
			ToestelComboBox.ItemsSource = beschikbareToestellen;
		}

		private void LaadAccountData() {
			string klantenInfo = domeinController.KlantOmschrijvingParsable;

			List<string> klantenInfoParsed = klantenInfo.Split("@|@").ToList();
			List<string> interesses = klantenInfoParsed.Last().Split("#|#", StringSplitOptions.RemoveEmptyEntries).ToList();

			//string klantenNummer = klantenInfoParsed[0] ?? "Geen klantennummer beschikbaar";
			string voornaam = klantenInfoParsed[1] ?? "Geen voornaam gekozen"; ;
			string achternaam = klantenInfoParsed[2] ?? "Geen achternaam gekozen"; ;
			string email = klantenInfoParsed[3] ?? "Geen email gekozen"; ;
			bool IsGeboorteDatumok = DateTime.TryParse(klantenInfoParsed[4], out DateTime geboorteDatum);
			if (IsGeboorteDatumok) {
				DashBordGeboorteDatumTextBox.Content = geboorteDatum.ToString("dd/MM/yyyy");
			} else
				DashBordGeboorteDatumTextBox.Content = "Geboortedatum niet correct ingesteld";

			string abonnement = klantenInfoParsed[5] ?? "Geen abonnement gekozen";
			string straatNaam = klantenInfoParsed[6] ?? "Geen straat naam gekozen";
			string huisNummer = klantenInfoParsed[7] ?? "Geen huis nummer gekozen";
			string plaats = klantenInfoParsed[8] ?? "Geen plaats gekozen";
			string postcode = klantenInfoParsed[9] ?? "Geen postcode gekozen";

			DashBordVoornaamTextBox.Content = voornaam;
			DashBordAchternaamTextBox.Content = achternaam;
			DashBordEmailTextBox.Content = email;
			DashBordAbonnementTextBox.Content = abonnement;
			DashBordStraatTextBox.Content = straatNaam;
			DashBordHuisNummerTextBox.Content = huisNummer;
			DashBordPlaatsTextBox.Content = plaats;
			DashBordPostcodeTextBox.Content = postcode;
			DashBordInteressesTextBox.Content = (string.Join(", ", interesses).Trim() != string.Empty) ? string.Join(", ", interesses) : "Geen Interesses";
		}

		private void ToestelGekozen(object sender, SelectionChangedEventArgs e) {
			TijdSlotComboBox.Items.Clear();
			DatumComboBox.Items.Clear();

			if (ToestelComboBox.SelectedIndex != -1) {
				List<DateTime> beschikbareDagen = domeinController.GeefBeschikbareDagen();
				beschikbareDagen.ForEach(datum => DatumComboBox.Items.Add(datum.ToString("d")));
			}

			ReservatieButtonKlikbaarMaken(null, null);
		}

		private void DatumGekozen(object sender, SelectionChangedEventArgs e) {
			TijdSlotComboBox.Items.Clear();

			if (DatumComboBox.SelectedIndex != -1 && DatumComboBox.SelectedItem != null) {
				bool isDateTimeOk = DateTime.TryParse(DatumComboBox.SelectedItem.ToString(), out DateTime dateTime);

				if (isDateTimeOk) {
					DateTime dag = new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
					bool heeftNogGeenVierReservaties;
					(beschikbareUren, heeftNogGeenVierReservaties) = domeinController.GeefBeschikbareReservatieUren(dag, ToestelComboBox.SelectedItem.ToString());

					if (beschikbareUren.Count != 0) {
						if (heeftNogGeenVierReservaties) {
							for (int i = 0; i < beschikbareUren.Count; i++) {
								string item = beschikbareUren[i].ToString().Length == 1 ? $"0{beschikbareUren[i]}" : beschikbareUren[i].ToString();
								TijdSlotComboBox.Items.Add($"{item}:00");
							}
						}
					} else {
						if (!heeftNogGeenVierReservaties)
							MessageBox.Show(" [max 4 reservaties per dag]", "Foutje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
						else
							MessageBox.Show(" [geen reservaties meer beschikbaar]", "Foutje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				}
			}

			ReservatieButtonKlikbaarMaken(null, null);
		}

		private void ReservatieButtonKlikbaarMaken(object sender, SelectionChangedEventArgs e) {
			if (ToestelComboBox.SelectedIndex != -1 && DatumComboBox.SelectedIndex != -1 && TijdSlotComboBox.SelectedIndex != -1) {
				ReservatieButton.IsEnabled = true;
			} else
				ReservatieButton.IsEnabled = false;
		}

		private void NieuweReservatieButton(object sender, RoutedEventArgs e) {
			if (ToestelComboBox.SelectedIndex != -1 && DatumComboBox.SelectedIndex != -1 && TijdSlotComboBox.SelectedIndex != -1) {
				bool isDatumGoed = DateTime.TryParse(DatumComboBox.SelectedItem.ToString(), out DateTime dag);
				if (isDatumGoed) {

					int _tijdSlot = beschikbareUren[TijdSlotComboBox.SelectedIndex];
					string toestelNaam = ToestelComboBox.SelectedItem.ToString();
					DateTime tijdSlot = new DateTime(dag.Year, dag.Month, dag.Day, _tijdSlot, 0, 0);
					int? toestelId = domeinController.GeefEenVrijToestelIdOpNaam(toestelNaam, tijdSlot);
					if (toestelId != null) {
						ResetKeuze();
						string reservatie = domeinController.VoegReservatieToe(tijdSlot, (int)toestelId);
						reservatieInfo.Add(new(reservatie));
						LaadReservaties(null, null);
					} else {
						MessageBox.Show("Geen toesel gevonden.");
					}
				} else
					MessageBox.Show($"Datum ({DatumComboBox.SelectedItem}) is niet correct.");
			} else
				MessageBox.Show("Niet alles is ingevult.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void ResetKeuze() {
			ToestelComboBox.SelectedIndex = -1;
			DatumComboBox.SelectedIndex = -1;
			TijdSlotComboBox.SelectedIndex = -1;
		}

		private void VerwijderToestelButton(object sender, RoutedEventArgs e) {
			var toestelRaw = (sender as Button).DataContext;
			ToestelInfo toestel = AdminListBox.Items.GetItemAt(AdminListBox.Items.IndexOf(toestelRaw)) as ToestelInfo;

			var confirmMessage = MessageBox.Show($"Ben je zeker dat je het toestel: [{toestel.ToestelNaam} | #{toestel.ToestelNummer}] Verwijderen?", "Pas Op", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (confirmMessage == MessageBoxResult.Yes) {
				domeinController.VerwijderToestelOpId(toestel.ToestelNummer);
			}

			LaadAdminDataInDashBord();
		}

		private void ZetInHerstellingButton(object sender, RoutedEventArgs e) {
			var toestelRaw = (sender as Button).DataContext;
			ToestelInfo toestel = AdminListBox.Items.GetItemAt(AdminListBox.Items.IndexOf(toestelRaw)) as ToestelInfo;

			domeinController.ZetToestelInOfUitHerstelling(toestel.ToestelNummer);
			toestel.InHerstelling = !toestel.InHerstelling;
			var txt = (sender as Button);

			if (toestel.InHerstelling)
				txt.Content = "Nee";
			else
				txt.Content = "Ja";
		}

		private void VoegToestelToe(object sender, RoutedEventArgs e) {
			string toestelNaam = ToestelToevoegenTextBox.Text.Trim();

			if (!string.IsNullOrEmpty(toestelNaam)) {
				ToestelToevoegenTextBox.Text = string.Empty;
				domeinController.VoegNieuwToestelToe(toestelNaam);
				LaadAdminDataInDashBord();
			}

		}

		private void Logout(object sender, MouseButtonEventArgs e) {
			domeinController.Logout();
			LoginWindow loginWindow = new(domeinController);
			loginWindow.Title = "Login";
			loginWindow.Show();
			this.Close();
		}

		private void LaadReservaties(object sender, MouseButtonEventArgs e) {
			UpdateToestelList();
			ReservatieListBox.ItemsSource = new List<string>();
			ReservatieListBox.ItemsSource = reservatieInfo.OrderByDescending(reservatie => reservatie.Datum).ThenByDescending(reservatie => reservatie.StartTijd).ToList();
		}

		private void EnableVoegToestelToe(object sender, TextChangedEventArgs e) {
			if (!string.IsNullOrEmpty(ToestelToevoegenTextBox.Text)) {
				VoegToeBtn.IsEnabled = true;
			} else
				VoegToeBtn.IsEnabled = false;
		}

		private void LaadAdmin(object sender, MouseButtonEventArgs e) {
			LaadAdminDataInDashBord();
		}

		private void VerwijderReservatie(object sender, MouseButtonEventArgs e) {
			if (ReservatieListBox.SelectedItem == null) return;

			var reservatie = ReservatieListBox.SelectedItem as ReservatieInfo;
			var result = MessageBox.Show($"Ben je zeker?\n\n({reservatie.Id}) - {reservatie.Toestel} (-> {reservatie.StartTijd.ToString("HH:mm")} -> {reservatie.EindTijd.ToString("HH:mm")}", "Opgepast", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes) {
				domeinController.VerwijderReservatie(reservatie.Id);
				LaadReservaties(null, null);
			}
		}

		string content = "";
		string contentKleur = "";
		private void ChangeToVerwijderBtn(object sender, MouseEventArgs e) {
			var border = sender as Border;
			var label = border.Child as Label;

			label.Cursor = Cursors.Hand;

			if (label.Content.ToString() != " VERWIJDER ") {
				content = label.Content.ToString();
				contentKleur = border.Background.ToString();
				label.Content = " VERWIJDER ";
				border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED5959"));
			} else {
				label.Content = $"{content}";
				border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(contentKleur));
			}
		}
	}
}
