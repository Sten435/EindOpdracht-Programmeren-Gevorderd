using Domein;
using System;
using System.Collections.Generic;

namespace CUI {
	public class KlantProgram {
		private static readonly FitnessApp _fitnessApp = new();
		//private static Klant klant;

		static string _naam = @"  ______ _ _                        
 |  ____(_) |                       
 | |__   _| |_ _ __   ___  ___ ___  
 |  __| | | __| '_ \ / _ \/ __/ __| 
 | |    | | |_| | | |  __/\__ \__ \ 
 |_|    |_|\__|_| |_|\___||___/___/ 
                                    ";

		// REMOVE:
		private static Klant klant = FitnessApp.DEBUGUSER;
		// REMOVE:

		static void Main(string[] args) {
			Console.ResetColor();
			do {
				try {
					if (klant == null)
						LoginOrRegisterScreen();
					Dashboard();
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
				int selectedIndex = Utility.OptieLijstConroller(optieLijst, $"{_naam}\n\nDruk op [ ▲ | ▼ ] om je keuze te wijzigen\nDruk op [Enter] om te bevestigen.\n");
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

		#region Dashboard()
		private static void Dashboard() {
			FitnessApp.SelectedIndex = 0;
			bool heeftUitgelogd = false;
			do {
				List<string> beschikbaretoestellen = _fitnessApp.GeefBeschikbareToestellen();
				List<string> optieLijst;
				if (beschikbaretoestellen.Count > 0) {
					optieLijst = new() { $"Reserveer Toestel", "Mijn Reservaties", "Toon User Details\n", FitnessApp.StopOpties[2] };
				} else
					optieLijst = new() { FitnessApp.DisabledOptie[0], "Mijn Reservaties", "Toon User Details\n", FitnessApp.StopOpties[2] };

				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen\n");

				switch (selectedIndex) {
					case 0:
						if (beschikbaretoestellen.Count > 0)
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
