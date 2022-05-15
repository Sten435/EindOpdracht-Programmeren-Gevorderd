using Domein;
using System;
using System.Collections.Generic;

namespace CUI {

	public class KlantProgram {
		private static DomeinController domeinController;
		private static FitnessApp fitnessApp;

		public KlantProgram(DomeinController _domeinController) {
			domeinController = _domeinController;
			fitnessApp = new(domeinController);
		}

		public void Start() {
			do {
#if DEBUG
				domeinController.Login("24");
#endif

				Console.ResetColor();
				if (!domeinController.LoggedIn) LoginOrRegisterScreen();
				else Dashboard();
			} while (true);
		}

		#region LoginOrRegisterSceen()

		private void LoginOrRegisterScreen() {
			bool heeftGeanuleerd;
			List<string> optieLijst = new() { "Login", "Nieuwe Gebruiker" };

			do {
				FitnessApp.SelectedIndex = 0;
				heeftGeanuleerd = false;
				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Druk op [ ▲ | ▼ ] om je keuze te wijzigen\nDruk op [Enter] om te bevestigen.\n");
				switch (selectedIndex) {
					case 0:
						fitnessApp.Login();
						break;

					case 1:
						heeftGeanuleerd = fitnessApp.RegistreerKlant();
						break;
				}
			} while (heeftGeanuleerd);
		}

		#endregion LoginOrRegisterSceen()

		#region Dashboard()

		private void Dashboard() {
			FitnessApp.SelectedIndex = 0;
			do {
				List<string> beschikbaretoestellen = fitnessApp.GeefBeschikbareToestellen();
				List<string> optieLijst;

				if (fitnessApp.LoggedIn) {
					int aantalKlantReservaties = fitnessApp.GeefKlantReservaties().Count;

					optieLijst = new() { $"{(beschikbaretoestellen.Count > 0 ? "Reserveer Toestel" : FitnessApp.DisabledOptie[0])}", $"{(aantalKlantReservaties > 0 ? "Mijn Reservaties" : FitnessApp.DisabledOptie[1])}", "Toon User Details\n", FitnessApp.StopOpties[2] };
					int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen\n");

					switch (selectedIndex) {
						case 0:
							if (beschikbaretoestellen.Count > 0)
								fitnessApp.RegistreerToestel();
							break;

						case 1:
							fitnessApp.ToonKlantReservaties();
							break;

						case 2:
							fitnessApp.ToonKlantDetails();
							break;

						case 3:
							fitnessApp.Logout();

							break;
					}
				}
			} while (fitnessApp.LoggedIn);
		}

		#endregion Dashboard()
	}
}