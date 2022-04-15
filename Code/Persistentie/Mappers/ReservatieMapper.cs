using Domein;
using System;
using System.Collections.Generic;

namespace Persistentie {
	public class ReservatieMapper : IReservatieRepository {
		private List<Reservatie> _reservaties = new List<Reservatie>() {
			new Reservatie(new Klant(1, "Stan", "Persoons", "stan.persoons@student.hogent.be",
				new List<string>() {
					"Programmeren",
					"Javascript",
					"Web Development"
				}, new DateTime(2002, 04, 08), new Adres("StanStraat", "75", "Oosterzele", 9860), TypeKlant.Beheerder),
				new TijdsSlot(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0)),
				new Toestel(1, "Fiets", false)),

			new Reservatie(new Klant(2, "Bart", "Brouwer", "bart.boer@student.hogent.be",
				new List<string>() {
					"SQL",
					"Mysql",
				}, new DateTime(2003, 01, 14), new Adres("BartStraat", "14", "Oostende", 8400), TypeKlant.Gold),
				new TijdsSlot(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0)),
				new Toestel(2, "Loopband", false)),

			new Reservatie(new Klant(3, "Dirk", "Hermans", "dirk.hermans@student.hogent.be",
				new List<string>() {
					"HTML",
					"JS",
				}, new DateTime(2006, 04, 6), new Adres("DirkStraat", "4", "Gent", 9000), TypeKlant.Bronze),
				new TijdsSlot(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0)),
				new Toestel(3, "Roeien", false))
		};

		public List<Reservatie> GeefAlleReservaties() => _reservaties;

		public void VerwijderReservatie(Reservatie reservatie) => _reservaties.Remove(reservatie);

		public void VoegReservatieToe(Reservatie reservatie) => _reservaties.Add(reservatie);
	}
}
