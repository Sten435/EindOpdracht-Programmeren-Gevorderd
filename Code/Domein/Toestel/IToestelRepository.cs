﻿using System.Collections.Generic;

namespace Domein {

	public interface IToestelRepository {
		List<Toestel> GeefAlleToestellen();

		void VoegToestelToe(string naam);

		void ZetToestelInHerstelling(Toestel toestel);

		void VerwijderToestel(Toestel toestel);
	}
}