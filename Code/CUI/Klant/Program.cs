﻿using Domein;
using Persistentie;
using System;
using System.Collections.Generic;

namespace CUI {

	public class KlantProgram {
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
			} catch (NullReferenceException error) {
				Utility.Logger.Error($"Onbekende fout <001> (Contacteer een beheerder): {error}", clearConsole: true);
			} catch (ArgumentOutOfRangeException error) {
				Utility.Logger.Error($"Onbekende fout <002> (Contacteer een beheerder): {error}", clearConsole: true);
			} catch (ArgumentException error) {
				Utility.Logger.Error($"Onbekende fout <003> (Contacteer een beheerder): {error}", clearConsole: true);
			} catch (FormatException error) {
				Utility.Logger.Error($"Onbekende fout <004> (Contacteer een beheerder): {error}", clearConsole: true);
			} catch (IndexOutOfRangeException error) {
				Utility.Logger.Error($"Onbekende fout <005> (Contacteer een beheerder): {error}", clearConsole: true);
			} catch (Exception error) {
				Utility.Logger.Error($"Onbekende fout <006> (Contacteer een beheerder): {error}", clearConsole: true);
			}
			do {
				// REMOVE:
				_domeinController.Login("stan.persoons@student.hogent.be");
				// REMOVE:

				// TODO: Wanneer Toestel verwijderd dan geeft fout dat toestel niet bestaat.

				Console.ResetColor();
				try {
					if (!_domeinController.LoggedIn) LoginOrRegisterScreen();
					else Dashboard();
				} catch (NullReferenceException error) {
					Utility.Logger.Error($"Onbekende fout <007> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (ArgumentOutOfRangeException error) {
					Utility.Logger.Error($"Onbekende fout <008> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (ArgumentException error) {
					Utility.Logger.Error($"Onbekende fout <009> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (FormatException error) {
					Utility.Logger.Error($"Onbekende fout <010> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (IndexOutOfRangeException error) {
					Utility.Logger.Error($"Onbekende fout <011> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (Exception error) {
					Utility.Logger.Error($"Onbekende fout <012> (Contacteer een beheerder): {error}", clearConsole: true);
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