using Domein;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Domein.Tests {
	[TestClass()]
	public class KlantTests {

		[DataRow("Stan", "Persoons", "stan.persoons@student.hogent.be", TypeKlant.Bronze)]
		[TestMethod()]
		public void KlantNummerTest(string voorNaam, string achterNaam, string email, TypeKlant typeKlant) {
			DateTime geboorteDatum = DateTime.Now;
			Assert.ThrowsException<KlantenNummerException>(() => new Klant(0, voorNaam, achterNaam, email, new List<string>(), geboorteDatum, new Adres(), typeKlant));
		}

		[DataRow("s", "Persoons", "stan.persoons@hogent.be", TypeKlant.Bronze)]
		[TestMethod()]
		public void KlantVoorNaam(string voorNaam, string achterNaam, string email, TypeKlant typeKlant) {
			DateTime geboorteDatum = DateTime.Now;
			Assert.ThrowsException<VoorNaamException>(() => new Klant(1, voorNaam, achterNaam, email, new List<string>(), geboorteDatum, new Adres(), typeKlant));
		}

		[DataRow("Stan", "P", "stan.persoons@student.hogent.be", TypeKlant.Bronze)]
		[TestMethod()]
		public void KlantAchterNaam(string voorNaam, string achterNaam, string email, TypeKlant typeKlant) {
			DateTime geboorteDatum = DateTime.Now;
			Assert.ThrowsException<AchterNaamExpection>(() => new Klant(1, voorNaam, achterNaam, email, new List<string>(), geboorteDatum, new Adres(), typeKlant));
		}

		[DataRow("Stan", "Persoons", "stan&.pers@@osdqqns@student..hogent.be", TypeKlant.Bronze)]
		[TestMethod()]
		public void KlantEmail(string voorNaam, string achterNaam, string email, TypeKlant typeKlant) {
			DateTime geboorteDatum = DateTime.Now;
			Assert.ThrowsException<EmailExpection>(() => new Klant(1, voorNaam, achterNaam, email, new List<string>(), geboorteDatum, new Adres(), typeKlant));
		}
	}
}