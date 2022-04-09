using Domein;
using Domein.Exceptions;
using System;
using System.Collections.Generic;

namespace CUI {
	public class KlantProgram {
		static FitnessApp _fitnessApp = new();
		private static Klant klant;

		// Todo: Sneller Werken

		static void Main(string[] args) {
			Console.ResetColor();
			do {
				try {
					LoginOrRegisterScreen();
					LoggedInPanel();
				} catch (LoginFailedException error) {
					Utility.Logger.Error(error, clearConsole: true);
				} catch (ReservatieException error) {
					Utility.Logger.Error(error, clearConsole: true);
				} catch (LeeftijdException error) {
					Utility.Logger.Error(error, clearConsole: true);
				} catch (Exception error) {
					Utility.Logger.Error(error, clearConsole: true);
				}
			} while (true);
		}

		private static void LoginOrRegisterScreen() {
			FitnessApp.SelectedIndex = 0;
			bool heeftGeanuleerd;
			List<string> optieLijst = new() { "Login", "Nieuwe Gebruiker" };
			int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Maak een keuze door op je pijltjes te drukken.\n");

			switch (selectedIndex) {
				case 0:
					klant = _fitnessApp.Login();
					break;
				case 1:
					(klant, heeftGeanuleerd) = _fitnessApp.RegistreerKlant();
					if (heeftGeanuleerd)
						throw new LoginFailedException("Registratie geannuleerd");
					break;
			}
		}

		private static void LoggedInPanel() {
			bool heeftUitgelogd = false;
			do {
				List<string> optieLijst = new() { "Reserveer Toestel", "Toon User Details", "Uitloggen" };
				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Maak een keuze door op je pijltjes te drukken.\n");

				switch (selectedIndex) {
					case 0:
						_fitnessApp.RegistreerToestel();
						break;
					case 1:
						_fitnessApp.ToonKlantDetails(klant);
						break;
					case 2:
						heeftUitgelogd = true;
						klant = null;
						break;
				}
			} while (!heeftUitgelogd);
		}
	}
}
