using Domein;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Persistentie {

	public class KlantenMapper {

		public List<Klant> Klanten = new() {
			new Klant(1, "Stan", "Persoons", "stan.persoons@student.hogent.be",
				new List<string>() {
					"Programmeren",
					"Javascript",
					"Web Development"
				}, new DateTime(2002, 04, 08), new Adres("StanStraat", "75", "Oosterzele", 9860), TypeKlant.Beheerder)
		};
	}
}