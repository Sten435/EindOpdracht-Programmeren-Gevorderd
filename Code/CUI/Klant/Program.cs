using Domein;
using Domein.Exceptions;
using System;
using System.Collections.Generic;

namespace CUI {
	public class KlantProgram {
		static FitnessApp _fitnessApp = new();

		// Todo: Code Cleanen

		static void Main(string[] args) {
			bool ExceptionThrown = false;
			List<string> optieLijst = new() { "Login", "Nieuwe Gebruiker" };
			try {
				Klant klant;
				do {
					int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Maak een keuze door op je pijltjes te drukken.\n");

					switch (selectedIndex) {
						case 0:
							klant = _fitnessApp.Login();
							break;
						case 1:
							klant = _fitnessApp.RegistreerKlant();
							Console.WriteLine($"Type Klant: {klant.TypeKlant}");
							break;
					}
				} while (ExceptionThrown);
			} catch (LoginFailedException error) {
				Utility.Logger.Error(error);
			} catch (ReservatieException error) {
				Utility.Logger.Error(error);
			} catch (LeeftijdException error) {
				Utility.Logger.Error(error);
			} catch (Exception error) {
				Utility.Logger.Error(error);
			}
		}
	}
}
