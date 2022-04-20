using Microsoft.VisualStudio.TestTools.UnitTesting;
using Persistentie;

namespace Domein.Tests {

	[TestClass()]
	public class DomeinControllerTests {
		private DomeinController domeinController;

		[TestInitialize]
		private void Initial() {
			IKlantenRepository klantenRepo = new KlantenMapper();
			IReservatieRepository reservatienRepo = new ReservatieMapper();
			IToestelRepository toestellenRepo = new ToestellenMapper();

			domeinController = new(reservatienRepo, klantenRepo, toestellenRepo);
		}

		[TestMethod()]
		public void GeefAlleReservatiesTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void GeefKlantReservatiesTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void VoegReservatieToeTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void ReserveerToestelTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void VerwijderReservatieTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void GeefAlleTijdsSlotenTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void GeefBeschikbareUrenOpDatumTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void ResetUurIndexTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void GeefAlleKlantenTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void RegistreerKlantTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void LoginTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void GeefBeschikbareToestellenTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void GeefAlleToestellenTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void VoegNieuwToestelToeTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void VerwijderToestelTest() {
			Assert.Fail();
		}

		[TestMethod()]
		public void GeefToestelOpNaamTest() {
			Assert.Fail();
		}
	}
}