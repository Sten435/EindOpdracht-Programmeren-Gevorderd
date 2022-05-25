using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domein {
	internal class FitnessController : IControll {
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

		private List<DateTime> GeefAlleTijdSloten(string naam) {
			return _reservatieRepo.GeefAlleReservaties()
			.Where(rv => rv.Toestel.ToestelType.ToLower() == naam.ToLower())
			.Select(r => r.TijdsSlot.StartTijd).ToList();
		}

		private Toestel GeefToestelOpId(int toestelId) {
			return _toestselRepo.GeefToestel(toestelId);
		}

		public List<string> GeefKlantReservaties(bool parsed) {
			try {
				return _reservatieRepo.GeefAlleReservaties(klantenNummer: (int)_klant.KlantenNummer).OrderByDescending(r => r.TijdsSlot.StartTijd.Date)
				.Select(r => r.ToString(parsed))
				.ToList();
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
			return reservatie.ToString(true);
		}

		public void VerwijderReservatie(int reservatieNummer) {
			_reservatieRepo.VerwijderReservatie(reservatieNummer);
		}

		public (List<int>, bool) GeefBeschikbareReservatieUren(DateTime dag, string naam) {
			List<Reservatie> reservaties = _reservatieRepo.GeefAlleReservaties(vandaagPlusToekomst: true);
			List<Reservatie> klantReservaties = reservaties.Where(r => r.Klant.KlantenNummer == _klant.KlantenNummer).ToList();
			List<Reservatie> klantReservatiesDag = klantReservaties.Where(r => r.TijdsSlot.StartTijd.Day == dag.Day).ToList();
			if (klantReservatiesDag.Count == 4) return (new List<int>(), false);

			List<DateTime> klantReservatiesMetGeselecteerdeToestel = klantReservaties.Where(r => r.Toestel.ToestelType == naam && r.TijdsSlot.StartTijd.Day == dag.Day)
																					.Select(t => t.TijdsSlot.StartTijd)
																					.ToList();
			List<DateTime> gereserveerdeTijdSloten = GeefAlleTijdSloten(naam);

			int beginUur = TijdsSlot.LowerBoundUurReservatie;
			int eindUur = TijdsSlot.UpperBoundUurReservatie;

			List<int> beschikbareUrenFitness = Enumerable.Range(beginUur, (eindUur - beginUur) + 1).ToList();
			List<int> urenNaFilter = new();

			List<Toestel> toestellen = _toestselRepo.GeefAlleToestellen();
			List<Toestel> beschikbareToestellen = toestellen.Where(t => t.ToestelType == naam && t.InHerstelling == false).ToList();

			for (int i = 0; i < beschikbareUrenFitness.Count; i++) {
				int uur = beschikbareUrenFitness[i];

				DateTime tijdsSlot = new(dag.Year, dag.Month, dag.Day, uur, 0, 0);

				if (!gereserveerdeTijdSloten.Contains(tijdsSlot)) {
					urenNaFilter.Add(uur);
				} else if (beschikbareToestellen.Count > gereserveerdeTijdSloten.Where(d => d == tijdsSlot).ToList().Count) {
					urenNaFilter.Add(uur);
				}

				if (klantReservatiesMetGeselecteerdeToestel.Contains(tijdsSlot)) {
					urenNaFilter.Remove(uur);
				}

				if (klantReservatiesDag.Select(reservatie => reservatie.TijdsSlot.StartTijd)
					.Contains(tijdsSlot.AddHours(-1)) && klantReservatiesDag
					.Select(reservatie => reservatie.TijdsSlot.StartTijd)
					.Contains(tijdsSlot.AddHours(-2))) {
					urenNaFilter.Remove(uur);
				}

				if (tijdsSlot < DateTime.Now) {
					urenNaFilter.Remove(uur);
				}
			}

			return (urenNaFilter.ToList(), true);
		}

		public IEnumerable<DateTime> GeefBeschikbareDagen() {
			List<Reservatie> reservaties = _reservatieRepo.GeefAlleReservaties(alleenVandaag: true);

			for (int i = 0; i <= Reservatie.AantalDagenInToekomstReserveren; i++) {
				DateTime dag = DateTime.Now.AddDays(i);

				List<Reservatie> klantReservaties = reservaties.Where(r => r.Klant.KlantenNummer == _klant.KlantenNummer).ToList();
				List<Reservatie> klantReservatiesDag = klantReservaties.Where(r => r.TijdsSlot.StartTijd.Day == dag.Day).ToList();
				if (klantReservatiesDag.Count == 4) continue;

				yield return dag;
			}
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

		public List<string> GeefAlleToestellen() {
			return _toestselRepo.GeefAlleToestellen()
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
