using System;
using System.Collections.Generic;
using System.Linq;

namespace Domein {

	public class DomeinController {
		private IReservatieRepository _reservatieRepo;
		private IKlantenRepository _klantenRepo;
		private IToestelRepository _toestselRepo;

		public string KlantOmschrijving {
			get {
				if (_klant == null)
					return "Niet ingelogd";
				else return _klant.ToString();
			}
		}

		public int UurIndex = 0;
		public bool LoggedIn;

		private Klant _klant;

		public DomeinController(IReservatieRepository reservatieRepo, IKlantenRepository klantenRepo, IToestelRepository toestselRepo) {
			_reservatieRepo = reservatieRepo;
			_klantenRepo = klantenRepo;
			_toestselRepo = toestselRepo;
		}

		#region Reservatie

		/// <summary>
		/// Geef alle reservaties in string vorm.
		/// </summary>
		/// <returns>Geef een List van string's van reservaties terug</returns>
		public List<string> GeefAlleReservaties() => _reservatieRepo.GeefAlleReservaties()
															.Select(r => r.ToString())
															.ToList();

		/// <summary>
		/// Geef een Lijst van gereserveerde toestellen in string vorm.
		/// </summary>
		/// <returns>Geef een List van string's van geresveerde toestellen terug.</returns>
		public List<string> GeefGereserveerdeToestellen() => _reservatieRepo.GeefAlleReservaties()
																		.Where(r => r.TijdsSlot.EindTijd > DateTime.Now.ToUniversalTime().AddHours(2))
																		.Select(r => r.Toestel)
																		.Select(t => t.ToString())
																		.ToList();
		/// <summary>
		/// Geef alle klanten reservaties die gelijk zijn aan een bepaald klantid.
		/// </summary>
		/// <param name="klantId">De klantId van waar je de reservatie van wil</param>
		/// <returns>Geeft een List van klanten reservaties terug</returns>
		public List<string> GeefKlantReservaties() {
			//if (klantId == -1)
			//	return _reservatieRepo.GeefAlleReservaties()
			//					.Where(reservatie => reservatie.Klant.KlantenNummer == klantId)
			//					.Select(r => r.ToString())
			//					.ToList();
			//else
			return _reservatieRepo.GeefAlleReservaties()
							.Where(reservatie => reservatie.Klant.KlantenNummer == _klant.KlantenNummer)
							.Select(r => r.ToString())
							.ToList();

		}

		/// <summary>
		/// Voeg reservatie toe aan database op tijdsSlotdatum en op toestelid.
		/// </summary>
		/// <param name="tijdsSlotDatum">De datetime wanneer je wil reserveren</param>
		/// <param name="toestelId">ToestelId van toestel dat je wil reserveren</param>
		/// <returns>Geeft de reservatie terug in string representatie</returns>
		public string VoegReservatieToe(DateTime tijdsSlotDatum, long toestelId) {
			Reservatie reservatie = new(_klant, new TijdsSlot(tijdsSlotDatum), GeefToestelOpId(toestelId));
			_reservatieRepo.VoegReservatieToe(reservatie);
			return reservatie.ToString();
		}

		/// <summary>
		/// Verwijder reservatie op reservatieId.
		/// </summary>
		/// <param name="reservatieId">ReservatieId van toestel dat u wil verwijderen.</param>
		public void VerwijderReservatieOpId(long reservatieId) {
			Reservatie reservatie = _reservatieRepo.GeefAlleReservaties()
												.Find(r => r.ReservatieNummer == reservatieId);

			_reservatieRepo.VerwijderReservatie(reservatie);
		}

		/// <summary>
		/// Geef reservatieId op reservatieIndex.
		/// </summary>
		/// <param name="reservatieIndex">ReservatieIndex van toestel dat je de Id van wil krijgen.</param>
		/// <returns>ReservatieId van respectieve reservatieIndex</returns>
		public long GeefReservatieIdOpIndex(int reservatieIndex) => _reservatieRepo.GeefAlleReservaties()[reservatieIndex].ReservatieNummer;

		/// <summary>
		/// Geef reservatie in string vorm op reservatieId.
		/// </summary>
		/// <param name="reservatieId">ReservatieId van reservatie waar je een string vorm van wil.</param>
		/// <returns>Reservatie in string vorm</returns>
		public string GeefReservatieStringOpId(long reservatieId) => _reservatieRepo.GeefAlleReservaties()
																				.First(r => r.ReservatieNummer == reservatieId)
																				.ToString();
		#endregion Reservatie

		#region TijdsSlot

		/// <summary>
		/// Geef een lijst van TijdsSlot (Start tijd) 'en van een bepaald toestelId.
		/// </summary>
		/// <param name="toestel">Toestel van waar je de Datetime's van terug wil.</param>
		/// <returns>List van DateTime start tijden</returns>
		private List<DateTime> GeefAlleTijdsSloten(Toestel toestel) => _reservatieRepo.GeefAlleReservaties()
			.Where(rv => rv.Toestel.IdentificatieCode == toestel.IdentificatieCode)
			.Select(r => r.TijdsSlot.StartTijd).ToList();

		/// <summary>
		/// Geef beschikbare uren op dag, klant, toestelId.
		/// </summary>
		/// <param name="dag">Dag van reservatie</param>
		/// <param name="toestelId">Toestel die je met de reservatie wil koppelen</param>
		/// <returns>List int waarbij elke daarvan een uur voorsteld, bool steld voor of de klant nog kan reservaren op die datetime</returns>
		public (List<int>, bool) GeefBeschikbareUrenOpDatum(DateTime dag, long toestelId) {
			Toestel toestel = GeefToestelOpId(toestelId);

			List<Reservatie> klantReservaties = _reservatieRepo.GeefAlleReservaties().Where(r => r.Klant.KlantenNummer == _klant.KlantenNummer).ToList();
			List<Reservatie> klantReservatiesDag = klantReservaties.Where(r => r.TijdsSlot.StartTijd.Day == dag.Day).ToList();

			List<DateTime> klantReservatiesMetGeselecteerdeToestel = klantReservaties.Where(r => r.Toestel.ToestelType == toestel.ToestelType && r.TijdsSlot.StartTijd.Day == dag.Day).Select(t => t.TijdsSlot.StartTijd).ToList();
			List<DateTime> gereserveerdeTijdssloten = GeefAlleTijdsSloten(toestel);

			int beginUur = 8;
			int eindUur = 22;

			List<int> beschikbareUrenFitness = Enumerable.Range(beginUur, (eindUur - 8) + 1).ToList();
			List<int> urenNaFilter = new();

			List<Toestel> beschikbareToestellen = _toestselRepo.GeefAlleToestellen().Where(t => t.ToestelType == toestel.ToestelType && t.InHerstelling == false).ToList();

			if (klantReservatiesDag.Count == 4) return (urenNaFilter.ToList(), false);

			for (int i = 0; i < beschikbareUrenFitness.Count; i++) {
				int uur = beschikbareUrenFitness[i];

				DateTime tijdsSlot = new(dag.Year, dag.Month, dag.Day, uur, 0, 0);

				if (!gereserveerdeTijdssloten.Contains(tijdsSlot)) urenNaFilter.Add(uur);
				else if (beschikbareToestellen.Count > gereserveerdeTijdssloten.Where(d => d == tijdsSlot).ToList().Count) urenNaFilter.Add(uur);

				if (klantReservatiesMetGeselecteerdeToestel.Contains(tijdsSlot))
					urenNaFilter.Remove(uur);

				if (klantReservatiesDag.Select(reservatie => reservatie.TijdsSlot.StartTijd)
					.Contains(tijdsSlot.AddHours(-1)) && klantReservatiesDag
					.Select(reservatie => reservatie.TijdsSlot.StartTijd)
					.Contains(tijdsSlot.AddHours(-2))) urenNaFilter.Remove(uur);

				if (tijdsSlot < DateTime.Now) urenNaFilter.Remove(uur);
			}
			return (urenNaFilter.ToList(), true);
		}

		/// <summary>
		/// Geef toestel op id.
		/// </summary>
		/// <param name="toestelId">ToestelId van toestel dat je terug wil.</param>
		/// <returns>Toestel dat je op toestelId meegeeft</returns>
		private Toestel GeefToestelOpId(long toestelId) => _toestselRepo.GeefAlleToestellen()
																.Find(t => t.IdentificatieCode != toestelId);

		/// <summary>
		/// Reset uur index.
		/// </summary>
		public void ResetUurIndex() => UurIndex = 0;

		#endregion TijdsSlot

		#region Klant

		/// <summary>
		/// Geef een lijst van alle klanten informatie in string vorm.
		/// </summary>
		/// <returns>List strings met klanten info </returns>
		public List<string> GeefAlleKlanten() => _klantenRepo.GeefAlleKlanten()
														.Select(k => $"{k.Voornaam} {k.Achternaam} - {k.Email} - {k.GeboorteDatum} - {k.Adres.StraatNaam} {k.Adres.HuisNummer} | {k.Adres.Plaats}")
														.ToList();

		/// <summary>
		/// Registreer een klant met een klant id.
		/// </summary>
		/// <param name="klant"></param>
		public void RegistreerKlant(string voornaam, string achternaam, string email, DateTime geboorteDatum, List<string> interesses, string typeKlant, string straat, string plaats, string huisNummer, int postCode) {
			UniekeCode uniekeCode = UniekeCode.Instance;
			long klantenNummer = uniekeCode.GenereerRandomCode();

			TypeKlant _typeKlant = typeKlant switch {
				"Bronze" => TypeKlant.Bronze,
				"Silver" => TypeKlant.Silver,
				"Gold" => TypeKlant.Gold,
				"Beheerder" => TypeKlant.Beheerder,
				_ => TypeKlant.Bronze
			};

			Adres adres = new(straat, huisNummer, plaats, postCode);
			Klant klant = new(klantenNummer, voornaam, achternaam, email, interesses, geboorteDatum, adres, _typeKlant);

			_klantenRepo.RegistreerKlant(klant);
		}

		/// <summary>
		/// Logd een user in op basis van een email adress.
		/// </summary>
		/// <param name="email"></param>
		/// <param name="isBeheerder"></param>
		/// <returns>Returned True/False respectively of je een beheerder bent of niet.</returns>
		public bool Login(string email, bool isBeheerder = false) {
			_klant = _klantenRepo.Login(email);
			LoggedIn = true;

			if (isBeheerder && _klant.TypeKlant == TypeKlant.Beheerder) return true;
			else return false;
		}

		/// <summary>
		/// Log klant uit.
		/// </summary>
		public void Logout() {
			_klant = null;
			LoggedIn = false;
		}

		#endregion Klant

		#region Toestel

		/// <summary>
		/// Geef alle beschikbare toestellen in string vorm.
		/// </summary>
		/// <returns>List van informatie van alle toestellen</returns>
		public List<string> GeefBeschikbareToestellen() => _toestselRepo.GeefAlleToestellen()
																.Where(t2 => t2.InHerstelling == false)
																.GroupBy(t1 => t1.ToestelType)
																.Select(s => s.Key)
																.ToList();

		/// <summary>
		/// Geef alle toestellen in string vorm.
		/// </summary>
		/// <returns>List van toestellen informatie</returns>
		public List<string> GeefAlleToestellen() => _toestselRepo.GeefAlleToestellen()
																.Select(t => t.ToString())
																.ToList();

		/// <summary>
		/// Geef toestel naam op megegeven index.
		/// </summary>
		/// <param name="toestelIndex"></param>
		/// <returns>Toestel naam op megegeven index</returns>
		public string GeefToestelNaamOpIndex(int toestelIndex) => _toestselRepo.GeefAlleToestellen()[toestelIndex].ToestelType;

		/// <summary>
		/// Geef toestel id op toestel index.
		/// </summary>
		/// <param name="toestelIndex"></param>
		/// <returns>ToestelId van toestel op megegeven index</returns>
		public long GeefToestelIdOpIndex(int toestelIndex) => _toestselRepo.GeefAlleToestellen()[toestelIndex].IdentificatieCode;

		/// <summary>
		/// Voeg nieuw toestel toe op naam.
		/// </summary>
		/// <param name="naam"></param>
		public void VoegNieuwToestelToe(string naam) => _toestselRepo.VoegToestelToe(naam);

		public void UpdateToestelNaamOpIndex(int toestelIndex, string toestelNaam) {
			long toestelId = GeefToestelIdOpIndex(toestelIndex);
			_toestselRepo.UpdateToestelOpId(toestelId, toestelNaam);
		}

		/// <summary>
		/// Geef toestel herstel status op selected Index
		/// </summary>
		/// <param name="selectedIndex"></param>
		/// <returns>Herstel status van toestel op index</returns>
		public bool GeefToestelHerstelStatusOpIndex(int selectedIndex) {
			long toestelId = GeefToestelIdOpIndex(selectedIndex);
			return _toestselRepo.GeefToestelHerstelStatusOpId(toestelId);
		}

		/// <summary>
		/// Verwijder toestel op een toestelId
		/// </summary>
		/// <param name="toestelId"></param>
		/// <exception cref="VerkeerdToestelIdException">Wanneer een id niet bestaat.</exception>
		public void VerwijderToestelOpId(long toestelId) {
			Toestel toestel = _toestselRepo.GeefAlleToestellen()
										.Find(t => t.IdentificatieCode == toestelId);

			if (toestel == null) throw new ToestelIdException("Toestel Id bestaat niet !");
			_toestselRepo.VerwijderToestel(toestel);
		}

		/// <summary>
		/// Zet toestel in of uit herstelling
		/// </summary>
		/// <param name="toestelIndex"></param>
		public void ZetToestelInOfUitHerstelling(int toestelIndex, bool nieuweHerstellingValue) => _toestselRepo.ZetToestelInOfUitHerstelling(GeefToestelIdOpIndex(toestelIndex), nieuweHerstellingValue);

		#endregion Toestel
	}
}