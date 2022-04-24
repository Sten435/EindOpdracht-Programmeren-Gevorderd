using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Persistentie;
using System.Collections.Generic;
using System.Linq;

namespace Domein.Tests {
	[TestClass()]
	public class DomeinControllerTests {
		Reservatie reservatie;
		Klant klant;

		private IReservatieRepository _reservatieRepo;
		private IKlantenRepository _klantenRepo;
		private IToestelRepository _toestselRepo;


		[TestInitialize()]
		public void Initialize() {
			_reservatieRepo = new ReservatieRepository();
			_klantenRepo = new KlantenRepository();
			_toestselRepo = new ToestellenRepository();

			reservatie = new(new Klant(), new TijdsSlot(DateTime.Now), new Toestel(2, "Stantoestel", false));
			klant = new(1, "Stan", "P", "@gmail.com", new List<string>(), DateTime.Now, new Adres(), TypeKlant.Silver);
		}

		[TestMethod()]
		public void VoegReservatieToeTest() {
			_reservatieRepo.VoegReservatieToe(reservatie);
			bool isOk = _reservatieRepo.GeefAlleReservaties().Any(r => r.Toestel.ToestelType == "Stantoestel");
			Assert.IsTrue(isOk);
		}

		[TestMethod()]
		public void VerwijderReservatieTest() {
			_reservatieRepo.VoegReservatieToe(reservatie);
			bool isOk = _reservatieRepo.GeefAlleReservaties().Any(r => r.Toestel.ToestelType == "Stantoestel");
			Assert.IsTrue(isOk);

			_reservatieRepo.VerwijderReservatie(reservatie);
			isOk = _reservatieRepo.GeefAlleReservaties().Any(r => r.Toestel.ToestelType == "Stantoestel");
			Assert.IsFalse(isOk);
		}

		[TestMethod()]
		public void RegistreerKlantTest() {
			_klantenRepo.RegistreerKlant(klant);
			bool isOk = _klantenRepo.GeefAlleKlanten().Any(klant => klant.Email == "@gmail.com");
			Assert.IsTrue(isOk);
		}

		[TestMethod()]
		public void VerwijderKlantTest() {
			_klantenRepo.RegistreerKlant(klant);
			Klant klant1 = _klantenRepo.GeefAlleKlanten().FirstOrDefault(klant => klant.Email == "@gmail.com");
			Assert.IsNotNull(klant1);

			_klantenRepo.VerwijderKlant(klant);
			klant1 = _klantenRepo.GeefAlleKlanten().FirstOrDefault(klant => klant.Email == "@gmail.com");
			Assert.IsNull(klant1);
		}

		[TestMethod()]
		public void LoginTest() {
			_klantenRepo.RegistreerKlant(klant);
			Klant klant1 = _klantenRepo.Login("@gmail.com");
			Assert.IsNotNull(klant1);
		}

		[TestMethod()]
		public void VoegNieuwToestelToeTest() {
			_toestselRepo.VoegToestelToe("Sten435");
			bool isOk = _toestselRepo.GeefAlleToestellen().Any(t => t.ToestelType == "Sten435");
			Assert.IsTrue(isOk);
		}

		[TestMethod()]
		public void VerwijderToestelTest() {
			_toestselRepo.VoegToestelToe("Sten435");
			bool isOk = _toestselRepo.GeefAlleToestellen().Any(t => t.ToestelType == "Sten435");
			Assert.IsTrue(isOk);

			Toestel t = GeefToestelOpNaamTest("Sten435");
			_toestselRepo.VerwijderToestel(t);
			Assert.IsNull(GeefToestelOpNaamTest("Sten435"));
		}

		[TestMethod()]
		public Toestel GeefToestelOpNaamTest(string toestelType) => _toestselRepo.GeefAlleToestellen().FirstOrDefault(t => t.ToestelType == toestelType);
	}
}