using System;

namespace Domein {

	public class TijdsSlot {
		public DateTime StartTijd { get; }
		public DateTime EindTijd { get; }

		private const int slotDuurTijd = 1;

		public TijdsSlot(DateTime startTijd) {
			StartTijd = startTijd;
			EindTijd = startTijd.AddHours(slotDuurTijd);
		}
	}
}