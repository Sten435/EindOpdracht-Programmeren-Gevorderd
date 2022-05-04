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
			toestel = new("Fiets", false);
			toestelCopy = toestel;
		}

		[TestMethod()]
		public void ToestelTest() {
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

		[TestMethod()]
		public void ToestelNaamTest() {
			Assert.ThrowsException<ToestelTypeException>(() => new Toestel("S", true));
			Assert.ThrowsException<ToestelTypeException>(() => new Toestel("", true));
		}
	}
}