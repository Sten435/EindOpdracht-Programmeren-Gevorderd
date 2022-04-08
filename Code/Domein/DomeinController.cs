using System;
using System.Collections.Generic;

namespace Domein {

	public class DomeinController {
		private IReservatieRepository _reservatieRepo;
		private IKlantenRepository _klantenRepo;
		private IToestelRepository _toestselRepo;

		public DomeinController(IReservatieRepository reservatieRepo, IKlantenRepository klantenRepo, IToestelRepository toestselRepo) {
			_reservatieRepo = reservatieRepo;
			_klantenRepo = klantenRepo;
			_toestselRepo = toestselRepo;
		}

		public List<Reservatie> GeefAlleReservaties() => _reservatieRepo.GeefAlleReservaties();

		public void VoegReservatieToe(Reservatie reservatie) => _reservatieRepo.VoegReservatieToe(reservatie);

		public List<Klant> GeefAlleKlanten() => _klantenRepo.GeefAlleKlanten();

		public void RegistreerKlant(Klant klant) => _klantenRepo.RegistreerKlant(klant);

		public Klant Login(string email) => _klantenRepo.Login(email);

		public List<Toestel> GeefAlleToestellen() => _toestselRepo.GeefAlleToestellen();

		public void ReserveerToestel(Reservatie reservatie) => _reservatieRepo.VoegReservatieToe(reservatie);

		public void VoegNieuwToestelToe(string naam) => _toestselRepo.VoegToestelToe(naam);
	}
}