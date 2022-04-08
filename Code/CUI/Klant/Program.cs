using System;
using System.Collections.Generic;
using Domein;
using Domein.Exceptions;

namespace CUI {
	public class KlantProgram {
		static FitnessApp _fitnessApp = new FitnessApp();
		static void Main(string[] args) {
			bool ExceptionThrown = false;
			List<string> optieLijst = new() { "Login", "Nieuwe Gebruiker" };
			try {
				Klant klant;
				do {
					int selectedIndex = _fitnessApp.OptieLijstConroller(optieLijst, "Maak een keuze door op je pijltjes te drukken.\n");

					switch (selectedIndex) {
						case 0:
							klant = _fitnessApp.Login();
							break;
						case 1:
							klant = _fitnessApp.RegistreerKlant();
							break;
					}
				} while (ExceptionThrown);
			} catch (LoginFailedException error) {
				FitnessApp.Logger.Error(error);
			} catch (ReservatieException error) {
				FitnessApp.Logger.Error(error);
			} catch (LeeftijdException error) {
				FitnessApp.Logger.Error(error);
			} catch (Exception error) {
				FitnessApp.Logger.Error(error);
			}
		}
	}
}
