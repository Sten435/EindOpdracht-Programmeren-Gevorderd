using Microsoft.VisualStudio.TestTools.UnitTesting;
using Persistentie;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domein.Tests {

	[TestClass()]
	public class DomeinControllerTests {
		private Reservatie reservatie;
		private Klant klant;

		private IReservatieRepository _reservatieRepo;
		private IKlantenRepository _klantenRepo;
		private IToestelRepository _toestselRepo;
		private IConfigRepository _configRepository;

		[TestInitialize()]
		public void Initialize() {
			_reservatieRepo = new ReservatieRepository();
			_klantenRepo = new KlantenRepository();
			_toestselRepo = new ToestellenRepository();
			new ConfigRepository().LoadConfig();

			reservatie = new(int.MaxValue - 2468, new Klant(), new TijdsSlot(DateTime.Now.AddHours(5)), new Toestel(1, "Stantoestel"));
			klant = new(1, "Stan", "Persoons", "stan.persoons1@student.hogent.be", new List<string>(), DateTime.Now, new Adres(), TypeKlant.Silver);
		}

		[TestMethod()]
		public void LoginTest() {
			Klant klant1 = _klantenRepo.Login("stan.persoons@student.hogent.be");
			Assert.IsNotNull(klant1);
		}

		[TestMethod()]
		public Toestel GeefToestelOpNaamTest(string toestelType) => _toestselRepo.GeefAlleToestellen().FirstOrDefault(t => t.ToestelType == toestelType);
	}
}