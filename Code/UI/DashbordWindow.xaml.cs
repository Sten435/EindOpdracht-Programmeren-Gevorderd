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
		private bool kanNogReservaren;

		private DateTime dag;
		private string toestelNaam;
		const int maximumDagenReserverenToekomst = 7;

		public DashbordWindow(DomeinController _domeincontroller) {
			domeinController = _domeincontroller;
			InitializeComponent();
			this.DataContext = this;
			domeinController.Login("stan.persoons@student.hogent.be");
			LaadDataInDashBord();
		}

		private void LaadDataInDashBord() {
			List<string> beschikbareToestellen = domeinController.GeefBeschikbareToestellen();
			beschikbareToestellen.ForEach(t => ToestelComboBox.Items.Add(t));
		}

		private void ToestelGekozen(object sender, SelectionChangedEventArgs e) {
			TijdSlotComboBox.Items.Clear();
			DatumComboBox.Items.Clear();

			int selectedToestelIndex = ToestelComboBox.SelectedIndex;

			if (selectedToestelIndex != -1) {
				toestelNaam = domeinController.GeefBeschikbareToestellen()[selectedToestelIndex];
				int toestelId = domeinController.GeefRandomToestelIdOpNaam(toestelNaam);

				for (int i = 0; i < maximumDagenReserverenToekomst; i++) {
					DatumComboBox.Items.Add($"{DateTime.Now.AddDays(i):d}");
				}
			}
		}

		private void DatumGekozen(object sender, SelectionChangedEventArgs e) {
			TijdSlotComboBox.Items.Clear();

			if (DatumComboBox.SelectedIndex != -1 && DatumComboBox.SelectedItem != null) {
				bool isDateTimeOk = DateTime.TryParse(DatumComboBox.SelectedItem.ToString(), out DateTime dateTime);

				if (isDateTimeOk) {
					dag = new(dateTime.Year, dateTime.Month, dateTime.Day, TijdsSlot.LowerBoundUurReservatie, 0, 0);

					(beschikbareUren, kanNogReservaren) = domeinController.GeefBeschikbareUrenOpDatum(dag, toestelNaam);

					if (beschikbareUren.Count != 0) {
						for (int i = 0; i < beschikbareUren.Count; i++) {
							string item = beschikbareUren[i].ToString().Length == 1 ? $"0{beschikbareUren[i]}" : beschikbareUren[i].ToString();
							TijdSlotComboBox.Items.Add($"{item}:00");
						}
					} else MessageBox.Show($"Geen tijdsSlot voor {dag:d}");
				}
			}
		}

		private void NieuweReservatieButton(object sender, RoutedEventArgs e) {
			bool isDatumGoed = DateTime.TryParse(DatumComboBox.SelectedItem.ToString(), out DateTime dag);
			if (isDatumGoed) {
				string toestel = ToestelComboBox.SelectedItem.ToString();
				int tijdSlot = beschikbareUren[TijdSlotComboBox.SelectedIndex];
				DateTime datum = new DateTime(dag.Year, dag.Month, dag.Day, tijdSlot, 0, 0);

				MessageBox.Show($"Reservatie \n Toestel -> {toestel}\n Datum -> {datum:g}");

			} else
				MessageBox.Show($"Datum ({DatumComboBox.SelectedItem}) is niet correct.");
		}
	}
}
