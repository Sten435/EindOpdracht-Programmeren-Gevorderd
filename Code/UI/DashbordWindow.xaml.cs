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
using System.Diagnostics;

namespace UI {
	/// <summary>
	/// Interaction logic for Reservaties.xaml
	/// </summary>
	public partial class DashbordWindow : Window {
		public Visibility IsAdmin { get; } = Visibility.Collapsed;

		private readonly DomeinController domeinController;
		private List<int> beschikbareUren;
		private List<ReservatieInfo> reservatieInfo = new();
		private List<ToestelInfo> toestelInfo = new();

		public DashbordWindow(DomeinController _domeincontroller) {
			domeinController = _domeincontroller;
			InitializeComponent();
			this.DataContext = this;
			LaadAccountData();

			if (domeinController.isAdmin) {
				IsAdmin = Visibility.Visible;
				LaadAdminDataInDashBord();
			} else {
				AdminItem.Visibility = Visibility.Collapsed;
				ReservatieItem.IsSelected = true;
			}

			HashSet<string> reservaties = domeinController.GeefKlantReservaties(true);

			if (reservaties.Count > 0) {
				ReservatieListBox.Visibility = Visibility.Visible;
				NogGeenReservatieDockPl.Visibility = Visibility.Collapsed;
			} else {
				NogGeenReservatieDockPl.Visibility = Visibility.Visible;
				ReservatieListBox.Visibility = Visibility.Collapsed;
			}

			reservatieInfo = reservaties.Select(r => new ReservatieInfo(r)).ToList();
			LaadReservaties(null, null);
		}

		private void LaadAdminDataInDashBord() {
			AdminListBox.Items.Clear();
			List<string> toestellenZonderReservatie = domeinController.GeefToestellenZonderReservatie();
			List<string> toestellen = domeinController.GeefAlleToestellen(true);
			List<string> toestellenMetReservatie = toestellen.Except(toestellenZonderReservatie).ToList();
			List<string> toestellenMetReservatieModded = toestellenMetReservatie.Select(toestel => $"{toestel}@|@True").ToList();

			toestellen.RemoveAll(toestel => toestellenMetReservatie.Contains(toestel));
			toestellen.AddRange(toestellenMetReservatieModded);

			toestelInfo = toestellen.Select(t => new ToestelInfo(t)).ToList();
			toestelInfo = toestelInfo.OrderBy(toes => toes.VerwijderdDisplay).ThenBy(toes => toes.HeeftReservatie).ThenBy(toes => toes.ToestelNaam).ToList();
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

		private void SorteerCheckHandler(object sender, RoutedEventArgs e) {
			AdminListBox.Items.Clear();
			IOrderedEnumerable<ToestelInfo> toestelInfoOrder = null;

			if (sender != null) {
				switch ((sender as RadioButton).Name) {
					case "IdRadio":
						if ((OplopendSorteerBox.Content as Label).Content.ToString() == "Oplopend")
							toestelInfoOrder = toestelInfo.OrderBy(toes => toes.ToestelNummer);
						else
							toestelInfoOrder = toestelInfo.OrderByDescending(toes => toes.ToestelNummer);
						break;

					case "ToestelRadio":
						if ((OplopendSorteerBox.Content as Label).Content.ToString() == "Oplopend")
							toestelInfoOrder = toestelInfo.OrderBy(toes => toes.ToestelNaam);
						else
							toestelInfoOrder = toestelInfo.OrderByDescending(toes => toes.ToestelNaam);
						break;

					case "BeschikbaarRadio":
						if ((OplopendSorteerBox.Content as Label).Content.ToString() == "Oplopend")
							toestelInfoOrder = toestelInfo.OrderBy(toes => toes.InHerstellingDisplay);
						else
							toestelInfoOrder = toestelInfo.OrderByDescending(toes => toes.InHerstellingDisplay);
						break;

					case "StatusRadio":
						if ((OplopendSorteerBox.Content as Label).Content.ToString() == "Oplopend")
							toestelInfoOrder = toestelInfo.OrderBy(toes => toes.VerwijderdDisplay);
						else
							toestelInfoOrder = toestelInfo.OrderByDescending(toes => toes.VerwijderdDisplay);
						break;
				}
			}

			toestelInfo = toestelInfoOrder.ThenBy(toes => toes.VerwijderdDisplay)
											.ThenBy(toes => toes.HeeftReservatie)
											.ThenBy(toes => toes.ToestelNaam).ToList();

			toestelInfo.ForEach(to => AdminListBox.Items.Add(to));
		}

		private async void ToestelGekozen(object sender, SelectionChangedEventArgs e) {
			TijdSlotComboBox.IsEnabled = false;
			TijdSlotComboBox.Items.Clear();
			DatumComboBox.Items.Clear();

			if (ToestelComboBox.SelectedIndex != -1) {
				DatumComboBox.IsEnabled = true;
				await Task.Run(() => domeinController.UpdateklantReservaties());
				List<DateTime> beschikbareDagen = await Task.Run(() => domeinController.GeefBeschikbareDagen());

				bool heeftNogGeenVierReservatie = true;
				beschikbareDagen.ForEach(datum => {
					(beschikbareUren, heeftNogGeenVierReservatie) = domeinController.GeefBeschikbareReservatieUren(new DateTime(datum.Year, datum.Month, datum.Day, 0, 0, 0), ToestelComboBox.SelectedItem.ToString());
					if (beschikbareUren.Count != 0) {
						DatumComboBox.Items.Add(datum.ToString("d"));
					}
				});
			}

			ReservatieButtonKlikbaarMaken(null, null);
		}

		private void DatumGekozen(object sender, SelectionChangedEventArgs e) {
			TijdSlotComboBox.Items.Clear();

			if (DatumComboBox.SelectedIndex != -1 && DatumComboBox.SelectedItem != null) {
				bool isDateTimeOk = DateTime.TryParse(DatumComboBox.SelectedItem.ToString(), out DateTime dateTime);

				if (isDateTimeOk) {
					TijdSlotComboBox.IsEnabled = true;
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

		private async void NieuweReservatieButton(object sender, RoutedEventArgs e) {
			if (ToestelComboBox.SelectedIndex != -1 && DatumComboBox.SelectedIndex != -1 && TijdSlotComboBox.SelectedIndex != -1) {
				bool isDatumGoed = DateTime.TryParse(DatumComboBox.SelectedItem.ToString(), out DateTime dag);
				if (isDatumGoed) {
					DatumComboBox.IsEnabled = false;
					int _tijdSlot = beschikbareUren[TijdSlotComboBox.SelectedIndex];
					string toestelNaam = ToestelComboBox.SelectedItem.ToString();

					DateTime tijdSlot = new DateTime(dag.Year, dag.Month, dag.Day, _tijdSlot, 0, 0);
					int? toestelId = domeinController.GeefEenVrijToestelIdOpNaam(toestelNaam, tijdSlot);

					if (toestelId != null) {
						ResetKeuze();
						string reservatie = await Task.Run(() => domeinController.VoegReservatieToe(tijdSlot, (int)toestelId));
						reservatieInfo.Add(new(reservatie));
						LaadReservaties(null, null);
						if (reservatieInfo.Count > 0) {
							ReservatieListBox.Visibility = Visibility.Visible;
							NogGeenReservatieDockPl.Visibility = Visibility.Collapsed;
						} else {
							NogGeenReservatieDockPl.Visibility = Visibility.Visible;
							ReservatieListBox.Visibility = Visibility.Collapsed;
						}
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

			ToestelComboBox.Text = ". . .";
			DatumComboBox.Text = ". . .";
			TijdSlotComboBox.Text = ". . .";
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
				VoegToeBtn.IsEnabled = false;
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

		private void LaadReservatieUitDb() {
			HashSet<string> reservaties = domeinController.GeefKlantReservaties(true);
			reservatieInfo = reservaties.Select(r => new ReservatieInfo(r)).ToList();
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
			var reservatie = ((sender as Border).DataContext) as ReservatieInfo;
			var result = MessageBox.Show($"Ben je zeker?\n\n({reservatie.Id}) - {reservatie.Toestel} {reservatie.StartTijd.ToString("HH:mm")} -> {reservatie.EindTijd.ToString("HH:mm")}", "Opgepast", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes) {
				domeinController.VerwijderReservatie(reservatie.Id);
				LaadReservatieUitDb();
				LaadReservaties(null, null);

				if (reservatieInfo.Count > 0) {
					ReservatieListBox.Visibility = Visibility.Visible;
					NogGeenReservatieDockPl.Visibility = Visibility.Collapsed;
				} else {
					NogGeenReservatieDockPl.Visibility = Visibility.Visible;
					ReservatieListBox.Visibility = Visibility.Collapsed;
				}
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

		private void SorteerOrderBox(object sender, RoutedEventArgs e) {
			if ((OplopendSorteerBox.Content as Label).Content.ToString() == "Aflopend") {
				(OplopendSorteerBox.Content as Label).Content = "Oplopend";
				OplopendSorteerBox.IsChecked = true;
			}
			else {
				(OplopendSorteerBox.Content as Label).Content = "Aflopend";
				OplopendSorteerBox.IsChecked = true;
			}

			IdRadio.IsChecked = false;
			ToestelRadio.IsChecked = false;
			BeschikbaarRadio.IsChecked = false;
			StatusRadio.IsChecked = false;
		}
	}
}
