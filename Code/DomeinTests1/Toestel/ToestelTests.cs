using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Domein.Tests {

	[TestClass()]
	public class ToestelTests {
		private Toestel toestel;
		private Toestel toestelCopy;

		[TestInitialize()]
		public void Initialize() {
			toestel = new(999996, "Fiets");
			toestelCopy = toestel;
		}

		[TestMethod()]
		public void ToestelTest() {
			Assert.AreEqual("Fiets", toestel.ToestelType);
			Assert.AreEqual(false, toestel.InHerstelling);
		}

		[TestMethod()]
		public void ToStringTest() {
			Assert.AreEqual("Fiets - InHerstelling: False", toestel.ToString());
			Assert.AreEqual("999996@|@Fiets@|@False@|@False", toestel.ToString(true));
		}

		[TestMethod()]
		public void EqualsTest() {
			Assert.AreEqual(toestel, toestelCopy);
		}

		[TestMethod()]
		public void ToestelNaamTest() {
			Assert.ThrowsException<ToestelException>(() => new Toestel(1, "S"));
			Assert.ThrowsException<ToestelException>(() => new Toestel(1, ""));
			Assert.ThrowsException<ToestelException>(() => new Toestel(1, "1"));
			Assert.ThrowsException<ToestelException>(() => new Toestel(1, "-)^"));
			Assert.ThrowsException<ToestelException>(() => new Toestel(1, "fsf3"));
		}
	}
}