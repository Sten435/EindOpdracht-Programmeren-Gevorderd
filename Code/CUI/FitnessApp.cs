using Domein;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CUI {

	public class FitnessApp {

		public FitnessApp(DomeinController d) {
			_domeinController = d;
		}

		#region Private Fields

		private readonly DomeinController _domeinController;
		private const char _requiredCharacter = '#';
		private const int _maximumDagenReserverenToekomst = 7;

		#endregion Private Fields

		#region Public Properties

		public static ConsoleColor DefaultReadLineColor { get => ConsoleColor.Cyan; }

		public static ConsoleColor DefaultInfoBackgroundPrintLineColor { get => ConsoleColor.DarkYellow; }
		public static ConsoleColor DefaultInfoForeGrountPrintLineColor { get => ConsoleColor.Black; }

		public static ConsoleColor DefaultErrorBackgroundPrintLineColor { get => ConsoleColor.DarkRed; }
		public static ConsoleColor DefaultErrorForeGrountPrintLineColor { get => ConsoleColor.White; }

		public bool LoggedIn { get => _domeinController.LoggedIn; }

		public static readonly List<string> GaVerderOpties = new() {
			Utility.SchrijfUnderline("Ga Verder"),
		};

		public static readonly List<string> StopOpties = new() {
			Utility.SchrijfUnderline("Stop Registratie"),
			Utility.SchrijfUnderline("Ga Terug"),
			Utility.SchrijfUnderline("Uitloggen")
		};

		public static readonly List<string> DisabledOptie = new() {
			"Reserveer Toestel [Geen Toestellen Beschikbaar]",
			"Mijn Reservaties [Geen Reservaties Beschikbaar]"
		};

		public static int SelectedIndex = 0;
		public static int OudeselectedIndex;

		#endregion Public Properties

		#region VoegToestelToe()

		public void VoegToestelToe() {
			string naam;
			bool naamIsOk;
			do {
				naam = Utility.AskUser.ReadInput(prompt: "Welk toestel wil je toevoegen:", metAchtergrond: true, promptColor: ConsoleColor.Black);
				if (string.IsNullOrWhiteSpace(naam)) {
					naamIsOk = false;
					Console.Clear();
				} else
					naamIsOk = true;
			} while (!naamIsOk);
			_domeinController.VoegNieuwToestelToe(naam.Trim());
		}

		#endregion VoegToestelToe()

		#region VerwijderToestel()

		public void VerwijderToestel() {
			ResetPositionIndex();
			Table table = new();
			bool gaTerug = false;

			do {
				List<string> toestellen = _domeinController.GeefAlleToestellen();
				List<string> gereserveerdeToestellen = _domeinController.GeefGereserveerdeToestellen();

				List<string> beschikbareToestellen = toestellen.Where(toestel => !gereserveerdeToestellen.Contains(toestel)).ToList();
				List<string> optieLijst = beschikbareToestellen.Select(toestel => {
					table.AddRow(toestel);
					string row = table.ToString();
					table.Clear();
					return row;
				}).ToList();

				optieLijst.Add(StopOpties[1]);
				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Druk op [ ▲ | ▼ ] om de dag te selecteren\nDruk op [Enter] om te verwijderen\n\n", metPijl: false, metEinde: true);

				if (selectedIndex == optieLijst.Count - 1)
					gaTerug = true;
				else {
					string toestelNaam = _domeinController.GeefToestelNaamOpIndex(selectedIndex);
					int toestelId = _domeinController.GeefToestelIdOpIndex(selectedIndex);

					string input = Utility.AskUser.ReadInput(prompt: $"Weet je het zeker dat je: {toestelNaam} wil verwijderen ! (ja/NEE):", metAchtergrond: true, promptColor: ConsoleColor.Black, color: DefaultReadLineColor);

					Console.Clear();

					if (input.ToLower().Trim() == "ja") {
						_domeinController.VerwijderToestelOpId(toestelId);
						Utility.Logger.Info($"{toestelNaam} is verwijdered.");
						Utility.AskUser.ReadKnop(promptColor: DefaultReadLineColor);
					} else {
						Utility.Logger.Info("\nToestel verwijderen is geannuleerd door beheerder.");
						Utility.AskUser.ReadKnop(promptColor: DefaultReadLineColor);
					}
				}
			} while (!gaTerug);
			AssignOudePositie();
		}

		#endregion VerwijderToestel()

		#region ToonAlleReservaties()

		public void ToonAlleReservaties() {
			ResetPositionIndex();

			List<string> reservaties = _domeinController.GeefAlleReservaties();

			Table table = new();
			table.SetHeaders("Reservaties");

			reservaties.ForEach(reservatie => table.AddRow(reservatie));

			Utility.Logger.Info(table.ToString());
			Utility.AskUser.ReadKnop(prompt: "Druk op een knop om verder te gaan");

			AssignOudePositie();
		}

		#endregion ToonAlleReservaties()

		#region ToonAlleToestellen()

		public void ToonAlleToestellen() {
			ResetPositionIndex();
			Table table = new();

			bool gaTerug = false;
			bool gaTerugConfig = false;

			do {
				List<string> toestellen = _domeinController.GeefAlleToestellen();

				List<string> optieLijst = toestellen.Select(toestel => {
					table.AddRow(toestel);
					string row = table.ToString();
					table.Clear();
					return row;
				}).ToList();

				optieLijst.Add(StopOpties[1]);
				int selectedIndexToestel = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen\n", metPijl: false, metEinde: true);

				ResetPositionIndex();

				if (selectedIndexToestel != optieLijst.Count - 1) {
					do {
						string toestelNaam = _domeinController.GeefAlleToestellen()[selectedIndexToestel];

						bool toestelInHerstelling = _domeinController.GeefToestelHerstelStatusOpIndex(selectedIndexToestel);

						optieLijst.Clear();
						optieLijst.Add($"InHerstelling: {Utility.SchrijfUnderline(toestelInHerstelling.ToString())}");
						optieLijst.Add($"Naam: {Utility.SchrijfUnderline(toestelNaam.Split("-")[0].Trim())}");
						optieLijst.Add(StopOpties[1]);

						int selectedIndexToestelInfo = Utility.OptieLijstConroller(optieLijst, "", metEinde: true);
						gaTerugConfig = false;

						switch (selectedIndexToestelInfo) {
							case 0:
								ConsoleKey key;
								toestelInHerstelling = _domeinController.GeefToestelHerstelStatusOpIndex(selectedIndexToestel);

								bool currentHerstelling = toestelInHerstelling;
								do {
									Utility.Logger.Info($"InHerstelling:", newLine: false);
									Utility.Logger.Info($" {Utility.SchrijfUnderline(currentHerstelling.ToString())}", metAchtergrond: false, color: ConsoleColor.Gray);

									key = Utility.AskUser.ReadKnop(prompt: "Druk op [Spactiebar] om een toestel in/uit herstelling te halen.\nDruk op [Enter] om te bevestigen", promptColor: DefaultInfoBackgroundPrintLineColor);

									if (key == ConsoleKey.Spacebar)
										currentHerstelling = !currentHerstelling;
									Console.Clear();
								} while (key != ConsoleKey.Enter);

								_domeinController.ZetToestelInOfUitHerstelling(selectedIndexToestel, currentHerstelling);
								break;

							case 1:
								string naam;
								do {
									toestelNaam = _domeinController.GeefAlleToestellen()[selectedIndexToestel];
									Utility.Logger.Info($"Naam op dit moment:", newLine: false);
									Utility.Logger.Info($" {Utility.SchrijfUnderline(toestelNaam.Split("-")[0].Trim())}", metAchtergrond: false, color: ConsoleColor.Gray);

									naam = Utility.AskUser.ReadInput(prompt: "Nieuwe naam:", metAchtergrond: true, promptColor: ConsoleColor.Black);

									Console.Clear();
								} while (string.IsNullOrWhiteSpace(naam));
								_domeinController.UpdateToestelNaamOpIndex(selectedIndexToestel, naam);
								break;

							case 2:
								gaTerugConfig = true;
								break;
						}
						ResetPositionIndex();
					} while (!gaTerugConfig);

					Console.Clear();
				} else gaTerug = true;
			} while (!gaTerug);
			SelectedIndex = 3;
		}

		#endregion ToonAlleToestellen()

		#region RegistreerKlant(bool isBeheerder)

		public bool RegistreerKlant(bool isBeheerder = false) {
			ResetPositionIndex();

			string voornaam = string.Empty;
			string achternaam = string.Empty;
			string email = string.Empty;
			string straatNaam = string.Empty;
			string huisNummer = string.Empty;
			string plaats = string.Empty;
			string typeAlsString = isBeheerder ? "Beheerder" : string.Empty;

			int selectedAdresIndex, postCode = 0;
			int selectedAbonnementIndex;
			int selectedInteresseIndex;
			int selectedRegistreerOptieIndex;

			DateTime geboorteDatum = new();

			Adres adres = new();

			TypeKlant type = TypeKlant.Bronze;  // Info: Moet er staat anders krijg je een null reference error, Hieronder krijgt hij een waarde.

			List<string> interesses = new();
			List<string> interesseOpties = new() { "Voeg interesse toe.", StopOpties[1] };
			List<string> optieLijst;

			bool straatNaamIngevuld = false, huisNummerIngevuld = false, plaatsIngevuld = false, postCodeIngevuld = false;
			bool isOk, VnaamOk = false, AnaamOk = false, InterOk = isBeheerder, AdrOk = false, TypOk = isBeheerder, GebrOk = false, EmailOk = false, gaTerug, CompleetOk = false;

			do {
				optieLijst = new() {
					$"Voornaam: {(string.IsNullOrEmpty(voornaam) ? _requiredCharacter : voornaam)}",
					$"Achternaam: {(string.IsNullOrEmpty(achternaam) ? _requiredCharacter : achternaam)}",
					$"Email: {(string.IsNullOrEmpty(email) ? _requiredCharacter : email)}",
					$"GeboorteDatum: {(GebrOk ? geboorteDatum.ToShortDateString() : _requiredCharacter)}",
					$"Adres: {(AdrOk ? $"{adres.StraatNaam}..." : _requiredCharacter)}",
					$"Abonnement: {(TypOk ? typeAlsString : _requiredCharacter)}",
					$"Interesses: {(interesses.Count == 0 ? (isBeheerder ? "Niet Van Toepassing" : _requiredCharacter) : string.Join(", ", interesses))}\n",
					StopOpties[0]
				};

				if (VnaamOk && AnaamOk && AdrOk && GebrOk && InterOk && TypOk && EmailOk) optieLijst.Add(GaVerderOpties[0]);

				selectedRegistreerOptieIndex = Utility.OptieLijstConroller(optieLijst, metEinde: true);
				switch (selectedRegistreerOptieIndex) {

					#region |=> Voornaam

					case 0:
						Utility.Logger.Info($"--- Voornaam ---", true);

						voornaam = Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
						VnaamOk = true;

						SchuifIndexPositieOp();
						break;

					#endregion |=> Voornaam

					#region |=> Achternaam

					case 1:
						Utility.Logger.Info($"--- Achternaam ---", true);

						achternaam = Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
						AnaamOk = true;

						SchuifIndexPositieOp();
						break;

					#endregion |=> Achternaam

					#region |=> Email

					case 2:

						bool isEmailOk;
						do {
							try {
								Utility.Logger.Info($"--- Email ---", true);
								email = Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
								Klant.CheckEmail(email);
								isEmailOk = true;
							} catch (EmailExpection error) {
								Console.Clear();
								Utility.Logger.Info(error.Message, true);
								isEmailOk = false;
							}
						} while (!isEmailOk);

						EmailOk = true;

						SchuifIndexPositieOp();
						break;

					#endregion |=> Email

					#region |=> GeboorteDatum

					case 3:
						do {
							Utility.Logger.Info($"--- GeboorteDatum (DD/MM/YYYY) ---");

							isOk = DateTime.TryParse(Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix), out geboorteDatum);
							if (!isOk) {
								Console.Clear();
								Utility.Logger.Error("GeboorteDatum is niet in de correcte vorm!", metKeyPress: false);
							}
						} while (!isOk);
						GebrOk = true;

						SchuifIndexPositieOp();
						break;

					#endregion |=> GeboorteDatum

					#region |=> Adres

					case 4:
						ResetPositionIndex();
						do {
							gaTerug = false;

							List<string> adresOpties = new() {
								$"Straat: {(straatNaamIngevuld ? straatNaam : _requiredCharacter)}",
								$"HuisNr: {(huisNummerIngevuld ? huisNummer : _requiredCharacter)}",
								$"Plaats: {(plaatsIngevuld ? plaats : _requiredCharacter)}",
								$"PostCode: {(postCodeIngevuld ? postCode.ToString() : _requiredCharacter)}\n",
								StopOpties[1]
							};

							selectedAdresIndex = Utility.OptieLijstConroller(adresOpties, "Adres:");

							switch (selectedAdresIndex) {

								#region Straat

								case 0:
									Utility.Logger.Info("--- Straat ---");

									straatNaam = Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
									straatNaamIngevuld = true;

									SchuifIndexPositieOp();
									break;

								#endregion Straat

								#region Nummer

								case 1:
									Utility.Logger.Info("--- Nummer ---");

									huisNummer = Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
									huisNummerIngevuld = true;

									SchuifIndexPositieOp();
									break;

								#endregion Nummer

								#region Plaats

								case 2:
									Utility.Logger.Info("--- Plaats ---");

									plaats = Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
									plaatsIngevuld = true;

									SchuifIndexPositieOp();
									break;

								#endregion Plaats

								#region PostCode

								case 3:
									do {
										Utility.Logger.Info("--- PostCode ---");
										isOk = int.TryParse(Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix), out postCode);

										if (!isOk) {
											Console.Clear();
											Utility.Logger.Error("Postcode is niet in de correcte vorm! -> B.v [9000, 9860, ...]", metKeyPress: false);
										}
									} while (!isOk);

									postCodeIngevuld = true;
									break;

								#endregion PostCode

								#region Back

								case 4:
									gaTerug = true;
									break;

									#endregion Back
							}

							if (straatNaamIngevuld && huisNummerIngevuld && plaatsIngevuld && postCodeIngevuld) {
								gaTerug = true;
								AdrOk = true;
							}
						} while (!gaTerug);

						AssignOudePositie();
						SchuifIndexPositieOp();
						break;

					#endregion |=> Adres

					#region |=> Abonnement

					case 5:
						ResetPositionIndex();

						List<string> TypeAbonementen = new() { "Gold", "Silver", "Bronze" };
						if (isBeheerder) TypeAbonementen = new() { "Beheerder" };

						selectedAbonnementIndex = Utility.OptieLijstConroller(TypeAbonementen, "Abonnement:");

						if (isBeheerder) {
							type = selectedAbonnementIndex switch {
								0 => TypeKlant.Beheerder,
							};
						} else {
							type = selectedAbonnementIndex switch {
								0 => TypeKlant.Gold,
								1 => TypeKlant.Silver,
								2 => TypeKlant.Bronze,
								_ => TypeKlant.Bronze,
							};
						}

						typeAlsString = type.ToString();
						TypOk = true;

						AssignOudePositie();
						SchuifIndexPositieOp();
						break;

					#endregion |=> Abonnement

					#region |=> Interesses

					case 6:
						ResetPositionIndex();

						do {
							selectedInteresseIndex = Utility.OptieLijstConroller(interesseOpties, "interesses:", metEinde: true);

							if (selectedInteresseIndex == 0) {
								Utility.Logger.Info("--- Interesse ---");

								interesses.Add(Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix));
								Console.Clear();
							}
						} while (selectedInteresseIndex != 1);
						InterOk = true;

						AssignOudePositie();
						break;

					#endregion |=> Interesses

					#region |=> Stop Registratie

					case 7:
						return true;

					#endregion |=> Stop Registratie

					#region |=> Compleet Registratie

					case 8:
						CompleetOk = true;
						break;

						#endregion |=> Compleet Registratie
				}
			} while (!CompleetOk);

			_domeinController.RegistreerKlant(voornaam, achternaam, email, geboorteDatum, interesses, type.ToString(), straatNaam, plaats, huisNummer, postCode);

			return false;
		}

		#endregion RegistreerKlant(bool isBeheerder)

		#region ToonKlantReservaties()

		public void ToonKlantReservaties() {
			ResetPositionIndex();

			bool heeftUitgelogd = false;

			List<string> reservaties;
			List<string> optieLijst;

			optieLijst = new() { $"Bekijken", "Verwijderen", StopOpties[1] };
			do {
				reservaties = GeefKlantReservaties();

				if (reservaties.Count != 0) {
					int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen");

					switch (selectedIndex) {
						case 0:
							Table table = new();
							table.SetHeaders("Reservaties");

							reservaties.ForEach(reservatie => table.AddRow(reservatie));

							Utility.Logger.Info(table.ToString());
							Utility.AskUser.ReadKnop();
							break;

						case 1:
							VerwijderReservatie();
							break;

						case 2:
							heeftUitgelogd = true;
							Console.Clear();
							break;
					}
				} else {
					heeftUitgelogd = true;
					Utility.Logger.Info("U heeft tot nu toe nog geen reservaties.", newLine: false);
				}
			} while (!heeftUitgelogd);

			AssignOudePositie();
		}

		#endregion ToonKlantReservaties()

		#region GeefKlantReservaties()

		public List<string> GeefKlantReservaties() => _domeinController.GeefKlantReservaties();

		#endregion GeefKlantReservaties()

		#region VerwijderReservatie()

		public void VerwijderReservatie() {
			ResetPositionIndex();

			Table table = new();
			bool gaTerug = false;

			do {
				table.Clear();

				List<string> reservaties = _domeinController.GeefKlantReservaties();
				List<string> optieLijst = reservaties.Select(toestel => {
					table.AddRow(toestel);
					string row = table.ToString();
					table.Clear();
					return row;
				}).ToList();

				if (optieLijst.Count > 0) {
					optieLijst.Add(StopOpties[1]);

					int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Druk op [ ▲ | ▼ ] om de dag te selecteren\nDruk op [Enter] om te verwijderen\n\n", metPijl: false, metEinde: true);
					if (selectedIndex == optieLijst.Count - 1) gaTerug = true;
					else {
						int reservatieId = _domeinController.GeefReservatieIdOpIndex(selectedIndex);

						table.Clear();
						table.AddRow(_domeinController.GeefReservatieStringOpId(reservatieId));

						string input = Utility.AskUser.ReadInput(prompt: $"Weet je het zeker dat je onderstaande reservatie wil verwijderen !\n\n{table}\n\n(ja/NEE):", metAchtergrond: true, promptColor: ConsoleColor.Black, color: DefaultReadLineColor);
						Console.Clear();

						if (input.ToLower().Trim() == "ja") {
							_domeinController.VerwijderReservatieOpId(reservatieId);

							Utility.Logger.Info($"\nReservatie is verwijdered.");
							Utility.AskUser.ReadKnop(promptColor: DefaultReadLineColor);
						} else {
							Utility.Logger.Info("\nReservatie verwijderen is geannuleerd.");
							Utility.AskUser.ReadKnop(promptColor: DefaultReadLineColor);
						}
					}
				} else {
					Console.Clear();

					Utility.Logger.Info($"Je heb geen verdere reservaties.");
					Utility.AskUser.ReadKnop(promptColor: DefaultReadLineColor);

					gaTerug = true;
				}
			} while (!gaTerug);

			AssignOudePositie();
		}

		#endregion VerwijderReservatie()

		#region Login()

		public void Login() {
			Utility.Logger.Info("Wat is je E-mailAdres:");
			string email = Utility.AskUser.ReadInput(color: DefaultInfoBackgroundPrintLineColor, prompt: Utility.SelectPrefix).ToLower();
			_domeinController.Login(email);
		}
		#endregion Login()

		#region Logout()

		public void Logout() => _domeinController.Logout();

		#endregion Logout()

		#region ToonKlantDetails()

		public void ToonKlantDetails() => ToonKlantDetails(new List<string>() { _domeinController.KlantOmschrijving });

		public void ToonKlantDetails(List<string> klanten) {
			Table table = new();

			klanten.ForEach(klant => {
				table.AddRow(klant);

				Utility.Logger.Info($"{table}", metAchtergrond: false, color: DefaultInfoBackgroundPrintLineColor);

				table.Clear();
			});

			Utility.AskUser.ReadKnop();
		}

		#endregion ToonKlantDetails()

		#region ToonAlleKlanten()

		public void ToonAlleKlanten() {
			ResetPositionIndex();

			List<string> klanten = _domeinController.GeefAlleKlanten();
			ToonKlantDetails(klanten);

			Utility.AskUser.ReadKnop();
			AssignOudePositie();
		}

		#endregion ToonAlleKlanten()

		#region RegistreerToestel()

		public void RegistreerToestel() {
			ResetPositionIndex();

			Table table = new();
			DateTime tijdsSlotDatum;

			List<string> beschikbaretoestellen = _domeinController.GeefBeschikbareToestellen();
			beschikbaretoestellen.Add(StopOpties[1]);

			if (beschikbaretoestellen.Count > 0) {
				int selectedIndex = Utility.OptieLijstConroller(beschikbaretoestellen, prompt: "Welk toestel wil je gebruiken.");
				if (selectedIndex == beschikbaretoestellen.Count - 1) {
					AssignOudePositie();
					return;
				}

				string naam = _domeinController.GeefBeschikbareToestellen()[selectedIndex];
				tijdsSlotDatum = TijdsDisplayPicker(naam);
				int? toestelId = _domeinController.GeefEenVrijToestelIdOpNaam(naam, tijdsSlotDatum);

				string reservatieString = "";
				if (toestelId != null) {
					reservatieString = _domeinController.VoegReservatieToe(tijdsSlotDatum, (int)toestelId);
				} else
					throw new ToestelException("Geen toestel gevonden die vrij is.");
					

				table.SetHeaders("Reservatie");
				table.AddRow(reservatieString);

				Console.Clear();

				Utility.Logger.Info(table.ToString());
				Utility.AskUser.ReadKnop();
			}
			AssignOudePositie();
		}

		#endregion RegistreerToestel()

		#region Functionaliteit TijdsDisplayControl()

		private DateTime TijdsDisplayPicker(string toestelNaam) {

			DateTime lowerBoundDag = DateTime.Now;
			DateTime upperBoundDag = DateTime.Now.Date.AddDays(_maximumDagenReserverenToekomst);
			DateTime dag = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, TijdsSlot.LowerBoundUurReservatie, 0, 0);
			DateTime datum;

			List<int> beschikbareUren;
			bool kanNogReservaren;

			ConsoleKey keyPressed;

			Console.CursorVisible = false;

			Utility.Logger.Info($"\rDruk op [ ▲ | ▼ ] om de dag te wijzigen.\nDruk op [Enter] om te bevestigen.");

			do {
				(beschikbareUren, kanNogReservaren) = _domeinController.GeefBeschikbareReservatieUren(dag, toestelNaam);

				if (beschikbareUren.Count == 0) {
					beschikbareUren.Clear();
					Console.SetCursorPosition(0, Console.CursorTop);
					Utility.Logger.Info(new string(' ', Console.WindowWidth), newLine: false, metAchtergrond: false);
					Console.SetCursorPosition(0, Console.CursorTop);

					Console.ResetColor();
					Utility.Logger.Info("\rKies een dag: ", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);

					Console.ForegroundColor = ConsoleColor.DarkRed;
					Utility.Logger.Info($"{Utility.SchrijfUnderline(dag.Day.ToString("00"))}", newLine: false, metAchtergrond: false);

					Console.ResetColor();
					Utility.Logger.Info($"/{dag.Month:00}/{dag.Year}", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);

					if (!kanNogReservaren)
						Utility.Logger.Info(" [max 4 reservaties per dag]", newLine: false, metAchtergrond: false, color: DefaultInfoBackgroundPrintLineColor);
					else
						Utility.Logger.Info(" [geen reservaties meer beschikbaar]", newLine: false, metAchtergrond: false, color: DefaultInfoBackgroundPrintLineColor);

					do {
						keyPressed = Console.ReadKey(false).Key;

						if (keyPressed == ConsoleKey.UpArrow) {
							if (dag < upperBoundDag) dag = dag.AddDays(1);
						} else if (keyPressed == ConsoleKey.DownArrow)
							if (dag > lowerBoundDag) dag = dag.AddDays(-1);
					} while (keyPressed == ConsoleKey.Enter);
				} else {
					Console.SetCursorPosition(0, Console.CursorTop);
					Utility.Logger.Info(new string(' ', Console.WindowWidth), newLine: false, metAchtergrond: false);
					Console.SetCursorPosition(0, Console.CursorTop);

					Console.ResetColor();

					Utility.Logger.Info("\rKies een dag: ", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);
					Utility.Logger.Info($"{Utility.SchrijfUnderline(dag.Day.ToString("00"))}", newLine: false, metAchtergrond: false);
					Utility.Logger.Info($"/{dag.Month:00}/{dag.Year}", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);

					keyPressed = Console.ReadKey(false).Key;

					if (keyPressed == ConsoleKey.UpArrow) {
						if (dag < upperBoundDag) dag = dag.AddDays(1);
					} else if (keyPressed == ConsoleKey.DownArrow)
						if (dag > lowerBoundDag) dag = dag.AddDays(-1);
				}
			} while (keyPressed != ConsoleKey.Enter);

			Console.Clear();
			Utility.Logger.Info($"\rDruk op [ ▲ | ▼ ] om het uur te wijzigen.\nDruk op [Enter] om te bevestigen.");

			do {
				datum = new DateTime(dag.Year, dag.Month, dag.Day, beschikbareUren[_domeinController.UurIndex], 0, 0);

				Utility.Logger.Info("\rKies een uur: ", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);
				Utility.Logger.Info($"{Utility.SchrijfUnderline((datum.Hour.ToString().Length == 1 ? $"0{datum.Hour}" : $"{datum.Hour}"))}", newLine: false, metAchtergrond: false);
				Utility.Logger.Info(":00/uur", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);

				keyPressed = Console.ReadKey(false).Key;

				if (keyPressed == ConsoleKey.UpArrow && beschikbareUren[_domeinController.UurIndex] < TijdsSlot.UpperBoundUurReservatie && beschikbareUren[_domeinController.UurIndex] != beschikbareUren[^1])
					_domeinController.UurIndex++;
				else if (keyPressed == ConsoleKey.DownArrow && beschikbareUren[_domeinController.UurIndex] > TijdsSlot.UpperBoundUurReservatie && beschikbareUren[_domeinController.UurIndex] != beschikbareUren[0])
					_domeinController.UurIndex--;
			} while (keyPressed != ConsoleKey.Enter);
			Console.CursorVisible = true;

			_domeinController.ResetUurIndex();
			return datum;
		}

		#endregion Functionaliteit TijdsDisplayControl()

		#region GeefBeschikbareToestellen()

		public List<string> GeefBeschikbareToestellen() => _domeinController.GeefBeschikbareToestellen();

		#endregion GeefBeschikbareToestellen()

		#region ConsoleIndexPostion()

		private static void ResetPositionIndex() {
			OudeselectedIndex = SelectedIndex;
			SelectedIndex = 0;
		}

		private static void AssignOudePositie() {
			SelectedIndex = OudeselectedIndex;
		}

		private static void SchuifIndexPositieOp() {
			SelectedIndex++;
		}

		#endregion ConsoleIndexPostion()
	}
}