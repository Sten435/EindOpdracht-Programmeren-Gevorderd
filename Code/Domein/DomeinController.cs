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
		#endregion

		#region TijdsSlot
		public List<TijdsSlot> GeefAlleTijdsSloten() {
			List<TijdsSlot> slots = _reservatieRepo.GeefAlleReservaties().Select(reservatie => reservatie.TijdsSlot).ToList();
			return slots;
		}
		public List<int> GeefBeschikbareUrenOpDatum(DateTime dag) {
			List<DateTime> onbeschibareTijdsSloten = GeefAlleTijdsSloten().Select(tijdsSlot => tijdsSlot.StartTijd).ToList();
			List<int> beschikbareUrenFitness = Enumerable.Range(8, 15).ToList();
			List<int> urenNaFilter = new();

			for (int i = 0; i < beschikbareUrenFitness.Count; i++) {
				int uur = beschikbareUrenFitness[i];
				DateTime tijdsSlot = new(dag.Year, dag.Month, dag.Day, uur, 0, 0);

				if (!onbeschibareTijdsSloten.Contains(tijdsSlot)) urenNaFilter.Add(uur);
			}
			return urenNaFilter;
		}
		public void ResetUurIndex() => UurIndex = 0;
		#endregion

		#region Klant
		public List<Klant> GeefAlleKlanten() => _klantenRepo.GeefAlleKlanten();

		public void RegistreerKlant(Klant klant) => _klantenRepo.RegistreerKlant(klant);

		public Klant Login(string email) => _klantenRepo.Login(email);
		#endregion

		#region Toestel
		public List<Toestel> GeefAlleToestellen() => _toestselRepo.GeefAlleToestellen();

		public void VoegNieuwToestelToe(string naam) => _toestselRepo.VoegToestelToe(naam);

		public Toestel GeefToestelOpNaam(string toestelNaam) => GeefAlleToestellen().SingleOrDefault(toestel => toestel.ToestelType.ToLower() == toestelNaam.ToLower());
		#endregion
	}
}