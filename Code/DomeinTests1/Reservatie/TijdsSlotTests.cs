using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Domein.Tests {
	[TestClass()]
	public class TijdsSlotTests {
		TijdsSlot tijdsSlot;
		DateTime StartTijd = DateTime.Now.AddHours(5);
		DateTime EindTijd;

		[TestInitialize()]
		public void Initialize() {
			EindTijd = StartTijd.AddHours(1);
			tijdsSlot = new(StartTijd);
		}

		[TestMethod()]
		public void TijdsSlotTest() => Assert.AreEqual(EindTijd, tijdsSlot.EindTijd);

		[TestMethod()]
		public void TijdsSlotTestDateTime() => Assert.ThrowsException<TijdsSlotException>(() => new TijdsSlot(DateTime.Now.AddDays(-15)));
	}
}