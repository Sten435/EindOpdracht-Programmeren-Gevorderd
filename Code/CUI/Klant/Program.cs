using Domein;
using Persistentie;
using System;
using System.Collections.Generic;

namespace CUI {

	public class KlantProgram {
		private static IKlantenRepository _klantenRepository = new KlantenRepository();
		private static IReservatieRepository _reservatieRepository = new ReservatieRepository();
		private static IToestelRepository _toestelRepository = new ToestellenRepository();

		private static DomeinController _domeinController = new(_reservatieRepository, _klantenRepository, _toestelRepository);
		private static readonly FitnessApp _fitnessApp = new(_domeinController);

		private static void Main(string[] args) {
			// REMOVE:
			_domeinController.Login("stan.persoons@student.hogent.be");
			// REMOVE:

			Console.ResetColor();
			do {
				try {
					if (!_domeinController.LoggedIn) LoginOrRegisterScreen();
					else Dashboard();
				} catch (LoginException error) {
					Utility.Logger.Error(error, clearConsole: true);
				} catch (Exception error) {
					Utility.Logger.Error(error, clearConsole: true);
				}
			} while (true);
		}

		#region LoginOrRegisterSceen()

		private static void LoginOrRegisterScreen() {
			bool heeftGeanuleerd;
			List<string> optieLijst = new() { "Login", "Nieuwe Gebruiker" };

			do {
				FitnessApp.SelectedIndex = 0;
				heeftGeanuleerd = false;
				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Druk op [ ▲ | ▼ ] om je keuze te wijzigen\nDruk op [Enter] om te bevestigen.\n");
				switch (selectedIndex) {
					case 0:
						_fitnessApp.Login();
						break;

					case 1:
						heeftGeanuleerd = _fitnessApp.RegistreerKlant();
						break;
				}
			} while (heeftGeanuleerd);
		}

		#endregion LoginOrRegisterSceen()

		#region Dashboard()

		private static void Dashboard() {
			FitnessApp.SelectedIndex = 0;
			do {
				List<string> beschikbaretoestellen = _fitnessApp.GeefBeschikbareToestellen();
				List<string> optieLijst;

				if (_fitnessApp.LoggedIn) {
					int aantalKlantReservaties = _fitnessApp.GeefKlantReservaties().Count;

					optieLijst = new() { $"{(beschikbaretoestellen.Count > 0 ? "Reserveer Toestel" : FitnessApp.DisabledOptie[0])}", $"{(aantalKlantReservaties > 0 ? "Mijn Reservaties" : FitnessApp.DisabledOptie[1])}", "Toon User Details\n", FitnessApp.StopOpties[2] };
					int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen\n");

					switch (selectedIndex) {
						case 0:
							if (beschikbaretoestellen.Count > 0)
								_fitnessApp.RegistreerToestel();
							break;

						case 1:
							_fitnessApp.ToonKlantReservaties();
							break;

						case 2:
							_fitnessApp.ToonKlantDetails();
							break;

						case 3:
							_fitnessApp.Logout();

							break;
					}
				}
			} while (_fitnessApp.LoggedIn);
		}

		#endregion Dashboard()
	}
}