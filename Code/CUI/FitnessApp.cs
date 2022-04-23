﻿using Domein;
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

		#endregion Private Fields

		#region Public Properties

		public static readonly ConsoleColor DefaultReadLineColor = ConsoleColor.Cyan;

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
			"Reserveer Toestel [Geen Toestellen Beschikbaar]",
			"Mijn Reservaties [Geen Reservaties Beschikbaar]"
		};

		public static int SelectedIndex = 0;
		public static int OudeselectedIndex;

		#endregion Public Properties

		#region VoegToestelToe

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

		#endregion VoegToestelToe

		#region VerwijderToestel()

		public void VerwijderToestel() {
			ResetPositionIndex();
			Utility.Table table = new();
			bool gaTerug = false;
			do {
				List<Toestel> toestellen = _domeinController.GeefAlleToestellen();
				List<Toestel> gereserveerdeToestellen = _domeinController.GeefAlleReservaties()
								.Where(reservatie => reservatie.TijdsSlot.EindTijd > DateTime.Now.ToUniversalTime().AddHours(2))
								.Select(reservatie => reservatie.Toestel)
								.ToList();

				List<Toestel> beschikbareToestellen = toestellen.Where(toestel => !gereserveerdeToestellen.Contains(toestel)).ToList();

				List<string> optieLijst = beschikbareToestellen.Select(toestel => {
					table.AddRow(toestel.ToString());
					string row = table.ToString();
					table.Clear();
					return row;
				}).ToList();

				optieLijst.Add(StopOpties[1]);

				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Druk op [ ▲ | ▼ ] om de dag te selecteren\nDruk op [Enter] om te verwijderen\n\n", metPijl: false, metEinde: true);

				if (selectedIndex == optieLijst.Count - 1)
					gaTerug = true;
				else {
					Toestel toestel = beschikbareToestellen[selectedIndex];
					string input = Utility.AskUser.ReadInput(prompt: $"Weet je het zeker dat je: {toestel.ToestelType} wil verwijderen ! (ja/NEE):", metAchtergrond: true, promptColor: ConsoleColor.Black, color: DefaultReadLineColor);

					Console.Clear();

					if (input.ToLower().Trim() == "ja") {
						_domeinController.VerwijderToestel(toestel);
						Utility.Logger.Info($"{toestel.ToestelType} is verwijdered.");
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

		#region Public Debug Properties

		public static Klant DEBUGUSER = _klantenRepo.GeefAlleKlanten().Single(klant => klant.Email == "stan.persoons@student.hogent.be");

		#endregion Public Debug Properties

		#region ToonAlleReservaties()

		public void ToonAlleReservaties() {
			ResetPositionIndex();

			List<Reservatie> reservaties = _domeinController.GeefAlleReservaties();

			Utility.Table table = new();
			table.SetHeaders("Reservaties");

			reservaties.ForEach(reservatie => table.AddRow(reservatie.ToString()));

			Utility.Logger.Info(table.ToString());
			Utility.AskUser.ReadKnop(prompt: "Druk op een knop om verder te gaan");

			AssignOudePositie();
		}

		#endregion ToonAlleReservaties()

		#region ToonAlleToestellen()

		public void ToonAlleToestellen() {
			ResetPositionIndex();
			List<Toestel> toestellen = _domeinController.GeefAlleToestellen();
			Utility.Table table = new();
			Toestel toestel;

			bool gaTerug = false;
			bool gaTerugConfig = false;

			do {
				List<string> optieLijst = toestellen.Select(toestel => {
					table.AddRow(toestel.ToString());
					string row = table.ToString();
					table.Clear();
					return row;
				}).ToList();

				optieLijst.Add(StopOpties[1]);
				int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen\n", metPijl: false, metEinde: true);

				ResetPositionIndex();

				if (selectedIndex != optieLijst.Count - 1) {
					toestel = _domeinController.GeefAlleToestellen()[selectedIndex];

					do {
						optieLijst.Clear();
						optieLijst.Add($"InHerstelling: {Utility.SchrijfUnderline(toestel.InHerstelling.ToString())}");
						optieLijst.Add($"Naam: {Utility.SchrijfUnderline(toestel.ToestelType)}");
						optieLijst.Add(StopOpties[1]);

						selectedIndex = Utility.OptieLijstConroller(optieLijst, "", metEinde: true);
						gaTerugConfig = false;

						switch (selectedIndex) {
							case 0:
								ConsoleKey key;
								do {
									Utility.Logger.Info($"InHerstelling:", newLine: false);
									Utility.Logger.Info($" {Utility.SchrijfUnderline(toestel.InHerstelling.ToString())}", metAchtergrond: false, color: ConsoleColor.Gray);

									key = Utility.AskUser.ReadKnop(prompt: "Druk op [Spactiebar] om een toestel in/uit herstelling te halen.\nDruk op [Enter] om te bevestigen", promptColor: DefaultInfoBackgroundPrintLineColor);
									bool currentHerstelling = toestel.InHerstelling;

									if (key == ConsoleKey.Spacebar) _toestellenRepo.ZetToestelInOfUitHerstelling(toestel);
									Console.Clear();
								} while (key != ConsoleKey.Enter);
								break;

							case 1:
								string naam;
								do {
									Utility.Logger.Info($"Naam op dit moment:", newLine: false);
									Utility.Logger.Info($" {Utility.SchrijfUnderline(toestel.ToestelType)}", metAchtergrond: false, color: ConsoleColor.Gray);

									naam = Utility.AskUser.ReadInput(prompt: "Nieuwe naam:", metAchtergrond: true, promptColor: ConsoleColor.Black);

									Console.Clear();
								} while (string.IsNullOrWhiteSpace(naam));
								toestel.ToestelType = naam.Trim();
								break;

							case 2:
								gaTerugConfig = true;
								break;
						}
					} while (!gaTerugConfig);

					Console.Clear();
				} else gaTerug = true;
			} while (!gaTerug);
			SelectedIndex = 3;
		}

		#endregion ToonAlleToestellen()

		#region LaadToestellen()
		public bool LaadToestellen() => _domeinController.LaadToestellen();
		#endregion

		#region ToonAlleKlanten()

		public void ToonAlleKlanten() {
			ResetPositionIndex();

			List<Klant> klanten = _domeinController.GeefAlleKlanten();
			ToonKlantDetails(klanten);

			Utility.AskUser.ReadKnop();
			AssignOudePositie();
		}

		#endregion ToonAlleKlanten()

		#region LaadKlanten
		public bool LaadKlanten() => _domeinController.LaadKlanten();
		#endregion

		#region RegistreerKlant(bool isBeheerder)

		public (Klant, bool) RegistreerKlant(bool isBeheerder = false) {
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
						Utility.Logger.Info($"--- Email ---", true);

						email = Utility.AskUser.ReadInput(DefaultInfoBackgroundPrintLineColor, Utility.SelectPrefix);
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
						adres = new Adres(straatNaam, huisNummer, plaats, postCode);

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
						return (null, true);

					#endregion |=> Stop Registratie

					#region |=> Compleet Registratie

					case 8:
						CompleetOk = true;
						break;

						#endregion |=> Compleet Registratie
				}
			} while (!CompleetOk);

			Klant klant = new(_uniekeCode.GenereerRandomCode(), voornaam, achternaam, email, interesses, geboorteDatum, adres, type);
			_domeinController.RegistreerKlant(klant);

			return (klant, false);
		}

		#endregion RegistreerKlant(bool isBeheerder)

		#region ToonKlantReservaties(Klant klant)

		public void ToonKlantReservaties(Klant klant) {
			ResetPositionIndex();

			bool heeftUitgelogd = false;

			List<Reservatie> reservaties;
			List<string> optieLijst;

			optieLijst = new() { $"Bekijken", "Verwijderen", StopOpties[1] };
			do {
				reservaties = GeefKlantReservaties(klant);

				if (reservaties.Count != 0) {
					int selectedIndex = Utility.OptieLijstConroller(optieLijst, "\rDruk op [ ▲ | ▼ ] om de dag te wijzigen\nDruk op [Enter] om te bevestigen");

					switch (selectedIndex) {
						case 0:
							Utility.Table table = new();
							table.SetHeaders("Reservaties");

							reservaties.ForEach(reservatie => table.AddRow(reservatie.ToString()));

							Utility.Logger.Info(table.ToString());
							Utility.AskUser.ReadKnop();
							break;

						case 1:
							VerwijderReservatie(klant);
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

		#endregion ToonKlantReservaties(Klant klant)

		#region GeefKlantReservaties(Klant klant)

		public List<Reservatie> GeefKlantReservaties(Klant klant) => _domeinController.GeefKlantReservaties(klant);

		#endregion GeefKlantReservaties(Klant klant)

		#region VerwijderReservatie(klant klant)

		public void VerwijderReservatie(Klant klant) {
			ResetPositionIndex();

			Utility.Table table = new();
			bool gaTerug = false;

			do {
				table.Clear();

				List<Reservatie> reservaties = _domeinController.GeefKlantReservaties(klant);
				List<string> optieLijst = reservaties.Select(toestel => {
					table.AddRow(toestel.ToString());
					string row = table.ToString();
					table.Clear();
					return row;
				}).ToList();

				if (optieLijst.Count > 0) {
					optieLijst.Add(StopOpties[1]);

					int selectedIndex = Utility.OptieLijstConroller(optieLijst, "Druk op [ ▲ | ▼ ] om de dag te selecteren\nDruk op [Enter] om te verwijderen\n\n", metPijl: false, metEinde: true);
					if (selectedIndex == optieLijst.Count - 1) gaTerug = true;
					else {
						Reservatie reservatie = reservaties[selectedIndex];

						table.Clear();
						table.AddRow(reservatie.ToString());

						string input = Utility.AskUser.ReadInput(prompt: $"Weet je het zeker dat je onderstaande reservatie wil verwijderen !\n\n{table}\n\n(ja/NEE):", metAchtergrond: true, promptColor: ConsoleColor.Black, color: DefaultReadLineColor);
						Console.Clear();

						if (input.ToLower().Trim() == "ja") {
							_domeinController.VerwijderReservatie(reservatie);

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

		#endregion VerwijderReservatie(klant klant)

		#region Login()

		public Klant Login(bool isBeheerder = false) {
			string email;

			Utility.Logger.Info("Wat is je E-mailAdres:");
			email = Utility.AskUser.ReadInput(color: DefaultInfoBackgroundPrintLineColor, prompt: Utility.SelectPrefix).ToLower();

			Klant klant = _domeinController.Login(email);
			if (isBeheerder && klant.TypeKlant != TypeKlant.Beheerder) throw new LoginException($"{klant.Voornaam} {klant.Achternaam} is geen beheerder.");

			return klant;
		}

		#endregion Login()

		#region ToonKlantDetails()

		public void ToonKlantDetails(Klant klant) {
			List<Klant> klanten = new();

			klanten.Add(klant);
			ToonKlantDetails(klanten);

			Utility.AskUser.ReadKnop();
		}

		public void ToonKlantDetails(List<Klant> klanten) {
			Utility.Table table = new();

			klanten.ForEach(klant => {
				table.SetHeaders("KlantenNummer", "Naam", "Email", "GeboorteDatum", "Abonnement", "Straat", "Huis Nr", "Plaats", "PostCode");
				table.AddRow(klant.KlantenNummer.ToString(), $"{klant.Voornaam} {klant.Achternaam}", klant.Email, klant.GeboorteDatum.ToShortDateString(), klant.TypeKlant.ToString(), klant.Adres.StraatNaam, klant.Adres.HuisNummer, klant.Adres.Plaats, klant.Adres.PostCode.ToString());

				Utility.Logger.Info($"{table}", metAchtergrond: false, color: DefaultInfoBackgroundPrintLineColor);

				table.Clear();
			});
		}

		#endregion ToonKlantDetails()

		#region RegistreerToestel()

		public void RegistreerToestel(Klant klant) {
			ResetPositionIndex();

			Utility.Table table = new();
			DateTime tijdsSlotDatum;
			Toestel toestel;

			List<string> beschikbaretoestellen = _domeinController.GeefBeschikbareToestellen();
			beschikbaretoestellen.Add(StopOpties[1]);

			if (beschikbaretoestellen.Count > 0) {
				do {
					int selectedIndex = Utility.OptieLijstConroller(beschikbaretoestellen, prompt: "Welk toestel wil je gebruiken.");
					if (selectedIndex == beschikbaretoestellen.Count - 1) {
						AssignOudePositie();
						return;
					}

					string toestelNaamRaw = beschikbaretoestellen[selectedIndex];
					int toestelNaamIndexOf = toestelNaamRaw.IndexOf("[");

					if (toestelNaamIndexOf == -1)
						toestel = _domeinController.GeefToestelOpNaam(toestelNaamRaw);
					else
						toestel = _domeinController.GeefToestelOpNaam(toestelNaamRaw[..toestelNaamIndexOf].Trim());

					if (toestel == null) Console.Clear();
				} while (toestel == null);

				tijdsSlotDatum = TijdsDisplayPicker(klant, toestel);

				TijdsSlot tijdsSlot = new(tijdsSlotDatum);
				Reservatie reservatie = new(klant, tijdsSlot, toestel);

				_domeinController.VoegReservatieToe(reservatie);

				table.SetHeaders("Reservatie");
				table.AddRow(reservatie.ToString());

				Console.Clear();

				Utility.Logger.Info(table.ToString());
				Utility.AskUser.ReadKnop();
			}
			AssignOudePositie();
		}

		#endregion RegistreerToestel()

		#region Functionaliteit TijdsDisplayControl()

		private DateTime TijdsDisplayPicker(Klant klant, Toestel toestel) {
			int lowerBoundUur = 8;
			int upperBoundUur = 22;

			DateTime lowerBoundDag = DateTime.Now;
			DateTime upperBoundDag = DateTime.Now.Date.AddDays(_maximumDagenReserverenToekomst);
			DateTime dag = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, lowerBoundUur, 0, 0);
			DateTime datum;

			List<int> beschikbareUren;
			bool kanNogReservaren;

			ConsoleKey deafaultConsoleKey = ConsoleKey.A;
			ConsoleKey keyPressed = deafaultConsoleKey;

			Console.CursorVisible = false;

			Utility.Logger.Info($"\rDruk op [ ▲ | ▼ ] om de dag te wijzigen.\nDruk op [Enter] om te bevestigen.");

			do {
				(beschikbareUren, kanNogReservaren) = _domeinController.GeefBeschikbareUrenOpDatum(dag, klant, toestel);
				keyPressed = deafaultConsoleKey;

				if (beschikbareUren.Count == 0) {

					beschikbareUren.Clear();
					Console.SetCursorPosition(0, Console.CursorTop);
					Utility.Logger.Info(new string(' ', Console.WindowWidth), newLine: false, metAchtergrond: false);
					Console.SetCursorPosition(0, Console.CursorTop);

					Console.ResetColor();
					Utility.Logger.Info("\rKies een dag: ", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);

					Console.ForegroundColor = ConsoleColor.DarkRed;
					Utility.Logger.Info($"{Utility.SchrijfUnderline(dag.Day.ToString())}", newLine: false, metAchtergrond: false);

					Console.ResetColor();
					Utility.Logger.Info($"/{dag.Month}/{dag.Year}", newLine: false, metAchtergrond: false, color: ConsoleColor.DarkYellow);

					if (!kanNogReservaren)
						Utility.Logger.Info(" [max 4 reservaties per dag]", newLine: false, metAchtergrond: false, color: DefaultInfoBackgroundPrintLineColor);
					else
						Utility.Logger.Info(" [geen reservaties meer beschikbaar]", newLine: false, metAchtergrond: false, color: DefaultInfoBackgroundPrintLineColor);

					do {
						keyPressed = Console.ReadKey(false).Key;

						if (keyPressed == ConsoleKey.UpArrow) {
							if (dag.Day < upperBoundDag.Day) dag = dag.AddDays(1);
						} else if (keyPressed == ConsoleKey.DownArrow)
							if (dag.Day > lowerBoundDag.Day) dag = dag.AddDays(-1);
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

					if (keyPressed == ConsoleKey.UpArrow) {
						if (dag.Day < upperBoundDag.Day) dag = dag.AddDays(1);
					} else if (keyPressed == ConsoleKey.DownArrow)
						if (dag.Day > lowerBoundDag.Day) dag = dag.AddDays(-1);
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

				if (keyPressed == ConsoleKey.UpArrow && beschikbareUren[_domeinController.UurIndex] < upperBoundUur && beschikbareUren[_domeinController.UurIndex] != beschikbareUren[^1])
					_domeinController.UurIndex++;
				else if (keyPressed == ConsoleKey.DownArrow && beschikbareUren[_domeinController.UurIndex] > lowerBoundUur && beschikbareUren[_domeinController.UurIndex] != beschikbareUren[0])
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

		#endregion ConsoleIndexPostion()
	}
}