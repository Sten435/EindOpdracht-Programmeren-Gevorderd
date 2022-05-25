using Microsoft.VisualStudio.TestTools.UnitTesting;
using Persistentie;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domein.Tests {

	[TestClass()]
	public class DomeinControllerTests {

		private IReservatieRepository _reservatieRepo;
		private IKlantenRepository _klantenRepo;
		private IToestelRepository _toestselRepo;
		private IConfigRepository _configRepository;

		static private DomeinController domeinController;

		[TestInitialize()]
		public void Initialize() {
			_reservatieRepo = new ReservatieRepository();
			_klantenRepo = new KlantenRepository();
			_toestselRepo = new ToestellenRepository();

			_configRepository = new ConfigRepository(true);
			_configRepository.LoadConfig();

			domeinController = new(_reservatieRepo, _klantenRepo, _toestselRepo, _configRepository);
		}

		[TestMethod()]
		[DataRow("stan.persoons@student.hogent.be", false)]
		[DataRow("Goedele.Wild@gmail.com", false)]
		[DataRow("Jesse.Desmet@orange.BE", false)]
		[DataRow("Carl.Goeminne@hotmail.com", false)]

		[DataRow("stan.persoons@student.be", true)]
		[DataRow("stan.persoons#student.be", true)]
		[DataRow("stan..@student.be", true)]
		[DataRow("@student.be", true, true)]
		[DataRow("", true, true)]
		[DataRow("#stan", true)]
		[DataRow("stan.1.-persoons@student.be", true)]
		public void LoginTest(string email, bool isfalse = false, bool emailEx = false) {
			if (!isfalse) {
				Klant klant1 = _klantenRepo.Login(email);
				Assert.IsNotNull(klant1);
			} else {
				if (emailEx) {
					Assert.ThrowsException<EmailExpection>(() => _klantenRepo.Login(email));
				} else
					Assert.ThrowsException<LoginException>(() => _klantenRepo.Login(email));
			}
		}

		[TestMethod()]
		[DataRow("ToestelNaam")]
		[DataRow("Ab")]
		[DataRow("dqs-", true)]
		[DataRow("190'jfds", true)]
		[DataRow("2", true)]
		[DataRow("^", true)]
		[DataRow("", true)]
		public void Toestellen(string naam, bool error = false) {
			if (!error) {
				int toestelId = domeinController.VoegNieuwToestelToe(naam);

				Assert.AreEqual(true, domeinController.GeefAlleToestellen().Select(toestel => toestel.Split("@|@")[1]).Any(toestel => toestel == naam.Substring(0, 1).ToUpper() + naam.Substring(1).ToLower()));
			} else {
				Assert.ThrowsException<ToestelException>(() => domeinController.VoegNieuwToestelToe(naam));
			}
		}

		[TestMethod()]
		[DataRow("ToestelNaam", "25/05/2023 15:00:00")]
		[DataRow("Ab", "26/05/2023 14:00:00")]
		[DataRow("Loop", "21/05/2023 7:00:00", true)]
		[DataRow("Test", "26/05/2023 1:00:00", true)]
		[DataRow("Fietsen", "25/05/2022 15:00:00", true)]
		[DataRow("Autocar", "18/05/2022", true)]
		public void Reservatie(string naam, string reservatie, bool error = false) {
			domeinController.Login("1");

			if (!error) {
				int toestelId = domeinController.VoegNieuwToestelToe(naam);

				string reservatieParsed = domeinController.VoegReservatieToe(DateTime.Parse(reservatie), toestelId);

				Assert.AreNotEqual("", reservatieParsed);
			} else {
				int toestelId = domeinController.VoegNieuwToestelToe(naam);

				Assert.ThrowsException<ReservatieException>(() => domeinController.VoegReservatieToe(DateTime.Parse(reservatie), toestelId));
			}
		}
	}
}