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
		public static readonly string GaVerderOptie = Utility.SchrijfUnderline("Ga Verder");
		public static int SelectedIndex = 0;
		public static int OudeselectedIndex;

		public Klant RegistreerKlant() {
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
			List<string> interesseOpties = new() { "Voeg interesse toe.", "Ga terug." };

			//GeboorteDatum
			DateTime geboorteDatum = new();

			List<string> optieLijst;
			int selectedRegistreerOptieIndex;
			bool isOk, VnaamOk = false, AnaamOk = false, InterOk = false, AdrOk = false, TypOk = false, GebrOk = false, gaTerug, CompleetOk = false;

			do {
				optieLijst = new() {
					$"Voornaam: {(string.IsNullOrEmpty(voornaam) ? _requiredCharacter : voornaam)}",
					$"Achternaam: {(string.IsNullOrEmpty(achternaam) ? _requiredCharacter : achternaam)}",
					$"GeboorteDatum: {(GebrOk ? geboorteDatum.ToShortDateString() : _requiredCharacter)}",
					$"Adres: {(AdrOk ? $"{adres.StraatNaam}..." : _requiredCharacter)}",
					$"Abonnement: {(TypOk ? typeAlsString : _requiredCharacter)}",
					$"Interesses: {(interesses.Count == 0 ? _requiredCharacter : string.Join(", ", interesses))}"
				};

				if (VnaamOk && AnaamOk && AdrOk && GebrOk && InterOk && TypOk) {
					optieLijst[^1] = $"Interesses: {(interesses.Count == 0 ? _requiredCharacter : string.Join(", ", interesses))}\n";
					optieLijst.Add(GaVerderOptie);
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

					#region |=> GeboorteDatum
					case 2:
						do {
							Utility.Logger.Info($"--- GeboorteDatum (DD/MM/YYYY) ---", true);
							isOk = DateTime.TryParse(Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix), out geboorteDatum);
							Console.Clear();
							Utility.Logger.Error("GeboorteDatum is niet in de correcte vorm!", true);
						} while (!isOk);
						GebrOk = true;
						SchuifIndexPositieOp();
						break;
					#endregion

					#region |=> Adres
					case 3:
						ResetPositionIndex();
						do {
							gaTerug = false;
							List<string> adresOpties = new() { 
								$"Straat: {(straatNaamIngevuld ? straatNaam : _requiredCharacter)}",
								$"HuisNr: {(huisNummerIngevuld ? huisNummer : _requiredCharacter)}",
								$"Plaats: {(plaatsIngevuld ? plaats : _requiredCharacter)}",
								$"PostCode: {(postCodeIngevuld ? postCode.ToString() /* Als je de Tostring wegdoet dan geeft hij een random value meestal 35... Raar... */ : _requiredCharacter)}",
								"Ga Terug" };
							selectedAdresIndex = Utility.OptieLijstConroller(adresOpties, "Adres:");
							switch (selectedAdresIndex) {
								case 0:
									Utility.Logger.Info("--- Straat ---", true);
									straatNaam = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix);
									straatNaamIngevuld = true;
									SchuifIndexPositieOp();
									break;
								case 1:
									Utility.Logger.Info("--- Nummer ---", true);
									huisNummer = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix);
									huisNummerIngevuld = true;
									SchuifIndexPositieOp();
									break;
								case 2:
									Utility.Logger.Info("--- Plaats ---", true);
									plaats = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix);
									plaatsIngevuld = true;
									SchuifIndexPositieOp();
									break;
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
								case 4:
									gaTerug = true;
									break;
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
					case 4:
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
					case 5:
						ResetPositionIndex();
						do {
							selectedInteresseIndex = Utility.OptieLijstConroller(interesseOpties, "interesses:");
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

					#region |=> Compleet Registratie
					case 6:
						CompleetOk = true;
						break;
						#endregion
				}
			} while (!CompleetOk);

			Klant klant = new(_uniekeCode.GenereerRandomCode(), voornaam, achternaam, email, interesses, geboorteDatum, adres, type);
			_domeinController.RegistreerKlant(klant);
			return klant;
		}

		public Klant Login() {
			Utility.Logger.Info("Wat is je E-mailAdress:", true);
			string email = Utility.ColorInput.ReadInput(ConsoleColor.Cyan, Utility.SelectPrefix).ToLower();
			return _domeinController.Login(email);
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
