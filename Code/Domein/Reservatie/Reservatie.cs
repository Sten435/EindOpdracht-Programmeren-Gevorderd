﻿using System.Collections.Generic;

namespace Domein {

	public class Reservatie {
		public int ReservatieNummer { get; set; }
		public Klant Klant { get; }
		public TijdsSlot TijdsSlot { get; }
		public Toestel Toestel { get; }

		public Reservatie(Klant klant, TijdsSlot tijdsSlot, Toestel toestel) {
			Klant = klant;
			TijdsSlot = tijdsSlot;
			Toestel = toestel;
		}

		public override string ToString() {
			return $"Toestel: {Toestel.ToestelType} - Begin Tijd: {TijdsSlot.StartTijd:g} - Eind Tijd: {TijdsSlot.EindTijd:g}";
		}
	}
}