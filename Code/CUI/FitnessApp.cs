using Domein;
using Persistentie;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CUI {
	public class FitnessApp {
		#region Private Fields
		private static readonly IKlantenRepository _klantenRepo = new KlantenMapper();
		private static readonly IReservatieRepository _reservatienRepo = new ReservatieMapper();
		private static readonly IToestelRepository _toestellenRepo = new ToestellenMapper();
		private static readonly UniekeCode _uniekeCode = UniekeCode.Instance;
		private readonly DomeinController _domeinController = new(_reservatienRepo, _klantenRepo, _toestellenRepo);
		private const char _requiredCharacter = '#';
		private const int _maximumDagenReserverenToekomst = 7;
		#endregion

		#region Public Properties
		public static readonly ConsoleColor DefaultReadLineColor = ConsoleColor.DarkCyan;

		public static readonly ConsoleColor DefaultInfoBackgroundPrintLineColor = ConsoleColor.DarkYellow;
		public static readonly ConsoleColor DefaultInfoForeGrountPrintLineColor = ConsoleColor.Black;

		public static readonly ConsoleColor DefaultErrorBackgroundPrintLineColor = ConsoleColor.DarkRed;
		public static readonly ConsoleColor DefaultErrorForeGrountPrintLineColor = ConsoleColor.White;

		public static readonly List<string> GaVerderOpties = new() {
			Utility.SchrijfUnderline("Ga Verder"),
		};
		public static readonly List<string> StopOpties = new() {
			Utility.SchrijfUnderline("Stop Registratie"),
			Utility.SchrijfUnderline("Ga Terug"),
			Utility.SchrijfUnderline("Uitloggen")
		};
		public static readonly List<string> DisabledOptie = new() {
			"Reserveer Toestel - (Geen toestellen beschikbaar)"
		};

		public static int SelectedIndex = 0;
		public static int OudeselectedIndex;
		#endregion

		#region VoegToestelToe
		public void VoegToestelToe() {
			string naam;
			bool naamIsOk = true;
			do {
				naam = Utility.ColorInput.ReadInput(prompt: "Welk toestel wil je toevoegen ?");
				if (string.IsNullOrWhiteSpace(naam)) {
					naamIsOk = false;
					Console.Clear();
				}
			} while (naamIsOk);
			_domeinController.VoegNieuwToestelToe(naam);
		}
		#endregion

		#region Public Debug Properties
		public static Klant DEBUGUSER = _klantenRepo.GeefAlleKlanten().Single(klant => klant.Email == "stan.persoons@student.hogent.be");
		#endregion

		#region ToonAlleReservaties()
		public void ToonAlleReservaties() {
			List<Reservatie> reservaties = _domeinController.GeefAlleReservaties();
			Utility.Table table = new();
			table.SetHeaders("Reservaties");
			reservaties.ForEach(reservatie => table.AddRow(reservatie.ToString()));
			Utility.Logger.Info(table.ToString());
		}
		#endregion

		#region ToonAlleToestellen()
		public void ToonAlleToestellen() {
			SelectedIndex = 0;
			List<Toestel> toestellen = _domeinController.GeefAlleToestellen();
			Utility.Table table = new();
			Toestel toestel;
			List<string> optieLijst = toestellen.Select(toestel => {
				table.AddRow(toestel.ToString());
				string row = table.ToString();
				table.Clear();
				return row;
			}).ToList();

			optieLijst.Add(StopOpties[1]);

			int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen\n", metPijl: false, metEinde: true);

			if (selectedIndex != optieLijst.Count - 1) {
				toestel = _domeinController.GeefAlleToestellen()[selectedIndex];
				optieLijst.Clear();
				optieLijst.Add("InHerstelling");
				optieLijst.Add(StopOpties[1]);
				selectedIndex = Utility.OptieLijstConroller(optieLijst, "", metPijl: false, metEinde: true);

				if (selectedIndex != 2) {
					Console.WriteLine(optieLijst[selectedIndex]);
					Utility.ColorInput.ReadKnop();
				}
			}

		}
		#endregion

		#region ToonAlleKlanten()
		public void ToonAlleKlanten() => _domeinController.GeefAlleKlanten();
		#endregion

		#region RegistreerKlant(bool isBeheerder)
		public (Klant, bool) RegistreerKlant(bool isBeheerder = false) {
			ResetPositionIndex();

			// Voornaam
			string voornaam = string.Empty;

			// Achternaam
			string achternaam = string.Empty;

			// Email
			string email = string.Empty;

			// Adres
			Adres adres = new();
			int selectedAdresIndex, postCode = 0;
			string straatNaam = string.Empty, huisNummer = string.Empty, plaats = string.Empty;
			bool straatNaamIngevuld = false, huisNummerIngevuld = false, plaatsIngevuld = false, postCodeIngevuld = false;

			//Abonnement
			int selectedAbonnementIndex;
			string typeAlsString = isBeheerder ? "Beheerder" : string.Empty;
			TypeKlant type = TypeKlant.Bronze;  // Info: Moet er staat anders krijg je een null reference error, Hieronder krijgt hij een waarde.

			//Interesses
			int selectedInteresseIndex;
			List<string> interesses = new();
			List<string> interesseOpties = new() { "Voeg interesse toe.", StopOpties[1] };

			//GeboorteDatum
			DateTime geboorteDatum = new();

			List<string> optieLijst;
			int selectedRegistreerOptieIndex;
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

				if (VnaamOk && AnaamOk && AdrOk && GebrOk && InterOk && TypOk && EmailOk) {
					optieLijst.Add(GaVerderOpties[0]);
				}

				selectedRegistreerOptieIndex = Utility.OptieLijstConroller(optieLijst, metEinde: true);
				switch (selectedRegistreerOptieIndex) {
					#region |=> Voornaam
					case 0:
						Utility.Logger.Info($"--- Voornaam ---", true);
						voornaam = Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
						VnaamOk = true;
						SchuifIndexPositieOp();
						break;
					#endregion

					#region |=> Achternaam
					case 1:
						Utility.Logger.Info($"--- Achternaam ---", true);
						achternaam = Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
						AnaamOk = true;
						SchuifIndexPositieOp();
						break;
					#endregion

					#region |=> Email
					case 2:
						Utility.Logger.Info($"--- Email ---", true);
						email = Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
						EmailOk = true;
						SchuifIndexPositieOp();
						break;
					#endregion

					#region |=> GeboorteDatum
					case 3:
						do {
							Utility.Logger.Info($"--- GeboorteDatum (DD/MM/YYYY) ---");
							isOk = DateTime.TryParse(Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix), out geboorteDatum);
							if (!isOk) {
								Console.Clear();
								Utility.Logger.Error("GeboorteDatum is niet in de correcte vorm!", metKeyPress: false);
							}
						} while (!isOk);
						GebrOk = true;
						SchuifIndexPositieOp();
						break;
					#endregion

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
									straatNaam = Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
									straatNaamIngevuld = true;
									SchuifIndexPositieOp();
									break;
								#endregion
								#region Nummer
								case 1:
									Utility.Logger.Info("--- Nummer ---");
									huisNummer = Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
									huisNummerIngevuld = true;
									SchuifIndexPositieOp();
									break;
								#endregion
								#region Plaats
								case 2:
									Utility.Logger.Info("--- Plaats ---");
									plaats = Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
									plaatsIngevuld = true;
									SchuifIndexPositieOp();
									break;
								#endregion
								#region PostCode
								case 3:
									do {
										Utility.Logger.Info("--- PostCode ---");
										isOk = int.TryParse(Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix), out postCode);
										if (!isOk) {
											Console.Clear();
											Utility.Logger.Error("Postcode is niet in de correcte vorm! -> B.v [9000, 9860, ...]", metKeyPress: false);
										}
									} while (!isOk);
									postCodeIngevuld = true;
									break;
								#endregion
								#region Back
								case 4:
									gaTerug = true;
									break;
									#endregion
							}
							if (straatNaamIngevuld && huisNummerIngevuld && plaatsIngevuld && postCodeIngevuld) {
								gaTerug = true;
								AdrOk = true;
							}
						} while (!gaTerug);
						adres = new Adres(straatNaam, huisNummer, plaats, postCode);
						AssignOudePositie();
						SchuifIndexPositieOp();
						break;
					#endregion

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
					#endregion

					#region |=> Interesses
					case 6:
						ResetPositionIndex();
						do {
							selectedInteresseIndex = Utility.OptieLijstConroller(interesseOpties, "interesses:", metEinde: true);
							if (selectedInteresseIndex == 0) {
								Utility.Logger.Info("--- Interesse ---");
								interesses.Add(Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix));
								Console.Clear();
							}
						} while (selectedInteresseIndex != 1);
						InterOk = true;
						AssignOudePositie();
						break;
					#endregion

					#region |=> Stop Registratie
					case 7:
						return (null, true);
					#endregion

					#region |=> Compleet Registratie
					case 8:
						CompleetOk = true;
						break;
						#endregion
				}
			} while (!CompleetOk);

			Klant klant = new(_uniekeCode.GenereerRandomCode(), voornaam, achternaam, email, interesses, geboorteDatum, adres, type);
			_domeinController.RegistreerKlant(klant);
			return (klant, false);
		}
		#endregion

		#region ToonKlantReservaties(Klant klant)
		public void ToonKlantReservaties(Klant klant) {
			List<Reservatie> reservaties = _domeinController.GeefKlantReservaties(klant);

			if (reservaties.Count != 0) {
				Utility.Table table = new();
				table.SetHeaders("Reservaties");
				reservaties.ForEach(reservatie => table.AddRow(reservatie.ToString()));
				Utility.Logger.Info(table.ToString());
				Utility.ColorInput.ReadKnop();
			} else {
				Utility.Logger.Info("U heeft tot nu toe nog geen reservaties.", newLine: false);
			}
		}
		#endregion

		#region Login()
		public Klant Login(bool isBeheerder = false) {
			Utility.Logger.Info("Wat is je E-mailAdres:");
			string email = Utility.ColorInput.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix).ToLower();
			Klant klant = _domeinController.Login(email);
			if (isBeheerder && klant.TypeKlant != TypeKlant.Beheerder) throw new LoginException($"{klant.Voornaam} {klant.Achternaam} is geen beheerder.");
			return klant;
		}
		#endregion

		#region ToonKlantDetails(Klant klant)
		public void ToonKlantDetails(Klant klant) {
			Utility.Table table = new();
			table.SetHeaders("KlantenNummer", "Naam", "Email", "GeboorteDatum", "Abonnement", "Interesses");
			table.AddRow(klant.KlantenNummer.ToString(), $"{klant.Voornaam} {klant.Achternaam}", klant.Email, klant.GeboorteDatum.ToShortDateString(), klant.TypeKlant.ToString(), string.Join(',', klant.Interesses));
			Utility.Logger.Info($"{table}\n", metAchtergrond: true);
			table.Clear();

			table.SetHeaders("Adres");
			table.AddRow("Straat", "Nr", "Plaats", "PostCode");
			table.AddRow(klant.Adres.StraatNaam, klant.Adres.HuisNummer, klant.Adres.Plaats, klant.Adres.PostCode.ToString());
			Utility.Logger.Info($"{table}\n\n\n", metAchtergrond: true);
			Utility.ColorInput.ReadKnop();
		}
		#endregion

		#region RegistreerToestel(Klant klant)
		public void RegistreerToestel(Klant klant) {
			Utility.Table table = new();
			DateTime tijdsSlotDatum;
			Toestel toestel;

			List<string> beschikbaretoestellen = _domeinController.GeefBeschikbareToestellen();

			if (beschikbaretoestellen.Count > 0) {
				do {
					int selectedIndex = Utility.OptieLijstConroller(beschikbaretoestellen, prompt: "Welk toestel wil je gebruiken.");
					toestel = _domeinController.GeefAlleToestellen()[selectedIndex];
					if (toestel == null) Console.Clear();
				} while (toestel == null);

				tijdsSlotDatum = TijdsDisplayControl();

				TijdsSlot tijdsSlot = new(tijdsSlotDatum);
				Reservatie reservatie = new(klant, tijdsSlot, toestel);

				_domeinController.VoegReservatieToe(reservatie);

				table.SetHeaders("Reservatie");
				table.AddRow(reservatie.ToString());

				Console.Clear();

				Utility.Logger.Info(table.ToString());
				Utility.ColorInput.ReadKnop();

				#region Functionaliteit TijdsDisplayControl()
				DateTime TijdsDisplayControl() {
					int lowerBoundUur = 8;
					int lowerBoundMinuten = 0;
					DateTime lowerBoundDag = DateTime.Now;

					int upperBoundUur = 22;
					int upperBoundMinuten = 59;
					DateTime upperBoundDag = DateTime.Now.Date.AddDays(_maximumDagenReserverenToekomst);

					int minuten = lowerBoundMinuten;
					DateTime dag = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, lowerBoundUur, 0, 0);
					DateTime datum;

					List<int> beschikbareUren;

					bool hasMinuten = false;
					bool pressedRight = false;

					ConsoleKey deafaultConsoleKey = ConsoleKey.A;
					ConsoleKey keyPressed = deafaultConsoleKey;

					Console.CursorVisible = false;
					StartDatumSelectie();
					Console.CursorVisible = true;

					void StartDatumSelectie() {
						SelecteerDag();
					}

					void SelecteerUur() {
						do {
							keyPressed = deafaultConsoleKey;
							datum = new DateTime(dag.Year, dag.Month, dag.Day, beschikbareUren[_domeinController.UurIndex], 0, 0);
							Utility.Logger.Info("\rKies een uur: ", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);
							Utility.Logger.Info($"{Utility.SchrijfUnderline((datum.Hour.ToString().Length == 1 ? $"0{datum.Hour}" : $"{datum.Hour}"))}", newLine: false, metAchtergrond: false);
							Utility.Logger.Info(":00/uur", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);
							keyPressed = Console.ReadKey(false).Key;

							if (keyPressed == ConsoleKey.UpArrow && beschikbareUren[_domeinController.UurIndex] < upperBoundUur && beschikbareUren[_domeinController.UurIndex] != beschikbareUren[^1]) {
								_domeinController.UurIndex++;
							} else if (keyPressed == ConsoleKey.DownArrow && beschikbareUren[_domeinController.UurIndex] > lowerBoundUur && beschikbareUren[_domeinController.UurIndex] != beschikbareUren[0]) {
								_domeinController.UurIndex--;
							}
							//else if (keyPressed == ConsoleKey.RightArrow) {
							//	pressedRight = true;
							//	SelecteerMinuten();
							//}


						} while (keyPressed != ConsoleKey.Enter);
						//if (!pressedRight) SelecteerMinuten();
					}

					void SelecteerMinuten() {
						do {
							Console.Write($"\r{(dag.Hour.ToString().Length == 1 ? $"0{dag.Hour}" : $"{dag.Hour}")}:");
							Utility.Logger.Info($"{Utility.SchrijfUnderline((minuten.ToString().Length == 1 ? $"0{minuten}" : $"{minuten}"))}", newLine: false, metAchtergrond: false);
							keyPressed = Console.ReadKey(false).Key;
							hasMinuten = true;

							if (keyPressed == ConsoleKey.UpArrow) {
								if (minuten < upperBoundMinuten) {
									minuten++;
								}
							} else if (keyPressed == ConsoleKey.DownArrow) {
								if (minuten > lowerBoundMinuten) {
									minuten--;
								}
							} else if (keyPressed == ConsoleKey.LeftArrow) {
								SelecteerUur();
							}
						} while (keyPressed != ConsoleKey.Enter);
					}

					void SelecteerDag() {
						//if (!hasMinuten) {
						Utility.Logger.Info($"\rDruk op [ ▲ | ▼ ] om de dag te wijzigen.\nDruk op [Enter] om te bevestigen.");
						do {
							keyPressed = deafaultConsoleKey;
							beschikbareUren = _domeinController.GeefBeschikbareUrenOpDatum(dag);

							if (beschikbareUren.Count == 0) {
								Console.SetCursorPosition(0, Console.CursorTop);
								Utility.Logger.Info(new string(' ', Console.WindowWidth), newLine: false, metAchtergrond: false);
								Console.SetCursorPosition(0, Console.CursorTop);
								Console.ResetColor();
								Utility.Logger.Info("\rKies een dag: ", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);
								Console.ForegroundColor = ConsoleColor.DarkRed;
								Utility.Logger.Info($"{Utility.SchrijfUnderline(dag.Day.ToString())}", newLine: false, metAchtergrond: false);
								Console.ResetColor();
								Utility.Logger.Info($"/{dag.Month}/{dag.Year}", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);
								Utility.Logger.Info(" [volboekt]", newLine: false, metAchtergrond: false, color: DefaultInfoBackgroundPrintLineColor);

								do {
									keyPressed = Console.ReadKey(false).Key;

									if (keyPressed == ConsoleKey.UpArrow) {
										if (dag.Day < upperBoundDag.Day) {
											dag = dag.AddDays(1);
										}
									} else if (keyPressed == ConsoleKey.DownArrow) {
										if (dag.Day > lowerBoundDag.Day) {
											dag = dag.AddDays(-1);
										}
									}
								} while (keyPressed == ConsoleKey.Enter);
							} else {
								Console.SetCursorPosition(0, Console.CursorTop);
								Utility.Logger.Info(new string(' ', Console.WindowWidth), newLine: false, metAchtergrond: false);
								Console.SetCursorPosition(0, Console.CursorTop);
								Console.ResetColor();
								Utility.Logger.Info("\rKies een dag: ", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);
								Utility.Logger.Info($"{Utility.SchrijfUnderline(dag.Day.ToString())}", newLine: false, metAchtergrond: false);
								Utility.Logger.Info($"/{dag.Month}/{dag.Year}", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);
								keyPressed = Console.ReadKey(false).Key;
								//hasMinuten = true;

								if (keyPressed == ConsoleKey.UpArrow) {
									if (dag.Day < upperBoundDag.Day) {
										dag = dag.AddDays(1);
									}
								} else if (keyPressed == ConsoleKey.DownArrow) {
									if (dag.Day > lowerBoundDag.Day) {
										dag = dag.AddDays(-1);
									}
								}
							}
						} while (keyPressed != ConsoleKey.Enter);
						//}
						Console.Clear();
						//Utility.Logger.Info($"\rDruk op [ ‹ | › ] om het uur te wijzigen, en [Enter] om te bevestigen.\nReservaties duren telkens 1 uur.");
						Utility.Logger.Info($"\rDruk op [ ▲ | ▼ ] om het uur te wijzigen.\nDruk op [Enter] om te bevestigen.");
						SelecteerUur();
					}

					_domeinController.ResetUurIndex();
					return datum;
				}
				#endregion
			} else {

			}
		}
		#endregion

		#region GeefBeschikbareToestellen()
		public List<string> GeefBeschikbareToestellen() => _domeinController.GeefBeschikbareToestellen();
		#endregion

		#region ConsoleIndexPostion()
		private void ResetPositionIndex() {
			OudeselectedIndex = SelectedIndex;
			SelectedIndex = 0;
		}
		private void AssignOudePositie() {
			SelectedIndex = OudeselectedIndex;
		}
		private void SchuifIndexPositieOp() {
			SelectedIndex++;
		}
		#endregion
	}
}
