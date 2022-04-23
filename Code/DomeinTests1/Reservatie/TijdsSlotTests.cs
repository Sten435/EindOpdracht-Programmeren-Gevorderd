using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Domein.Tests {
	[TestClass()]
	public class TijdsSlotTests {
		TijdsSlot tijdsSlot;
		DateTime StartTijd = DateTime.Now;
		DateTime EindTijd;

		[TestInitialize()]
		public void Initialize() {
			EindTijd = StartTijd.AddHours(1);
			tijdsSlot = new(StartTijd);
		}

		[TestMethod()]
		public void TijdsSlotTest() {
			Assert.AreEqual(EindTijd, tijdsSlot.EindTijd);
		}
	}
}