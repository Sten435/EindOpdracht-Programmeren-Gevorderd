using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domein.Tests {
	[TestClass()]
	public class ToestelTests {
		Toestel toestel;
		Toestel toestelCopy;

		[TestInitialize()]
		public void Initialize() {
			toestel = new(4, "Fiets", false);
			toestelCopy = new(4, "Fiets", false);
		}

		[TestMethod()]
		public void ToestelTest() {
			Assert.AreEqual(4, toestel.IdentificatieCode);
			Assert.AreEqual("Fiets", toestel.ToestelType);
			Assert.AreEqual(false, toestel.InHerstelling);
		}

		[TestMethod()]
		public void ToStringTest() {
			Assert.AreEqual("Fiets - InHerstelling: False", toestel.ToString());
		}

		[TestMethod()]
		public void EqualsTest() {
			Assert.AreEqual(toestel, toestelCopy);
		}
	}
}