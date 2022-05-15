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

namespace UI {
	/// <summary>
	/// Interaction logic for Reservaties.xaml
	/// </summary>
	public partial class DashbordWindow : Window {
		private DomeinController domeinController;
		private List<int> beschikbareUren;
		private DateTime dag;
		Thickness borderNone = new Thickness(0, 0, 0, 0);
		Thickness newLeftBorder = new Thickness(5, 0, 0, 0);

		bool kanNogReservaren;

		const int maximumDagenReserverenToekomst = 7;

		public DashbordWindow(DomeinController _domeincontroller) {
			domeinController = _domeincontroller;
			InitializeComponent();
			this.DataContext = this;
			LaadDataInDashBord();
			UpdateReservaties();
		}

		private void LaadDataInDashBord() {
			List<string> beschikbareToestellen = domeinController.GeefBeschikbareToestellen();
			beschikbareToestellen.ForEach(t => ToestelComboBox.Items.Add(t));
		}

		private void UpdateReservaties() {
			ReservatieListBox.Items.Clear();
			List<string> reservaties = domeinController.GeefKlantReservaties();
			reservaties.Reverse();

			reservaties.ForEach(r => {
				StackPanel st = new();
				Label textlb = new();
				Label txtbox = new();
				Border br = new();

				string rs = r.ToString();
				DateTime datum = DateTime.Parse(rs[..10]);
				textlb.FontWeight = FontWeights.Bold;

				txtbox.FontWeight = FontWeights.Bold;
				txtbox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));

				if (datum.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")) {
					txtbox.Content = "VANDAAG";
					br.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF59A6ED"));
				}
				else if (datum < DateTime.Now) {
					txtbox.Content = "VERLEDEN";
					br.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA5A5A5"));
				}
				else if (datum > DateTime.Now) {
					txtbox.Content = "TOEKOMST";
					br.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4DCA6C"));
				}

				br.CornerRadius = new CornerRadius(5);
				txtbox.HorizontalAlignment = HorizontalAlignment.Center;
				txtbox.VerticalAlignment = VerticalAlignment.Center;
				br.Child = txtbox;
				br.Width = 80;
				br.Margin = new Thickness(0, 0, 62, 0);

				textlb.Content = r.ToString();
				st.HorizontalAlignment = HorizontalAlignment.Center;
				st.Orientation = Orientation.Horizontal;
				st.Children.Add(br);
				st.Children.Add(textlb);

				ReservatieListBox.Items.Add(st);
			});
		}

		private void ToestelGekozen(object sender, SelectionChangedEventArgs e) {
			TijdSlotComboBox.Items.Clear();
			DatumComboBox.Items.Clear();

			int selectedToestelIndex = ToestelComboBox.SelectedIndex;

			if (selectedToestelIndex != -1) {
				for (int i = 0; i < maximumDagenReserverenToekomst; i++) {
					DatumComboBox.Items.Add($"{DateTime.Now.AddDays(i):d}");
				}
			}

			ReservatieButtonKlikbaarMaken(null, null);
		}

		private void DatumGekozen(object sender, SelectionChangedEventArgs e) {
			TijdSlotComboBox.Items.Clear();

			if (DatumComboBox.SelectedIndex != -1 && DatumComboBox.SelectedItem != null) {
				bool isDateTimeOk = DateTime.TryParse(DatumComboBox.SelectedItem.ToString(), out DateTime dateTime);

				if (isDateTimeOk) {
					dag = new(dateTime.Year, dateTime.Month, dateTime.Day, TijdsSlot.LowerBoundUurReservatie, 0, 0);

					(beschikbareUren, kanNogReservaren) = domeinController.GeefBeschikbareUrenOpDatum(dag, ToestelComboBox.SelectedItem.ToString());

					if (beschikbareUren.Count != 0) {
						if (kanNogReservaren) {
							for (int i = 0; i < beschikbareUren.Count; i++) {
								string item = beschikbareUren[i].ToString().Length == 1 ? $"0{beschikbareUren[i]}" : beschikbareUren[i].ToString();
								TijdSlotComboBox.Items.Add($"{item}:00");
							}
						}
					} else {
						if (!kanNogReservaren)
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
						domeinController.VoegReservatieToe(tijdSlot, (int)toestelId);
						MessageBox.Show($"Reservatie \n Toestel -> {toestelNaam}:{toestelId}\n Datum -> {tijdSlot:g}", "Success", MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

						ResetKeuze();
						UpdateReservaties();
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

		private void LogoutButton(object sender, MouseButtonEventArgs e) {
			domeinController.Logout();
			LoginWindow loginWindow = new(domeinController);
			loginWindow.Title = "Login";
			this.Close();
			loginWindow.Show();
		}

		private void VeranderKleurLogoutButtonHoverStart(object sender, MouseEventArgs e) {
			logoutLabel.Opacity = 1;
			logoutImage.Opacity = 1;
		}
		private void VeranderKleurLogoutButtonHoverExit(object sender, MouseEventArgs e) {
			logoutLabel.Opacity = .5;
			logoutImage.Opacity = .5;
		}

		private void SelecteerReservatiesItem(object sender, MouseButtonEventArgs e) {
			ReservatieScherm.Visibility = Visibility.Visible;

			ReservatieItem.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED5959"));
			ReservatieItem.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED5959"));

			AccountItem.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEBEB"));
			AccountScherm.Visibility = Visibility.Collapsed;

			UpdateReservaties();
		}

		private void SelecteerAccountItem(object sender, MouseButtonEventArgs e) {
			ReservatieItem.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
			ReservatieScherm.Visibility = Visibility.Collapsed;

			AccountItem.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED5959"));
			AccountScherm.Visibility = Visibility.Visible;
		}
	}
}
