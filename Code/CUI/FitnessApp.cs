using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domein;
using Domein.Exceptions;
using Persistentie;

namespace CUI {
	public class FitnessApp {
		private static IKlantenRepository _klantenRepo = new KlantenMapper();
		private static IReservatieRepository _reservatienRepo = new ReservatieMapper();
		private static IToestelRepository _toestellenRepo = new ToestellenMapper();

		static UniekeCode _uniekeCode = UniekeCode.Instance;

		DomeinController _domeinController = new DomeinController(_reservatienRepo, _klantenRepo, _toestellenRepo);

		public Klant RegistreerKlant() {
			string voornaam = String.Empty, achternaam = String.Empty, email = String.Empty, typeAlsString = String.Empty;
			List<string> interesses = new List<string>();
			DateTime geboorteDatum = new DateTime();
			Adres adres = new Adres();
			TypeKlant type;

			const int overgeblevenOpties = 5;

			bool isOk = false,
				 VoornaamIngevuld = false,
				 AchternaamIngevuld = false,
				 InteresseIngevuld = false,
				 AdresIngevuld = false,
				 typeIngevuld = false,
				 geboorteDatumIngevuld = false;

			do {
				List<string> optieLijst = new List<string>() { $"Voornaam {(string.IsNullOrEmpty(voornaam) ? '*' : voornaam)}", $"Achternaam {(string.IsNullOrEmpty(achternaam) ? '*' : achternaam)}", $"GeboorteDatum {(geboorteDatumIngevuld ? geboorteDatum.ToShortDateString() : '*')}", $"Adres {(AdresIngevuld ? $"{adres.StraatNaam}..." : '*')}", $"Abonnoment {(typeIngevuld ? typeAlsString : '*')}", $"Interesses {string.Join(", ", interesses)}" };
				int selectedIndex = OptieLijstConroller(optieLijst);
				switch (selectedIndex) {
					case 0:
						Console.Write($"Voornaam: ");
						voornaam = Console.ReadLine();
						VoornaamIngevuld = true;
						break;

					case 1:
						Console.Write($"Achternaam: ");
						achternaam = Console.ReadLine();
						AchternaamIngevuld = true;
						break;

					case 2:
						Console.Write($"GeboorteDatum (DD-MM-YYYY): ");
						DateTime datum;
						do {
							isOk = DateTime.TryParse(Console.ReadLine(), out datum);
						} while (!isOk);
						geboorteDatum = datum;
						geboorteDatumIngevuld = true;
						break;

					case 3:
						Console.Write($"Adres: ");
						int selectedAdresIndex;

						string straatNaam = String.Empty,
							   straatNummer = String.Empty,
							   plaats = String.Empty;

						int postCode = 0;

						bool straatNaamIngevuld = false,
							 straatNummerIngevuld = false,
							 plaatsIngevuld = false,
							 postCodeIngevuld = false;

						do {
							List<string> adresOpties = new List<string>() { $"Straat: {straatNaam}", $"HuisNr: {straatNummer}", $"Plaats: {plaats}", $"PostCode: {postCode}" };
							selectedAdresIndex = OptieLijstConroller(adresOpties);

							switch (selectedAdresIndex) {
								case 0:
									Console.Write("Straat: ");
									straatNaam = Console.ReadLine();
									straatNaamIngevuld = true;
									break;
								case 1:
									Console.Write("Nummer: ");
									straatNummer = Console.ReadLine();
									straatNummerIngevuld = true;
									break;
								case 2:
									Console.Write("Plaats: ");
									plaats = Console.ReadLine();
									plaatsIngevuld = true;
									break;
								case 3:
									do {
										Console.Write("PostCode: ");
										isOk = int.TryParse(Console.ReadLine(), out postCode);
									} while (!isOk);
									postCodeIngevuld = true;
									break;
							}
						} while (!straatNaamIngevuld || !straatNummerIngevuld || !plaatsIngevuld || !postCodeIngevuld);
						adres = new Adres(straatNaam, straatNummer, plaats, postCode);
						AdresIngevuld = true;
						break;

					case 4:
						List<string> TypeAbonementen = new List<string>() {
							"Gold",
							"Silver",
							"Bronze"
						};
						int selectedAbonnementIndex;

						selectedAbonnementIndex = OptieLijstConroller(TypeAbonementen, "Abonnement: ");
						type = selectedAbonnementIndex switch {
							0 => TypeKlant.Gold,
							1 => TypeKlant.Silver,
							2 => TypeKlant.Bronze,
							_ => TypeKlant.Bronze,
						};
						typeAlsString = type.ToString();
						typeIngevuld = true;
						break;

					case 5:
						Console.Write($"interesses: ");
						List<string> interesseOpties = new List<string>() {
							"Voeg interesse toe.",
							"Ga terug."
						};
						int selectedInteresseIndex;
						do {
							selectedInteresseIndex = OptieLijstConroller(interesseOpties);

							if (selectedInteresseIndex == 0) {
								Console.Write("Interesse: ");
								interesses.Add(Console.ReadLine());
								Console.Clear();
							}
						} while (selectedInteresseIndex != 1);
						InteresseIngevuld = true;
						break;
				}
			} while (!VoornaamIngevuld || !AchternaamIngevuld || !AdresIngevuld || !geboorteDatumIngevuld || !InteresseIngevuld || !typeIngevuld);

			Klant klant = new Klant(_uniekeCode.GenereerRandomCode(), voornaam, achternaam, email, interesses, geboorteDatum, adres, type);
			_domeinController.RegistreerKlant(klant);
			return null;
		}

		public Klant Login() {
			Console.Write("Wat is je E-mailAdress: ");
			string email = CustomInput.ReadInput(ConsoleColor.Green).ToLower();
			return _domeinController.Login(email);
		}

		private static int selectedIndex = 0;

		public void DisplayOptions(List<string> optieLijst) {
			for (int i = 0; i < optieLijst.Count; i++) {
				if (i == selectedIndex) {
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(@$"-> {optieLijst[i]}");
					Console.ResetColor();
				} else {
					Console.ResetColor();
					Console.WriteLine(@$"   {optieLijst[i]}");
				}
			}
		}


		public int OptieLijstConroller(List<string> optieLijst, string prompt = "") {
			Console.Clear();
			ConsoleKey consoleKey;
			do {
				Console.WriteLine(prompt);
				DisplayOptions(optieLijst);
				consoleKey = Console.ReadKey().Key;
				if (consoleKey == ConsoleKey.UpArrow) {
					if (selectedIndex > 0)
						selectedIndex--;
				} else if (consoleKey == ConsoleKey.DownArrow) {
					if (selectedIndex < optieLijst.Count - 1)
						selectedIndex++;
				}
				Console.Clear();
			} while (consoleKey != ConsoleKey.Enter);
			int _selectedIndex = selectedIndex;
			selectedIndex = 0;
			return _selectedIndex;
		}

		public static class Logger {
			public static void Error(Exception error) {
				Console.BackgroundColor = ConsoleColor.DarkRed;
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(error.Message);
				Console.ResetColor();
			}

			public static void Info(Exception error) {
				Console.BackgroundColor = ConsoleColor.DarkYellow;
				Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine(error.Message);
				Console.ResetColor();
			}
		}

		public static class CustomInput {
			public static string ReadInput(ConsoleColor color = ConsoleColor.White) {
				Console.ForegroundColor = color;
				string input = Console.ReadLine();
				if (color != ConsoleColor.White)
					Console.ResetColor();
				return input;
			}
		}
	}
}
