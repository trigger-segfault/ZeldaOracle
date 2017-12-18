using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConscriptDesigner.Util {
	// NOTE: This code is free to use in any program.
	// ... It was developed by Dot Net Perls.
	//https://www.dotnetperls.com/alphanumeric-sorting
	public class AlphanumComparator : IComparer {

		private bool ignoreCase;

		public AlphanumComparator() {
			this.ignoreCase = false;
		}
		public AlphanumComparator(bool ignoreCase) {
			this.ignoreCase = ignoreCase;
		}
		public static int Compare(object x, object y, bool ignoreCase) {
			string s1 = x as string;
			if (s1 == null) {
				return 0;
			}
			string s2 = y as string;
			if (s2 == null) {
				return 0;
			}

			int len1 = s1.Length;
			int len2 = s2.Length;
			int marker1 = 0;
			int marker2 = 0;

			// Walk through two the strings with two markers.
			while (marker1 < len1 && marker2 < len2) {
				char ch1 = s1[marker1];
				char ch2 = s2[marker2];

				// Some buffers we can build up characters in for each chunk.
				char[] space1 = new char[len1];
				int loc1 = 0;
				char[] space2 = new char[len2];
				int loc2 = 0;

				// Walk through all following characters that are digits or
				// characters in BOTH strings starting at the appropriate marker.
				// Collect char arrays.
				do {
					space1[loc1++] = ch1;
					marker1++;

					if (marker1 < len1) {
						ch1 = s1[marker1];
					}
					else {
						break;
					}
				} while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

				do {
					space2[loc2++] = ch2;
					marker2++;

					if (marker2 < len2) {
						ch2 = s2[marker2];
					}
					else {
						break;
					}
				} while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

				// If we have collected numbers, compare them numerically.
				// Otherwise, if we have strings, compare them alphabetically.
				string str1 = new string(space1);
				string str2 = new string(space2);

				int result;

				if (char.IsDigit(space1[0]) && char.IsDigit(space2[0])) {
					int numericChunk1 = int.Parse(str1);
					int numericChunk2 = int.Parse(str2);
					result = numericChunk1.CompareTo(numericChunk2);
				}
				else {
					result = string.Compare(str1, str2, ignoreCase);
				}

				if (result != 0) {
					return result;
				}
			}
			return len1 - len2;
		}
		public int Compare(object x, object y) {
			return Compare(x, y, ignoreCase);
		}

		public bool IgnoreCase {
			get { return ignoreCase; }
			set { ignoreCase = value; }
		}
	}
}
