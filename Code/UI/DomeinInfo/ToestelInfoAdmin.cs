using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UI {
	public class ToestelInfo {
		public int ToestelNummer { get; set; }
		public string ToestelNaam { get; set; }
		public bool InHerstelling { get; set; }
		public bool Verwijderd { get; set; }
		public Visibility HeeftReservatieOfNiet { get; set; }
		public bool HeeftReservatie { get; set; } = false;
		public string InHerstellingDisplay { get; set; }
		public string VerwijderdDisplay { get; set; }

		public ToestelInfo(string toestelString) {
			string[] parsedToestel = toestelString.Split("@|@");

			ToestelNummer = int.Parse(parsedToestel[0]);
			ToestelNaam = parsedToestel[1];
			InHerstelling = bool.Parse(parsedToestel[2]);
			Verwijderd = bool.Parse(parsedToestel[3]);

			if (parsedToestel.Length == 5)
				HeeftReservatie = bool.Parse(parsedToestel[4]);

			if (InHerstelling)
				InHerstellingDisplay = "Nee";
			else
				InHerstellingDisplay = "Ja";

			if (Verwijderd) {
				VerwijderdDisplay = "Verwijderd";
			} else
				VerwijderdDisplay = "Actief";

			if (!HeeftReservatie && !Verwijderd) {
				HeeftReservatieOfNiet = Visibility.Visible;
			} else
				HeeftReservatieOfNiet = Visibility.Hidden;
		}
	}
}