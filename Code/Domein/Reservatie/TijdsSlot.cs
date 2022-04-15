using System;

namespace Domein {

	public class TijdsSlot {
		public DateTime StartTijd { get; }
		public DateTime EindTijd { get; }

		private const int _tijdsZone = 2;
		private const int _slotDuurTijd = 1;

		public TijdsSlot(DateTime startTijd) {
			StartTijd = startTijd.ToUniversalTime().AddHours(_tijdsZone);
			EindTijd = StartTijd.AddHours(_slotDuurTijd);
		}
	}
}