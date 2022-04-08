using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domein;
using Domein.Exceptions;

namespace Persistentie {
	public class KlantenMapper : IKlantenRepository {
		public List<Klant> Klanten { get; } = new List<Klant>() {
			new Klant(1, "Stan", "Persoons", "stan.persoons@student.hogent.be", 
				new List<string>() {
					"Programmeren",
					"Javascript",
					"Web Development"
				}, new DateTime(2002, 04, 08), new Adres("StanStraat", "75", "Oosterzele", 9860), TypeKlant.Beheerder),

			new Klant(2, "Bart", "Brouwer", "bart.boer@student.hogent.be",
				new List<string>() {
					"SQL",
					"Mysql",
				}, new DateTime(2003, 01, 14), new Adres("BartStraat", "14", "Oostende", 8400), TypeKlant.Gold),

			new Klant(3, "Dirk", "Hermans", "dirk.hermans@student.hogent.be",
				new List<string>() {
					"HTML",
					"JS",
				}, new DateTime(2006, 04, 6), new Adres("DirkStraat", "4", "Gent", 9000), TypeKlant.Bronze)
		};

		public List<Klant> GeefAlleKlanten() => Klanten;

		public Klant Login(string email) {
			Klant? Klant = GeefAlleKlanten().FirstOrDefault(_klant => _klant.Email == email);
			if (Klant == null) throw new LoginFailedException("Email is niet correct.");
			return Klant;
		}

		public void RegistreerKlant(Klant klant) => Klanten.Add(klant);

		public void VerwijderKlant(Klant klant) => Klanten.Remove(klant);
	}
}
