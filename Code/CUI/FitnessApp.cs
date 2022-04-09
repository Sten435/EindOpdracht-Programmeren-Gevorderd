using Domein;
using Persistentie;
using System;
using System.Collections.Generic;

namespace CUI {
	public class FitnessApp {
		private static readonly IKlantenRepository _klantenRepo = new KlantenMapper();
		private static readonly IReservatieRepository _reservatienRepo = new ReservatieMapper();
		private static readonly IToestelRepository _toestellenRepo = new ToestellenMapper();
		private static readonly UniekeCode _uniekeCode = UniekeCode.Instance;
		private readonly DomeinController _domeinController = new(_reservatienRepo, _klantenRepo, _toestellenRepo);
		private const char _requiredCharacter = '#';

		public static readonly ConsoleColor DefaultReadLineColor = ConsoleColor.DarkYellow;
		public static readonly List<string> GaVerderOpties = new() {
			Utility.SchrijfUnderline("Ga Verder"),
		};

		public static readonly List<string> StopOpties = new() {
			Utility.SchrijfUnderline("Stop Registratie"),
			Utility.SchrijfUnderline("Ga Terug"),
		};
		public static int SelectedIndex = 0;
		public static int OudeselectedIndex;

		public (Klant, bool) RegistreerKlant() {
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
			string typeAlsString = string.Empty;
			TypeKlant type = TypeKlant.Bronze;  // Info: Moet er staat anders krijg je een null reference error, Hieronder krijgt hij een waarde.
			List<string> TypeAbonementen = new() { "Gold", "Silver", "Bronze" };

			//Interesses
			int selectedInteresseIndex;
			List<string> interesses = new();
			List<string> interesseOpties = new() { "Voeg interesse toe.", StopOpties[1] };

			//GeboorteDatum
			DateTime geboorteDatum = new();

			List<string> optieLijst;
			int selectedRegistreerOptieIndex;
			bool isOk, VnaamOk = false, AnaamOk = false, InterOk = false, AdrOk = false, TypOk = false, GebrOk = false, EmailOk = false, gaTerug, CompleetOk = false;

			do {
				optieLijst = new() {
					$"Voornaam: {(string.IsNullOrEmpty(voornaam) ? _requiredCharacter : voornaam)}",
					$"Achternaam: {(string.IsNullOrEmpty(achternaam) ? _requiredCharacter : achternaam)}",
					$"Email: {(string.IsNullOrEmpty(email) ? _requiredCharacter : email)}",
					$"GeboorteDatum: {(GebrOk ? geboorteDatum.ToShortDateString() : _requiredCharacter)}",
					$"Adres: {(AdrOk ? $"{adres.StraatNaam}..." : _requiredCharacter)}",
					$"Abonnement: {(TypOk ? typeAlsString : _requiredCharacter)}",
					$"Interesses: {(interesses.Count == 0 ? _requiredCharacter : string.Join(", ", interesses))}\n",
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
						voornaam = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix);
						VnaamOk = true;
						SchuifIndexPositieOp();
						break;
					#endregion

					#region |=> Achternaam
					case 1:
						Utility.Logger.Info($"--- Achternaam ---", true);
						achternaam = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix);
						AnaamOk = true;
						SchuifIndexPositieOp();
						break;
					#endregion

					#region |=> Email
					case 2:
						Utility.Logger.Info($"--- Email ---", true);
						email = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix);
						EmailOk = true;
						SchuifIndexPositieOp();
						break;
					#endregion

					#region |=> GeboorteDatum
					case 3:
						do {
							Utility.Logger.Info($"--- GeboorteDatum (DD/MM/YYYY) ---", true);
							isOk = DateTime.TryParse(Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix), out geboorteDatum);
							if (!isOk) {
								Console.Clear();
								Utility.Logger.Error("GeboorteDatum is niet in de correcte vorm!", true);
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
									Utility.Logger.Info("--- Straat ---", true);
									straatNaam = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix);
									straatNaamIngevuld = true;
									SchuifIndexPositieOp();
									break;
								#endregion
								#region Nummer
								case 1:
									Utility.Logger.Info("--- Nummer ---", true);
									huisNummer = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix);
									huisNummerIngevuld = true;
									SchuifIndexPositieOp();
									break;
								#endregion
								#region Plaats
								case 2:
									Utility.Logger.Info("--- Plaats ---", true);
									plaats = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix);
									plaatsIngevuld = true;
									SchuifIndexPositieOp();
									break;
								#endregion
								#region PostCode
								case 3:
									do {
										Utility.Logger.Info("--- PostCode ---", true);
										isOk = int.TryParse(Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix), out postCode);
										if (!isOk) {
											Console.Clear();
											Utility.Logger.Error("Postcode is niet in de correcte vorm! -> B.v [9000, 9860, ...]", true);
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
						selectedAbonnementIndex = Utility.OptieLijstConroller(TypeAbonementen, "Abonnement:");
						type = selectedAbonnementIndex switch {
							0 => TypeKlant.Gold,
							1 => TypeKlant.Silver,
							2 => TypeKlant.Bronze,
							_ => TypeKlant.Bronze,
						};
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
								Utility.Logger.Info("--- Interesse ---", true);
								interesses.Add(Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix));
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

		internal void RegistreerToestel() {
			throw new NotImplementedException();
		}

		public Klant Login() {
			Utility.Logger.Info("Wat is je E-mailAdress:", true);
			string email = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix).ToLower();
			return _domeinController.Login(email);
		}

		public void ToonKlantDetails(Klant klant) {
			Utility.Table table = new();
			table.SetHeaders("KlantenNummer", "Naam", "Email", "GeboorteDatum", "Abonnement", "Interesses");
			table.AddRow(klant.KlantenNummer.ToString(), $"{klant.Voornaam} {klant.Achternaam}", klant.Email, klant.GeboorteDatum.ToShortDateString(), klant.TypeKlant.ToString(), string.Join(',', klant.Interesses));
			Console.WriteLine($"{table}\n");
			table.Clear();

			table.SetHeaders("Adres");
			table.AddRow("Straat", "Nr", "Plaats", "PostCode");
			table.AddRow(klant.Adres.StraatNaam, klant.Adres.HuisNummer, klant.Adres.Plaats, klant.Adres.PostCode.ToString());
			Console.WriteLine($"{table}\n\n\n");
			Utility.ColorInput.ReadKnop();
		}

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
	}
}
