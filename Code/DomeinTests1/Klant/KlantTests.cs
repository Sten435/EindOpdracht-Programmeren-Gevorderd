using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Domein.Tests {
	[TestClass()]
	public class KlantTests {
		Klant klant;

		[TestInitialize()]
		public void Initialize() {
			klant = new();
			klant = new(1,"testVoorNaam","testNaam","email", new List<string>(), DateTime.Now, new Adres(), TypeKlant.Silver);
		}

		[TestMethod()]
		public void KlantTest() {
			Assert.AreEqual("testVoorNaam", klant.Voornaam);
			Assert.AreEqual("testNaam", klant.Achternaam);
			Assert.AreEqual("email", klant.Email);
			Assert.AreEqual(1, klant.KlantenNummer);
			Assert.AreEqual(TypeKlant.Silver, klant.TypeKlant);
		}
	}
}