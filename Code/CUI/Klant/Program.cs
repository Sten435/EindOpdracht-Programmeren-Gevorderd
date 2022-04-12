using Domein;
using System;
using System.Collections.Generic;

namespace CUI {
	public class KlantProgram {
		private static readonly FitnessApp _fitnessApp = new();
		private static Klant klant;

		// Todo: Geregistreerde uren van andere klaten weglaten in keuze bij reservatie.

		// REMOVE:
		//private static Klant klant = FitnessApp.DEBUGUSER;
		// REMOVE:

		static void Main(string[] args) {
			Console.ResetColor();
			do {
				try {
					if (klant == null)
						LoginOrRegisterScreen();
					LoggedInPanel();
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
						klant = _fitnessApp.Login();
						break;
					case 1:
						(klant, heeftGeanuleerd) = _fitnessApp.RegistreerKlant();
						break;
				}
			} while (heeftGeanuleerd);
		}
		#endregion

		#region LoggedInPanel()
		private static void LoggedInPanel() {
			FitnessApp.SelectedIndex = 0;
			bool heeftUitgelogd = false;
			do {
				List<string> optieLijst = new() { "Reserveer Toestel", "Mijn Reservaties", "Toon User Details\n", FitnessApp.StopOpties[2] };
				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen\n");

				switch (selectedIndex) {
					case 0:
						_fitnessApp.RegistreerToestel(klant);
						break;
					case 1:
						_fitnessApp.ToonKlantReservaties(klant);
						break;
					case 2:
						_fitnessApp.ToonKlantDetails(klant);
						break;
					case 3:
						heeftUitgelogd = true;
						klant = null;
						break;
				}
			} while (!heeftUitgelogd);
		}
		#endregion
	}
}
