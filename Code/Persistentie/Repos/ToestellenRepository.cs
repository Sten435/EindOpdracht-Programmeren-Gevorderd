﻿using Domein;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistentie {
	public class ToestellenRepository : IToestelRepository {
		private static UniekeCode _uniekeCode = UniekeCode.Instance;
		private ToestellenMapper _mapper = new();

		public ToestellenRepository() {
			LaadToestellen();
		}

		private bool LaadToestellen() {
			string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string sFile = Path.Combine(sCurrentDirectory, @"Data\FitnessToestellen.txt");
			string sFilePath = Path.GetFullPath(sFile);

			try {
				using TextFieldParser parser = new(sFilePath);
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(",");

				int klantenNr = (int)parser.LineNumber + 3;

				while (!parser.EndOfData) {
					List<string> fields = parser.ReadFields().ToList();
					fields = fields.Select(field => field.Replace("\'", "")).ToList();

					VoegToestelToe(fields[1]);
				}
			} catch (Exception err) {
				Console.WriteLine(err.StackTrace);
				return false;
			}

			return true;
		}

		public List<Toestel> GeefAlleToestellen() => _mapper.Toestellen;

		public void VoegToestelToe(string naam) => _mapper.Toestellen.Add(new(_uniekeCode.GenereerRandomCode(), naam[0].ToString().ToUpper() + naam.ToLower().Substring(1), false));

		public void VerwijderToestel(Toestel toestel) => _mapper.Toestellen.Remove(toestel);

		public void UpdateToestelOpId(long toestelId, string toestelNaam) {
			_mapper.Toestellen = _mapper.Toestellen.Select(t => {
				if (t.IdentificatieCode == toestelId) t.ToestelType = toestelNaam;
				return t;
			}).ToList();
		}

		public void ZetToestelInOfUitHerstelling(long toestelId, bool nieuweHerstellingValue) {
			_mapper.Toestellen = _mapper.Toestellen.Select(t => {
				if (t.IdentificatieCode == toestelId) t.InHerstelling = nieuweHerstellingValue;
				return t;
			}).ToList();
		}

		public bool GeefToestelHerstelStatusOpId(long toestelId) => _mapper.Toestellen.Find(t => t.IdentificatieCode == toestelId).InHerstelling;
	}
}
