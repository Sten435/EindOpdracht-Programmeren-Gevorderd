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
			.Select(reservatie => reservatie.TijdsSlot.StartTijd).ToList();

		public (List<int>, bool) GeefBeschikbareUrenOpDatum(DateTime dag, Klant klant, Toestel toestel) {
			List<Reservatie> gemaakteReservaties = GeefAlleReservaties()
				.Where(reservatie => reservatie.Klant.KlantenNummer == klant.KlantenNummer && reservatie.TijdsSlot.StartTijd.Day == dag.Day).ToList();
			List<DateTime> onbeschibareTijdsSloten = GeefAlleTijdsSloten(toestel);

			List<int> beschikbareUrenFitness = Enumerable.Range(8, 15).ToList();
			List<int> urenNaFilter = new();

			if (gemaakteReservaties.Count == 4) return (urenNaFilter, false);
			else {
				for (int i = 0; i < beschikbareUrenFitness.Count; i++) {
					int uur = beschikbareUrenFitness[i];

					DateTime tijdsSlot = new(dag.Year, dag.Month, dag.Day, uur, 0, 0);

					if (!onbeschibareTijdsSloten.Contains(tijdsSlot)) urenNaFilter.Add(uur);

					if (gemaakteReservaties.Select(reservatie => reservatie.TijdsSlot.StartTijd)
						.Contains(tijdsSlot.AddHours(-1)) && gemaakteReservaties
						.Select(reservatie => reservatie.TijdsSlot.StartTijd)
						.Contains(tijdsSlot.AddHours(-2))) urenNaFilter.Remove(uur);

					if (tijdsSlot < DateTime.Now) urenNaFilter.Remove(uur);
				}
				return (urenNaFilter, true);
			}
		}

		public void ResetUurIndex() => UurIndex = 0;

		#endregion TijdsSlot

		#region Klant

		public List<Klant> GeefAlleKlanten() => _klantenRepo.GeefAlleKlanten();

		public void RegistreerKlant(Klant klant) => _klantenRepo.RegistreerKlant(klant);

		public Klant Login(string email) => _klantenRepo.Login(email);

		#endregion Klant

		#region Toestel

		public List<string> GeefBeschikbareToestellen() => _toestselRepo.GeefAlleToestellen().Where(toestel => toestel.InHerstelling == false).Select(toestel => toestel.ToestelType).ToList();

		public List<Toestel> GeefAlleToestellen() => _toestselRepo.GeefAlleToestellen();

		public void VoegNieuwToestelToe(string naam) => _toestselRepo.VoegToestelToe(naam);

		public void VerwijderToestel(Toestel toestel) => _toestselRepo.VerwijderToestel(toestel);

		public Toestel GeefToestelOpNaam(string toestelNaam) => GeefAlleToestellen().SingleOrDefault(toestel => toestel.ToestelType.ToLower() == toestelNaam.ToLower());

		#endregion Toestel
	}
}