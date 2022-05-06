using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Domein.Tests {
	[TestClass()]
	public class ReservatieTests {
		Reservatie reservatie;
		TijdsSlot tijdsSlot = new(DateTime.Now.AddHours(5));
		Klant klant = new(1, "Stan", "Persoons", "stan.persoons@student.hogent.be", new List<string>(), DateTime.Now, new Adres(), TypeKlant.Silver);
		Toestel toestel = new("Fiets");

		[TestInitialize()]
		public void Initialize() {
			reservatie = new(klant, tijdsSlot, toestel);
		}

		[TestMethod()]
		public void ToStringTest() {
			Assert.AreEqual($"Nr: {reservatie.ReservatieNummer} - {reservatie.Toestel.ToestelType} - Klant: {reservatie.Klant.Voornaam} {reservatie.Klant.Achternaam} - Datum: {reservatie.TijdsSlot.StartTijd:d} - Tijdslot: {reservatie.TijdsSlot.StartTijd:t}/uur -> {reservatie.TijdsSlot.EindTijd:t}/uur", reservatie.ToString().Trim());
		}
	}
}