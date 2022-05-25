using Domein;
using System;
using System.Collections.Generic;

namespace CUI {

	public class BeheerderProgram {
		private static DomeinController domeinController;
		private static FitnessApp fitnessApp;

		public BeheerderProgram(DomeinController _domeinController) {
			domeinController = _domeinController;
			fitnessApp = new(domeinController);
		}

		public void Start() {
			do {
#if DEBUG
				domeinController.Login("stan.persoons@student.hogent.be");
#endif
				Console.ResetColor();
				try {
					if (!domeinController.LoggedIn) LoginOrRegisterScreen();
					else Dashboard();
				} catch (LoginException error) {
					Utility.Logger.Error(error, clearConsole: true);
				}
			} while (true);
		}

		#region LoginOrRegisterSceen()

		private void LoginOrRegisterScreen() {
			bool heeftGeanuleerd;
			List<string> optieLijst = new() { "Login as Beheerder", "Nieuwe Beheerder Toevoegen" };

			do {
				FitnessApp.SelectedIndex = 0;

				heeftGeanuleerd = false;
				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Druk op [ ▲ | ▼ ] om je keuze te wijzigen\nDruk op [Enter] om te bevestigen.\n");

				switch (selectedIndex) {
					case 0:
						fitnessApp.Login();
						break;

					case 1:
						heeftGeanuleerd = fitnessApp.RegistreerKlant(true);
						break;
				}
			} while (heeftGeanuleerd);
		}

		#endregion LoginOrRegisterSceen()

		#region Dashboard()

		private void Dashboard() {
			FitnessApp.SelectedIndex = 0;
			do {
				List<string> optieLijst = new() { $"Voeg Toestel Toe", "Verwijder Toestel", "Toon Reservaties", "Toon Toestellen", "Toon Klanten\n", FitnessApp.StopOpties[2] };

				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen\n");

				switch (selectedIndex) {
					case 0:
						fitnessApp.VoegToestelToe();
						break;

					case 1:
						fitnessApp.VerwijderToestel();
						break;

					case 2:
						fitnessApp.ToonAlleReservaties();
						break;

					case 3:
						fitnessApp.ToonAlleToestellen();
						break;

					case 4:
						fitnessApp.ToonAlleKlanten();
						break;

					case 5:
						fitnessApp.Logout();
						break;
				}
			} while (fitnessApp.LoggedIn);
		}

		#endregion Dashboard()
	}
}