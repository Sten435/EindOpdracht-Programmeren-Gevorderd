using Domein;
using Persistentie;
using System;
using System.Collections.Generic;

namespace CUI {

	public class BeheerderProgram {
		private static IKlantenRepository _klantenRepository;
		private static IReservatieRepository _reservatieRepository;
		private static IToestelRepository _toestelRepository;
		private static IConfigRepository _configRepository;

		private static DomeinController _domeinController;
		private static FitnessApp _fitnessApp;

		private static void Main(string[] args) {
			try {
				_klantenRepository = new KlantenRepository();
				_reservatieRepository = new ReservatieRepository();
				_toestelRepository = new ToestellenRepository();
				_configRepository = new ConfigRepository();

				_domeinController = new(_reservatieRepository, _klantenRepository, _toestelRepository, _configRepository);
				_fitnessApp = new(_domeinController);
			} catch (Exception error) {
				Utility.Logger.Error(error, clearConsole: true);
			}
			do {
				// REMOVE:
				_domeinController.Login("stan.persoons@student.hogent.be");
				// REMOVE:

				Console.ResetColor();
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
			List<string> optieLijst = new() { "Login as Beheerder", "Nieuwe Beheerder Toevoegen" };

			do {
				FitnessApp.SelectedIndex = 0;

				heeftGeanuleerd = false;
				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Druk op [ ▲ | ▼ ] om je keuze te wijzigen\nDruk op [Enter] om te bevestigen.\n");

				switch (selectedIndex) {
					case 0:
						_fitnessApp.Login(true);
						break;

					case 1:
						heeftGeanuleerd = _fitnessApp.RegistreerKlant(true);
						break;
				}
			} while (heeftGeanuleerd);
		}

		#endregion LoginOrRegisterSceen()

		#region Dashboard()

		private static void Dashboard() {
			FitnessApp.SelectedIndex = 0;
			do {
				List<string> optieLijst = new() { $"Voeg Toestel Toe", "Verwijder Toestel", "Toon Reservaties", "Toon Toestellen", "Toon Klanten\n", FitnessApp.StopOpties[2] };

				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen\n");

				switch (selectedIndex) {
					case 0:
						_fitnessApp.VoegToestelToe();
						break;

					case 1:
						_fitnessApp.VerwijderToestel();
						break;

					case 2:
						_fitnessApp.ToonAlleReservaties();
						break;

					case 3:
						_fitnessApp.ToonAlleToestellen();
						break;

					case 4:
						_fitnessApp.ToonAlleKlanten();
						break;

					case 5:
						_fitnessApp.Logout();
						break;
				}
			} while (_fitnessApp.LoggedIn);
		}

		#endregion Dashboard()
	}
}