using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domein {
	internal class FitnessController : IControll {
		private IReservatieRepository _reservatieRepo;
		private IKlantenRepository _klantenRepo;
		private IToestelRepository _toestselRepo;

		private HashSet<Reservatie> klantReservaties = new();

		public string KlantOmschrijving {
			get {
				if (_klant == null)
					return "Niet ingelogd";
				else return _klant.ToString();
			}
		}

		public string KlantOmschrijvingParsable {
			get {
				if (_klant == null)
					return "Niet ingelogd";
				else return _klant.ToString(true);
			}
		}

		public int UurIndex = 0;
		public bool LoggedIn;

		private Klant _klant;
		public bool isAdmin;

		public FitnessController(IReservatieRepository reservatieRepo, IKlantenRepository klantenRepo, IToestelRepository toestselRepo, IConfigRepository configRepo) {
			_reservatieRepo = reservatieRepo;
			_klantenRepo = klantenRepo;
			_toestselRepo = toestselRepo;
			configRepo.LoadConfig();
		}

		private List<DateTime> GeefAlleTijdSloten(string naam, DateTime dag) {
			return _reservatieRepo.GeefReservatiesPerToestel(naam, dag);
		}

		private Toestel GeefToestelOpId(int toestelId) {
			return _toestselRepo.GeefToestel(toestelId);
		}

		public HashSet<string> GeefKlantReservaties(bool parsed) {
			try {
				return _reservatieRepo.GeefAlleReservaties(klantenNummer: (int)_klant.KlantenNummer).OrderByDescending(r => r.TijdsSlot.StartTijd.Date)
				.Select(r => r.ToString(parsed)).ToHashSet();
			} catch (Exception) {
				throw new KlantenExeption("Klant is niet ingevuld");
			}
		}

		public string VoegReservatieToe(DateTime tijdsSlotDatum, int toestelId) {
			if (tijdsSlotDatum < DateTime.Now)
				throw new ReservatieException("Reservatie mag niet in het verleden zijn.");

			if (tijdsSlotDatum.Hour < TijdsSlot.LowerBoundUurReservatie || tijdsSlotDatum.Hour > TijdsSlot.UpperBoundUurReservatie)
				throw new ReservatieException($"Reservatie mag vroeger dan {TijdsSlot.LowerBoundUurReservatie}uur starten en moet voor {TijdsSlot.UpperBoundUurReservatie} zijn.");

			Reservatie reservatie = new(null, _klant, new TijdsSlot(tijdsSlotDatum), GeefToestelOpId(toestelId));
			_reservatieRepo.VoegReservatieToe(reservatie);
			UpdateklantReservaties();
			return reservatie.ToString(true);
		}

		public void VerwijderReservatie(int reservatieNummer) {
			_reservatieRepo.VerwijderReservatie(reservatieNummer);
			UpdateklantReservaties();
		}

		public void UpdateklantReservaties() {
			klantReservaties = _reservatieRepo.GeefAlleReservaties(vandaagPlusToekomst: true, klantenNummer: (int)_klant.KlantenNummer);
		}

		public (List<int>, bool) GeefBeschikbareReservatieUren(DateTime dag, string naam) {
			List<Reservatie> klantReservatiesDag = klantReservaties.Where(r => r.TijdsSlot.StartTijd.Day == dag.Day).ToList();
			if (klantReservatiesDag.Count == 4) return (new List<int>(), false);

			List<DateTime> klantReservatiesMetGeselecteerdeToestel = klantReservaties.Where(r => r.Toestel.ToestelType == naam && r.TijdsSlot.StartTijd.Day == dag.Day)
																					.Select(t => t.TijdsSlot.StartTijd)
																					.ToList();
			List<DateTime> gereserveerdeTijdSloten = GeefAlleTijdSloten(naam, dag);

			int beginUur = TijdsSlot.LowerBoundUurReservatie;
			int eindUur = TijdsSlot.UpperBoundUurReservatie;

			List<int> beschikbareUrenFitness = Enumerable.Range(beginUur, (eindUur - beginUur) + 1).ToList();
			List<int> urenLijstNaFilter = new();

			List<Toestel> beschikbareToestellen = _toestselRepo.GeefAlleBeschikbareToestellenOpNaam(naam);

			for (int i = 0; i < beschikbareUrenFitness.Count; i++) {
				int uur = beschikbareUrenFitness[i];

				DateTime tijdsSlot = new(dag.Year, dag.Month, dag.Day, uur, 0, 0);

				if (gereserveerdeTijdSloten.Contains(tijdsSlot) == false) {
					urenLijstNaFilter.Add(uur);

				} else if (beschikbareToestellen.Count > gereserveerdeTijdSloten.Where(d => d == tijdsSlot).ToList().Count) {
					urenLijstNaFilter.Add(uur);
				}

				IEnumerable<DateTime> klantReservatiesStartTijdenLijstDag = klantReservatiesDag.Select(reservatie => reservatie.TijdsSlot.StartTijd);

				if (tijdsSlot < DateTime.Now || klantReservatiesStartTijdenLijstDag.Contains(tijdsSlot.AddHours(-1)) && klantReservatiesStartTijdenLijstDag.Contains(tijdsSlot.AddHours(-2)) || klantReservatiesStartTijdenLijstDag.Contains(tijdsSlot.AddHours(1)) && klantReservatiesStartTijdenLijstDag.Contains(tijdsSlot.AddHours(2)) || klantReservatiesMetGeselecteerdeToestel.Contains(tijdsSlot)) {
					urenLijstNaFilter.Remove(uur);
				}
			}

			return (urenLijstNaFilter.ToList(), true);
		}

		public List<DateTime> GeefBeschikbareDagen() {
			List<DateTime> beschikbaarLijst = new();

			for (int i = 0; i <= Reservatie.AantalDagenInToekomstReserveren; i++) {
				DateTime dag = DateTime.Now.AddDays(i);

				List<Reservatie> klantReservatiesDag = klantReservaties.Where(r => r.TijdsSlot.StartTijd.Day == dag.Day).ToList();
				if (klantReservatiesDag.Count == 4) continue;

				beschikbaarLijst.Add(dag);
			}
			return beschikbaarLijst;
		}

		public int? GeefEenVrijToestelIdOpNaam(string toestelNaam, DateTime tijdsSlot) {
			return _reservatieRepo.GeefBeschikbaarToestelOpTijdsSlot(tijdsSlot, toestelNaam);
		}

		public List<string> GeefAlleKlanten() {
			return _klantenRepo.GeefAlleKlanten()
					.Select(k => $"{k.Voornaam} {k.Achternaam} - {k.Email} - {k.GeboorteDatum} - {k.Adres.StraatNaam} {k.Adres.HuisNummer} | {k.Adres.Plaats}")
					.ToList();
		}

		public void RegistreerKlant(string voornaam, string achternaam, string email, DateTime geboorteDatum, List<string> interesses, string typeKlant, string straat, string plaats, string huisNummer, int postCode) {
			if (_klantenRepo.GeefAlleKlanten().Any(kl => kl.Email == email.ToLower())) throw new KlantenExeption("Email adres bestaat al.");

			TypeKlant _typeKlant = typeKlant switch {
				"Bronze" => TypeKlant.Bronze,
				"Silver" => TypeKlant.Silver,
				"Gold" => TypeKlant.Gold,
				"Beheerder" => TypeKlant.Beheerder,
				_ => TypeKlant.Bronze
			};

			Adres adres = new(straat, huisNummer, plaats, postCode);
			Klant klant = new(null, voornaam, achternaam, email, interesses, geboorteDatum, adres, _typeKlant);

			_klantenRepo.RegistreerKlant(klant);
		}

		public void Login(string email) {
			_klant = _klantenRepo.Login(email);
			LoggedIn = true;

			if (_klant.TypeKlant == TypeKlant.Beheerder) {
				isAdmin = true;
			} else {
				isAdmin = false;
			}
		}

		public void Logout() {
			_klant = null;
			LoggedIn = false;
		}

		public List<string> GeefBeschikbareToestellen() {
			return _toestselRepo.GeefAlleToestellen()
					.Where(t2 => t2.InHerstelling == false)
					.GroupBy(t1 => t1.ToestelType)
					.Select(s => s.Key)
					.ToList();
		}

		public List<string> GeefAlleToestellen(bool metVerwijderde = false) {
			return _toestselRepo.GeefAlleToestellen(metVerwijderde)
					.Select(t => t.ToString(parsed: true))
					.ToList();
		}

		public List<string> GeefToestellenZonderReservatie() {
			return _toestselRepo.GeefToestellenZonderReservatie()
					.Select(t => t.ToString(parsed: true))
					.ToList();
		}

		public int VoegNieuwToestelToe(string naam) {
			if (!string.IsNullOrEmpty(naam) && naam.ToList().All(letter => char.IsLetter(letter) == true)) {
				return _toestselRepo.VoegToestelToe(naam);
			} else throw new ToestelException("Toestelnaam mag niet leeg zijn.");
		}

		public void VerwijderToestelOpId(int toestelId) {
			_toestselRepo.VerwijderToestel(toestelId);
		}

		public void ZetToestelInOfUitHerstelling(int toestelId) {
			_toestselRepo.ZetToestelInOfUitHerstelling(toestelId);
		}
	}
}
