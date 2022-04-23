using System;
using System.Collections.Generic;
using System.Linq;

namespace Domein {

	public class DomeinController {
		private IReservatieRepository _reservatieRepo;
		private IKlantenRepository _klantenRepo;
		private IToestelRepository _toestselRepo;

		public int UurIndex = 0;

		public DomeinController(IReservatieRepository reservatieRepo, IKlantenRepository klantenRepo, IToestelRepository toestselRepo) {
			_reservatieRepo = reservatieRepo;
			_klantenRepo = klantenRepo;
			_toestselRepo = toestselRepo;
		}

		#region Reservatie

		public List<Reservatie> GeefAlleReservaties() => _reservatieRepo.GeefAlleReservaties();

		public List<Reservatie> GeefKlantReservaties(Klant klant) => _reservatieRepo.GeefAlleReservaties().Where(reservatie => reservatie.Klant.KlantenNummer == klant.KlantenNummer).ToList();

		public void VoegReservatieToe(Reservatie reservatie) => _reservatieRepo.VoegReservatieToe(reservatie);

		public void ReserveerToestel(Reservatie reservatie) => _reservatieRepo.VoegReservatieToe(reservatie);

		public void VerwijderReservatie(Reservatie reservatie) => _reservatieRepo.VerwijderReservatie(reservatie);

		#endregion Reservatie

		#region TijdsSlot

		public List<DateTime> GeefAlleTijdsSloten(Toestel toestel) => _reservatieRepo.GeefAlleReservaties()
			.Where(rv => rv.Toestel.IdentificatieCode == toestel.IdentificatieCode)
			.Select(r => r.TijdsSlot.StartTijd).ToList();

		public (List<int>, bool) GeefBeschikbareUrenOpDatum(DateTime dag, Klant klant, Toestel toestel) {
			List<Reservatie> klantReservaties = GeefAlleReservaties().Where(r => r.Klant.KlantenNummer == klant.KlantenNummer).ToList();
			List<Reservatie> klantReservatiesDag = klantReservaties.Where(r => r.TijdsSlot.StartTijd.Day == dag.Day).ToList();

			List<DateTime> klantReservatiesMetGeselecteerdeToestel = klantReservaties.Where(r => r.Toestel.ToestelType == toestel.ToestelType && r.TijdsSlot.StartTijd.Day == dag.Day).Select(t => t.TijdsSlot.StartTijd).ToList();
			List<DateTime> gereserveerdeTijdssloten = GeefAlleTijdsSloten(toestel);

			List<int> beschikbareUrenFitness = Enumerable.Range(8, 15).ToList();
			HashSet<int> urenNaFilter = new();

			List<Toestel> beschikbareToestellen = GeefAlleToestellen().Where(t => t.ToestelType == toestel.ToestelType && t.InHerstelling == false).ToList();

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

		public void ResetUurIndex() => UurIndex = 0;

		#endregion TijdsSlot

		#region Klant

		public List<Klant> GeefAlleKlanten() => _klantenRepo.GeefAlleKlanten();

		public void RegistreerKlant(Klant klant) => _klantenRepo.RegistreerKlant(klant);

		public Klant Login(string email) => _klantenRepo.Login(email);

		public bool LaadKlanten() => _klantenRepo.LaadKlanten();

		#endregion Klant

		#region Toestel

		public List<string> GeefBeschikbareToestellen() => _toestselRepo.GeefAlleToestellen().Where(t2 => t2.InHerstelling == false).GroupBy(t1 => t1.ToestelType).Select(s => {
			if (s.Count() > 1)
				return $"{s.Key} [{s.Count()}]";
			else return s.Key;
		}).ToList();

		public List<Toestel> GeefAlleToestellen() => _toestselRepo.GeefAlleToestellen();

		public bool LaadToestellen() => _toestselRepo.LaadToestellen();

		public void VoegNieuwToestelToe(string naam) => _toestselRepo.VoegToestelToe(naam);

		public void VerwijderToestel(Toestel toestel) => _toestselRepo.VerwijderToestel(toestel);

		public Toestel GeefToestelOpNaam(string toestelNaam) => GeefAlleToestellen().First(toestel => toestel.ToestelType.ToLower() == toestelNaam.ToLower());

		#endregion Toestel
	}
}